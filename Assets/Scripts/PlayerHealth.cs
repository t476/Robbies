using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//用于重置
public class PlayerHealth : MonoBehaviour
{
    public GameObject deathVFXprefab;
    public GameObject deathVFXprefab2;

    int trapsLayer;
    void Start()
    {
        trapsLayer = LayerMask.NameToLayer("Traps");
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.layer == 10) ;
        if (collision.gameObject.layer == trapsLayer)
        {
            Instantiate(deathVFXprefab2, transform.position, transform.rotation);
            Instantiate(deathVFXprefab, transform.position, Quaternion.Euler(0,0,Random.Range(-90,90)));
            gameObject.SetActive(false);
            AudioManager.PlayDeathAudio();
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameManager.PlayerDied();
        }
    }
   
}
