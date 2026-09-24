using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BotMotor : MonoBehaviour
{
    // Start is called before the first frame update
    private CharacterController controller;
    private Animator animat;
    private float gravityMagnitude = 9.81f;
    private Vector3 Gravity;
    private float yVelocity = 0f;

    public GameObject volleyball;
    public GameObject Shadow;
    public gameManager gameManager;

    private GameObject nearestBall = null; // ref in hit ball function
    private float DistanceToBall; // ref in hit ball function

    private Vector3 storedDefaultPos;
    private Coroutine currentCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animat = GetComponent<Animator>();
    }

    public void setDefaultPos(Vector3 defaultPos)
    {
        storedDefaultPos = defaultPos;
    }

    public void PosBot()
    {
        transform.position = storedDefaultPos;
    }

    public void MoveBot(Vector3 Location)
    {
        Location.y = 0f;
        if(currentCoroutine != null && Vector3.Distance(transform.position, Location) < 0.15f)
        {
            return; // should just tell character not to move once we reach the location
        }

        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(Move(Location));
    }

    private IEnumerator Move(Vector3 Location)
    {
        float speed = 5f;
        float rotationSpeed = 5f;


        while(Vector3.Distance(transform.position, Location) > 0.15f)
        {
            Vector3 direction = Location - transform.position;
            direction.y = 0f;

            Vector3 moveDirection = direction.normalized;

            if(moveDirection != Vector3.zero)
            {

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                
            }

            controller.Move(moveDirection * speed * Time.deltaTime);
            yield return null;
        }

        currentCoroutine = null;
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

    public void HitBall(string HitType, string team, Vector3 direction, float hitMultiplier, string botPos)
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

                BallController ballScript = nearestBall.GetComponent<BallController>();

                DistanceToBall = Vector3.Distance(nearestBall.transform.position, transform.position);
                //Bump here
                if (DistanceToBall <= 1.5f && ballScript != null && controller.isGrounded && HitType == "Hit")
                {
                    transform.LookAt(direction);
                    ballScript.Bump(direction, hitMultiplier);
                    gameManager.updateLastPlayerTouch(team + botPos);
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
                    transform.LookAt(direction);

                    ballScript.Spike(direction, hitMultiplier);
                    gameManager.updateLastPlayerTouch(team + botPos);
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
                    transform.LookAt(direction);
                    ballScript.frontSet(direction);
                    gameManager.updateLastPlayerTouch(team + botPos);
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
        if(controller.velocity.magnitude == 0f)
        {
            animat.Play("Idle");
        }

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
