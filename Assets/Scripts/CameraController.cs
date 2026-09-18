using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CameraController : MonoBehaviour
{
    private bool camLock = false;
    private float netxRotation = 0f;
    private Camera currentCam;
    private Quaternion xRotationQuaternion;
    public Camera Fpv;
    public Camera Tpv;

    // Start is called before the first frame update
    void Start()
    {
        Tpv.enabled = false;
        Fpv.enabled = true;

        currentCam = Fpv;
    }

    public void RotateCamera(float xRotation)
    {
        netxRotation -= xRotation;
        
        if(currentCam == Fpv)
        {
            netxRotation = Mathf.Clamp(netxRotation, -70f, 70f); 
        }

        else if(currentCam == Tpv)
        {
            netxRotation = Mathf.Clamp(netxRotation, -52f, 70f);
        }

        currentCam.transform.localRotation = Quaternion.Euler(netxRotation, 0f, 0f);
        xRotationQuaternion = Quaternion.Euler(netxRotation, 0f, 0f);
        
    }
    
    public void Dive(float t)
    {
        if(Tpv.enabled == false && Fpv.enabled == true)
        {
        // Kinda unsmooth
        Vector3 currentPos = currentCam.transform.localPosition;

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
        if(Fpv.enabled == true)
        {
            Fpv.enabled = false;
            Tpv.enabled = true;
            currentCam = Tpv;
        } 

        else if(Tpv.enabled == true)
        {
            Fpv.enabled = true;
            Tpv.enabled = false;
            currentCam = Fpv;
        }
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

        if (Input.GetKeyDown(KeyCode.P))
        {
            toggleCam();
        }

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
        if(Tpv.enabled == true)
        {
            currentCam = Tpv;
            Vector3 CurrentPos = transform.position;
            Vector3 TpvOffset = xRotationQuaternion * new Vector3(0f, 0f, -3.33f);
            TpvOffset.y += 1.8f;
            Tpv.transform.position = CurrentPos + transform.TransformDirection(TpvOffset);

            Vector3 tpvPos = Tpv.transform.position;

            if(Tpv.transform.position.y < 0.1f)
            {
                tpvPos.y = 0.1f;
                Tpv.transform.position = tpvPos;
            }
        }
    }
}
