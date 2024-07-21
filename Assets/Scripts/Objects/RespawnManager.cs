using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    // Variáveis para controle do spawn
    [HideInInspector] public bool activeSpawn;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject spawnSetEffect;
    [SerializeField] Animator anim;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !activeSpawn)
        {
            other.gameObject.GetComponent<PlayerController>().respawnPoint = spawnPoint.position;
            anim.SetTrigger("PointSaved");
            GameManager.instance.ChangeCurrentSpawn(Instantiate(spawnSetEffect, spawnPoint.position, spawnPoint.rotation), gameObject.GetComponent<RespawnManager>());
            activeSpawn = true;
        }
    }

}
