using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public float touchCount;
    public float T1pts;
    public float T2pts;
    public string lastTouch;
    public string lastTouchPlayer;
    private GameData GameData;

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


    void Awake()
    {
        GameData = FindFirstObjectByType<GameData>();
    }

  // Start is called before the first frame update
    void Start()
  {
        if(GameData.currentGameMode == GameData.GameMode.VBFriendly)
        {
            touchCount = 0f;

            int OpStart = Random.Range(1, 3);

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
        }
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
        serveNumberTeam = Random.Range(1, 3);
        int servePlayerNumber;
        servePlayerNumber = Random.Range(1, 3);
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


        Invoke(nameof(internalServeRequest), 5f);
    }

    public void assignServerAfterPt(string team)
    {
        int servePlayerNumber = Random.Range(1, 3);
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

        Invoke(nameof(internalServeRequest), 4f);

    }

    public void internalServeRequest()
    {
        resetPositions(); //double reset should hopefully prevent when bots are already moving
        if(server == "T1Player")
        {
            //HumanInput.requestServe(); add later
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

   // Update is called once per frame
   void Update()
   {
        OpponentAI1Input.touchInfo(touchCount, lastTouch, lastTouchPlayer);
        OpponentAI2Input.touchInfo(touchCount, lastTouch, lastTouchPlayer);
        TeammateInput.touchInfo(touchCount, lastTouch, lastTouchPlayer);

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
   }

}
