using System.Collections;
using System.Net.Mail;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;
using UnityEngine.Rendering.UI;

public class Game : MonoBehaviour
{
    // public float delay = 3f;
    public DrawTrail drawTrail;
    public float timer = 60f;
    public float multTimer = 5f;
    
    public GameObject gameScripts;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    public int score = 0;
    
    public AudioClip startSound1;
    public AudioClip startSound2;
    
    
    public AudioClip coinSound;
    public AudioClip multSound;
    public AudioClip timeBonusSound;
    public AudioClip boostSound;

    public GameObject gate1;
    public GameObject gate2;
    public GameObject gate3;
    public GameObject gate4;

    public Material gateMaterial;
    public Material baseMaterial;
    public Material startGate;
    public Material failGate;

    private int _mult = 1;
    private bool _gameStarted = false;
    private int _checkpoints = 0;

    private float _startTime;
    
    void StartGame()
    {
        if (_gameStarted) return;
        _gameStarted = true;
        gameScripts.SetActive(true);
        StartCoroutine(Timer());
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gate1") && _checkpoints == 0)
        {
            _startTime = Time.time;
            gate1.GetComponentInChildren<ParticleSystem>().Play();
            other.gameObject.SetActive(false);
            
            EnableGate(gate2);
        }
        
        if (other.CompareTag("Gate2") && _checkpoints == 1)
        {
            gate2.GetComponentInChildren<ParticleSystem>().Play();
            other.gameObject.SetActive(false);
            
            EnableGate(gate3);
        }
        
        if (other.CompareTag("Gate3") && _checkpoints == 2)
        {
            gate3.GetComponentInChildren<ParticleSystem>().Play();
            other.gameObject.SetActive(false);
            
            var elapsedTime = Time.time - _startTime;
            if (elapsedTime is < 2.75f or > 4f)
            {
                EnableGate(gate4, speedFault:true);
            }
            else
            {
                EnableGate(gate4, final:true);
            }
        }

        if (other.CompareTag("Gate4") && _checkpoints == 3)
        {
            gate4.GetComponentInChildren<ParticleSystem>().Play();
            GetComponent<AudioSource>().PlayOneShot(startSound2);
            
            other.gameObject.SetActive(false);
            Invoke(nameof(StartGame), 0);
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
        // todo make boost work for multiplayer
        // else if (other.CompareTag("Boost"))
        // {
        //     
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
        for (int i = 0; i < change; i++)
        {
            GetComponent<AudioSource>().PlayOneShot(coinSound);
            drawTrail.segmentTotal += 3;
        }
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

    void EnableGate(GameObject g, bool final = false, bool speedFault = false)
    {
        GetComponent<AudioSource>().PlayOneShot(startSound1);
        _checkpoints++;
        var gate = g.transform.Find("Trigger");
        gate.GetComponent<Renderer>().material = final ? startGate : (speedFault ? failGate : gateMaterial);
        gate.transform.Find("Base").GetComponent<Renderer>().material = baseMaterial;
        gate.transform.Find(speedFault ? "Icon (X)" : "Icon").GameObject().SetActive(true);
    }
}
