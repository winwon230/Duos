using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    public gameManager manager;
    public string targetType;
    // Start is called before the first frame update
    void Start()
    {
        if(targetType == "SERVETARGET")
        {
            transform.localScale = transform.localScale * 1.5f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Volleyball"))
        {
            manager.targetHit();
            Debug.Log("Registered hit");
        }

        if(targetType == "SPIKETARGET" || targetType == "SERVETARGET")
        {
            Vector3 pos = transform.position;
            pos.x = Random.Range(-4f, 2f);
            transform.position = pos;
            GameObject ball = other.gameObject;
            if(ball != null)
            {
                Destroy(ball, 1f);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
