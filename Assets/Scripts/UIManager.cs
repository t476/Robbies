using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //也是用单例模式这样比较方便从其他代码中访问uimanager
    static UIManager instance;
    public TextMeshProUGUI orbText, timeText, deathText, gameOverText;
    private void Awake()
    {
        //if (!instance)//问题的出现往往是由傻逼引起
        if (instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
   public static void UpdateOrbUI(int orbCount)
    {
        instance.orbText.text = orbCount.ToString();

    }
    
    public static void UpdateDeathUI(int deathCount)
    {
        instance.deathText.text = deathCount.ToString();
    }
    public static void UpdateTimeUI(float time)
    {   int min = (int)(time / 60);
        float sec = time % 60;
        instance.timeText.text = min.ToString("00")+":"+sec.ToString("00");

    }
    public static void DisplayGameOver()
    {
        instance.gameOverText.enabled = true;

    }
}
