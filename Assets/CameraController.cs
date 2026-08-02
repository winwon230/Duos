using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool camLock = false;
    private float netxRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void RotateCamera(float xRotation)
    {
        netxRotation -= xRotation;
        netxRotation = Mathf.Clamp(netxRotation, -70f, 70f);

        transform.localRotation = Quaternion.Euler(netxRotation, 0f, 0f);
        
    }
    
    public void Dive(float t)
    {
        // Kinda unsmooth
        Vector3 currentPos = transform.localPosition;

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


        transform.localPosition = currentPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            camLock = !camLock;
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
}
