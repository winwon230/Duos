using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooberSplashScript : MonoBehaviour
{
    public float xVelocity; // use 0.005f or smth close to that
    private Animator animat;
    // Start is called before the first frame update
    void Start()
    {
        animat = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if(pos.x > 15f)
        {
            pos.x = -15f;
        }

        else
        {
            pos.x += xVelocity;
        }
        animat.Play("running");
        transform.position = pos;
    }
}
