using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanInput : MonoBehaviour
{

    private float mouseSensitivity = 5f;
    private float JumpHeight = 2f;
    private float nextHitTime = 0f;
    private float nextDiveTime = 0f;
    private string pos;
    public CrosshairController crosshairController;
    public CharacterMotor myMotor;
    [SerializeField] Camera cam;
    [SerializeField] CameraController camcon;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void setClass(string startPos)
    {
        if(startPos == "R")
        {
            Vector3 defaultPos = new Vector3(-2.09f, 0.075f, 11.99f);
            myMotor.setDefaultPos(defaultPos);
            pos = "R";
        }

        if(startPos == "L")
        {
            Vector3 defaultPos = new Vector3(2.09f, 0.075f, 11.99f);
            myMotor.setDefaultPos(defaultPos);
            pos = "L";
        }
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");
        float yRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        float xRotation = Input.GetAxis("Mouse Y") * mouseSensitivity;

        Vector3 MoveDirection = new Vector3(xInput, 0f, zInput);
        Vector3 RelativeMoveDirection = transform.TransformDirection(MoveDirection);
        Vector3 PlayerRotation = new Vector3(0f, yRotation, 0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            myMotor.Jump(JumpHeight);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Vector3 Pos = transform.position;
            myMotor.SpawnBall(Pos);

            Debug.Log("Registered");
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) // Hitting including spiking/bumping and cooldown timer - links to CharacterMotor
        {
            float HitCooldown = 1f;

            if (Time.time >= nextHitTime)
            {
                myMotor.HitBall("Hit");
                crosshairController.Cooldown();
                nextHitTime = Time.time + HitCooldown;
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Q)) // Diving + cooldown timer
        {

            float DiveCooldown = 1f;

            if(Time.time >= nextDiveTime)
            {
                myMotor.TriggerDive();
                nextDiveTime = Time.time + DiveCooldown;
            }

        }

        if(Input.GetKeyDown(KeyCode.F)) // Front set
        {
            float HitCooldown = 1f;

            if(Time.time >= nextHitTime)
            {
                myMotor.HitBall("Front Set");
                crosshairController.Cooldown();
                nextHitTime = Time.time + HitCooldown;
                Debug.Log("Front Set step 1");
            }
        }
        myMotor.RotateCharacter(PlayerRotation);
        myMotor.MoveCharacter(RelativeMoveDirection);
        camcon.RotateCamera(xRotation);
    }

}
