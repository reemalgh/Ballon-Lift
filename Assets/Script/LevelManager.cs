using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject winPanel;
    public GameObject losePanel;

    public AudioSource audioSource;
    public AudioClip winSound;
    public AudioClip loseSound;

    public string levelSelectSceneName = "LevelSelect";
    public float returnDelay = 10f;

    public int balloonsLostToLose = 3;

    private int lostBalloons = 0;
    private bool gameEnded = false;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (winPanel != null)
            winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);
    }

    public void AddLostBalloon()
    {
        if (gameEnded) return;

        lostBalloons++;

        if (lostBalloons >= balloonsLostToLose)
        {
            Lose();
        }
    }

    public void Win()
    {
        if (gameEnded) return;

        gameEnded = true;

        if (winPanel != null)
            winPanel.SetActive(true);

        if (audioSource != null && winSound != null)
            audioSource.PlayOneShot(winSound);

        StartCoroutine(ReturnToLevelSelectAfterDelay());
    }

    public void Lose()
    {
        if (gameEnded) return;

        gameEnded = true;

        if (losePanel != null)
            losePanel.SetActive(true);

        if (audioSource != null && loseSound != null)
            audioSource.PlayOneShot(loseSound);

        Time.timeScale = 0f;
    }

    IEnumerator ReturnToLevelSelectAfterDelay()
    {
        yield return new WaitForSecondsRealtime(returnDelay);
        SceneManager.LoadScene(levelSelectSceneName);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSelectSceneName);
    }
}