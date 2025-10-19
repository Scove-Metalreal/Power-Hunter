using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // UI references are now private and found by tag
    private GameObject gamePauseUI;
    private GameObject MenuOptionUI;
    private GameObject FullMapUI;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clickClip;

    private PlayerStat playerStat;
    public bool isGameEnd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerStat = FindObjectOfType<PlayerStat>();

        // Find UI elements by tag. This is more flexible than public references.
        // You will need to tag your UI GameObjects in the inspector.
        gamePauseUI = GameObject.FindWithTag("PauseUI");
        MenuOptionUI = GameObject.FindWithTag("OptionsUI");
        if (scene.name != "MainMenu")
        {
            FullMapUI = GameObject.FindWithTag("FullMapUI");
        }

        // Reset UI state
        if (gamePauseUI != null) gamePauseUI.SetActive(false);
        if (MenuOptionUI != null) MenuOptionUI.SetActive(false);
        if (FullMapUI != null) FullMapUI.SetActive(false);
        isGameEnd = false;

        // Set cursor state
        if (scene.name == "MainMenu")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // ... The rest of the file is the same ...
    void Update()
    {
        if (playerStat != null && !isGameEnd && SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
            if (Input.GetKeyDown(KeyCode.M) && FullMapUI != null)
            {
                FullMapUI.SetActive(!FullMapUI.activeSelf);
            }
        }
    }

    public void TogglePause()
    {
        if (gamePauseUI == null) return;
        if (Time.timeScale > 0) Pause();
        else Remuse();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        if (gamePauseUI != null) gamePauseUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PlayClickSound();
    }

    public void Remuse()
    {
        Time.timeScale = 1f;
        if (gamePauseUI != null) gamePauseUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayClickSound();
    }

    public void NewGame()
    {
        // Delete old save data for a clean start
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSaveData();
        }

        SceneManager.LoadScene("Level 1");
        Time.timeScale = 1f;
    }

    public void ContinueGame()
    {
        if (SaveManager.Instance != null)
        {
            SaveData data = SaveManager.Instance.LoadGame();
            if (!string.IsNullOrEmpty(data.lastScene) && data.lastScene != "MainMenu")
            {
                SceneManager.LoadScene(data.lastScene);
                Time.timeScale = 1f;
            }
            else
            {
                NewGame();
            }
        }
    }

    public void SaveAtCheckpoint()
    {
        if (playerStat != null)
        {
            playerStat.SaveStats();
            Debug.Log("Game saved at checkpoint!");
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        PlayClickSound();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        if (playerStat != null)
        {
            playerStat.SaveStats();
        }
    }

    public void ShowGameOverUI()
    {
        isGameEnd = true;
        if (gamePauseUI != null) gamePauseUI.SetActive(false);

        // Find the Lose UI by tag and activate it.
        GameObject loseUIPanel = GameObject.FindWithTag("LoseUI");
        if (loseUIPanel != null)
        {
            loseUIPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Could not find GameObject with tag 'LoseUI'");
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // --- UI Methods ---

    public void Option()
    {
        if (MenuOptionUI != null) MenuOptionUI.SetActive(true);
        PlayClickSound();
    }

    public void Back()
    {
        if (MenuOptionUI != null && MenuOptionUI.activeSelf) MenuOptionUI.SetActive(false);
        PlayClickSound();
    }

    private void PlayClickSound()
    {
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
}
