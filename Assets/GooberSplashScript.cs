using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooberSplashScript : MonoBehaviour
{
    private Animator animat;
    // Start is called before the first frame update
    void Start()
    {
        animat = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        animat.Play("running");
    }
}
