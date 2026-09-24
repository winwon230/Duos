using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeammateInput : MonoBehaviour
{
    public gameManager manager;
    public HumanInput Player;
    public BotMotor botMotor;
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
    // Start is called before the first frame update
    void Awake()
    {
        botMotor = GetComponent<BotMotor>();
        if(tag == "TeammateBot")
        {
            botTeam = "T1";
        }
        controller = GetComponent<CharacterController>();
    }

    public void setClass(string startPos)
    {
        if(startPos == "R" && botTeam == "T1")
        {
            defaultPos = new Vector3(-2.09f, 0.075f, 11.99f);
            botMotor.setDefaultPos(defaultPos);
            botPos = "R";
        }

        if(startPos == "L" && botTeam == "T1")
        {
            defaultPos = new Vector3(2.09f, 0.075f, 11.99f);
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

    public void sendBallPos(Vector3 finalPos) // this pos is already given as within the correct area of the court
    {
            float indvDistance = Vector3.Distance(transform.position, finalPos);
            float teammateDistance = Vector3.Distance(Player.transform.position, finalPos);

            predictedBallPos = finalPos; // predicted ball pos is the public one

            if(indvDistance < teammateDistance && lastTouchPlayer != botTeam + botPos)
            {
                botMotor.MoveBot(finalPos);         
            }

            else if(indvDistance > teammateDistance && lastTouchPlayer != botTeam + botPos &&! lastTouchPlayer.StartsWith("T2"))
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
    // Update is called once per frame
    void Update()
    {
        if(hitCooldown > 0f)
        {
            hitCooldown -= Time.deltaTime;
        }

        float distanceToTeammate = Vector3.Distance(Player.transform.position, transform.position);

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
            if(nearestBall.transform.position.z > 17.31f && lastTouchPlayer.StartsWith("T1") && touchCount == 3f)
            {
                botMotor.MoveBot(defaultPos);   
            }
        }

        if(nearestBall != null)
        {
            rb = nearestBall.GetComponent<Rigidbody>();   
        }

        if(Vector3.Distance(transform.position, predictedBallPos) <= 0.3f && lastTouchTeam == "T2" && hitCooldown <=0f && lastTouchPlayer != botTeam + botPos) //code for first touch (bump)
        {
            if (distanceToTeammate <= 3f)
            {
                hitMultiplier = 1f;   
            }

            else if(distanceToTeammate > 3f)
            {
                hitMultiplier = Mathf.Clamp(1f + distanceToTeammate * 0.05f, 1f, 2.5f);
            }
            Vector3 hitDirection = Player.transform.position - transform.position;
            botMotor.HitBall("Hit", botTeam, hitDirection, hitMultiplier, botPos); //don't add touches bc its alr specified in botmotor
            hitCooldown = 0.25f;
        }

        if(Vector3.Distance(transform.position, predictedBallPos) <= 0.3f && lastTouchTeam == "T1" && touchCount == 1f && hitCooldown <= 0f && lastTouchPlayer != botTeam + botPos) //second touch/set
        {
            Vector3 otherBotPos = Player.transform.position;
            otherBotPos.z += 3f;

            Vector3 hitDirection = otherBotPos - transform.position;
            botMotor.HitBall("Front Set", botTeam, hitDirection, 1f, botPos);
            hitCooldown = 0.25f;
        }

        if(Vector3.Distance(transform.position, predictedBallPos) <= 0.3f && lastTouchTeam == "T1" && touchCount == 2f && hitCooldown <= 0f && lastTouchPlayer != botTeam + botPos) //3rd touch(spike/bump)
        {
            if(transform.position.z < 17.31f)// issue is here bc teammate has diff z coords
            {
                Vector3 randomPos = new Vector3(0, 0.26f, 26f);
                randomPos.x = Random.Range(4.5f, -4.5f);
                
                Vector3 hitDirection = randomPos - transform.position;
                hitMultiplier = 1.6f;
                botMotor.HitBall("Hit", botTeam, hitDirection, hitMultiplier, botPos);
            }

            if(transform.position.z > 14.02f && transform.position.z < 17.31f && nearestBall.transform.position.y >= 5f && rb.velocity.y < 0f && hitCooldown <= 0f && lastTouchPlayer != botTeam + botPos)
            {
                Vector3 randomPos = new Vector3(0, 0.26f, 26f);
                randomPos.x = Random.Range(4.5f, -4.5f);
                Vector3 hitDirection = randomPos - transform.position;

                hitMultiplier = Random.Range(0.6f, 2.4f);
                botMotor.Jump(2.35f);
                StartCoroutine(spikeAfterJump(hitDirection)); // reference below
                hitCooldown = 0.25f;
            }
        }

    }


    private IEnumerator spikeAfterJump(Vector3 hitDirection)
    {
        yield return new WaitForSeconds(0.65f);
        botMotor.HitBall("Hit", botTeam, hitDirection, hitMultiplier, botPos);
        manager.updateLastPlayerTouch(botTeam + botPos);
    }

}
