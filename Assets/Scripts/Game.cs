using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Game : MonoBehaviour
{
    // public float delay = 3f;
    public float timer = 60f;
    public float multTimer = 5f;
    
    public GameObject gameScripts;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    public int score = 0;
    
    public AudioClip startSound;
    public AudioClip coinSound;
    public AudioClip multSound;
    public AudioClip timeBonusSound;
    public AudioClip boostSound;

    public Material litGate;
    public Material startGate;
    public Material litBase;

    private int _mult = 1;
    private bool _gameStarted = false;
    private int _checkpoints = 0;
    
    void StartGame()
    {
        if (_gameStarted) return;
        _gameStarted = true;
        gameScripts.SetActive(true);
        StartCoroutine(Timer());
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gate1"))
        {
            Destroy(other.gameObject);
            GetComponent<AudioSource>().PlayOneShot(startSound);
            Invoke(nameof(StopAudio), 0.75f); 
            _checkpoints++;

            var gate = GameObject.FindGameObjectWithTag("Gate2");
            gate.GetComponent<Renderer>().material = litGate;
            gate.GameObject().transform.Find("Base").GetComponent<Renderer>().material = litBase;
            gate.GameObject().transform.Find("Icon").GameObject().SetActive(true);
        }
        
        if (other.CompareTag("Gate2") && _checkpoints == 1)
        {
            Destroy(other.gameObject);
            GetComponent<AudioSource>().PlayOneShot(startSound);
            Invoke(nameof(StopAudio), 0.75f); 
            _checkpoints++;

            var gate = GameObject.FindGameObjectWithTag("Gate3");
            gate.GetComponent<Renderer>().material = litGate;
            gate.GameObject().transform.Find("Base").GetComponent<Renderer>().material = litBase;
            gate.GameObject().transform.Find("Icon").GameObject().SetActive(true);
        }
        
        if (other.CompareTag("Gate3") && _checkpoints == 2)
        {
            Destroy(other.gameObject);
            GetComponent<AudioSource>().PlayOneShot(startSound);
            Invoke(nameof(StopAudio), 0.75f); 
            _checkpoints++;

            var gate = GameObject.FindGameObjectWithTag("Trigger");
            gate.GetComponent<Renderer>().material = startGate;
            gate.GameObject().transform.Find("Base").GetComponent<Renderer>().material = litBase;
            gate.GameObject().transform.Find("Icon").GameObject().SetActive(true);
        }

        if (other.CompareTag("Trigger") && _checkpoints == 3)
        {
            GetComponent<AudioSource>().clip = startSound;
            GetComponent<AudioSource>().time = 3f; 
            GetComponent<AudioSource>().Play();
            
            Destroy(other.gameObject);
            // Invoke(nameof(StartGame), delay);
            StartGame();
        }
        else if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            UpdateScore(1*_mult);
        } 
        else if (other.CompareTag("3Coin"))
        {
            UpdateScore(3*_mult);
            Destroy(other.gameObject);
        } 
        else if (other.CompareTag("TimeBonus"))
        {
            Destroy(other.gameObject);
            timer += 5;
            GetComponent<AudioSource>().PlayOneShot(timeBonusSound);
        } 
        // else if (other.CompareTag("Boost"))
        // {
        //     // todo make boost work for multiplayer
        //     Destroy(other.gameObject);
        //     GetComponent<AudioSource>().PlayOneShot(boostSound);
        // } 
        else if (other.CompareTag("Multiplier"))
        {
            Destroy(other.gameObject);
            GetComponent<AudioSource>().PlayOneShot(multSound);
            StartCoroutine(Multiplier());
        }
    }
    
    void UpdateScore(int change)
    {
        score += change * _mult;
        scoreText.text = "" + score;
        GetComponent<AudioSource>().PlayOneShot(coinSound);
    }

    IEnumerator Timer()
    {
        while (timer > 0)
        {
            timer -= 1;
            timerText.text = "" + timer;
            yield return new WaitForSeconds(1f);
        }
        gameScripts.SetActive(false);
    }

    IEnumerator Multiplier()
    {
        _mult = 2;
        yield return new WaitForSeconds(multTimer);
        _mult = 1;
    }
    
    void StopAudio()
    {
        GetComponent<AudioSource>().Stop();
    }
}
