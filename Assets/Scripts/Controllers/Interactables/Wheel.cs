using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : InteractableControl
{
    [Header("Wheel Variables")]
    [SerializeField] Transform powerUpSpawn;
    [SerializeField] GameObject[] powerUp;
    [SerializeField] bool spawnRandom;
    [SerializeField] int powerUpIndex;
    GameObject powerUpRef;

    public void SpinWheel()
    {
        canInteract = false;
        if (powerUp != null) Destroy(powerUpRef);
        if (PlayerPrefs.HasKey("HasSpined"))
        {
            if (anim != null) anim.SetTrigger("Spin");
        }
        else
        {
            dialogue.StartDialogue();
            PlayerPrefs.SetInt("HasSpined", 1);
        }
    }

    public void SpawnPowerUp()
    {
        if (spawnRandom) powerUpIndex = Random.Range(0, powerUp.Length);
        powerUpRef = Instantiate(powerUp[powerUpIndex], powerUpSpawn.position, powerUpSpawn.rotation);
        canInteract = true;
    }
}
