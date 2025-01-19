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
    
    void StartAfterDelay()
    {
        gameScripts.SetActive(true);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trigger"))
        {
            Invoke(nameof(StartAfterDelay), 3f);
        }
        else if (other.CompareTag("Coin"))
        {
            score += 1;
            UpdateScore();
            Destroy(other.gameObject);
        } else if (other.CompareTag("3Coin"))
        {
            score += 3;
            UpdateScore();
            Destroy(other.gameObject);
        } else if (other.CompareTag("TimeBonus"))
        {
            timer += 10;
            Destroy(other.gameObject);
        } else if (other.CompareTag("Boost"))
        {
            // todo make boost work
            Destroy(other.gameObject);
        } else if (other.CompareTag("Multiplier"))
        {
            // todo make multiplier
            Destroy(other.gameObject);
        }
    }
    
    void UpdateScore()
    {
        scoreText.text = "score: " + score;
    }
}
