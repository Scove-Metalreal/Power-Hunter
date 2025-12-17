using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Required for Coroutines

public class GameManager : MonoBehaviour
{
    // Singleton Instance for the current scene
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public GameObject gamePauseUI;
    public GameObject MenuOptionUI;
    public GameObject FullMapUI;
    public GameObject TutorialUI;

    [Header("Components & State")]
    public PlayerController playerController;
    public bool isGameEnd;
    
    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clickClip;

    [Header("Shop")] 
    [SerializeField] private GameObject button1;[SerializeField] private GameObject button2;[SerializeField] private GameObject button3;[SerializeField] private GameObject button4;
    [SerializeField] private GameObject SOLD1;[SerializeField] private GameObject SOLD2;[SerializeField] private GameObject SOLD3;[SerializeField] private GameObject SOLD4;
    [SerializeField] private TextMeshProUGUI Text1;[SerializeField] private TextMeshProUGUI Text2;[SerializeField] private TextMeshProUGUI Text3;[SerializeField] private TextMeshProUGUI Text4;
    private static int buyCount1 = 0;private static int buyCount2 = 0;private static int buyCount3 = 0;private static int buyCount4 = 0;
    // Data container for loading state across scenes
    public static SaveData dataToLoad = null;

    // Autosave settings
    private float autoSaveInterval = 300f; // 300 seconds = 5 minutes
    public static float playTime = 0f;
    public PlayerStat playerStat;
    public GameObject[] LifeUI;
    private FBHeath fbHeath;
    private void Awake()
    {
        // Set the static instance to this GameManager for the current scene.
        Instance = this;

        // Subscribe to the scene unloaded event to act as a safety net.
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe when this GameManager is destroyed to prevent memory leaks.
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        if (playerStat != null)
        {
            playerStat.OnStatsChanged -= SaveGameState;
        }
    }

    // This method is a safety net that automatically preserves player stats
    // if a scene is changed by calling SceneManager.LoadScene() directly,
    // instead of using the proper GameManager.GoToScene() method.
    private void OnSceneUnloaded(Scene current)
    {
        // If dataToLoad is null, it means GoToScene() was NOT called.
        if (dataToLoad == null && current.buildIndex != 0)
        {
            Debug.LogWarning("GameManager: Unmanaged scene transition detected! Saving stats...");
            
            SaveData data = CreateSaveDataObject();
            if (data != null)
            {
                // Primary method: use the static variable.
                dataToLoad = data;

                // Backup method: save data to PlayerPrefs in case the static variable is lost.
                string json = JsonUtility.ToJson(data);
                PlayerPrefs.SetString("stat_transfer_backup", json);
                PlayerPrefs.Save();
                Debug.Log("GameManager: Player stats backed up to PlayerPrefs for scene transition.");
            }
        }
    }

    void Start()
    {
        fbHeath = FindAnyObjectByType<FBHeath>();
        playerController = FindAnyObjectByType<PlayerController>();
        playerStat = FindAnyObjectByType<PlayerStat>();
        if (playerStat != null)
        {
            playerStat.OnStatsChanged += SaveGameState;
            Debug.Log("GameManager: Subscribed to PlayerStat changes for auto-saving.");
        }

        if (gamePauseUI != null) gamePauseUI.SetActive(false);
        if (MenuOptionUI != null) MenuOptionUI.SetActive(false);
        if (FullMapUI != null) FullMapUI.SetActive(false);
        if (TutorialUI != null)
        {
            TutorialUI.SetActive(false);
        }
        else { }

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

        // If dataToLoad (static variable) is null, check for a PlayerPrefs backup.
        if (dataToLoad == null && PlayerPrefs.HasKey("stat_transfer_backup"))
        {
            Debug.LogWarning("GameManager.Start: dataToLoad was null, loading from PlayerPrefs backup.");
            string json = PlayerPrefs.GetString("stat_transfer_backup");
            dataToLoad = JsonUtility.FromJson<SaveData>(json);
            PlayerPrefs.DeleteKey("stat_transfer_backup"); // Clean up the backup key.
        }

        // If there is data to load, apply it. Otherwise, set up a new game.
        if (dataToLoad != null)
        {
            Debug.Log("GameManager.Start: Found data to load. Applying stats.");
            StartCoroutine(ApplyLoadedDataRoutine());
        }
        else
        {
            Debug.LogWarning("GameManager.Start: No data to load found. Resetting stats for a new game.");
            // This is a new game
            playerStat = FindAnyObjectByType<PlayerStat>();
            if (playerStat != null)
            {
                playerStat.ResetStats();
            }
           
            StartCoroutine(SetupNewGamePlayerState());
        }
        
        StartCoroutine(AutoSaveRoutine());
    }

