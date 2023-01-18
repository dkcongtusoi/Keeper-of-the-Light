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
    private bool canCheck = true;
    private BoxCollider2D boxCollider2D;
    [SerializeField] private int jumpCount = 0;
    private float coyoteTimer;
    private Rigidbody2D rb;

    void Start()
    {
        boxCollider2D = transform.GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (IsGrounded() == true)
        {
            isJumping = false;
            canJump = true;
            if (jumpCount != 0)
            {
                jumpCount = 0;
            }
        }

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
            jumpCount++;
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
    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(boxCollider2D.bounds.center, Vector2.down, boxCollider2D.bounds.extents.y + .01f, LayerMask.GetMask("Ground"));
        Color rayColor;
        
        if (hit.collider != null)
        {
            rayColor = Color.green;
            //isJumping = false;
            //canJump = true;
            //jumpCount = 0;
        }
        else
        {
            rayColor = Color.red;
        }
        
        Debug.DrawRay(transform.position, Vector2.down * (boxCollider2D.bounds.extents.y + .01f), rayColor);
        return hit.collider != null;

    }
}


