using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UnavailableFeatureScript : MonoBehaviour
{
    private Image image;
    private float decreaseRate = 0.5f;
    private Color colour;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        colour = image.color;
        colour.a = 0f;
        image.color = colour;
    }

    public void showInactiveFeature()
    {
        colour.a = 1f;
        image.color = colour;
    }
    // Update is called once per frame
    void Update()
    {
        if(colour.a > 0f)
        {
            colour.a -= decreaseRate * Time.deltaTime;
            image.color = colour;
        }
    }
}
