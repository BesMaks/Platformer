using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{

    [SerializeField] float runSpeed = 4f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(1f, 10f);

    [SerializeField] float deathSlowDuration = 2f;
    [SerializeField] float deathSlowness = 0.3f;

    Rigidbody2D myRigidbody;
    Animator myAnimator;
    BoxCollider2D myBodyCollider2D;
    CapsuleCollider2D myFeetCollider2D;
    float gravityScaleAtStart;

    bool isAlive;
    void Start()
    {
        isAlive = true;
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<BoxCollider2D>();
        myFeetCollider2D = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if (!isAlive) return;

        Run();
        Jump();
        ClimbLadder();
        FlipSprite();
        Die();
    }

    public void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // [-1; 1]
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        //if player is running
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Running", playerHasHorizontalSpeed);
    }

    private void Jump()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump")
            && myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidbody.velocity += jumpVelocityToAdd;
        }
    }

    private void ClimbLadder()
    {
        if (!myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ladders")))
        {
            myAnimator.SetBool("Climbing", false);
            myRigidbody.gravityScale = gravityScaleAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, controlThrow * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        //if player is climbing
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), transform.localScale.y);
        }
    }

    private void Die()
    {
        if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");

            // Slowmo death
            IEnumerator death() {
                Time.timeScale = deathSlowness;
                yield return new WaitForSecondsRealtime(deathSlowDuration);
                Time.timeScale = 1f;
            }

            GetComponent<Rigidbody2D>().velocity = deathKick;
            myFeetCollider2D.size = new Vector2(myFeetCollider2D.size.x, myFeetCollider2D.size.y * 0.7f);

            FindObjectOfType<GameSession>().processPlayerDeath();
            StartCoroutine(death());
        }
    }

}
