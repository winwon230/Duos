using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{

    private float opacity;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        opacity = 1f;
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Color c = image.color;
        opacity = Mathf.MoveTowards(opacity, 1f, Time.deltaTime);

        c.a = opacity;
        image.color = c;
    }

    public void Cooldown()
    {
        opacity = 0.2f;
    }
}
