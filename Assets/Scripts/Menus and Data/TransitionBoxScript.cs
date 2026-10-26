using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionBoxScript : MonoBehaviour
{
    public SceneLoader sceneLoader;
    private float xVelocity = 0f;
    private Vector3 startPos;
    private Vector3 currentPos;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = new Vector3(-2000f, 0f, -524f);
        rectTransform.localPosition = startPos;
    }

    public void runTransition()
    {
        xVelocity = 6000f;
        StartCoroutine(screenChange());
        StartCoroutine(resetPos());
    }

    private IEnumerator screenChange()
    {
        yield return new WaitUntil(()=> Mathf.Abs(currentPos.x) < 20f);
        sceneLoader.switchScreen();
    }

    private IEnumerator resetPos()
    {
        yield return new WaitUntil(()=> currentPos.x > 1800f);
        rectTransform.localPosition = startPos;
        xVelocity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        currentPos = rectTransform.localPosition;
        currentPos.x += xVelocity * Time.deltaTime;
        rectTransform.localPosition = currentPos;
    }
}
