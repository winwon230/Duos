using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class WaveScroller : MonoBehaviour
{
    public float scrollSpeed = 0.1f;
    private RawImage rawImage;
    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();
        if(rawImage != null)
        {
            mat = Instantiate(rawImage.material);
            rawImage.material = mat;
        }

        else
        {
            mat = GetComponent<Renderer>().material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(mat != null)
        {
            Vector2 offset = mat.mainTextureOffset;
            offset.x += scrollSpeed * Time.deltaTime;
            mat.mainTextureOffset = offset;            
        }
    }
}
