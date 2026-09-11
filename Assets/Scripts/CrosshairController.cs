using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{

    private float opacity;
    private Image image;
    private Color crosshairColour;
    private Color normalColour;
    // Start is called before the first frame update
    void Start()
    {
        opacity = 1f;
        image = GetComponent<Image>();
        crosshairColour = image.color;
        normalColour = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        Color c = crosshairColour;
        opacity = Mathf.MoveTowards(opacity, 1f, Time.deltaTime);

        c.a = opacity;
        image.color = c;
    }

    public void Cooldown()
    {
        opacity = 0.2f;
    }

    public void Green()
    {
        crosshairColour = Color.green;
        Invoke(nameof(normal), 0.2f);
    }

    public void normal()
    {
        crosshairColour = normalColour;
        Debug.Log("Back to normal");
    }

}
