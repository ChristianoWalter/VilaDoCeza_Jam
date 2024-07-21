using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxControl : MonoBehaviour
{
    Vector2 length;
    Vector2 startPos;
    Transform cam;

    [SerializeField] Vector2 parallaxEffect;
    [SerializeField] bool repeat = true;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size;
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 restPos = cam.position * (new Vector2(1, 1) - parallaxEffect);
        Vector2 distance = cam.transform.position * parallaxEffect;
        transform.position = startPos + distance;

        if (!repeat) return;
        if (restPos.x > startPos.x + length.x)
        {
            startPos.x += length.x;
        }
        else if (restPos.x < startPos.x - length.x)
        {
            startPos.x -= length.x;
        }

        if (restPos.y > startPos.y + length.y)
        {
            startPos.y += length.y;
        }
        else if (restPos.y < startPos.y - length.y)
        {
            startPos.y -= length.y;
        }
    }
}
