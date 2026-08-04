using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{

    private CharacterController controller;
    private Animator animat;
    private CameraController cameraScript;
    private float gravityMagnitude = 9.81f;
    private Vector3 Gravity;
    private float yVelocity = 0f;

    public GameObject volleyball;
    public Transform fpvCamTransform;

    private GameObject nearestBall = null; // ref in hit ball function
    private float DistanceToBall; // ref in hit ball function

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animat = GetComponent<Animator>();
        cameraScript = Camera.main.GetComponent<CameraController>();
    }


    public void MoveCharacter(Vector3 RelativeMoveDirection)
    {

        float speed = 5f;
        if (RelativeMoveDirection.magnitude >= 0.1f)
        {
            controller.Move(RelativeMoveDirection * speed * Time.deltaTime);
            animat.Play("running");
        }

        else if (RelativeMoveDirection.magnitude <= 0.1f)
        {
            animat.Play("Idle");
        }

    }

    public void RotateCharacter(Vector3 PlayerRotation)
    {
        transform.Rotate(PlayerRotation);
    }

    public void Jump(float JumpHeight)
    {
        if (controller.isGrounded == true)
        {
            float Power = Mathf.Sqrt(JumpHeight * 1.5f * gravityMagnitude);
            yVelocity = Power;
            animat.Play("Jump");

            Debug.Log("Deployed jump");
        }
    }

    public void SpawnBall(Vector3 Pos)
    {

        GameObject SpawnedBall = Instantiate(volleyball, Pos + transform.forward, Quaternion.identity);
        Rigidbody rbBall = SpawnedBall.GetComponent<Rigidbody>();

        BallController ballScript = SpawnedBall.GetComponent<BallController>();

        ballScript.setRotation(controller);

        if (rbBall != null)
        {
            float TossForce = 2.65f;
            rbBall.AddForce(Vector3.up * TossForce, ForceMode.Impulse);
        }
    }

    public void HitBall(string HitType)
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
                }
                //Put Spike below

                if (DistanceToBall <= 2f && ballScript != null && controller.isGrounded == false && HitType == "Hit")
                {
                    Vector3 SpikeDirection = fpvCamTransform.transform.forward;

                    ballScript.Spike(SpikeDirection);
                }

                // Front set below

                if(DistanceToBall <= 1.5f && ballScript != null && HitType == "Front Set")
                {
                    ballScript.frontSet(transform.forward);
                    Debug.Log("Front Set Step 2 succesful");
                }



            }

        }
    }


    public void TriggerDive()
    {
        if(controller.isGrounded == true)
        {
            StartCoroutine(Dive()); 
        }

        CheckDiveHit();
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

            cameraScript.Dive(t);

            yield return null;
        }

    }

    public void CheckDiveHit()
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
