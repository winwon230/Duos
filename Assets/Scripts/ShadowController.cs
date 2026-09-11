using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ShadowController : MonoBehaviour
{

    private Vector3 ballLocation;
    private float targetSize;
    private GameObject ball;
    private Renderer shadowRenderer;


    // Start is called before the first frame update
    void Start()
    {
        shadowRenderer = GetComponent<Renderer>();
        Color c = shadowRenderer.material.color;
        c.a = 0.8f; // Shadow transparency
        shadowRenderer.material.color = c;
    }

    public void sendBall(GameObject givenBall)
    {
        ball = givenBall;
    }

    // Update is called once per frame
    void Update()
    {
        ballLocation = ball.transform.position;

        if (ball.transform.position.y <= 6f && ball.transform.position.y > 0.4f)
        {
            targetSize = 0.22f + 0.4f * ball.transform.position.y;
        }

        else if (ball.transform.position.y > 6f)
        {
            targetSize = 2.62f;
        }

        else if (ball.transform.position.y <= 0.4f)
        {
            targetSize = 0.38f;
        }

        transform.localScale = new Vector3(targetSize, targetSize, 0f);

        ballLocation.y = 0.1f;
        transform.position = ballLocation;
    }
}
