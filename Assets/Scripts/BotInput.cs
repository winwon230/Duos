using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BotInput : MonoBehaviour
{
    public BotInput otherBot;
    public gameManager manager;
    private BotMotor botMotor;
    private string botTeam;
    private string botPos;
    private float DistanceToBall;
    private GameObject nearestBall = null;
    private float touchCount = 0f;
    private string lastTouchTeam = null;
    public string lastTouchPlayer;
    private CharacterController controller;
 // just hit may not be relevant?
    private Vector3 predictedBallPos;
    private Vector3 defaultPos;
    private Vector3 targetServePosition;
    private bool spiking = false;

    // Start is called before the first frame update
    void Awake()
    {
        botMotor = GetComponent<BotMotor>();
        if(tag == "OpponentBot")
        {
            botTeam = "T2";
        }
        controller = GetComponent<CharacterController>();
    }

    public void setClass(string startPos)
    {
        if(startPos == "R" && botTeam == "T2")
        {
            defaultPos = new Vector3(-2.09f, 0.075f, 22.8f);
            botMotor.setDefaultPos(defaultPos);
            botPos = "R";
        }

        else if(startPos == "L" && botTeam == "T2")
        {
            defaultPos = new Vector3(2.09f, 0.075f, 22.8f);
            botMotor.setDefaultPos(defaultPos);
            botPos = "L";
        }
    }

    public void touchInfo(float touches, string lastTouch, string lastPlayer)
    {
        touchCount = touches;
        lastTouchTeam = lastTouch;
        lastTouchPlayer = lastPlayer;
    }

    public void requestServe()
    {
        Vector3 servePos = defaultPos;
        servePos.z = 27.62f;

            targetServePosition = new Vector3(Random.Range(-4f, 4f), 7f, Random.Range(9.62f, 15.62f)); //change this for the opponents

        botMotor.motorServe(servePos, targetServePosition, botTeam, botPos, "Flat"); // change here when adding different serve types
    }    

    public void sendBallPos(Vector3 finalPos) // this pos is already given as within the correct area of the court
    {

            float indvDistance = Vector3.Distance(transform.position, finalPos);
            float teammateDistance = Vector3.Distance(otherBot.transform.position, finalPos);

            predictedBallPos = finalPos; // predicted ball pos is the public one

            if(indvDistance < teammateDistance && lastTouchPlayer != botTeam + botPos)
            {
                botMotor.MoveBot(finalPos);
            }

            else if(indvDistance > teammateDistance && lastTouchPlayer != botTeam + botPos &&! lastTouchPlayer.StartsWith("T1"))
            {
                botMotor.MoveBot(finalPos);
            }

            else if(indvDistance > teammateDistance && lastTouchPlayer == botTeam + botPos)
            {

                return;
            }
        
    }

    public float hitCooldown = 0.5f;
    private float hitMultiplier = 1f;
    private Rigidbody rb;
    private Vector3 randomPos;
    // Update is called once per frame
    void Update()
    {
        if(hitCooldown > 0f)
        {
            hitCooldown -= Time.deltaTime;
        }

        float distanceToTeammate = Vector3.Distance(otherBot.transform.position, transform.position);

        GameObject[] AllBalls = GameObject.FindGameObjectsWithTag("Volleyball");
        float shortestDistance = Mathf.Infinity;


        foreach (GameObject ball in AllBalls)
        {
            DistanceToBall = Vector3.Distance(transform.position, ball.transform.position);

            if(DistanceToBall < shortestDistance)
            {
                shortestDistance = DistanceToBall;
                nearestBall = ball;
            }

        }

        if(nearestBall != null)
        {
            if(nearestBall.transform.position.z < 17.31f && lastTouchPlayer.StartsWith("T2") && touchCount == 3f)
            {
                botMotor.MoveBot(defaultPos);   
            }
        }

        if(nearestBall != null)
        {
            rb = nearestBall.GetComponent<Rigidbody>();   

        if(Vector3.Distance(transform.position, predictedBallPos) <= 1.5f && lastTouchTeam == "T1" && hitCooldown <=0f && lastTouchPlayer != botTeam + botPos) //code for first touch (bump)
        {
            if (distanceToTeammate <= 3f)
            {
                hitMultiplier = 1f;   
            }

            else if(distanceToTeammate > 3f)
            {
                hitMultiplier = Mathf.Clamp(1f + distanceToTeammate * 0.05f, 1f, 2.5f);
            }
            Vector3 hitDirection = otherBot.transform.position - transform.position;
            botMotor.HitBall("Hit", botTeam, hitDirection, hitMultiplier, botPos); //don't add touches bc its alr specified in botmotor
            hitCooldown = 0.05f;
        }

        if(Vector3.Distance(transform.position, predictedBallPos) <= 1.5f && lastTouchTeam == "T2" && touchCount == 1f && hitCooldown <= 0f && lastTouchPlayer != botTeam + botPos) //second touch/set
        {
            Vector3 otherBotPos = otherBot.transform.position;
            otherBotPos.z -= 3.5f;

            Vector3 hitDirection = otherBotPos - transform.position;
            botMotor.HitBall("Front Set", botTeam, hitDirection, 1f, botPos);
            hitCooldown = 0.05f;
        }

        if(Vector3.Distance(transform.position, predictedBallPos) <= 1.5f && lastTouchTeam == "T2" && touchCount == 2f && hitCooldown <= 0f && lastTouchPlayer != botTeam + botPos) //3rd touch(spike/bump)
        {
            if(transform.position.z > 20f)
            {
                randomPos = new Vector3(Random.Range(-4.5f, 4.5f), 0.26f, 8.35f);
                
                Vector3 hitDirection = randomPos - transform.position;
                hitMultiplier = 1.6f;
                botMotor.HitBall("Hit", botTeam, hitDirection, hitMultiplier, botPos);

            }

            else if(transform.position.z <= 20.6f && transform.position.z > 17.31f && nearestBall.transform.position.y >= 5.2f && rb.velocity.y <= 0f && !spiking)
            {
                randomPos = new Vector3(Random.Range(-4.5f, 4.5f), 0.6f, 8.35f);

                float distanceFromNet = transform.position.z - 17.31f;

                randomPos.z = 17.31f - distanceFromNet * 2.4f;
                if(randomPos.z < 12f)
                {
                    randomPos.y = 2.8f; // this should loosen angle to prevent hitting straight into the net
                    hitMultiplier = Random.Range(0.8f, 1.1f);
                }

                else
                {
                    hitMultiplier = Random.Range(1f, 1.8f);                    
                }
                spiking = true;
                botMotor.Jump(2.4f);
                StartCoroutine(spikeAfterJump()); // reference below
            }
            hitCooldown = 0.1f;
        }
        }

    }


    private IEnumerator spikeAfterJump()
    {
        yield return new WaitUntil(()=> nearestBall != null && nearestBall.transform.position.y <= 5.4f && Vector3.Distance(transform.position, nearestBall.transform.position) < 1.5f && hitCooldown <= 0f);
        Vector3 hitDirection = randomPos - transform.position;
        botMotor.HitBall("Hit", botTeam, hitDirection, hitMultiplier, botPos);
        Debug.Log("hit the ball");
        spiking = false;
    }

}
