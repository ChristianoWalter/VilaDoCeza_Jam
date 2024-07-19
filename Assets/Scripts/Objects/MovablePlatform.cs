using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    [Header("Moving Variables")]
    [SerializeField] Transform platform;
    [SerializeField] Transform[] movePoints;
    [SerializeField] float smoothMovement;
    [SerializeField] float timeToWait;
    bool switchDestiny;

    [HideInInspector] public bool canMove;
    Vector2 destiny;
    int index;

    private void Start()
    {
        if (movePoints.Length == 0) return;

        foreach (Transform pPoint in movePoints)
        {
            pPoint.SetParent(null);
        }

        index = 0;
        platform.position = movePoints[index].position;

        StartCoroutine(NewDestination());

    }

    private void Update()
    {
        if (Vector2.Distance(destiny, platform.position) <= 0.3f && switchDestiny)
        {
            switchDestiny = false;
            canMove = false;
            StartCoroutine(NewDestination());
        }
        if (canMove) platform.position = Vector2.MoveTowards(platform.position, destiny, smoothMovement * Time.deltaTime);
    }

    // Método para detectar colisão do player e fazê-lo se mover junto à plataforma
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }

    // Rotina para mudar trajetória da plataforma
    IEnumerator NewDestination()
    {
        yield return new WaitForSeconds(timeToWait);
        index++;
        if (index >= movePoints.Length) index = 0;
        destiny = movePoints[index].position;
        canMove = true;
        switchDestiny = true;
    }
}
