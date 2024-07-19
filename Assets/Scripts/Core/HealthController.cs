using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [Header("---Health Settings---")]
    [SerializeField] protected float maxHealth;
    protected float currentHealth;
    [HideInInspector] public bool dead;
    protected bool isInvencible;

    // Área destinada a variáveis externas para visual da vida
    [Header("Visual for Health")]
    [SerializeField] protected HealthBarScript healthBar;
    [SerializeField] protected TextMeshProUGUI lifeCounter;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
        if (lifeCounter != null) lifeCounter.text = maxHealth.ToString("0");
    }

    // Método destinado à aplicação de dano no objeto
    public virtual void TakeDamage(float _damage)
    {
        if (isInvencible) return;

        if (currentHealth > 0)
        {
            currentHealth = Mathf.Max(currentHealth - _damage, 0);
            DamageEffect();

            if(currentHealth == 0)
            {
                Death();
            }
        }
        else if (currentHealth == 0)
        {
            Death();
        }
    }

    // Método destinado à aplicação de cura no objeto
    public virtual void TakeHeal(float _heal)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + _heal, maxHealth);
        }
        if (healthBar != null) healthBar.SetHealth(currentHealth);
        if (lifeCounter != null) lifeCounter.text = currentHealth.ToString("0");
    }

    // Método destinado à aplicação de um efeito ao tomar dano
    protected virtual void DamageEffect()
    {
        if (healthBar != null) healthBar.SetHealth(currentHealth);
        if (lifeCounter != null) lifeCounter.text = currentHealth.ToString("0");
    }

    // Método destinado à função de morte do objeto
    protected virtual void Death()
    {

    }
}
