using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    int openID;

    void Start()
    {
        anim = GetComponent<Animator>();
        openID = Animator.StringToHash("Open");
        //register door to gamemanager
        GameManager.RegisterDoor(this);


    }
    public void Open()
    {
        anim.SetTrigger(openID);
        AudioManager.PlayDoorOpenAudio();
        //打开门后播放audio
    }
  
}
