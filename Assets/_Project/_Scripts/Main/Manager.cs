using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pause;
    public GameObject lose;
    public GameObject win;
    public GameObject pauseButton;


private bool isPaused = false;
    private bool isGameOver = false;

    void Start()
    {
        pause.SetActive(false);
        lose.SetActive(false);
        win.SetActive(false);

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
 
        if (isGameOver) return;

        isPaused = !isPaused;
        pause.SetActive(isPaused);
        pauseButton.SetActive(!isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pause.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1f;
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowLose()
    {
        isGameOver = true;
        lose.SetActive(true);
        pauseButton.SetActive(false); 
        Time.timeScale = 0f;
        Debug.Log(">>> ShowLose chạy, pauseButton đã tắt");
    }

    public void ShowWin()
    {
        isGameOver = true;
        win.SetActive(true);
        pauseButton.SetActive(false); 
        Time.timeScale = 0f; 
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }


}
