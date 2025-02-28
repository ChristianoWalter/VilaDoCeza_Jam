using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public enum PlayerForms
{
    normal,
    clown,
    cowboy
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
    float originalGravity;

    // Vari�veis que definem o formato de combate do Player
    [Header("--Combat Variables--")]
    [SerializeField] float jumpDamage;
    [SerializeField] float jumpForceAfterDamage;
    [SerializeField] PlayerForms playerForms;
    //[SerializeField] LayerMask enemiesMask;
    public bool canUseHability;
    float currentTimeToReload;
    //[SerializeField] HabilitiesUIControl reloadUI;

    // Vari�vel para retornar ao spawnpoint quando ferido
    [HideInInspector] public Vector2 respawnPoint = new Vector2(0, .1f);


    // Vari�veis referentes ao ataque da segunda transforma��o
    [Header("Cowboy habilities variables")]
    [SerializeField] float dashPower;
    [SerializeField] float dashingTime;
    [SerializeField] float dashReloadTime;
    [SerializeField] TrailRenderer dashTrail;
    private bool isDashing;

    // Vari�veis referentes ao ataque da transforma��o de palha�o
    [Header("Clown habilities variables")]
    [SerializeField] GameObject projectile;
    [SerializeField] float clownReloadTime;
    float timeToReload;

    // Vari�veis para controle de efeitos sonoros
    [Header("SFX Controls")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource stepSource;
    [SerializeField] AudioClip[] sfx;
    [SerializeField] AudioManager audioManager;

    // Componentes externos que ser�o atrelados ao script externamente
    [Header("Components")]
    [SerializeField] GameObject transformationEffect;
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

        originalGravity = rb.gravityScale;
        canMove = true;
        audioManager = FindObjectOfType<AudioManager>();
        if (lifeCounter == null) lifeCounter = FindObjectOfType<GameManager>().lifeTxt;
    }

    // Update is called once per frame
    void Update()
    {
        if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
        if (lifeCounter == null) lifeCounter = FindObjectOfType<GameManager>().lifeTxt;
        if (!canMove || isDashing) return;

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
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
        else
        {
            canMove = true;
            rb.velocity = afterPauseSpeed;
            rb.gravityScale = originalGravity;
        }
    }
    #endregion

    // M�todo para alterar vari�veis do player ao iniciar uma fase
    public void SetPlayerToNewLvl()
    {
        if (dead)
        {
            dead = false;
            anim.SetBool("Dead", dead);
        }
        if (isInvencible) isInvencible = false;
        TakeHeal(maxHealth);
    }

    // M�todo destinado � aplica��o de sons ao andar do player
    public void StepSound()
    {
        stepSource.PlayOneShot(sfx[Random.Range(1, 4)]);
    }

    // M�todo destinado � transforma��o do player
    public void SwitchPlayerForm(PlayerForms _newForm)
    {
        playerForms = _newForm;
        anim.SetInteger("PlayerForm", (int)_newForm);
        switch (_newForm)
        {
            case PlayerForms.normal:
                canUseHability = false;
                //if (reloadUI != null) reloadUI.UpdateFill(1f);
                break;
            case PlayerForms.clown:
                canUseHability = true;
                currentTimeToReload = clownReloadTime;
                //if (reloadUI != null) reloadUI.UpdateFill(0f);
                break;
            case PlayerForms.cowboy:
                canUseHability = true;
                currentTimeToReload = dashReloadTime;
                //if (reloadUI != null) reloadUI.UpdateFill(0f);
                break;
        }
        if (!dead) Instantiate(transformationEffect, transform.position, transform.rotation).transform.SetParent(gameObject.transform);
        anim.SetTrigger("ChangeForm");
    }

    // Regi�o destinada � m�todos e fun��es das habilidades
    #region Habilities Methods
    // M�todo para execu��o de ataques � dist�nca
    public void Attack()
    {
        if (canUseHability)
        {
            switch (playerForms) 
            {
                case PlayerForms.normal:
                    canUseHability = false;
                    break;
                case PlayerForms.clown:
                    canUseHability = false;
                    Instantiate(projectile, attackPoint.position, attackPoint.rotation).GetComponent<ProjectileController>().direction = new Vector2(transform.localScale.x, .15f);
                    currentTimeToReload = clownReloadTime;
                    StartCoroutine(AttackReload());
                    break;
            }
            timeToReload = currentTimeToReload;
            anim.SetFloat("ReloadAttack", timeToReload);
            //reloadUI.UpdateFill(currentTimeToReload / currentTimeToReload);
        } 
    }

    // m�todo para ativa��o do dash
    public void Dash()
    {
        if (canUseHability)
        {
            switch (playerForms)
            {
                case PlayerForms.normal:
                    canUseHability = false;
                    break;
                case PlayerForms.cowboy:
                    canUseHability = false;
                    StartCoroutine(DashRoutine());
                    break;
            }
        }
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        if (OnGround()) stepSource.PlayOneShot(sfx[5]);
        dashTrail.emitting = true;
        anim.SetTrigger("Dash");
        yield return new WaitForSeconds(dashingTime);
        dashTrail.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashReloadTime);
        canUseHability = true;
    }

    // M�todo para execu��o de recarga do ataque � dist�ncia
    public IEnumerator AttackReload()
    {
        for (float i = currentTimeToReload; i != 0; i = Mathf.Max(i - .1f, 0))
        {
            timeToReload = i;
            //reloadUI.UpdateFill(timeToReload/currentTimeToReload);
            yield return new WaitForSeconds(.1f);
        }
        canUseHability = true;
        timeToReload = 0f;
        anim.SetFloat("ReloadAttack", timeToReload);
        //reloadUI.UpdateFill(timeToReload / currentTimeToReload);
    }
    #endregion

    // Regi�o destinada a m�todos de recep��o de input
    #region Inputs Methods
    // M�todo de recep��o de input para movimenta��o horizontal
    public void MovementAction(InputAction.CallbackContext value)
    {
        direction = value.ReadValue<float>();
    }

    // M�todo de recep��o de input para dash
    public void DashAction(InputAction.CallbackContext value)
    {
        if (value.performed && canMove)
        {
            Dash();
        }
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
        if (playerForms != PlayerForms.normal) Instantiate(transformationEffect, transform.position, transform.rotation).transform.SetParent(gameObject.transform);
        dead = true;
        anim.SetBool("Dead", dead);
        PausePlayerMovement(true);
        audioManager.StopMusic();
        audioManager.PlaySFX(sfx[0]);

        if (currentHealth > 0)
        {
            isInvencible = true;
            gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;
            StartCoroutine(Respawn());
        }
    }

    // Rotina para ressurgimento do player ap�s dano
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1f);
        GameManager.instance.FadeIn();
        yield return new WaitForSeconds(1f);
        SwitchPlayerForm(PlayerForms.normal);
        transform.SetParent(null);
        transform.position = respawnPoint;
        dead = false;
        anim.SetBool("Dead", dead);
        isInvencible = false;
        gameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;
        yield return new WaitForSeconds(1f);
        audioManager.PlayMusic();
        GameManager.instance.FadeOut();
        PausePlayerMovement(false);
    }

    protected override void Death()
    {
        GameManager.instance.SwitchGameToMainMenu();
    }

    // Rotina para sequ�ncia de morte
    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(2f);
    }
    #endregion
}
