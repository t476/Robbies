using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator anim;
    PlayerMoverment movement;
    Rigidbody2D rb;


    int groundID;
    int fallID;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //获取父级组件
        movement = GetComponentInParent<PlayerMoverment>();
        groundID = Animator.StringToHash("isOnGround");
        fallID = Animator.StringToHash("verticalVelocity");
     }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("speed", Mathf.Abs(movement.xVelocity));
        // anim.SetBool("isOnGround", movement.isOnGround);
        anim.SetBool(groundID, movement.isOnGround);
        anim.SetBool("isHanging", movement.isHanging);
        anim.SetBool("isCrouching", movement.isCrouch);
        anim.SetFloat(fallID, rb.velocity.y);

    }
    public void StepAudio()
    {
        AudioManager.PlayFootStepAudio();
    }
    public void CrouchStepAudio()
    {
        AudioManager.PlayCrouchFootStepAudio();
    }
}
