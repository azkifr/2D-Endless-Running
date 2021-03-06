using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;
    private Rigidbody2D rig;

    [Header("Jump")]
    public float jumpAccel;
    
    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    [Header("Scoring")]
    public ScoreController score;
    [Header("GameOver")]
    public GameObject gameOverScreen;
    public float fallPositionY;
    [Header("Camera")]
    public CameraMoveController gameCamera;
    public float scoringRatio;
    private float lastPositionX;
    public LayerMask groundLayerMask;
    private Animator anim;
    private CharacterSoundController sound;
    private bool isJumping;
    private bool isOnGround;

        // Start is called before the first frame update
    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isJumping = true;
            sound.Playjump();          
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);
        if (hit)
        {
            if (!isOnGround && rig.velocity.y <= 0)
            {
                isOnGround = true;
            }
        }
        else
        {
             isOnGround = false;
        }
        anim.SetBool("isOnGround", isOnGround);
         int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
         int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

         if (scoreIncrement>0)
         {
             score.IncreaseCurrentScore(scoreIncrement);
             lastPositionX += distancePassed;
         }
         if(transform.position.y<fallPositionY)
         {
             GameOver();
         }
        
    }
    private void GameOver()
    {
        score.FinishScoring();
        gameCamera.enabled = false;
        gameOverScreen.SetActive(true);
         this.enabled = false;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {      
        Vector2 velocityVector = rig.velocity;
        if (isJumping)
        {
            velocityVector.y += jumpAccel;
            isJumping = false;
        }
        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);
        rig.velocity = velocityVector;
    }
    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }
}
