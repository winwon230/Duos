using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Microsoft.Unity.VisualStudio.Editor;
using Palmmedia.ReportGenerator.Core;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;


public class gameManager : MonoBehaviour
{
    public float touchCount;
    public float T1pts;
    public float T2pts;
    public float T1sets;
    public float T2sets;
    public float ptsInSet;
    public float noSets;
    public bool winByTwo = false;
    private float setsToWin;
    public bool gameWon = false;

    public string lastTouch;
    public string lastTouchPlayer;
    private GameSettingsManager settingsManager;

    public BotInput OpponentAI1Input;
    public BotInput OpponentAI2Input;
    public BotMotor OpponentAI1Motor;
    public BotMotor OpponentAI2Motor;
    public TeammateInput TeammateInput;
    public BotMotor TeammateMotor;
    public HumanInput PlayerInput;
    public CharacterMotor PlayerMotor;

// below is serving mechanics stuff
    public string server;
    public string lastServer;
    private bool teachingServe = false;

// below is tutorial stuff
    private GameObject TutorialTextObject;
    private TMP_Text TutorialText;
    private GameObject TutorialTextGroup;
    public float successfulHitCount = 0f;
    public GameObject volleyball;
    public GameObject hitGroup;
    public UnityEngine.UI.Image hitCircle1;
    public UnityEngine.UI.Image hitCircle2;
    public UnityEngine.UI.Image hitCircle3;
    public TMP_Text niceText;
    public GameObject bumpTarget;
    public GameObject setTarget;
    public GameObject spikeTarget;
    public GameObject serveTarget;
    public GameObject standingZone;

    //Below is points UI stuff
    public TMP_Text t1PtsText; //player and its teammates
    public TMP_Text t2PtsText; //opponents
    public TMP_Text t1SetText;
    public TMP_Text t2SetText;
    public GameObject pointsUIGroup;
    public TMP_Text winText;
    public GameObject finishMenu;
    public GameObject pauseMenu;
    public TMP_Text gameStartTXT;
    public TMP_Text t1ServingTXT;
    public TMP_Text t2ServingTXT;

    private SceneLoader loader;

    void Awake()
    {
        settingsManager = FindFirstObjectByType<GameSettingsManager>();
    }

  // Start is called before the first frame update
    void Start()
  {
settingsManager = FindFirstObjectByType<GameSettingsManager>();
    Time.timeScale = 1f;
        if(hitGroup != null)
        {
            hitGroup.SetActive(false);
        }
        if(bumpTarget != null)
        {
            bumpTarget.SetActive(false);
        }
        if(setTarget != null)
        {
            setTarget.SetActive(false);
        }
        if(spikeTarget != null)
        {
            spikeTarget.SetActive(false);
        }
        if(serveTarget != null)
        {
            serveTarget.SetActive(false);
        }
        if(niceText != null)
        {
            niceText.alpha = 0f;
        }
        if(standingZone != null)
        {
            standingZone.SetActive(false);
        }
        if(pointsUIGroup != null)
        {
            pointsUIGroup.SetActive(false);
        }
        if(winText != null)
        {
            winText.gameObject.SetActive(false);
        }
        if(finishMenu != null)
        {
            finishMenu.SetActive(false);
        }
        if(gameStartTXT.gameObject != null)
        {
            gameStartTXT.gameObject.SetActive(false);
        }
        if(t1ServingTXT.gameObject != null)
        {
            t1ServingTXT.gameObject.SetActive(false);
        }
        if(t2ServingTXT.gameObject != null)
        {
            t2ServingTXT.gameObject.SetActive(false);
        }
        TutorialTextGroup = GameObject.Find("TutorialTextGroup");
        TutorialTextObject = GameObject.Find("InstructionsText");
        if(TutorialTextObject != null)
        {
            TutorialText = TutorialTextObject.GetComponent<TMP_Text>();  //hopefully this wont mess up ordering and stuff 
        }
        if(settingsManager.currentGameMode == GameSettingsManager.GameMode.VBFriendly)
        {
            pointsUIGroup.SetActive(true);
            if(TutorialTextGroup != null)
            {
                TutorialTextGroup.SetActive(false);
            }
            touchCount = 0f;

            int OpStart = UnityEngine.Random.Range(1, 3);

            if(OpStart == 1)
            {
                OpponentAI1Input.setClass("R");
                OpponentAI2Input.setClass("L");
                PlayerInput.setClass("R");
                TeammateInput.setClass("L");
            }

            else if (OpStart == 2)
            {
                OpponentAI1Input.setClass("L");
                OpponentAI2Input.setClass("R");
                PlayerInput.setClass("L");
                TeammateInput.setClass("R");
            }

            resetPositions();

            assignRandomServe();
            ptsInSet = settingsManager.ptsToWinSet;
            noSets = settingsManager.noSets;
            winByTwo = settingsManager.winByTwo;
            setsToWin = Mathf.CeilToInt(noSets / 2f); 
            StartCoroutine(showStartText());
            

        }
        else if(settingsManager.currentGameMode == GameSettingsManager.GameMode.VBTutorial)
        {
            PlayerInput.activateTutorialMode();
            StartCoroutine(startTutorial());
        }
        loader = FindFirstObjectByType<SceneLoader>();
  }

