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
    thirdForm // Nome provis�rio
}

public class PlayerController : HealthController
{
    public static PlayerController instance;

    [Header("---Player Settings---")]

    // Vari�veis que definem a movimenta��o do Player
    [Header("Movement Variables")]
    public bool canMove;
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask groundMask;

    // Vari�vel para a recep��o de input e altera��o da dire��o do personagem
    float direction;

    // Vari�veis para recuperar velocidade e gravidade quando pausar o jogo
    Vector2 afterPauseSpeed;
    float afterPauseGravity;

    // Vari�veis que definem o formato de combate do Player
    [Header("--Combat Variables--")]
    [SerializeField] float jumpDamage;
    [SerializeField] float jumpForceAfterDamage;
    [SerializeField] PlayerForms playerForms;
    [SerializeField] LayerMask enemiesMask;
    public bool canAttack;
    float currentTimeToReload;
    [SerializeField] HabilitiesUIControl reloadUI;

    // Vari�vel para retornar ao spawnpoint quando ferido
    [HideInInspector] public Vector2 respawnPoint;


    // Vari�veis referentes ao ataque da segunda transforma��o
    [Header("Second Form habilities variables")]
    [SerializeField] float damage;
    [SerializeField] float range;
    [SerializeField] float secondReloadTime;

    // Vari�veis referentes ao ataque da transforma��o de palha�o
    [Header("Clown habilities")]
    [SerializeField] GameObject projectile;
    [SerializeField] float clownReloadTime;
    float timeToReload;

    // Componentes externos que ser�o atrelados ao script externamente
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

    // M�todo destinado � entrada de colis�o no player
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemys Head")
        {
            Debug.Log("foi");
            other.gameObject.GetComponentInParent<HealthController>()?.TakeDamage(jumpDamage);
            Jump(jumpForceAfterDamage);
        }
    }

    // M�todo destinado � detec��o de colis�o (tipo trigger) no player
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

    // Regi�o destinada para m�todos relacionados � movimenta��o do Player
    #region Movement Methods
    // M�todo para retorno de booleana (caso o player esteja no ch�o) para execu��o de um pulo via input
    public bool OnGround()
    {
        return Physics2D.OverlapCircle(foot.position, .2f, groundMask);
    }

    // M�todo para executar a��o de pulo, podendo receber um valor externo para tal
    public void Jump(float _jumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, _jumpForce);
    }

    // M�todo destinado � pausar e despausar a movimenta��o do Player quando desejado
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

    // M�todo destinado � transforma��o do player
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

    // Regi�o destinada � m�todos e fun��es de ataque
    #region Attack Methods
    // M�todo para execu��o de ataques � dist�nca
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

    // M�todo para execu��o de recarga do ataque � dist�ncia
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

    // Regi�o destinada a m�todos de recep��o de input
    #region Inputs Methods
    // M�todo de recep��o de input para movimenta��o horizontal
    public void MovementAction(InputAction.CallbackContext value)
    {
        direction = value.ReadValue<float>();
    }
    
    // M�todo de recep��o de input para ataque a dist�ncia
    public void RangedAction(InputAction.CallbackContext value)
    {
        if (value.performed && canMove)
        {
            Attack();
        }
    }
    
    // M�todo para recep��o de input para execu��o de pulos
    public void JumpAction(InputAction.CallbackContext value)
    {
        if (value.performed && canMove)
        {
            if(OnGround()) Jump(jumpForce);
        }
    }
    #endregion

    // �rea para reescritura de m�todos do HealthController
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

    // Rotina para ressurgimento do player ap�s dano
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

    // Rotina para sequ�ncia de morte
    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(2f);
    }
    #endregion
}
