using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CameraController : MonoBehaviour
{
    public GameObject crosshair;
    private bool camLock = false;
    private float netxRotation = 0f;
    private Camera currentCam;
    private Quaternion xRotationQuaternion;
    public Camera Fpv;
    //public Camera Tpv;
    private HumanInput playerInput;
    private BallController ball;
    private GameObject nearestBall;
    private float closestDistance;

    // Start is called before the first frame update
    void Start()
    {
        //Tpv.enabled = false;
        Fpv.enabled = true;

        currentCam = Fpv;
        playerInput = FindFirstObjectByType<HumanInput>();
    }

    public void RotateCamera(float xRotation)
    {
        if(currentCam == Fpv)
        {
        netxRotation -= xRotation;
        
        if(currentCam == Fpv)
        {
            netxRotation = Mathf.Clamp(netxRotation, -70f, 70f); 
        }

        //else if(currentCam == Tpv)
        {
            netxRotation = Mathf.Clamp(netxRotation, -52f, 70f);
        }

        currentCam.transform.localRotation = Quaternion.Euler(netxRotation, 0f, 0f);
        xRotationQuaternion = Quaternion.Euler(netxRotation, 0f, 0f);            
        }
        
    }
    
    public void Dive(float t)
    {
        if(Fpv.enabled == true)
        {
        // Kinda unsmooth
        Vector3 currentPos = Fpv.transform.localPosition;//dive should only be for fpv

        if (t <= 0.3f)
        {
            currentPos.y = 1.838f - 5.3f * t;
        }

        if (t > 0.3f && t <= 0.8f)
        {
            currentPos.y = 0.238f;
        }

        if (t > 0.8f && t <= 1.0f)
        {
            currentPos.y = 1.838f - 8f * (1f - t);
        }


        Fpv.transform.localPosition = currentPos;
        }
    }

    public void toggleCam()
    {
        /*if(Fpv.enabled == true)
        {
            Fpv.enabled = false;
            Tpv.enabled = true;
            currentCam = Tpv;
            crosshair.SetActive(false);
        } */

        /*else if(Tpv.enabled == true)
        {
            Fpv.enabled = true;
            Tpv.enabled = false;
            currentCam = Fpv;
            crosshair.SetActive(true);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if(Fpv.enabled == true)
        {
            currentCam = Fpv;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            camLock = !camLock;
        }

        /*if (Input.GetKeyDown(KeyCode.C))
        {
            toggleCam();
        }*/

        if (camLock == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void LateUpdate()
    {
        /*
        if(Tpv.enabled == true)
        {
            crosshair.SetActive(false);
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Volleyball");
            closestDistance = Mathf.Infinity;
            foreach(GameObject ball in balls)
            {
                float distanceToBall = Vector3.Distance(transform.position, ball.transform.position);
                if(distanceToBall < closestDistance)
                {
                    closestDistance = distanceToBall;
                    nearestBall = ball;
                }
            }

            Vector3 pos = Tpv.transform.position;
            //add warning to turn up cursor sensitivity when using tpv
            currentCam = Tpv;
            if(nearestBall != null)
            {
                float targetY = 3f + 0.5f*(nearestBall.transform.position.y - 3f);
                targetY = Mathf.Max(3f, targetY);
                pos.y = Mathf.Lerp(pos.y, targetY, 2f* Time.deltaTime);//3f = smooth speed
                //pos.x = playerInput.transform.position.x;
                //pos.z = playerInput.transform.position.z - 4f;
                //Tpv.transform.position = pos;
            }
            else
            {
                pos.y = 3f;         
            }
                Vector3 offset = new Vector3(0f, 0f, -4f);
                pos.x = playerInput.transform.position.x + offset.x;
                pos.z = playerInput.transform.position.z + offset.z;
            Tpv.transform.position = pos;
            Vector3 playerPos = playerInput.transform.position;
            Vector3 ballPos = nearestBall.transform.position;
            Vector3 target = (playerPos + ballPos) / 2f;
            Tpv.transform.LookAt(target);
                Vector3 playerPos = playerInput.transform.position;
                
                Quaternion yRotation = Quaternion.Euler(0f, playerInput.transform.eulerAngles.y, 0f);
                Vector3 offset = yRotation * new Vector3(0f, 0f, -4f);
                pos.x = playerPos.x + offset.x;
                pos.z = playerPos.z + offset.z;
                Tpv.transform.position = pos;       
        }

            //Vector3 tpvPos = Tpv.transform.position;

            if(Tpv.transform.position.y < 0.1f)
            {
                tpvPos.y = 0.1f;
                Tpv.transform.position = tpvPos;
            } // used in the old system that was bad like roblox camera
        */
    }
}
