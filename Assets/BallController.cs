using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    private Rigidbody rb;
    private Quaternion reqRotation;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();

    }


    public void setRotation(CharacterController controller)
    {
        reqRotation = Quaternion.Euler(Vector3.up * controller.transform.eulerAngles.y);
        transform.rotation = reqRotation;
    }

    public void Bump(Vector3 HitDirection)
    {
        float BumpForceZ = 0.5f;
        float BumpForceY = 2f;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 ForwardPush = HitDirection * BumpForceZ;
        Vector3 UpwardPush = Vector3.up * BumpForceY;
        Vector3 finalForce = ForwardPush + UpwardPush;

        rb.AddForce(finalForce, ForceMode.Impulse);
    }

    public void Spike(Vector3 SpikeDirection)
    {
        float spikeForce = 2.5f;

        rb.AddForce(SpikeDirection * spikeForce, ForceMode.Impulse);
    }

    public void frontSet(Vector3 SetDirection)
    {
        float setForceX = 0.3f;
        float setForceY = 2.5f;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 ForwardPush = SetDirection * setForceX;
        Vector3 UpwardPush = Vector3.up * setForceY;
        Vector3 finalForce = ForwardPush + UpwardPush;

        rb.AddForce(finalForce, ForceMode.Impulse);
        Debug.Log("Front Set step 3");

    }

    public void DigBall()
    {
        float digPower = 2f;
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * digPower, ForceMode.Impulse);
        Debug.Log("Dive Hit registered");
    }


    // Update is called once per frame
    void Update()
    {

    }
}
