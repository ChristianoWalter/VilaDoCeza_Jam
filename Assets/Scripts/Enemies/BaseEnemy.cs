using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : HealthController
{
    [Header("---Enemy Settings---")]
    [SerializeField] float damage;


    // M�todo destinado a detec��o de colis�o
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<HealthController>().TakeDamage(damage);
        }
    }

    // Reescritura do m�todo de morte
    protected override void Death()
    {
        Destroy(gameObject);
    }
}
