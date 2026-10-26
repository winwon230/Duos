using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject transitionImage;
    public GameObject splashScreenGroup;
    public GameObject menuScreenGroup;
    public GameObject selectionScreenGroup;
    public GameObject dotRibbons;
    public GameObject indoorVBImage;
    public TransitionBoxScript transitionScript;
    private string currentScreen;
    public void loadMenu() //this is for the initial press
    {
        transitionScript.runTransition();
    }

    public void switchScreen()
    {
        if(currentScreen == "SplashScreen")
        {
            menuScreenGroup.SetActive(true);
            splashScreenGroup.SetActive(false);
            dotRibbons.SetActive(true);
            currentScreen = "MenuScreen";
        }
        else if(currentScreen == "MenuScreen")
        {
            selectionScreenGroup.SetActive(true);
            splashScreenGroup.SetActive(false);
            menuScreenGroup.SetActive(false);
            currentScreen = "SelectionScreen";// settings for vb to choose match settings
            indoorVBImage.SetActive(true);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        splashScreenGroup.SetActive(true);
        selectionScreenGroup.SetActive(false);
        menuScreenGroup.SetActive(false);
        indoorVBImage.SetActive(false);
        currentScreen = "SplashScreen";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
