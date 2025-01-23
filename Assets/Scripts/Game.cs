using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour
{
    public float delay = 3f;
    public float timer = 100f;
    public float multTimer = 10f;
    
    public GameObject gameScripts;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private int _score = 0;
    public AudioClip startSound;
    public AudioClip coinSound;
    public AudioClip multSound;
    public AudioClip timeBonusSound;
    public AudioClip boostSound;

    private int _mult = 1;
    
    void StartAfterDelay()
    {
        gameScripts.SetActive(true);
        StartCoroutine(Timer());
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trigger"))
        {
            GetComponent<AudioSource>().PlayOneShot(startSound);
            Destroy(other.gameObject);
            Invoke(nameof(StartAfterDelay), 3f);
        }
        else if (other.CompareTag("Coin"))
        {
            UpdateScore(1*_mult);
            Destroy(other.gameObject);
        } else if (other.CompareTag("3Coin"))
        {
            UpdateScore(3*_mult);
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
        _score += change * _mult;
        scoreText.text = "" + _score;
        GetComponent<AudioSource>().PlayOneShot(coinSound);
    }

    IEnumerator Timer()
    {
        while (timer > 0)
        {
            timerText.text = "" + timer;
            yield return new WaitForSeconds(1f);
            timer -= 1;
        }
        gameScripts.SetActive(false);
    }

    IEnumerator Multiplier()
    {
        _mult = 2;
        yield return new WaitForSeconds(multTimer);
        _mult = 1;
    }
}
