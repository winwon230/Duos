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
    // Start is called before the first frame update
    void Awake()
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
        controller = GetComponent<CharacterController>();
    }

    public void setClass(string startPos)
    {
        if(startPos == "R" && botTeam == "T2")
        {
            Vector3 defaultPos = new Vector3(-2.09f, 0.075f, 22.8f);
            botMotor.setDefaultPos(defaultPos);
            botPos = "R";
        }

        if(startPos == "L" && botTeam == "T2")
        {
            Vector3 defaultPos = new Vector3(2.09f, 0.075f, 22.8f);
            botMotor.setDefaultPos(defaultPos);
            botPos = "L";
        }
    }

    public void touchInfo(float touches, string lastTouch)
    {
        touchCount = touches;
        lastTouchTeam = lastTouch;
    }

    public void sendBallPos(Vector3 finalPos) // this pos is already given as within the correct area of the court
    {
            float indvDistance = Vector3.Distance(transform.position, finalPos);
            float teammateDistance = Vector3.Distance(otherBot.transform.position, finalPos);

            predictedBallPos = finalPos; // predicted ball pos is the public one

            if(indvDistance < teammateDistance)
            {
                botMotor.MoveBot(finalPos);         
            }

            /*else if(indvDistance > teammateDistance && justHit == false)
            {
                botMotor.MoveBot(finalPos);
            }*/

            else if(indvDistance > teammateDistance)
            {
                return;
            }
        
    }

    public float hitCooldown = 0.5f;
    private float hitMultiplier = 1f;
    private Rigidbody rb;
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
            rb = nearestBall.GetComponent<Rigidbody>();   
        }

        if(Vector3.Distance(transform.position, predictedBallPos) <= 0.3f && lastTouchTeam == "T1" && hitCooldown <=0f) //code for first touch (bump)
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
            manager.updateLastPlayerTouch(botTeam + botPos);
            botMotor.HitBall("Hit", botTeam, hitDirection, hitMultiplier); //don't add touches bc its alr specified in botmotor
            hitCooldown = 0.25f;
        }

        if(Vector3.Distance(transform.position, predictedBallPos) <= 0.3f && lastTouchTeam == "T2" && touchCount == 1f && hitCooldown <= 0f) //second touch/set
        {
            Vector3 otherBotPos = otherBot.transform.position;
            otherBotPos.z -= 3f;

            manager.updateLastPlayerTouch(botTeam + botPos);

            Vector3 hitDirection = otherBotPos - transform.position;
            botMotor.HitBall("Front Set", botTeam, hitDirection, 1f);
            hitCooldown = 0.25f;
        }

        if(Vector3.Distance(transform.position, predictedBallPos) <= 0.3f && lastTouchTeam == "T2" && touchCount == 2f && hitCooldown <= 0f) //3rd touch(spike/bump)
        {
            if(transform.position.z > 20f)
            {
                Vector3 randomPos = new Vector3(0, 0.26f, 8.35f);
                randomPos.x = Random.Range(4.5f, -4.5f);
                
                Vector3 hitDirection = randomPos - transform.position;
                hitMultiplier = 1.6f;
                botMotor.HitBall("Hit", botTeam, hitDirection, hitMultiplier);
                manager.updateLastPlayerTouch(botTeam + botPos);
            }

            if(transform.position.z < 20f && transform.position.z > 17.31f && nearestBall.transform.position.y >= 5f && rb.velocity.y < 0f && hitCooldown <= 0f)
            {
                Vector3 randomPos = new Vector3(0, 0.26f, 8.35f);
                randomPos.x = Random.Range(4.5f, -4.5f);
                Vector3 hitDirection = randomPos - transform.position;

                hitMultiplier = 1f;
                botMotor.Jump(2f);
                StartCoroutine(spikeAfterJump(hitDirection)); // reference below
                hitCooldown = 0.25f;
            }
        }

    }

    private IEnumerator spikeAfterJump(Vector3 hitDirection)
    {
        yield return new WaitForSeconds(0.7f);
        botMotor.HitBall("Hit", botTeam, hitDirection, hitMultiplier);
        manager.updateLastPlayerTouch(botTeam + botPos);
    }

}
