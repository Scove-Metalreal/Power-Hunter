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
    }

    void Start()
    {
        fbHeath = FindAnyObjectByType<FBHeath>();
        playerController = FindAnyObjectByType<PlayerController>();
        playerStat = FindAnyObjectByType<PlayerStat>();
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

        // If there is data to load, apply it. Otherwise, set up a new game.
        if (dataToLoad != null)
        {
            StartCoroutine(ApplyLoadedDataRoutine());
        }
        else
        {
            // This is a new game
            PlayerStat playerStat = FindAnyObjectByType<PlayerStat>();
            if (playerStat != null)
            {
                playerStat.ResetStats();
            }
        }

        for (int i = 0; i< playerStat.CurrentLives;i++){
            LifeUI[i].SetActive(true);
        }
        StartCoroutine(AutoSaveRoutine());

        
    }

    void Update()
    {
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

        if (playerStat.CurrentLives > 0)
        {
            for (int i = 0; (i) < playerStat.CurrentLives; (i)++)
            {
                LifeUI[i].SetActive(true);
            }

            for (int i = 0; i >= playerStat.CurrentLives; i++)
            {
                LifeUI[i].SetActive(false);
            }
        }

        if (fbHeath != null && (fbHeath.Bossheath <= 0 || playerStat.CurrentLives <= 0))
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
        // Set dataToLoad to null to signify a new game.
        dataToLoad = null;

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

    public void SaveGameState()
    {
        if (playerController == null) playerController = FindAnyObjectByType<PlayerController>();
        if (playerController == null) return;
        PlayerStat playerStat = playerController.GetComponent<PlayerStat>();
        if (playerStat == null) return;

        SaveData data = new SaveData();
        data.lastScene = SceneManager.GetActiveScene().name;
        data.playerPositionX = playerController.transform.position.x;
        data.playerPositionY = playerController.transform.position.y;
        data.playerPositionZ = playerController.transform.position.z;

        // Determine and save current gravity direction
        if (playerController.GrafityUp) data.gravityDirection = GravityDirection.Up;
        else if (playerController.GrafityLeft) data.gravityDirection = GravityDirection.Left;
        else if (playerController.GrafityRight) data.gravityDirection = GravityDirection.Right;
        else data.gravityDirection = GravityDirection.Down;

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1.0f;
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