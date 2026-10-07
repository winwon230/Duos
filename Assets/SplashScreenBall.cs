using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenBall : MonoBehaviour
{
    public float xVelocity; //-0.005f used
    public float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if(pos.x < -15f)
        {
            pos.x = 15f;
        }

        else
        {
            pos.x += xVelocity;
        }
        transform.position = pos;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

    }
}
