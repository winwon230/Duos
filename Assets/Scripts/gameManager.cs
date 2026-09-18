using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public float touchCount;
    public float T1pts;
    public float T2pts;
    public string lastTouch;




  // Start is called before the first frame update
  void Start()
  {
      touchCount = 0f;
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
   }

}
