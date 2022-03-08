using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoverment : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;

    public float xVelocity;//速度
    [Header("跳跃参数")]
    public float jumpForce = 6.3f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration = 0.1f;
    public float crouchJumpBoost = 3.0f;
    public float hangingJumpForce = 15f;
    float jumpTime;


    [Header("状态")]
    public bool isCrouch;
    public bool isOnGround;
    public bool isJump;
    public bool isHeadBlocked;
    public bool isHanging;

    [Header("环境检测")]
    public float footOffset = 0.39f;
    public float headClearance = 0.5f;
    public float groundDistance = 0.2f;
    float playerHeight;
    public float eyeHeight = 1.5f;
    public float grabDistance = 0.4f;
    public float reachOffset = 0.7f;
    


    public LayerMask groundLayer;


    //按键设置
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;
    bool crouchPressed;
        
    //碰撞体尺寸
    Vector2 colliderStandSize;
    Vector2 colliderStandOffset;
    Vector2 colliderCrouchSize;
    Vector2 colliderCrouchOffset;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        playerHeight = coll.size.y;
        colliderStandSize = coll.size;
        colliderStandOffset = coll.offset;
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);
        colliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);


        
    }

    
    void Update()
    {//api手册有写:所有跟getbutton有关的放在update里比较好
        if (GameManager.GameOver())
        {
            return;
        }
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
        crouchPressed = Input.GetButtonDown("Crouch");
        
    }
    private void FixedUpdate()
    {
        if (GameManager.GameOver())
        {
            return;
        }
        PhysicsCheck();
        MidAirMovement();
        GroundMovement();
    }
    //判断是否在地面上
    void PhysicsCheck()
    {   
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0.1f), Vector2.down,groundDistance,groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0.1f), Vector2.down, groundDistance,groundLayer);


        if (leftCheck||rightCheck)
      //if(coll.IsTouchingLayers(groundLayer))
            isOnGround = true;
        else isOnGround = false;
        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);
        if (headCheck)
            isHeadBlocked = true;
        else isHeadBlocked = false;

        float direction = transform.localScale.x;//-1/1;
        Vector2 grabDir = new Vector2(direction, 0f);

        RaycastHit2D blockCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance, groundLayer);
        RaycastHit2D wallCheck =Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance, groundLayer);
        RaycastHit2D ledgeCheck= Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance, groundLayer);

        if(!isOnGround&&rb.velocity.y<0f&&ledgeCheck&&wallCheck&&!blockCheck)
        {
            Vector3 pos = transform.position;
            pos.x += (wallCheck.distance-0.05f)*direction;//厉害，射线到最近碰撞体得长度
            pos.y -= ledgeCheck.distance;
            transform.position = pos;

            rb.bodyType = RigidbodyType2D.Static;
            isHanging = true;
        }

    }
    void GroundMovement()
    {   if (isHanging)
            return;
        if (crouchHeld && !isCrouch && isOnGround)
            Crouch();
        else if (!crouchHeld && isCrouch&&!isHeadBlocked)
        {
            StandUp();
        }
        else if (!isOnGround && isCrouch)
            StandUp();
        xVelocity = Input.GetAxis("Horizontal");//轴
        if (isCrouch)
            xVelocity /= crouchSpeedDivisor;

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);
        FilpDirction();
    }
    void MidAirMovement()
    {
        if (isHanging)
        {
            if (jumpPressed)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = new Vector2(rb.velocity.x, hangingJumpForce);
                isHanging = false;
            }
            if (crouchPressed)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                isHanging = false;
            }
        }

        if (jumpPressed && isOnGround && !isJump && !isHeadBlocked)
        {
            if (isCrouch & isOnGround)
            {
                StandUp();
                rb.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            }
            isOnGround = false;
            isJump = true;

            jumpTime = Time.time + jumpHoldDuration;

            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);//突然得力模式

            AudioManager.PlayJumpAudio();
        }
        else if (isJump)
        {
            if (jumpHeld)
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);//力更小了？？？
            if (jumpTime < Time.time)
                isJump = false;
        }
    }
    void FilpDirction()
    {//不用三维的话z轴默认变回0了
        if (xVelocity < 0)
            transform.localScale = new Vector3(-1, 1,1);
        if (xVelocity > 0)
            transform.localScale = new Vector3(1, 1,1);

    }
    void Crouch()
    {
        isCrouch = true;
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffset;


    }
    void StandUp()
    {   

        isCrouch = false;
        coll.size = colliderStandSize;
        coll.offset = colliderStandOffset;


    }
    //重载/重写射线，比碰撞体判断更优秀，因为要判断的点更多
    RaycastHit2D Raycast(Vector2 offset,Vector2 rayDiraction,float length,LayerMask layer)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);
        Debug.DrawRay(pos + offset, rayDiraction *length);
        return hit;

    }

}
