using System.Collections;
using System.Net.Mail;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;
using UnityEngine.Rendering.UI;

public class Game : MonoBehaviour
{
    public float delay = 0f;
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

    public GameObject activeGate;
    public GameObject blockedGate;
    public GameObject finalGate;

    private int _mult = 1;
    private bool _gameStarted = false;
    private int _checkpoints = 0;

    private float _startTime;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    void StartGame()
    {
        if (_gameStarted) return;
        _gameStarted = true;
        gameScripts.SetActive(true);
        StartCoroutine(Timer());
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gate"))
        {
            StartCoroutine(HandleCheckpoint(other));
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
            drawTrail.segmentTotal += 5;
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
    
    private IEnumerator HandleCheckpoint(Collider gate)
    {
        switch (_checkpoints)
        {
            case 0:
                _startTime = Time.time;
                gate1.GetComponentInChildren<ParticleSystem>().Play();
                gate.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.01f);
                gate2 = EnableGate(gate2);
                break;

            case 1:
                gate2.GetComponentInChildren<ParticleSystem>().Play();
                gate.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.01f);
                gate3 = EnableGate(gate3);
                break;

            case 2:
                gate3.GetComponentInChildren<ParticleSystem>().Play();
                gate.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.01f);

                var elapsedTime = Time.time - _startTime;
                gate4 = elapsedTime is < 2.75f or > 4f ? EnableGate(gate4, speedFault: true) : EnableGate(gate4, final: true);
                break;

            case 3:
                gate4.GetComponentInChildren<ParticleSystem>().Play();
                _audioSource.PlayOneShot(startSound2);
                gate.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.01f);
                Invoke(nameof(StartGame), delay);
                break;
        }
    }

    GameObject EnableGate(GameObject g, bool final = false, bool speedFault = false)
    {
        _audioSource.PlayOneShot(startSound1);
        _checkpoints++;
        
        var position = g.transform.position;
        var rotation = g.transform.rotation;
        Destroy(g);
        
        return Instantiate(final ? finalGate : speedFault ? blockedGate : activeGate, position, rotation);

    }
}
