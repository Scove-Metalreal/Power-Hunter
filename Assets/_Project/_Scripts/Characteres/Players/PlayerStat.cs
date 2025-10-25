using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Needed for saving scene name

public class PlayerStat : MonoBehaviour
{
    [Header("Core Stats")]
    public float HeathPlayer;
    public float MaxHealth;
    public float StaminaPlayer;
    public float MaxStamina;
    public int CurrentLives;
    public int MaxLives;
    public int PowerValue;

    [Header("Upgrade Levels")]
    public int healthUpgradeLevel;
    public int staminaUpgradeLevel;
    public int livesUpgradeLevel;
    public int jumpCooldownLevel;
    public int dashCooldownLevel;

    [Header("Abilities")]
    public bool hasWallJump;

    private float targetHealth;
    private float targetStamina;

    [Header("UI")]
    public Slider HeathSilder;
    public TextMeshProUGUI HeathText;
    public Slider StaminaSlider;

    void Start()
    {
        // The GameManager is now solely responsible for loading stats.
        // This method now only ensures UI is initialized correctly on scene start.
        UpdateHealthUI();
        UpdateStaminaUI();
    }

    // Method called by GameManager after loading data to refresh all UI elements.
    public void UpdateUI()
    {
        UpdateHealthUI();
        UpdateStaminaUI();
        
        // TODO: Add any other UI updates here, for example, for PowerValue if you have a Text element for it.
        // if (powerValueText != null)
        // {
        //     powerValueText.text = PowerValue.ToString();
        // }
    }

    void Update()
    {
        // Smoothly update UI sliders
        HeathSilder.value = Mathf.MoveTowards(HeathSilder.value, targetHealth, 25f * Time.unscaledDeltaTime);
        StaminaSlider.value = Mathf.MoveTowards(StaminaSlider.value, targetStamina, 25f * Time.unscaledDeltaTime);

        // Update health text
        HeathText.text = $"{Math.Round(HeathSilder.value, 0)} / {MaxHealth}";

        // Regenerate stamina
        if (StaminaPlayer < MaxStamina)
        {
            StaminaPlayer += 10f * Time.unscaledDeltaTime;
            if (StaminaPlayer > MaxStamina)
                StaminaPlayer = MaxStamina;
            targetStamina = StaminaPlayer;
        }
    }
    
    // --- Save/Load Integration ---

    public void LoadStats()
    {
        SaveData data = SaveManager.Instance.LoadGame();

        MaxHealth = data.maxHealth;
        MaxStamina = data.maxStamina;
        MaxLives = data.maxLives;
        PowerValue = data.powerValue;
        healthUpgradeLevel = data.healthUpgradeLevel;
        staminaUpgradeLevel = data.staminaUpgradeLevel;
        livesUpgradeLevel = data.livesUpgradeLevel;
        hasWallJump = data.hasWallJump;
        jumpCooldownLevel = data.jumpCooldownLevel;
        dashCooldownLevel = data.dashCooldownLevel;

        // Set current stats
        HeathPlayer = MaxHealth;
        StaminaPlayer = MaxStamina;
        CurrentLives = MaxLives;

        // This logic handles loading the player into the correct scene and position.
        // We will need a corresponding script in the GameManager to handle the initial load.
        if (data.lastScene != null && SceneManager.GetActiveScene().name != data.lastScene)
        {
            SceneManager.LoadScene(data.lastScene);
        }
        else
        {
            transform.position = new Vector3(data.playerPositionX, data.playerPositionY, data.playerPositionZ);
        }

        UpdateHealthUI();
        UpdateStaminaUI();
    }

    public void SaveStats()
    {
        SaveData data = new SaveData();

        // Player Position and Scene
        data.lastScene = SceneManager.GetActiveScene().name;
        data.playerPositionX = transform.position.x;
        data.playerPositionY = transform.position.y;
        data.playerPositionZ = transform.position.z;

        // Stats and Upgrades
        data.maxHealth = MaxHealth;
        data.maxStamina = MaxStamina;
        data.maxLives = MaxLives;
        data.powerValue = PowerValue;
        data.healthUpgradeLevel = healthUpgradeLevel;
        data.staminaUpgradeLevel = staminaUpgradeLevel;
        data.livesUpgradeLevel = livesUpgradeLevel;
        data.hasWallJump = hasWallJump;
        data.jumpCooldownLevel = jumpCooldownLevel;
        data.dashCooldownLevel = dashCooldownLevel;

        SaveManager.Instance.SaveGame(data);
        Debug.Log("Player stats saved!");
    }

    // --- The rest of the methods are the same... ---

    public void UseStamina(float energy)
    {
        StaminaPlayer -= energy;
        if (StaminaPlayer < 0) { StaminaPlayer = 0; }
        targetStamina = StaminaPlayer;
    }

    public void TakeDamage(float damage)
    {
        HeathPlayer -= damage;
        if (HeathPlayer <= 0)
        {
            HeathPlayer = 0;
            HandlePlayerDeath();
        }
        targetHealth = HeathPlayer;
    }

    private void HandlePlayerDeath()
    {
        CurrentLives--;
        if (CurrentLives > 0)
        {
            HeathPlayer = MaxHealth;
            targetHealth = MaxHealth;
            Debug.Log("Life lost. Lives remaining: " + CurrentLives);
        }
        else
        {
            Debug.Log("Game Over!");
        }
    }

    public void UpgradeHealth(float amount)
    {
        MaxHealth += amount;
        HeathPlayer = MaxHealth;
        healthUpgradeLevel++;
        UpdateHealthUI();
    }

    public void UpgradeStamina(float amount)
    {
        MaxStamina += amount;
        StaminaPlayer = MaxStamina;
        staminaUpgradeLevel++;
        UpdateStaminaUI();
    }

    public void AddLife()
    {
        MaxLives++;
        CurrentLives++;
        livesUpgradeLevel++;
    }
    
    public void UnlockWallJump()
    {
        hasWallJump = true;
    }

    public void AddPowerValue(int amount)
    {
        PowerValue += amount;
    }

    public bool UsePowerValue(int amount)
    {
        if (PowerValue >= amount)
        {
            PowerValue -= amount;
            return true;
        }
        return false;
    }

    private void UpdateHealthUI()
    {
        HeathSilder.maxValue = MaxHealth;
        HeathSilder.value = HeathPlayer;
        targetHealth = HeathPlayer;
    }

    private void UpdateStaminaUI()
    {
        StaminaSlider.maxValue = MaxStamina;
        StaminaSlider.value = StaminaPlayer;
        targetStamina = StaminaPlayer;
    }
}

