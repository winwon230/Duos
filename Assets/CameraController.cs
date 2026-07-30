using System.Collections;
using System.Collections.Generic;
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
