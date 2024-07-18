using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FindCamera : MonoBehaviour
{
    [SerializeField] BoxCollider2D col;
    [SerializeField] Canvas canvas;
    CameraController cam;

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null) cam = FindObjectOfType<CameraController>();

        

        if (col != null) cam.boundBox = col;
    }
}
