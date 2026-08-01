using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{

    private CharacterController controller;
    private Animator animat;
    private float gravityMagnitude = 9.81f;
    private Vector3 Gravity;
    private float yVelocity = 0f;
    private float DistanceToBall;

    public GameObject volleyball;
    public Transform fpvCamTransform;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animat = GetComponent<Animator>();
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

    public void HitBall()
    {
        GameObject[] AllBalls = GameObject.FindGameObjectsWithTag("Volleyball");
        GameObject nearestBall = null;
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

                float DistanceToBall = HitInfo.distance;

                //Bump here
                if (DistanceToBall <= 1f && ballScript != null && controller.isGrounded)
                {
                    ballScript.Bump(transform.forward);
                }
                //Put Spike below

                if (DistanceToBall <= 2f && ballScript != null && controller.isGrounded == false)
                {
                    Vector3 SpikeDirection = fpvCamTransform.transform.forward;

                    ballScript.Spike(SpikeDirection);
                }

            }

        }
    }

    public void Dive()
    {
 
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
