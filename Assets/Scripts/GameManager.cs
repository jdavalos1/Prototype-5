using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> targets;
    public GameObject titleScreen;
    public GameObject pauseScreen;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI livesText;
    public Button restartButton;
    public bool isGameActive;
    public int lives;
    public bool isPaused;
    public bool isSwiping = false;
    public ParticleSystem trailParticles;

    private int score = 0;
    private float spawnRate = 1.0f;

    private AudioSource bgmSource;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        bgmSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPause();
        HandleMouseSwipe();
    }
    
    // Check to see if the user has paused the game
    // If the game is running stop everything else resume
    void CheckPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isGameActive && !titleScreen.activeSelf)
        {
            if (isPaused)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Handle if the particles should be set or not
    void HandleMouseSwipe()
    {
        if (Input.GetMouseButtonDown(0) && IsReadyToPlay())
        {
            isSwiping = true;
            trailParticles.gameObject.SetActive(true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
            trailParticles.gameObject.SetActive(false);
        }
    }
    
    private bool IsReadyToPlay()
    {
        return !isPaused && isGameActive && !titleScreen.activeSelf;
    }

    // Continuously spawn until the game has ended
    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);
            int index = Random.Range(0, targets.Count);
            Instantiate(targets[index]);
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }
    
    // Decreases the lives based on the decrease
    public void UpdateLives(int decrease)
    {
        if (isGameActive)
        {
            lives -= decrease;
            livesText.text = $"Lives: {lives}";
        }

        if (lives < 1) GameOver();
    }

    // Called whenever live < 1
    public void GameOver()
    {
        isGameActive = false;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }
    
    // Completely restart the scene as if it's new
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Start the game including setting the score and lives
    public void StartGame(int difficulty)
    {
        isGameActive = true;
        spawnRate /= difficulty;
        StartCoroutine(SpawnTarget());
        UpdateScore(score);
        UpdateLives(0);
        titleScreen.SetActive(false);
    }

    public void SetVolume(float newVolume)
    {
        bgmSource.volume = newVolume;
    }
    
    // Pauses all elements of the game
    public void PauseGame()
    {
        trailParticles.gameObject.SetActive(false);
        isSwiping = false;
        isPaused = true;
        Time.timeScale = 0f;
        bgmSource.Pause();
        pauseScreen.SetActive(true);
    }

    // Resumes all elements of the game
    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        bgmSource.UnPause();
        pauseScreen.SetActive(false);
    }
}
