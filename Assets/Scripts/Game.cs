using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour
{
    public float delay = 3f;
    public int score = 0;
    public float timer = 100f;
    public GameObject gameScripts;
    public TextMeshProUGUI scoreText;
    public float multTimer = 10f;

    public AudioClip coinSound;
    public AudioClip multSound;
    public AudioClip timeBonusSound;
    public AudioClip boostSound;

    private int mult = 1;
    
    void StartAfterDelay()
    {
        gameScripts.SetActive(true);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trigger"))
        {
            Destroy(other.gameObject);
            Invoke(nameof(StartAfterDelay), 3f);
        }
        else if (other.CompareTag("Coin"))
        {
            UpdateScore(1*mult);
            Destroy(other.gameObject);
        } else if (other.CompareTag("3Coin"))
        {
            UpdateScore(3*mult);
            Destroy(other.gameObject);
        } else if (other.CompareTag("TimeBonus"))
        {
            timer += 10;
            GetComponent<AudioSource>().PlayOneShot(timeBonusSound);
            Destroy(other.gameObject);
        } else if (other.CompareTag("Boost"))
        {
            // todo make boost work
            GetComponent<AudioSource>().PlayOneShot(boostSound);
            Destroy(other.gameObject);
        } else if (other.CompareTag("Multiplier"))
        {
            GetComponent<AudioSource>().PlayOneShot(multSound);
            StartCoroutine(Multiplier());
            Destroy(other.gameObject);
        }
    }
    
    void UpdateScore(int change)
    {
        score += change * mult;
        scoreText.text = "" + score;
        GetComponent<AudioSource>().PlayOneShot(coinSound);
    }

    IEnumerator Multiplier()
    {
        mult = 2;
        yield return new WaitForSeconds(multTimer);
        mult = 1;
    }
}