    private IEnumerator showStartText()
    {
        gameStartTXT.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameStartTXT.gameObject.SetActive(false);
    }

    private IEnumerator startTutorial() //Tutorial is complete now
    {
        TutorialTextGroup.SetActive(true);
        TutorialText.text = "Welcome to Duos Volleyball! My name's PJ and I'll teach you how to play volleyball! Are you ready? Click to continue";
        yield return StartCoroutine(waitForClick());
        standingZone.SetActive(true);
        TutorialText.text = "Before you start, you need to press shift to lock your cursor into the screen. You're also recommended to stand in the blue zone for now. Click to continue";
        yield return StartCoroutine(waitForClick());
        TutorialText.text = "Great! First you'll learn to bump. When the ball comes to you, choose a direction and left click to bump! Also, use WASD/space to move and jump! Click to continue";
        yield return StartCoroutine(waitForClick());
        TutorialTextGroup.SetActive(false);
        hitGroup.SetActive(true);
        bumpTarget.SetActive(true);
        successfulHitCount = 0f; // remember to reset to 0 after everything taught
        yield return StartCoroutine(teachBump());
        Debug.Log("ready for next (this should only appear once)");
        yield return new WaitUntil(()=> successfulHitCount >= 3f);
        bumpTarget.SetActive(false);
        TutorialTextGroup.SetActive(true);
        TutorialText.text = "Nice! Now let's teach you how to set. Press F to set the ball, which sends the ball high and lets your teammates spike it over the net. Click to continue";
        yield return StartCoroutine(waitForClick());
        successfulHitCount = 0f;
        TutorialTextGroup.SetActive(false);
        setTarget.SetActive(true);
        resetHitCircles();
        hitGroup.SetActive(true);
        yield return StartCoroutine(teachSet()); // weird bug, sometimes it doesn't log the last ball hitting the ground or smth??
        yield return new WaitUntil(()=> successfulHitCount >= 3f);
        successfulHitCount = 0f;
        hitGroup.SetActive(false);
        TutorialTextGroup.SetActive(true);
        setTarget.SetActive(false);
        TutorialText.text = "Nice! Now it's time to learn to hit/spike. Jump once the ball is stationary in air, and left click, the ball will go in whatever direction you're facing. Click to continue";
        yield return StartCoroutine(waitForClick());
        TutorialText.text = "Also, while it's doable to spike from the standing zone, it's better to stand close to the net where you can hit more down. Click to continue";
        spikeTarget.SetActive(true);
        TutorialTextGroup.SetActive(false);
        resetHitCircles();// add spike object and reference
        hitGroup.SetActive(true);
        yield return StartCoroutine(teachSpike());
        yield return new WaitUntil(()=> successfulHitCount >= 3f);
        successfulHitCount = 0f;
        spikeTarget.SetActive(false);
        TutorialTextGroup.SetActive(true);
        hitGroup.SetActive(false);
        resetHitCircles();
        standingZone.SetActive(false);
        TutorialText.text = "Nice! The final part of playing volleyball is the serve. It starts the point and good placement gives you easy points. Click to continue";
        yield return StartCoroutine(waitForClick());
        TutorialText.text = "Remember to stand behind the baseline when tossing, or you will get teleported backward. Toss with G, jump, and spike the ball over the net. Click to continue";
        yield return StartCoroutine(waitForClick());
        serveTarget.SetActive(true);
        hitGroup.SetActive(true);
        TutorialTextGroup.SetActive(false);
        PlayerInput.tutorial = true;// add serve target somewhere around here
        PlayerInput.serving = true;
        GameObject playerObject = PlayerInput.gameObject;
        CharacterController playerController = playerObject.GetComponent<CharacterController>();
        playerController.enabled = false;
        playerObject.transform.position = new Vector3(0f, 0.1f, 7.3f);
        playerController.enabled = true;
        PlayerMotor.tpServePos();
        teachingServe = true;
        yield return new WaitUntil(()=> successfulHitCount >= 3f);
        PlayerInput.serving = false;
        teachingServe = false;
        serveTarget.SetActive(false);
        hitGroup.SetActive(false);
        TutorialTextGroup.SetActive(true);
        TutorialText.text = "Good work! Now you've mastered the basics, you're ready to play volleyball! Try the friendly match mode. Click to continue";
        yield return StartCoroutine(waitForClick());
        settingsManager.currentGameMode = GameSettingsManager.GameMode.VBFriendly;
        SceneLoader loader = FindObjectOfType<SceneLoader>();
        Destroy(loader.gameObject);
        Destroy(settingsManager.gameObject);
        SceneManager.LoadSceneAsync(0);
    }

