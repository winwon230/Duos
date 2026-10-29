using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettingsManager : MonoBehaviour
{
    public TMP_Text gamemodeText;
    public TMP_Text setPtsTXT;
    public TMP_Text noSetsTXT;
    public TMP_Text winByTwoTXT;
    public TMP_Text MapTXT;
    public MapType selectedMapType = MapType.Indoor;
    public GameMode currentGameMode = GameMode.VBFriendly;

    public GameObject beachVBImage;
    public GameObject indoorVBImage;
    public GameObject forestVBImage;
    public GameObject settingsGroup;

    public float ptsToWinSet;
    public float noSets;
    public bool winByTwo;
    // Start is called before the first frame update
    void Start() // should communicate with gameManager to configure how the games work and stuff
    {
        forestVBImage.SetActive(false);
        indoorVBImage.SetActive(false);
        beachVBImage.SetActive(false);
        DontDestroyOnLoad(gameObject);
        currentGameMode = GameMode.VBFriendly;
        gamemodeText.text = "<" + currentGameMode.ToString() + ">";       
        ptsToWinSet = 7f;
        setPtsTXT.text = "<" + ptsToWinSet.ToString() + ">";
        noSets = 1f;
        noSetsTXT.text = "<" + noSets.ToString() + ">";
        winByTwo = true;
        winByTwoTXT.text = "< on >";
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public enum GameMode
    {
        VBTutorial, VBFriendly
    }
    public enum MapType
    {
        Indoor, Beach, Forest
    }

    public void changeGameMode()
    {
        int gameModeCount = Enum.GetValues(typeof(GameMode)).Length;
        currentGameMode = (GameMode)(((int)currentGameMode + 1) % gameModeCount);
        gamemodeText.text = "<" + currentGameMode.ToString() + ">";
        if(currentGameMode == GameMode.VBTutorial)
        {
            settingsGroup.SetActive(false);
        }

        else
        {
            settingsGroup.SetActive(true);
        }
    }
    public void updateSetPts()
    {
        ptsToWinSet += 2f;
        if(ptsToWinSet > 13f)
        {
            ptsToWinSet = 7f;
        }
        setPtsTXT.text = "<" + ptsToWinSet.ToString() + ">";
    }

    public void updateNoSets()
    {
        noSets += 2f;
        if(noSets > 5f)
        {
            noSets = 1f;
        }
        noSetsTXT.text = "<" + noSets.ToString() + ">";
    }

    public void updateMap()
    {
        int MapCount = Enum.GetValues(typeof(MapType)).Length;
        selectedMapType = (MapType)(((int)selectedMapType + 1) % MapCount);
        MapTXT.text = "<" + selectedMapType.ToString() + ">";

        if(selectedMapType == MapType.Beach)
        {
            beachVBImage.SetActive(true); // remember to set off for the other maps
            indoorVBImage.SetActive(false);
            forestVBImage.SetActive(false);
        }

        else if(selectedMapType == MapType.Indoor)
        {
            beachVBImage.SetActive(false);
            indoorVBImage.SetActive(true);
            forestVBImage.SetActive(false);
        }

        else if(selectedMapType == MapType.Forest)
        {
            beachVBImage.SetActive(false);
            indoorVBImage.SetActive(false);
            forestVBImage.SetActive(true);
        }
    }

    public void updateWinByTwo()
    {
        if(winByTwo == true)
        {
            winByTwo = false;
            winByTwoTXT.text = "< off >";
        }

        else if(winByTwo == false)
        {
            winByTwo = true;
            winByTwoTXT.text = "< on >";
        }
    }

    public void playButtonTriggered()
    {
        if(currentGameMode == GameMode.VBTutorial)
        {
            SceneManager.LoadSceneAsync(1);
        }
        else if(currentGameMode == GameMode.VBFriendly)
        {

            if(selectedMapType == MapType.Indoor)
            {
                SceneManager.LoadSceneAsync(2);
            }
            else if(selectedMapType == MapType.Beach)
            {
                SceneManager.LoadSceneAsync(3);
            }
            else if(selectedMapType == MapType.Forest)
            {
                SceneManager.LoadSceneAsync(4);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
