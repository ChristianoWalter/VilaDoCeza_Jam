using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : HealthController
{
    [Header("---Enemy Settings---")]
    [SerializeField] float damage;

    // Área destinada à variáveis de movimentação
    [Header("Movement variables")]
    public bool canMove;
    [SerializeField] Transform[] walkPoints;
    int currentPoint;
    [SerializeField] float moveSpeed;
    [SerializeField] float waitForPoints;
    float waitCounter;

    // Área destinada a componentes exernos do inimigo
    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator enemyAnim;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        waitCounter = waitForPoints;

        foreach (Transform pPoint in walkPoints)
        {
            pPoint.SetParent(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;

        if (Mathf.Abs(transform.position.x - walkPoints[currentPoint].position.x) > .2)
        {
            if (transform.position.x < walkPoints[currentPoint].position.x)
            {
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;

            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0)
            {
                waitCounter = waitForPoints;

                currentPoint = currentPoint + 1;

                if (currentPoint >= walkPoints.Length)
                {
                    currentPoint = 0;
                }
            }
        }

        if ((rb.velocity.x > 0 && transform.localScale.x < 0) || (rb.velocity.x < 0 && transform.localScale.x > 0))
        {
            Vector2 _localScale = transform.localScale;
            _localScale.x *= -1f;
            transform.localScale = _localScale;
        }
    }

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
        foreach (Transform pPoint in walkPoints)
        {
            pPoint.SetParent(gameObject.transform);
        }
        Destroy(gameObject);
    }
}
