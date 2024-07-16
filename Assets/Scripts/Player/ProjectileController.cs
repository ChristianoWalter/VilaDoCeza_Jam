using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Triggering the projectile")]
    [SerializeField] float projectileSpeed;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float damage;

    [HideInInspector] public Vector2 direction;

    [Header("Projectile effects")]
    [SerializeField] GameObject impactEffect;
    [SerializeField] Animator anim;


    private void Start()
    {
        //transform.localScale = new Vector2(direction.x, 1f);
        rb.velocity = direction * projectileSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(direction.x * projectileSpeed, rb.velocity.y);
    }

    // Destruindo o tiro ao encostar em outro objeto
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<HealthController>()?.TakeDamage(damage);
        }

        //efeito do impacto do tiro
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
