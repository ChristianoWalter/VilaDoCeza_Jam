using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public enum PlayerHabilities
{
    ranged,
    secondForm,
    thirdForm
}

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
    [Header("--Combat Variables--")]
    [SerializeField] float jumpDamage;
    [SerializeField] float jumpForceAfterDamage;
    [SerializeField] LayerMask enemiesMask;

    // Variáveis de combate a curta distância
    [Header("Melee combat vaiables")]
    public bool canMeleeAttack;
    [SerializeField] HabilitiesUIControl meleeUI;
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeReloadTime;
    float timeToMeleeReload;
    
    // Variáveis de combate a longa distância
    [Header("Ranged combat vaiables")]
    public bool canRangedAttack;
    [SerializeField] HabilitiesUIControl rangedUI;
    [SerializeField] GameObject projectile;
    //[SerializeField] float rangedDamage;
    [SerializeField] float rangedReloadTime;
    float timeToRangedReload;

    // Componentes externos que serão atrelados ao script externamente
    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform foot;
    [SerializeField] Transform attackPoint;

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
        if ((direction > 0 && transform.localScale.x < 0) || (direction < 0 && transform.localScale.x > 0))
        {
            Vector2 _localScale = transform.localScale;
            _localScale.x *= -1f;
            transform.localScale = _localScale;
        }
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

    // Região destinada à métodos e funções de ataque
    #region Attack Methods
    // Método para execução de ataques à distânca
    public void RangedAttack()
    {
        if (canRangedAttack && timeToRangedReload == 0)
        {
            Instantiate(projectile, attackPoint.position, attackPoint.rotation).GetComponent<ProjectileController>().direction = new Vector2(transform.localScale.x, .15f);
            timeToRangedReload = rangedReloadTime;
            rangedUI.UpdateFill(rangedReloadTime/rangedReloadTime);
            StartCoroutine(RangedReload());
        }
    }

    // Método para execução de recarga do ataque à distância
    public IEnumerator RangedReload()
    {
        for (float i = rangedReloadTime; i != 0; i = Mathf.Max(i - .1f, 0))
        {
            timeToRangedReload = i;
            rangedUI.UpdateFill(timeToRangedReload/rangedReloadTime);
            yield return new WaitForSeconds(.1f);
        }
        timeToRangedReload = 0f;
        rangedUI.UpdateFill(timeToRangedReload / rangedReloadTime);
    }

    // Método para execução de ataques à curta distânca
    public void MeleeAttack()
    {
        if (canMeleeAttack && timeToMeleeReload == 0)
        {

            timeToMeleeReload = meleeReloadTime;
            StartCoroutine (MeleeReload());
        }
    }

    // Método para execução de recarga do ataque à curta distância
    public IEnumerator MeleeReload()
    {
        for (float i = meleeReloadTime; i != 0; i = Mathf.Max(i - .1f, 0))
        {
            timeToMeleeReload = i;
            yield return new WaitForSeconds(.1f);
        }
        timeToMeleeReload = 0f;
    }
    #endregion

    // Região destinada a métodos de recepção de input
    #region Inputs Methods
    // Método de recepção de input para movimentação horizontal
    public void MovementAction(InputAction.CallbackContext value)
    {
        direction = value.ReadValue<float>();
    }
    
    // Método de recepção de input para ataque a distância
    public void RangedAction(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            RangedAttack();
        }
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
