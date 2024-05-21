using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBG : MonoBehaviour
{
    public float speed = 5f;
    public float clampPos;
    private Vector3 startpos;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newpos = Mathf.Repeat(Time.time * speed, clampPos);
        transform.position = startpos + Vector3.left * newpos;
    }
}