    private IEnumerator waitForClick()
    {
        yield return new WaitUntil(()=> !Input.GetMouseButtonDown(0)); // make sure the mouse button isn't currently down so it doesnt go twice with this
        yield return new WaitUntil(()=> Input.GetMouseButtonDown(0)); 
    }

    private IEnumerator teachBump()
    {
        if(successfulHitCount <= 2f) // this is 2 bc it makes you do an extra bc hit count is read before the target gets hit
        {
                yield return new WaitUntil(()=> GameObject.FindGameObjectsWithTag("Volleyball").Length < 1f);
                Vector3 ballSpawnPos = new Vector3(0.8f, 1.26f, 14.95f);
                GameObject ball = Instantiate(volleyball, ballSpawnPos, quaternion.identity);
                BallController ballScript = ball.GetComponent<BallController>();
                Vector3 offset = new Vector3(UnityEngine.Random.Range(-1f, 1f), 1.26f, UnityEngine.Random.Range(-1f, 1f));
                ballScript.hitTowards(PlayerInput.transform.position + offset, 1f);
                yield return new WaitUntil(()=> GameObject.FindGameObjectsWithTag("Volleyball").Length == 0f);
                yield return StartCoroutine(teachBump());                 
        }
    }

    private IEnumerator teachSet()
    {
        if(successfulHitCount <= 2f)
        {
            yield return new WaitUntil(()=> GameObject.FindGameObjectsWithTag("Volleyball").Length < 1f);
            Vector3 ballSpawnPos = new Vector3(0.8f, 1.26f, 14.95f);
            GameObject ball = Instantiate(volleyball, ballSpawnPos, quaternion.identity);
            BallController ballScript = ball.GetComponent<BallController>();
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-1f, 1f), 1.26f, UnityEngine.Random.Range(-1f, 1f));
            ballScript.hitTowards(PlayerInput.transform.position + offset, 1f);
            yield return new WaitUntil(()=> GameObject.FindGameObjectsWithTag("Volleyball").Length == 0f);
            yield return StartCoroutine(teachSet());
        }
    }

    private IEnumerator teachSpike()
    {
        if(successfulHitCount <= 2f && GameObject.FindGameObjectsWithTag("Volleyball").Length == 0f)
        {
            yield return new WaitUntil(()=> GameObject.FindGameObjectsWithTag("Volleyball").Length < 1f);
            Vector3 ballSpawnPos = new Vector3(0.8f, 1.26f, 14.95f);
            GameObject ball = Instantiate(volleyball, ballSpawnPos, quaternion.identity);
            BallController ballScript = ball.GetComponent<BallController>();
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 1.26f, UnityEngine.Random.Range(0f, 1f));
            ballScript.hitTowards(PlayerInput.transform.position + offset, 1.3f); //hits towards, dont change the start position also set doubled y multiplier is wayy too much lol
            yield return new WaitUntil(()=> GameObject.FindGameObjectsWithTag("Volleyball").Length == 0f);
            yield return StartCoroutine(teachSpike());            
        }
    }

    public void targetHit()
    {
        niceText.alpha = 1f;
        successfulHitCount += 1f;
        if(teachingServe == true)
        {
            PlayerMotor.tpServePos();
        }

        if(successfulHitCount == 1f)
        {
            hitCircle1.color = Color.green;
        }
        else if(successfulHitCount == 2f)
        {
            hitCircle1.color = Color.green;
            hitCircle2.color = Color.green;            
        }
        else if(successfulHitCount == 3f)
        {
            hitCircle1.color = Color.green;
            hitCircle2.color = Color.green;
            hitCircle3.color = Color.green;
            hitGroup.SetActive(false);
        }
    }

    public void resetHitCircles()
    {
        hitCircle1.color = Color.white;
        hitCircle2.color = Color.white;
        hitCircle3.color = Color.white;
    }

    public void resetPositions()
    {
        OpponentAI1Motor.tpBot();
        OpponentAI2Motor.tpBot();
        TeammateMotor.tpBot();
        PlayerMotor.posSet();
    }

    public void assignRandomServe()
    {
        int serveNumberTeam;
        serveNumberTeam = UnityEngine.Random.Range(1, 3);
        int servePlayerNumber;
        servePlayerNumber = UnityEngine.Random.Range(1, 3);
        if(serveNumberTeam == 1)
        {
            if(servePlayerNumber == 1)
            {
                server = "T1Player";
            }

            else if(servePlayerNumber == 2)
            {
                server = "T1Teammate";
            }
        }

        else if(serveNumberTeam == 2)
        {
            if(servePlayerNumber == 1)
            {
                server = "T2OpponentAI1";
            }

            else if(servePlayerNumber == 2)
            {
                server = "T2OpponentAI2";
            }
        }
        if (server.StartsWith("T1"))
        {
            t1ServingTXT.gameObject.SetActive(true);
            t2ServingTXT.gameObject.SetActive(false);
        }
        else if (server.StartsWith("T2"))
        {
            t1ServingTXT.gameObject.SetActive(false);
            t2ServingTXT.gameObject.SetActive(true);
        }


        Invoke(nameof(internalServeRequest), 5f);
    }

    public void assignServerAfterPt(string team)
    {
        int servePlayerNumber = UnityEngine.Random.Range(1, 3);
        if (team == "T1")
        {
            if (server.StartsWith("T1")) // won a pt off own serve
            {
                if(server == "T1Player")
                {
                    server = "T1Teammate";
                }

                else if(server == "T1Teammate")
                {
                    server = "T1Player";
                } 
            }

            if (server.StartsWith("T2")) // won a pt off opp serve
            {
                if(servePlayerNumber == 1)
                {
                    server = "T1Player";
                }

                else if(servePlayerNumber == 2)
                {
                    server = "T1Teammate";
                }
            }
        }

        if (team == "T2")
        {
            if (server.StartsWith("T1")) // won a pt off opp serve
            {
                if(servePlayerNumber == 1)
                {
                    server = "T2OpponentAI1";
                    // PlayerInput.requestServe(); do the bots first
                }

                else if(servePlayerNumber == 2)
                {
                    server = "T2OpponentAI2";
                }
            }

            else if (server.StartsWith("T2")) // won a pt off own serve
            {
                if(server == "T2OpponentAI1")
                {
                    server = "T2OpponentAI2";
                }

                else if(server == "T2OpponentAI2")
                {
                    server = "T2OpponentAI1";
                } 
            }
            
        }
        if (server.StartsWith("T1"))
        {
            t1ServingTXT.gameObject.SetActive(true);
            t2ServingTXT.gameObject.SetActive(false);
        }
        else if (server.StartsWith("T2"))
        {
            t1ServingTXT.gameObject.SetActive(false);
            t2ServingTXT.gameObject.SetActive(true);
        }

        Invoke(nameof(internalServeRequest), 4f);

    }

    public void internalServeRequest()
    {
        resetPositions(); //double reset should hopefully prevent when bots are already moving
        if(server == "T1Player")
        {
            PlayerInput.requestServeInput();
        }

        else if(server == "T1Teammate")
        {
            TeammateInput.requestServe();
        }

        else if(server == "T2OpponentAI1")
        {
            OpponentAI1Input.requestServe();
        }

        else if(server == "T2OpponentAI2")
        {
            OpponentAI2Input.requestServe();
        }

        lastTouchPlayer = server;
    }


    public void Touch(string team)
    {
    if(lastTouch == null)
        {
            lastTouch = team;
        }
    if(team == lastTouch)
        {
            touchCount += 1f;
            lastTouch = team;
        }

    else if(team != lastTouch)
        {
            touchCount = 1f;
            lastTouch = team;
        }
    }

    public void updateLastPlayerTouch(string lastPlayer)
    {
        lastTouchPlayer = lastPlayer; // if there are problems this last touch player is assigned before the ball actually gets hit midair
    }

    public void ballGroundHit(string side, string groundType)
    {
            Debug.Log("GROUND HIT | side = " + side +
              " | groundType = " + groundType +
              " | lastTouch = " + lastTouch +
              " | lastTouchPlayer = " + lastTouchPlayer);
        if(groundType == "VolleyballCourt") // if ball goes IN
        {
            if(side == "T1")
            {
                givePoint("T2");
            }

            else if(side == "T2")
            {
                givePoint("T1");
            }
        }

        else if(groundType == "Ground") // if ball goes OUT
        {
                if (lastTouchPlayer.StartsWith("T1"))
                {
                    givePoint("T2");
                }

                else if (lastTouchPlayer.StartsWith("T2"))
                {
                    givePoint("T1");
                }
        }
    }

    public void givePoint(string teamGiven) // need to make this apply for matches only
    {
        if(teamGiven == "T2")
        {
            T2pts += 1f;
            assignServerAfterPt("T2");
        }

        else if(teamGiven == "T1")
        {
            T1pts += 1f;
            assignServerAfterPt("T1");
        }
        
        GameObject[] allBalls = GameObject.FindGameObjectsWithTag("Volleyball"); //destroying stray balls before next pt
        GameObject[] allShadows = GameObject.FindGameObjectsWithTag("Shadow");

        foreach(GameObject ball in allBalls)
        {
            Destroy(ball, 1f);
        }

        foreach(GameObject shadow in allShadows)
        {
            Destroy(shadow, 1f);
        }
    }

    public void quitGameButton()
    {
        pauseMenu.SetActive(false);
        if(loader != null)
        {
            loader = FindFirstObjectByType<SceneLoader>();   
        }
        if(settingsManager != null)
        {
            settingsManager = FindFirstObjectByType<GameSettingsManager>();   
        }
        Destroy(loader.gameObject);
        Destroy(settingsManager.gameObject);
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    public void continueGameButton()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

   // Update is called once per 
   void Update()
   {
        if (Input.GetKeyDown(KeyCode.P) && pauseMenu != null)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        if(settingsManager.currentGameMode == GameSettingsManager.GameMode.VBFriendly)
        {
            OpponentAI1Input.touchInfo(touchCount, lastTouch, lastTouchPlayer);
            OpponentAI2Input.touchInfo(touchCount, lastTouch, lastTouchPlayer);
            TeammateInput.touchInfo(touchCount, lastTouch, lastTouchPlayer);

            if(t1PtsText != null && t2PtsText != null)
            {
                t1PtsText.text = T1pts.ToString();
                t2PtsText.text = T2pts.ToString();
            }
            if(t1SetText != null && t2SetText != null)
            {
                t1SetText.text = T1sets.ToString();
                t2SetText.text = T2sets.ToString();
            }

            if(touchCount > 3f)
            {
                if(lastTouch == "T1")
                {
                    givePoint("T2");
                    touchCount = 0f;
                }

                else if(lastTouch == "T2")
                {
                    givePoint("T1");
                    touchCount = 0f;
                }
            }

            if(T1pts >= ptsInSet && T2pts >= ptsInSet)
            {
                 if(winByTwo == true)
                {
                    if(T1pts - T2pts >= 2)
                    {
                        //maybe check if either side has the number of sets required?
                        T1pts = 0f;
                        T2pts = 0f;
                        T1sets += 1f;
                        resetPositions();
                        assignRandomServe();
                        touchCount = 0f;
                        lastTouch = null;
                    }
                    else if(T2pts - T1pts >= 2)
                    {
                        //maybe check if either side has the number of sets required?
                        T1pts = 0f;
                        T2pts = 0f;
                        T2sets += 1f;
                        resetPositions();
                        assignRandomServe();
                        touchCount = 0f;
                        lastTouch = null;
                    }
                }               
            }

            if(T1pts >= ptsInSet && T2pts < ptsInSet)
            {
                if(winByTwo == true)
                {
                    if(T1pts - T2pts >= 2)
                    {
                        //maybe check if either side has the number of sets required?
                        T1pts = 0f;
                        T2pts = 0f;
                        T1sets += 1f;
                        resetPositions();
                        assignRandomServe();
                        touchCount = 0f;
                        lastTouch = null;
                    }
                }
                else
                {
                    T1pts = 0f;
                    T2pts = 0f;
                    T1sets += 1f;
                    resetPositions();
                    assignRandomServe();
                    touchCount = 0f;
                    lastTouch = null;
                }
            }  
            else if(T2pts>= ptsInSet && T1pts < ptsInSet)
            {
                if(winByTwo == true)
                {
                    if(T2pts - T1pts >= 2)
                    {
                        //maybe check if either side has the number of sets required?
                        T1pts = 0f;
                        T2pts = 0f;
                        T2sets += 1f;
                        resetPositions();
                        assignRandomServe();
                        touchCount = 0f;
                        lastTouch = null;
                    }
                }
                else
                {
                    T1pts = 0f;
                    T2pts = 0f;
                    T2sets += 1f;
                    resetPositions();
                    assignRandomServe();
                    touchCount = 0f;
                    lastTouch = null;
                }                
            }
            if(T1sets >= setsToWin && gameWon == false && winText != null)
            {
                winText.gameObject.SetActive(true);
                winText.text = "You win";
                StartCoroutine(winTextDelay());
                gameWon = true;
            }
            else if(T2sets >= setsToWin && gameWon == false && winText != null)
            {
                winText.gameObject.SetActive(true);
                winText.text = "Opponents win";
                StartCoroutine(winTextDelay());
                gameWon = true;
            }          
        }
        if(settingsManager.currentGameMode == GameSettingsManager.GameMode.VBTutorial)
        {
            if(niceText != null)
            {
                if(niceText.alpha > 0f)
                {
                    niceText.alpha -= Time.deltaTime;
                }
            }
        }
   }

   private IEnumerator winTextDelay()
    {
        yield return new WaitForSeconds(3f);
        finishMenu.SetActive(true);
        winText.gameObject.SetActive(false);
        Time.timeScale = 0f;
    }

}
