using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;//singleton模式：单例类
    SceneFader fader;
    //用列表的方式生成宝珠
    List<Orb> orbs;
    Door lockedDoor;
    float gameTime=0f;
    bool gameIsOver;
    //public int orbNum;
    public int deathNum;
    private void Awake()
    {
        if(instance!=null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        orbs = new List<Orb>();//
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        if (gameIsOver)
        {
            return;
        }
        // orbNum = instance.orbs.Count;
        gameTime += Time.deltaTime;
        UIManager.UpdateTimeUI(gameTime);
    }
    public static void PlayerWon()
    {
        instance.gameIsOver = true;
        UIManager.DisplayGameOver();//赢了以后出现 gameover也太奇怪了，是一个可以改的点
        AudioManager.PlayerWonAudio();
        

    }
    public static bool GameOver()
    {
        return instance.gameIsOver;
    }
    public static void RegisterDoor(Door door)
    {
        instance.lockedDoor = door;//在gm里注册门,这样写的原因是为了其他组件都可以通过gm访问到，有一个统一的管理器
    }
    public static void RegisterOrb(Orb orb) 
    {
        if (!instance.orbs.Contains(orb))
            instance.orbs.Add(orb);
        UIManager.UpdateOrbUI(instance.orbs.Count);
    }
    public static void RegisterSceneFader(SceneFader obj)//这是我写的类
    {
        instance.fader = obj;

    }
    public static void PlayerGrabbedOrb(Orb orb)
    {
        if (!instance.orbs.Contains(orb))
            return;
        instance.orbs.Remove(orb);
        UIManager.UpdateOrbUI(instance.orbs.Count);
        if (instance.orbs.Count == 0)
        {
            instance.lockedDoor.Open();

        }
    }
    public static void PlayerDied()
    {
        instance.fader.FadeOut();//噢~
        instance.deathNum++;
        instance.Invoke("RestartScene", 1.5f);
        UIManager.UpdateDeathUI(instance.deathNum);
    }
    void RestartScene()
    {
        instance.orbs.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
} 




