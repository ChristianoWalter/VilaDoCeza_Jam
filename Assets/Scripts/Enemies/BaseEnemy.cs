using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : HealthController
{
    [Header("---Enemy Settings---")]
    [SerializeField] float damage;


    // Método destinado a detecção de colisão
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<HealthController>().TakeDamage(damage);
        }
    }

    // Reescritura do método de morte
    protected override void Death()
    {
        Destroy(gameObject);
    }
}
