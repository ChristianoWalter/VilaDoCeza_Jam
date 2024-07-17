using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public enum PlayerForms
{
    normal,
    clown,
    thirdForm // Nome provisório
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
    [SerializeField] PlayerForms playerForms;
    [SerializeField] LayerMask enemiesMask;
    public bool canAttack;
    float currentTimeToReload;
    [SerializeField] HabilitiesUIControl reloadUI;

    // Variável para retornar ao spawnpoint quando ferido
    [HideInInspector] public Vector2 respawnPoint;


    // Variáveis referentes ao ataque da segunda transformação
    [Header("Second Form habilities variables")]
    [SerializeField] float damage;
    [SerializeField] float range;
    [SerializeField] float secondReloadTime;

    // Variáveis referentes ao ataque da transformação de palhaço
    [Header("Clown habilities")]
    [SerializeField] GameObject projectile;
    [SerializeField] float clownReloadTime;
    float timeToReload;

    // Componentes externos que serão atrelados ao script externamente
    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform foot;
    [SerializeField] Transform attackPoint;
    [SerializeField] Animator anim;

    protected override void Awake()
    {
        base.Awake();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);       

        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;

        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        if ((direction > 0 && transform.localScale.x < 0) || (direction < 0 && transform.localScale.x > 0))
        {
            Vector2 _localScale = transform.localScale;
            _localScale.x *= -1f;
            transform.localScale = _localScale;
        }

        anim.SetBool("OnGround", OnGround());
    }

    // Método destinado à entrada de colisão no player
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemys Head")
        {
            Debug.Log("foi");
            other.gameObject.GetComponentInParent<HealthController>()?.TakeDamage(jumpDamage);
            Jump(jumpForceAfterDamage);
        }
    }

    // Método destinado à detecção de colisão (tipo trigger) no player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "SpawnPoint")
        {
            respawnPoint = other.gameObject.transform.position;
        }

        if (other.gameObject.tag == "Enemys Head" && !dead)
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
    public void PausePlayerMovement(bool pause)
    {
        if (pause)
        {
            canMove = false;
            afterPauseSpeed = rb.velocity;
            afterPauseGravity = rb.gravityScale;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
        else
        {
            canMove = true;
            rb.velocity = afterPauseSpeed;
            rb.gravityScale = afterPauseGravity;
        }
    }
    #endregion

    // Método destinado à transformação do player
    public void SwitchPlayerForm(PlayerForms _newForm)
    {
        playerForms = _newForm;
        anim.SetInteger("PlayerForm", (int)_newForm);
        StopAllCoroutines();
        switch (_newForm)
        {
            case PlayerForms.normal:
                canAttack = false;
                reloadUI.UpdateFill(1f);
                break;
            case PlayerForms.clown:
                canAttack = true;
                currentTimeToReload = clownReloadTime;
                reloadUI.UpdateFill(0f);
                break;
            case PlayerForms.thirdForm:
                canAttack = true;
                currentTimeToReload = secondReloadTime;
                reloadUI.UpdateFill(0f);
                break;
        }
        anim.SetTrigger("ChangeForm");
    }

    // Região destinada à métodos e funções de ataque
    #region Attack Methods
    // Método para execução de ataques à distânca
    public void Attack()
    {
        if (canAttack)// && timeToReload == 0)
        {
            canAttack = false;
            switch (playerForms) 
            {
                case PlayerForms.clown:
                    Instantiate(projectile, attackPoint.position, attackPoint.rotation).GetComponent<ProjectileController>().direction = new Vector2(transform.localScale.x, .15f);
                    currentTimeToReload = clownReloadTime;
                    StartCoroutine(AttackReload());
                    break;
                case PlayerForms.thirdForm:
                    currentTimeToReload = secondReloadTime;
                    StartCoroutine(AttackReload());
                    break;
            }
            timeToReload = currentTimeToReload;
            reloadUI.UpdateFill(currentTimeToReload / currentTimeToReload);
        } 
    }

    // Método para execução de recarga do ataque à distância
    public IEnumerator AttackReload()
    {
        for (float i = currentTimeToReload; i != 0; i = Mathf.Max(i - .1f, 0))
        {
            timeToReload = i;
            reloadUI.UpdateFill(timeToReload/currentTimeToReload);
            yield return new WaitForSeconds(.1f);
        }
        canAttack = true;
        timeToReload = 0f;
        reloadUI.UpdateFill(timeToReload / currentTimeToReload);
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
        if (value.performed && canMove)
        {
            Attack();
        }
    }
    
    // Método para recepção de input para execução de pulos
    public void JumpAction(InputAction.CallbackContext value)
    {
        if (value.performed && canMove)
        {
            if(OnGround()) Jump(jumpForce);
        }
    }
    #endregion

    // Área para reescritura de métodos do HealthController
    #region HealthController override Methods
    protected override void DamageEffect()
    {
        base.DamageEffect();
        dead = true;
        anim.SetBool("Respawning", false);
        isInvencible = true;
        anim.SetTrigger("Death");

        if (currentHealth > 0) StartCoroutine(Respawn());
    }

    // Rotina para ressurgimento do player após dano
    IEnumerator Respawn()
    {
        PausePlayerMovement(true);
        gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
        //GameManager.instance.SwitchScreen(GameScreens.loading);
        yield return new WaitForSeconds(2f);
        transform.position = respawnPoint;
        anim.SetBool("Respawning", true);
        SwitchPlayerForm(PlayerForms.normal);
        isInvencible = false;
        //GameManager.instance.SwitchScreen(GameScreens.gameUI);
        gameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
        dead = false;
        PausePlayerMovement(false);
    }

    protected override void Death()
    {
        base.Death();

        PausePlayerMovement(true);
        StartCoroutine(DeathRoutine());
    }

    // Rotina para sequência de morte
    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(2f);
    }
    #endregion
}
