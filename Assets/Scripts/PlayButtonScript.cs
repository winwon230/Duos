using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonScript : MonoBehaviour
{
    private float f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        f += Time.deltaTime;
        float scaleFactor = 0.005f * Mathf.Sin(f * 1.5f) + 0.35f;
        transform.localScale = new Vector3 (scaleFactor, scaleFactor, scaleFactor);
    }
}
