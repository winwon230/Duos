using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInput : MonoBehaviour
{
    private BotMotor botMotor;
    private string botTeam;
    // Start is called before the first frame update
    void Start()
    {
        botMotor = GetComponent<BotMotor>();
        if(tag == "OpponentBot")
        {
            botTeam = "T2";
        }

        if(tag == "TeammateBot")
        {
            botTeam = "T1";
        }
    }

    public void setClass(string startPos)
    {
        if(startPos == "R" && botTeam == "T2")
        {
            Vector3 defaultPos = new Vector3(-2.09f, 0.075f, 22.8f);
            botMotor.setDefaultPos(defaultPos);
            Debug.Log("R");
        }

        if(startPos == "L" && botTeam == "T2")
        {
            Vector3 defaultPos = new Vector3(2.09f, 0.075f, 22.8f);
            botMotor.setDefaultPos(defaultPos);
            Debug.Log("L");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
