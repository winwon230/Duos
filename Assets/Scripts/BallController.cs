using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private BotInput OpponentAI1Input;
    private BotInput OpponentAI2Input;
    private TeammateInput teammateInput;
    private CrosshairController crosshair;
    private Rigidbody rb;
    private Quaternion reqRotation;
    private gameManager gameManager;
    private bool justHit;
    private float groundDetectionTimer = 1.5f;
    //public GameObject locator; locator is ugly

    // Start is called before the first frame update
    void Awake()
    {
        justHit = false;
        rb = GetComponent<Rigidbody>();        
    }
    void Start()
    {
        crosshair = FindFirstObjectByType<CrosshairController>();

        GameObject bot1 = GameObject.Find("Opponent AI 1");
        OpponentAI1Input = bot1.GetComponent<BotInput>();

        GameObject bot2 = GameObject.Find("Opponent AI 2");
        OpponentAI2Input = bot2.GetComponent<BotInput>();

        GameObject teammate = GameObject.Find("Teammate AI");
        teammateInput = teammate.GetComponent<TeammateInput>();
        gameManager = FindFirstObjectByType<gameManager>();
    }


    public void setRotation(CharacterController controller)
    {
        reqRotation = Quaternion.Euler(Vector3.up * controller.transform.eulerAngles.y);
        transform.rotation = reqRotation;
    }

    public void Bump(Vector3 HitDirection, float hitMultiplier)
    {
        float BumpForceZ = 0.5f;
        float BumpForceY = 2f;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 ForwardPush = HitDirection.normalized * BumpForceZ;
        Vector3 UpwardPush = Vector3.up * BumpForceY;
        Vector3 finalForce;

        if(hitMultiplier == 1.6f) // this is specifically bc the over shot goes too high
        {
            finalForce = hitMultiplier * 1.3f * ForwardPush + hitMultiplier * 0.8f * UpwardPush;
        }

        else
        {
            finalForce = hitMultiplier * (ForwardPush + UpwardPush);
        }

        rb.AddForce(finalForce, ForceMode.Impulse);
        justHit = true;
    }

    public void Spike(Vector3 SpikeDirection, float hitMultiplier)
    {
        rb.velocity = Vector3.zero;
        float spikeForce = 3f * hitMultiplier;

        rb.AddForce(SpikeDirection.normalized * spikeForce, ForceMode.Impulse);
        justHit = true;
    }

    public void Serve(Vector3 ServeDirection, float hitMultiplier, string serveType)
    {
        rb.velocity = Vector3.zero;

        if(serveType == "Flat")
        {
            rb.AddForce(ServeDirection.normalized * hitMultiplier, ForceMode.Impulse);   
        }
        justHit = true;
    }

    public void frontSet(Vector3 SetDirection, float hitMultiplier)
    {
        float setForceX = 0.5f;
        float setForceY = 2.65f;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 ForwardPush = SetDirection.normalized * setForceX;
        Vector3 UpwardPush = Vector3.up * setForceY;
        Vector3 finalForce = ForwardPush + UpwardPush;

        rb.AddForce(finalForce * hitMultiplier, ForceMode.Impulse);
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

        if(finalPos.z > 17.31f && finalPos.z < 26.32f && finalPos.x < 4.5f && finalPos.x > -4.5f)
        {
            OpponentAI1Input.sendBallPos(finalPos);
            OpponentAI2Input.sendBallPos(finalPos);
        }

        if(finalPos.z > 8.51f && finalPos.z < 17.31f && finalPos.x < 4.5f && finalPos.x > -4.5f)
        {
            teammateInput.sendBallPos(finalPos);
        }
        //GameObject spawnedLocator = Instantiate(locator, finalPos, Quaternion.identity);
        //Destroy(spawnedLocator, 3f);
    }

    private void OnCollisionEnter(Collision collision) // need to code for if the ball is out as well
    {
        if (collision.gameObject.CompareTag("VolleyballCourtCollider") && groundDetectionTimer <= 0f)
        {
            if(transform.position.z < 17.31f && transform.position.z > 8.323f) // ball is IN at T1 side
            {
                gameManager.ballGroundHit("T1" , "VolleyballCourt");
            }
            else if(transform.position.z > 17.31f && transform.position.z < 26.297f) //ball is IN at T2 side
            {
                gameManager.ballGroundHit("T2", "VolleyballCourt");
            }
            groundDetectionTimer = 1.5f;
        }

        else if(collision.gameObject.CompareTag("Ground") && groundDetectionTimer <= 0f)
        {
            if(transform.position.z < 17.31f) // ball is OUT at T1 side
            {
                gameManager.ballGroundHit("T1", "Ground");
            }
            else if(transform.position.z > 17.31f) //ball is OUT at T2 side
            {
                gameManager.ballGroundHit("T2", "Ground");
            }    
            groundDetectionTimer = 1.5f;        
        }

        else
        {
            return;
        }
    }
    // Update is called once per frame
    void Update()
    {
        groundDetectionTimer -= Time.deltaTime;
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
