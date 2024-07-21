using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FindCamera : MonoBehaviour
{
    [SerializeField] BoxCollider2D col;
    CameraController cam;

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null) cam = FindObjectOfType<CameraController>();
        if (col != null) cam.boundBox = col;
    }

    private void Update()
    {
        if (cam == null)
        {
            cam = FindObjectOfType<CameraController>();
            cam.boundBox = col;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerController>()?.TakeDamage(1);
        }
    }
}
