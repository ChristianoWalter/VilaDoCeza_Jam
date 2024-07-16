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
    [SerializeField] LayerMask enemiesMask;

    // Vari�veis de combate a curta dist�ncia
    [Header("Melee combat vaiables")]
    public bool canMeleeAttack;
    [SerializeField] HabilitiesUIControl meleeUI;
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeReloadTime;
    float timeToMeleeReload;
    
    // Vari�veis de combate a longa dist�ncia
    [Header("Ranged combat vaiables")]
    public bool canRangedAttack;
    [SerializeField] HabilitiesUIControl rangedUI;
    [SerializeField] GameObject projectile;
    //[SerializeField] float rangedDamage;
    [SerializeField] float rangedReloadTime;
    float timeToRangedReload;

    // Componentes externos que ser�o atrelados ao script externamente
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

    // M�todo destinada � entrada de colis�o no player
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemys Head")
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

    // Regi�o destinada � m�todos e fun��es de ataque
    #region Attack Methods
    // M�todo para execu��o de ataques � dist�nca
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

    // M�todo para execu��o de recarga do ataque � dist�ncia
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

    // M�todo para execu��o de ataques � curta dist�nca
    public void MeleeAttack()
    {
        if (canMeleeAttack && timeToMeleeReload == 0)
        {

            timeToMeleeReload = meleeReloadTime;
            StartCoroutine (MeleeReload());
        }
    }

    // M�todo para execu��o de recarga do ataque � curta dist�ncia
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
        if (value.performed)
        {
            RangedAttack();
        }
    }
    
    // M�todo para recep��o de input para execu��o de pulos
    public void JumpAction(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            if(OnGround()) Jump(jumpForce);
        }
    }
    #endregion

}
