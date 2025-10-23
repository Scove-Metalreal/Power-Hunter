using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public GameObject gamePauseUI;
    public GameObject MenuOptionUI;
    public GameObject FullMapUI;

    [Header("Components & State")]
    public PlayerController playerController;
    public bool isGameEnd;
    public static int SceneIndex; // Note: This is kept for compatibility but new methods don't use it.
    
    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clickClip;

    // Data container for loading state across scenes
    public static SaveData dataToLoad = null;

    // Autosave settings
    private float autoSaveInterval = 300f; // 300 seconds = 5 minutes

    private void Awake()
    {
        // Implement Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        gamePauseUI.SetActive(false);
        MenuOptionUI.SetActive(false);
        isGameEnd = false;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        FullMapUI.SetActive(false);

        // If there is data to load from a continue action, apply it now.
        if (dataToLoad != null)
        {
            ApplyLoadedData();
        }

        // Start the autosave routine
        StartCoroutine(AutoSaveRoutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isGameEnd == false)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0) // Can pause in any playable scene
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                Pause();
                if (sfxSource != null && clickClip != null)
                {
                    sfxSource.PlayOneShot(clickClip);
                }
            }
        }
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                if (!gamePauseUI.activeSelf) // Don't hide cursor if game is paused
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        if (Input.GetKey(KeyCode.M))
        {
            FullMapUI.SetActive(true);
        }
        else { FullMapUI.SetActive(false); }
    }

    void OnApplicationQuit()
    {
        // This is called by Unity when the app is about to quit (e.g., Alt+F4).
        // We save here to prevent progress loss on a forced quit.
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Debug.Log("Application is quitting. Saving final game state...");
            SaveGameState();
        }
    }

    #region --- New Game Flow Methods ---

    // Method for the "New Game" button
    public void Play()
    {
        // A new game always starts from the first level (assuming build index 1)
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }

    // Method for the "Continue" button
    public void ContinueGame()
    {
        SaveData data = SaveManager.Instance.LoadGame();

        // Check if there is valid save data
        if (data != null && !string.IsNullOrEmpty(data.lastScene))
        {
            dataToLoad = data;
            SceneManager.LoadScene(data.lastScene);
        }
        else
        {
            // If no save data, start a new game instead
            Debug.LogWarning("No save data found! Starting a New Game.");
            Play();
        }
    }

    // Method for the "Exit" button
    public void Exitgame()
    {
        // Just tell the application to quit. The OnApplicationQuit() method will handle saving.
        Debug.Log("Exit button pressed. Application will now quit.");
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
        Application.Quit();
    }

    #endregion

    #region --- Save/Load Helpers ---

    public void SaveGameState()
    {
        if (playerController == null) return;
        PlayerStat playerStat = playerController.GetComponent<PlayerStat>();
        if (playerStat == null) return;

        SaveData data = new SaveData();
        // Player Position and Scene
        data.lastScene = SceneManager.GetActiveScene().name;
        data.playerPositionX = playerController.transform.position.x;
        data.playerPositionY = playerController.transform.position.y;
        data.playerPositionZ = playerController.transform.position.z;

        // Stats and Upgrades from PlayerStat
        data.maxHealth = playerStat.MaxHealth;
        data.maxStamina = playerStat.MaxStamina;
        data.maxLives = playerStat.MaxLives;
        data.powerValue = playerStat.PowerValue;
        data.healthUpgradeLevel = playerStat.healthUpgradeLevel;
        data.staminaUpgradeLevel = playerStat.staminaUpgradeLevel;
        data.livesUpgradeLevel = playerStat.livesUpgradeLevel;
        data.hasWallJump = playerStat.hasWallJump;
        data.jumpCooldownLevel = playerStat.jumpCooldownLevel;
        data.dashCooldownLevel = playerStat.dashCooldownLevel;

        SaveManager.Instance.SaveGame(data);
        Debug.Log("Game State Saved!");
    }

    private void ApplyLoadedData()
    {
        if (playerController == null) return;
        PlayerStat playerStat = playerController.GetComponent<PlayerStat>();
        if (playerStat == null) return;

        // Apply Player Position
        Vector3 position = new Vector3(dataToLoad.playerPositionX, dataToLoad.playerPositionY, dataToLoad.playerPositionZ);
        playerController.transform.position = position;

        // Apply Stats and Upgrades to PlayerStat
        playerStat.MaxHealth = dataToLoad.maxHealth;
        playerStat.MaxStamina = dataToLoad.maxStamina;
        playerStat.MaxLives = dataToLoad.maxLives;
        playerStat.PowerValue = dataToLoad.powerValue;
        playerStat.healthUpgradeLevel = dataToLoad.healthUpgradeLevel;
        playerStat.staminaUpgradeLevel = dataToLoad.staminaUpgradeLevel;
        playerStat.livesUpgradeLevel = dataToLoad.livesUpgradeLevel;
        playerStat.hasWallJump = dataToLoad.hasWallJump;
        playerStat.jumpCooldownLevel = dataToLoad.jumpCooldownLevel;
        playerStat.dashCooldownLevel = dataToLoad.dashCooldownLevel;
        
        // Restore current health/stamina to max after loading
        playerStat.HeathPlayer = playerStat.MaxHealth;
        playerStat.StaminaPlayer = playerStat.MaxStamina;
        playerStat.CurrentLives = playerStat.MaxLives;

        Debug.Log("Loaded data applied to player.");

        // Clear the static data holder after applying it
        dataToLoad = null;
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);

            // Only auto-save if in a playable scene and the game is not paused or over
            if (SceneManager.GetActiveScene().buildIndex != 0 && Time.timeScale > 0f && !isGameEnd)
            {
                Debug.Log("Auto-saving game state...");
                SaveGameState();
            }
        }
    }

    #endregion

    #region --- UI & Other Methods ---
    public void Option()
    {
        MenuOptionUI.SetActive(true);
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void Again()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1.0f;
        isGameEnd = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerController.GrafityDown = true;
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        gamePauseUI.SetActive(true);
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void Remuse()
    {
        Time.timeScale = 1f;
        gamePauseUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void MainMenu()
    {
        // Before going to the main menu, save the game state if we are in a playable scene.
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SaveGameState();
        }

        Time.timeScale = 1f; // Ensure time scale is reset
        SceneManager.LoadScene("MainMenu");
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void MainMenuInGameEndUI()
    {
        SceneManager.LoadScene("MainMenu");
        isGameEnd = false;
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void Back()
    {
        if (gamePauseUI.activeSelf == true && gamePauseUI != null)
        {
            Time.timeScale = 1f;
            gamePauseUI.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (sfxSource != null && clickClip != null)
            {
                sfxSource.PlayOneShot(clickClip);
            }
        }

        if (MenuOptionUI.activeSelf == true && MenuOptionUI != null)
        {
            MenuOptionUI.SetActive(false);
            if (sfxSource != null && clickClip != null)
            {
                sfxSource.PlayOneShot(clickClip);
            }
        }

    }
    #endregion
}