    private IEnumerator SetupNewGamePlayerState()
    {
        // Wait for all Start() methods to run, ensuring components like Rigidbody are initialized.
        yield return new WaitForEndOfFrame();

        if (playerController != null && playerController.Rigidbody != null)
        {
            playerController.ApplyGravityDirection(GravityDirection.Down);
            playerController.Rigidbody.gravityScale = playerController.DefaultGravityScale;
        }
        else
        {
            Debug.LogWarning("GameManager could not set initial player gravity. PlayerController or its Rigidbody was not found.");
        }
    }

    void Update()
    {
        // Proactively keep references to the player components cached.
        if (playerController == null)
        {
            playerController = FindAnyObjectByType<PlayerController>();
        }
        if (playerStat == null && playerController != null)
        {
            playerStat = playerController.GetComponent<PlayerStat>();
        }

        if (Time.timeScale == 1f)
        {
            string scene = SceneManager.GetActiveScene().name;

            if (scene == "Level1" || scene == "Level2" || scene == "FinalBoss")
            {
                playTime += Time.deltaTime;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameEnd)
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                Pause();
                if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
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
                if (gamePauseUI != null && !gamePauseUI.activeSelf)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        if (Input.GetKey(KeyCode.M))
        {
            if (FullMapUI != null) FullMapUI.SetActive(true);
        }
        else { if (FullMapUI != null) FullMapUI.SetActive(false); }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (playerStat != null)
            {
                playerStat.AddPowerValue(100);
                Debug.Log("CHEAT: +100 Power");
            }
        }

        if (playerStat != null && playerStat.CurrentLives > 0)
        {
            for (int i = 0; i < LifeUI.Length; i++)
            {
                if (i < playerStat.CurrentLives)
                    LifeUI[i].SetActive(true);
                else
                    LifeUI[i].SetActive(false);
            }
        }

        if (fbHeath != null && fbHeath.Bossheath <= 0 )
        {
            buyCount1 = 0;
            buyCount2 = 0;
            buyCount3 = 0;
            buyCount4 = 0;
        }
        if (playerStat != null && playerStat.CurrentLives <= 0 )
        {
            buyCount1 = 0;
            buyCount2 = 0;
            buyCount3 = 0;
            buyCount4 = 0;
        }
    }

