using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 500f;
    [SerializeField] private float jumpForce = 500f;
    [SerializeField] private float coyoteTime = 0.1f; // time frame for player to jump after leaving the ground
    [SerializeField] private float friction = 0.9f; // friction applied when player is on the ground
    [SerializeField] private float acceleration = 2f; // acceleration speed
    private bool isJumping = false;
    private bool canJump = true;
    [SerializeField] private int jumpCount = 0;
    private float coyoteTimer;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GroundCheck();
        float moveX = Input.GetAxisRaw("Horizontal");
        if (moveX != 0)
        {
            rb.AddForce(new Vector2(moveX * acceleration, 0f), ForceMode2D.Impulse);
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed), rb.velocity.y);
        }
        else
        {
            if (!isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x * friction, rb.velocity.y);
                //rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }

        if (Input.GetButtonDown("Jump") && canJump && jumpCount < 2)
        {
            jumpCount++;
            if (isJumping)
            {
                // peak jump
                if (rb.velocity.y > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(new Vector2(0f, jumpForce * 1.5f), ForceMode2D.Impulse);
                }
                // curve jump
                else
                {
                    rb.AddForce(new Vector2(0f, jumpForce * 1.15f), ForceMode2D.Impulse);
                }
            }
            else
            {
                // normal jump
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                isJumping = true;
                canJump = false;
            }
        }
        if (!isJumping)
        {
            coyoteTimer = Time.time + coyoteTime;
        }

        if (!canJump && Time.time > coyoteTimer)
        {
            canJump = true;
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + 0.5f));
    }
    void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            isJumping = false;
            canJump = true;
            jumpCount = 0;
        }
    }
}
