using System.Collections;
using System.Net.Mail;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public float startTimeout = 6f;
    public float startThreshold = 1.5f;
    
    public DrawTrail drawTrail;
    public float timer = 80f;
    public float multTimer = 7.5f;
    public int trailDelta = 5;
    public float timeDelta = 7.5f;
    
    public GameObject gameScripts;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public GameObject showMultiplier;

    public int score = 0;
    
    public AudioClip startSound1;
    public AudioClip startSound2;
    
    
    public AudioClip coinSound;
    public AudioClip multSound;
    public AudioClip multEndSound;
    public AudioClip timeBonusSound;
    public AudioClip errorSound;
    // public AudioClip boostSound;

    public GameObject gate1;
    public GameObject gate2;
    public GameObject gate3;
    public GameObject gate4;

    public GameObject activeGate;
    public GameObject blockedGate;
    public GameObject finalGate;
    
    public VignetteController vignette;

    public GameObject tryAgainText;
    public GameObject playAgainText;
    public GameObject startingArea;

    private int _mult = 1;
    private bool _gameStarted = false;
    private int _checkpoints = 0;

    private float _startTime;
    private AudioSource _audioSource;
    
    private readonly Color _defaultColor = new Color(0f, 0.7f, 0.7f);
    private readonly Color _coinColor = new Color(0.9f, 0.9f, 0.4f);
    private readonly Color _3CoinColor = new Color(0.9f, 0.6f, 0.3f);
    private readonly Color _timeBonusColor = new Color(0.2f, 0.4f, 0.9f);
    private readonly Color _multiplierColor = new Color(0.4f, 0.9f, 0.4f);
    private const float VignetteIntensity = 0.4f;

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
            StartCoroutine(SetVignette(_defaultColor));
            StartCoroutine(HandleCheckpoint(other));
        }
        else if (other.CompareTag("DenyGate"))
        {
            _audioSource.PlayOneShot(errorSound);
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("StartingArea"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (other.CompareTag("Coin"))
        {
            StartCoroutine(SetVignette(_coinColor));
            
            _audioSource.PlayOneShot(coinSound);
            Destroy(other.gameObject);
            UpdateScore(1*_mult);
        } 
        else if (other.CompareTag("3Coin"))
        {
            StartCoroutine(SetVignette(_3CoinColor));
            
            StartCoroutine(Handle3CoinSound());
            UpdateScore(3*_mult);
            Destroy(other.gameObject);
        } 
        else if (other.CompareTag("TimeBonus"))
        {
            StartCoroutine(SetVignette(_timeBonusColor));
            
            Destroy(other.gameObject);
            timer += timeDelta;
            _audioSource.PlayOneShot(timeBonusSound);
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
            StartCoroutine(Multiplier());
        }
    }
    void UpdateScore(int change)
    {
        score += change * _mult;
        scoreText.text = "" + score;
        for (int i = 0; i < change; i++)
        {
            drawTrail.segmentTotal += trailDelta;
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
        playAgainText.SetActive(true);
        startingArea.SetActive(true);
    }

    IEnumerator Multiplier()
    {
        _audioSource.PlayOneShot(multSound);
        _mult = 2;
        showMultiplier.SetActive(true);
        for (int i = 1; i < multTimer; i++)
        {
            StartCoroutine(SetVignette(_multiplierColor));
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(multTimer - math.floor(multTimer));
        _audioSource.PlayOneShot(multEndSound);
        showMultiplier.SetActive(false);
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
                if (elapsedTime < startThreshold || elapsedTime > startTimeout)
                {
                    tryAgainText.SetActive(true);
                    startingArea.SetActive(true);
                    gate4 = EnableGate(gate4, speedFault: true);
                }
                else
                {
                    gate4 = EnableGate(gate4, final: true);
                }
                break;

            case 3:
                gate4.GetComponentInChildren<ParticleSystem>().Play();
                _audioSource.PlayOneShot(startSound2);
                gate.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.01f);
                StartGame();
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

    private IEnumerator Handle3CoinSound()
    {
        _audioSource.PlayOneShot(coinSound);
        yield return new WaitForSeconds(0.1f);
        _audioSource.PlayOneShot(coinSound);
        yield return new WaitForSeconds(0.1f);
        _audioSource.PlayOneShot(coinSound);
    }

    public IEnumerator SetVignette(Color color, float intensity = VignetteIntensity)
    {
        vignette.SetColor(color, 0.25f);
        vignette.SetIntensity(intensity, 0.25f);
        yield return new WaitForSeconds(0.5f);
        vignette.SetIntensity(0, 0.5f);
    }
}