    void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Debug.Log("Application is quitting. Saving final game state...");
            SaveGameState();
        }
    }
    public static void ResetTimer()
    {
        playTime = 0f;
    }

    public static string FormatTime()
    {
        int minutes = Mathf.FloorToInt(playTime / 60);
        int seconds = Mathf.FloorToInt(playTime % 60);
        int milliseconds = Mathf.FloorToInt((playTime * 1000) % 1000);


        return $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }
    #region --- New Game Flow Methods ---

    public void NewGame()
    {
        // Clear any lingering transfer data to signify a new game.
        if(SaveManager.Instance != null) SaveManager.Instance.dataToTransfer = null;

        // Delete the existing save file to ensure a truly fresh start.
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSaveData();
            Debug.Log("GameManager: Existing save data deleted for New Game.");
        }

        SceneManager.LoadScene(1); // Assuming build index 1 is the first level
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
    }

    public void ContinueGame()
    {
        SaveData data = SaveManager.Instance.LoadGame();

        if (data != null && !string.IsNullOrEmpty(data.lastScene))
        {
            dataToLoad = data;
            SceneManager.LoadScene(data.lastScene);
        }
        else
        {
            Debug.LogWarning("No save data found! Starting a New Game.");
            NewGame();
        }
    }

    public void Exitgame()
    {
        Debug.Log("Exit button pressed. Application will now quit.");
        if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
        Application.Quit();
    }

    #endregion

    #region --- Save/Load Helpers ---

    // Creates a SaveData object from the current player state.
    private SaveData CreateSaveDataObject()
    {
        // Use the references cached by GameManager's Start() or Update() methods.
        if (this.playerController == null || this.playerStat == null)
        {
            Debug.LogError("Save Failed: GameManager is missing a required reference to PlayerController or PlayerStat.");
            return null;
        }

        SaveData data = new SaveData();
        data.lastScene = SceneManager.GetActiveScene().name;
        data.playerPositionX = this.playerController.transform.position.x;
        data.playerPositionY = this.playerController.transform.position.y;
        data.playerPositionZ = this.playerController.transform.position.z;

        // Determine and save current gravity direction
        if (this.playerController.GrafityUp) data.gravityDirection = GravityDirection.Up;
        else if (this.playerController.GrafityLeft) data.gravityDirection = GravityDirection.Left;
        else if (this.playerController.GrafityRight) data.gravityDirection = GravityDirection.Right;
        else data.gravityDirection = GravityDirection.Down;

        // Pull all stats from the cached playerStat reference.
        data.maxHealth = this.playerStat.MaxHealth;
        data.heathPlayer = this.playerStat.HeathPlayer;
        data.maxStamina = this.playerStat.MaxStamina;
        data.maxLives = this.playerStat.MaxLives;
        data.currentLives = this.playerStat.CurrentLives;
        data.powerValue = this.playerStat.PowerValue;
        data.healthUpgradeLevel = this.playerStat.healthUpgradeLevel;
        data.staminaUpgradeLevel = this.playerStat.staminaUpgradeLevel;
        data.livesUpgradeLevel = this.playerStat.livesUpgradeLevel;
        data.hasWallJump = this.playerStat.hasWallJump;
        data.jumpCooldownLevel = this.playerStat.jumpCooldownLevel;
        data.dashCooldownLevel = this.playerStat.dashCooldownLevel;

        return data;
    }

    public void SaveGameState()
    {
        SaveData data = CreateSaveDataObject();
        if (data != null)
        {
            SaveManager.Instance.SaveGame(data);
            Debug.Log("Game State Saved!");
        }
    }

    // Use this method to transition between gameplay scenes to ensure player stats are carried over.
    public void GoToScene(string sceneName)
    {
        // Create a data object with the current player state.
        SaveData dataForNextScene = CreateSaveDataObject();

        if (dataForNextScene != null)
        {
            // Set the static variable so the next scene can find the data.
            GameManager.dataToLoad = dataForNextScene;

            // Also perform a full save to disk before we leave the current scene.
            SaveGameState();

            Debug.Log($"Transitioning to scene: {sceneName} with player data.");
            Time.timeScale = 1f; // Ensure time is running for the next scene.
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            // Fallback if data can't be created for some reason.
            Debug.LogError($"Could not create save data. Transitioning to scene {sceneName} without saving stats.");
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }

    // This is now a coroutine to ensure it runs after all Start() methods
    private IEnumerator ApplyLoadedDataRoutine()
    {
        // Wait until the end of the frame to ensure all Start() methods have been called
        yield return new WaitForEndOfFrame();

        if (playerController == null) playerController = FindAnyObjectByType<PlayerController>();
        if (playerController == null || dataToLoad == null) yield break;
        
        PlayerStat playerStat = playerController.GetComponent<PlayerStat>();
        if (playerStat == null) yield break;

        // Apply Player Position
        Vector3 position = new Vector3(dataToLoad.playerPositionX, dataToLoad.playerPositionY, dataToLoad.playerPositionZ);
        playerController.transform.position = position;

        // Apply Gravity Direction by calling the new method on PlayerController
        playerController.ApplyGravityDirection(dataToLoad.gravityDirection);

        // CRITICAL FIX: Reset gravity scale to default after applying direction.
        // This prevents issues where the game was saved with a modified gravity scale (e.g., during a dash).
        if (playerController.Rigidbody != null)
        {
            playerController.Rigidbody.gravityScale = playerController.DefaultGravityScale;
        }

        // Apply Stats and Upgrades by calling the new method on PlayerStat
        playerStat.ApplySaveData(dataToLoad);

        Debug.Log("Loaded data applied to player.");

        // IMPORTANT: Update UI after applying all data
        playerStat.UpdateUI();

        // Clear the static data holder after applying it
        dataToLoad = null;
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);

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
        if (MenuOptionUI != null) MenuOptionUI.SetActive(true);
        if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
    }
    public void Tutorial()
    {
        if (TutorialUI != null) TutorialUI.SetActive(true);
        if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
    }
    public void Again()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        isGameEnd = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (playerController != null) playerController.GrafityDown = true;
        if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
        
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        if (gamePauseUI != null) gamePauseUI.SetActive(true);
        if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
        AudioManager.Instance.PlayPauseOn();
    }
    public void Remuse()
    {
        Time.timeScale = 1f;
        if (gamePauseUI != null) gamePauseUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
    }
    public void MainMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SaveGameState();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
        Debug.Log("da nhan");
    }
    public void MainMenuInGameEndUI()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }

        SceneManager.LoadScene("MainMenu");
        isGameEnd = false;
        if (sfxSource != null && clickClip != null) sfxSource.PlayOneShot(clickClip);
        Debug.Log("da nhan");
    }
    public void Back()
    {
        if (gamePauseUI != null && gamePauseUI.activeSelf)
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

        if (MenuOptionUI != null && MenuOptionUI.activeSelf)
        {
            MenuOptionUI.SetActive(false);
            if (sfxSource != null && clickClip != null)
            {
                sfxSource.PlayOneShot(clickClip);
            }
        }
        if (TutorialUI != null && TutorialUI.activeSelf)
        {
            TutorialUI.SetActive(false);
            if (sfxSource != null && clickClip != null)
            {
                sfxSource.PlayOneShot(clickClip);
            }
        }
    }
    #endregion

    #region --- SHOP ---
    public void Buy1()
    {
        buyCount1++;
        switch (buyCount1)
        {
            case 0:
                Text1.text = "20";
                button1.SetActive(true);
                SOLD1.SetActive(false);
                break;
            case 1:
                if (playerStat.PowerValue >= 20)
                {
                    playerStat.UpgradeHealth(50);
                    playerStat.UsePowerValue(20);
                    Text1.text = "30";
                    
                }
                else
                {
                    buyCount1--;
                }
                break;
            case 2:
                if (playerStat.PowerValue >= 30)
                {
                    playerStat.UpgradeHealth(50);
                    playerStat.UsePowerValue(30);
                    Text1.text = "40";
                }
                else
                {
                    buyCount1--;
                }
                break;
            case 3:
                if (playerStat.PowerValue >= 40)
                {
                    playerStat.UpgradeHealth(50);
                    playerStat.UsePowerValue(40);
                    button1.SetActive(false);
                    SOLD1.SetActive(true);
                    buyCount1 = 0;
                }
                else
                {
                    buyCount1--;
                }
                break;
            
        }
        if (buyCount1 > 3)
        {
            buyCount1 = 0;
        }
    }
    public void Buy3()
    {
        buyCount3++;
        switch (buyCount3)
        {
            case 0:
                Text3.text = "20";
                button3.SetActive(true);
                SOLD3.SetActive(false);
                break;
            case 1:
                if (playerStat.PowerValue >= 20)
                {
                    playerStat.UpgradeStamina(20);
                    playerStat.UsePowerValue(20);
                    Text3.text = "40";
                }
                else
                {
                    buyCount3--;
                }
                break;
            case 2:
                if (playerStat.PowerValue >= 40)
                {
                    playerStat.UpgradeStamina(20);
                    playerStat.UsePowerValue(40);
                    Text3.text = "60";
                }
                else
                {
                    buyCount3--;
                }
                break;
            case 3:
                if (playerStat.PowerValue >= 60)
                {
                    playerStat.UpgradeStamina(20);
                    playerStat.UsePowerValue(60);
                    button3.SetActive(false);
                    SOLD3.SetActive(true);
                    buyCount3 = 0;
                }
                else
                {
                    buyCount3--;
                }
                break;
            
        }
        if (buyCount3 > 3)
        {
            buyCount3 = 0;
        }
    }
    public void Buy2()
    {
        buyCount2++;
        switch (buyCount2)
        {
            case 0:
                Text2.text = "50";
                button2.SetActive(true);
                SOLD2.SetActive(false);
                break;
            case 1:
                if (playerStat.PowerValue >= 50)
                {
                    playerStat.AddLife();
                    playerStat.UsePowerValue(50);
                    Text2.text = "100";
                }
                else
                {
                    buyCount2--;
                }
                break;
            case 2:
                if (playerStat.PowerValue >= 100)
                {
                    playerStat.AddLife();
                    playerStat.UsePowerValue(100);
                    Text2.text = "150";
                }
                else
                {
                    buyCount2--;
                }
                break;
            case 3:
                if (playerStat.PowerValue >= 150)
                {
                    playerStat.AddLife();
                    playerStat.UsePowerValue(150);
                    button2.SetActive(false);
                    SOLD2.SetActive(true);
                    buyCount2 = 0;
                }
                else
                {
                    buyCount2--;
                }
                break;
            
        }

        if (buyCount2 > 3)
        {
            buyCount2 = 0;
        }
    }
    public void Buy4()
    {
        buyCount4++;
        switch (buyCount4) 
        {
            case 0:
                button4.SetActive(true);
                SOLD4.SetActive(false);
                break;
            case 1:
                if (playerStat.PowerValue >= 200)
                {
                    playerStat.UnlockWallJump();
                    playerStat.UsePowerValue(200);
                    button4.SetActive(false);
                    SOLD4.SetActive(true);
                }
                else
                {
                    buyCount4--;
                }
                break;
        }
        
    }
    #endregion
}