using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public float touchCount;
    public float T1pts;
    public float T2pts;
    public string lastTouch;
    public string lastTouchPlayer;

    public BotInput OpponentAI1Input;
    public BotInput OpponentAI2Input;
    public BotMotor OpponentAI1Motor;
    public BotMotor OpponentAI2Motor;


  // Start is called before the first frame update
  void Start()
  {
        touchCount = 0f;

        int OpStart = Random.Range(1, 3);

        if(OpStart == 1)
        {
            OpponentAI1Input.setClass("R");
            OpponentAI2Input.setClass("L");
        }

        else if (OpStart == 2)
        {
            OpponentAI1Input.setClass("L");
            OpponentAI2Input.setClass("R");
        }

        OpponentAI1Motor.PosBot();
        OpponentAI2Motor.PosBot();
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



    public void givePoint(string teamGiven)
    {
        if(teamGiven == "T2")
        {
            T2pts += 1f;
        }

        if(teamGiven == "T1")
        {
            T1pts += 1f;
        }
    }

   // Update is called once per frame
   void Update()
   {
      if(touchCount > 3f)
        {
            if(lastTouch == "T1")
            {
                givePoint("T2");
                touchCount = 0f;
            }

            if(lastTouch == "T2")
            {
                givePoint("T1");
                touchCount = 0f;
            }
        }

        OpponentAI1Input.touchInfo(touchCount, lastTouch);
        OpponentAI2Input.touchInfo(touchCount, lastTouch);
   }

}
