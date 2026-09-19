using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMotor : MonoBehaviour
{
    // Start is called before the first frame update
    private CharacterController controller;
    private Animator animat;
    private CameraController cameraScript;
    private float gravityMagnitude = 9.81f;
    private Vector3 Gravity;
    private float yVelocity = 0f;

    public GameObject volleyball;
    public GameObject Shadow;
    public gameManager gameManager;

    private GameObject nearestBall = null; // ref in hit ball function
    private float DistanceToBall; // ref in hit ball function

    private Vector3 storedDefaultPos;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animat = GetComponent<Animator>();
    }

    public void setDefaultPos(Vector3 defaultPos)
    {
        storedDefaultPos = defaultPos;
        Debug.Log("DefaultPosSet");
    }

    public void PosBot()
    {
        transform.position = storedDefaultPos;
        Debug.Log("positioned");
    }

    public void MoveBot(Vector3 Location)
    {
        float speed = 5f;
        float rotationSpeed = 5f;

        while(transform.position != Location)
        {
            Vector3 direction = Location - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            controller.Move(transform.forward * speed * Time.deltaTime);
        }

    }

    public void RotateBot(Vector3 inputRotation)
    {
        float rotationSpeed = 5f;
        Quaternion targetRotation = Quaternion.Euler(inputRotation);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void Jump(float JumpHeight)
    {
        if (controller.isGrounded == true)
        {
            float Power = Mathf.Sqrt(JumpHeight * 1.7f * gravityMagnitude);
            yVelocity = Power;
            animat.Play("jump");

        }
    }

/*
    public void SpawnBall(Vector3 Pos) // This needs to be a serving function
    {

        GameObject SpawnedBall = Instantiate(volleyball, Pos + transform.forward, Quaternion.identity);
        Rigidbody rbBall = SpawnedBall.GetComponent<Rigidbody>();
        GameObject SpawnedShadow = Instantiate(Shadow, Pos + transform.forward, Quaternion.Euler(90f, 0f, 0f));

        ShadowController shadowScript = SpawnedShadow.GetComponent<ShadowController>();
        BallController ballScript = SpawnedBall.GetComponent<BallController>();

        ballScript.setRotation(controller);
        shadowScript.sendBall(SpawnedBall);

        if (rbBall != null)
        {
            float TossForce = 2.65f;
            rbBall.AddForce(Vector3.up * TossForce, ForceMode.Impulse);
        }
    }
    */

    public void HitBall(string HitType, string team)
    {
        GameObject[] AllBalls = GameObject.FindGameObjectsWithTag("Volleyball");
        float shortestDistance = Mathf.Infinity;


        foreach (GameObject ball in AllBalls)
        {
            DistanceToBall = Vector3.Distance(transform.position, ball.transform.position);

            if(DistanceToBall < shortestDistance)
            {
                shortestDistance = DistanceToBall;
                nearestBall = ball;
            }

        }

        if (nearestBall != null)
        {
            Vector3 Origin = transform.position + Vector3.up * 1f;
            Vector3 RayDirection = nearestBall.transform.position - Origin;
            float MaxRaycastDistance = 50f;

            RaycastHit HitInfo;

            if(Physics.Raycast(Origin, RayDirection, out HitInfo, MaxRaycastDistance))
            {
                BallController ballScript = HitInfo.collider.GetComponent<BallController>();

                DistanceToBall = HitInfo.distance;

                //Bump here
                if (DistanceToBall <= 1.5f && ballScript != null && controller.isGrounded && HitType == "Hit")
                {
                    ballScript.Bump(transform.forward);
                    if(team == "T1")
                    {
                        gameManager.Touch("T1");
                    }

                    else if(team == "T2")
                    {
                        gameManager.Touch("T2");
                    }
                }
                //Put Spike below

                if (DistanceToBall <= 2f && ballScript != null && controller.isGrounded == false && HitType == "Hit")
                {
                    Vector3 SpikeDirection = Camera.main.transform.forward;

                    ballScript.Spike(SpikeDirection);
                    if(team == "T1")
                    {
                        gameManager.Touch("T1");
                    }

                    else if(team == "T2")
                    {
                        gameManager.Touch("T2");
                    }
                }

                // Front set below

                if(DistanceToBall <= 1.5f && ballScript != null && HitType == "Front Set")
                {
                    ballScript.frontSet(transform.forward);
                    if(team == "T1")
                    {
                        gameManager.Touch("T1");
                    }

                    else if(team == "T2")
                    {
                        gameManager.Touch("T2");
                    }
                }



            }

        }
    }


    public void TriggerDive(string team)
    {
        if(controller.isGrounded == true)
        {
            StartCoroutine(Dive()); 
        }

        CheckDiveHit(team);
    }
        private IEnumerator Dive()
    {

        float DiveDistance = 4f;
        float DiveDuration = 0.5f;
        float TimePassed = 0f;

        Vector3 StartingPosition = transform.position;
        Vector3 TargetPosition = StartingPosition + transform.forward * DiveDistance;

        while(TimePassed < DiveDuration)
        {
            TimePassed += Time.deltaTime;
            float t = TimePassed/DiveDuration;

            transform.position = Vector3.Lerp(StartingPosition, TargetPosition, t);

            yield return null;
        }

    }

    public void CheckDiveHit(string team)
    {
        Vector3 boxCentre = transform.position + transform.forward * 1.5f + transform.up * 1.25f;
        Vector3 boxSize = new Vector3(2f, 2f, 3f) / 2f;

        Collider [] inBoxColliders = Physics.OverlapBox(boxCentre, boxSize, transform.rotation);

        foreach (Collider hit in inBoxColliders)
        {
            if(hit.CompareTag("Volleyball"))
            {
                BallController ballScript = hit.GetComponent<BallController>();
                ballScript.DigBall();

                if(team == "T2")
                {
                    gameManager.Touch("T2");
                }

                else if(team == "T1")
                {
                    gameManager.Touch("T1");
                }
            }
        }

    }

    void OnDrawGizmos()
    {
        // Basically describes where the hitbox area for the dive needs to be
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Vector3 localCentre = new Vector3(0f, 1.25f, 1.5f);
        Vector3 size = new Vector3(2f, 2f, 3f);
        Gizmos.DrawWireCube(localCentre, size);
    }
    // Update is called once per frame
    void Update()
    {


        if(controller.isGrounded == true && yVelocity < 0f)
        {
            yVelocity = -2f;
        }

        else
        {
            yVelocity -= gravityMagnitude * Time.deltaTime;
        }

        Gravity = new Vector3(0f, yVelocity * Time.deltaTime, 0f);
        controller.Move(Gravity);
    }
}
