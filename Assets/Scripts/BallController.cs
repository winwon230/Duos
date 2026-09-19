using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private CrosshairController crosshair;
    private Rigidbody rb;
    private Quaternion reqRotation;
    private bool justHit;
    //public GameObject locator; locator is ugly

    // Start is called before the first frame update
    void Start()
    {
        justHit = false;
        rb = GetComponent<Rigidbody>();
        crosshair = FindFirstObjectByType<CrosshairController>();

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
        justHit = true;
    }

    public void Spike(Vector3 SpikeDirection)
    {
        rb.velocity = Vector3.zero;
        float spikeForce = 3f;

        rb.AddForce(SpikeDirection * spikeForce, ForceMode.Impulse);
        justHit = true;
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
        justHit = true;

    }

    public void DigBall()
    {
        float digPower = 2f;
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * digPower, ForceMode.Impulse);

        justHit = true;
    }

    public void CaclulatePos()
    {
        Vector3 originalPos = transform.position;
        float u = rb.velocity.y;
        float a = Physics.gravity.y;
        float s = 0.7f - transform.position.y;

        float A = 0.5f * a;
        float B = u;
        float C = -s;

        float discriminant = B * B - 4f * A * C;

        float t1 = (-B + Mathf.Sqrt(discriminant)) / (2f * A);
        float t2 = (-B - Mathf.Sqrt(discriminant)) / (2f * A);

        float t;

        if (t1 > 0f && t2 > 0f)
        {
            t = Mathf.Max(t1, t2);
        }

        else if (t1 > 0f && t2 < 0f)
        {
            t = t1;
        }

        else if (t1 < 0f && t2 > 0f)
        {
            t = t2;
        }

        else
        {
            Debug.Log("didn't work");
            t = -4f;
        }

        float vx = rb.velocity.x;
        float sx = vx * t;

        float vz = rb.velocity.z;
        float sz = vz * t;

        Vector3 finalPos = Vector3.zero;

        finalPos.x = originalPos.x + sx;
        finalPos.z = originalPos.z + sz;
        finalPos.y = 0.075f;

        //GameObject spawnedLocator = Instantiate(locator, finalPos, Quaternion.identity);
        //Destroy(spawnedLocator, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        float height = transform.position.y;
        if(Mathf.Abs(height - 5.4f) < 0.1f)
        {
            crosshair.Green();
        }
    }

    void FixedUpdate()
    {
        if (justHit && rb.velocity.y != 0f)
        {
            CaclulatePos(); 
            justHit = false;
        }

    }
}
