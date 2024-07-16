using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : HealthController
{
    public static PlayerController instance;

    [Header("---Player Settings---")]

    // Variáveis que definem a movimentação do Player
    [Header("Movement Variables")]
    public bool canMove;
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask groundMask;

    // Variável para a recepção de input e alteração da direção do personagem
    float direction;

    // Variáveis para recuperar velocidade e gravidade quando pausar o jogo
    Vector2 afterPauseSpeed;
    float afterPauseGravity;

    // Variáveis que definem o formato de combate do Player
    [Header("Combat Variables")]
    [SerializeField] float jumpDamage;
    [SerializeField] float jumpForceAfterDamage;
    [SerializeField] LayerMask enemiesMask;

    // Componentes externos que serão atrelados ao script externamente
    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform foot;

    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;

        rb.velocity = new Vector2 (direction * moveSpeed, rb.velocity.y);
    }

    // Método destinada à entrada de colisão no player
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemys Head")
        {
            other.gameObject.GetComponentInParent<HealthController>()?.TakeDamage(jumpDamage);
            Jump(jumpForceAfterDamage);
        }
    }

    // Região destinada para métodos relacionados à movimentação do Player
    #region Movement Methods
    // Método para retorno de booleana (caso o player esteja no chão) para execução de um pulo via input
    public bool OnGround()
    {
        return Physics2D.OverlapCircle(foot.position, .2f, groundMask);
    }

    // Método para executar ação de pulo, podendo receber um valor externo para tal
    public void Jump(float _jumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, _jumpForce);
    }

    // Método destinado à pausar e despausar a movimentação do Player quando desejado
    public void PausePlayerMovement()
    {
        if (canMove)
        {
            afterPauseSpeed = rb.velocity;
            afterPauseGravity = rb.gravityScale;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            canMove = false;
        }
        else
        {
            canMove = true;
            rb.velocity = afterPauseSpeed;
            rb.gravityScale = afterPauseGravity;
        }
    }
    #endregion

    // Região destinada a métodos de recepção de input
    #region Inputs Methods
    // Método de recepção de input para movimentação horizontal
    public void MovementAction(InputAction.CallbackContext value)
    {
        direction = value.ReadValue<float>();
    }
    
    // Método para recepção de input para execução de pulos
    public void JumpAction(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            if(OnGround()) Jump(jumpForce);
        }
    }
    #endregion

}
