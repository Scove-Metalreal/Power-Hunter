using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Needed for saving scene name

public class PlayerStat : MonoBehaviour
{
    public event Action OnStatsChanged;
    
    private PlayerController _playerController;
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
    public TextMeshProUGUI powerValueText; // Reference to the TextMeshPro UI for power value

    void Start()
    {
        // The GameManager is responsible for telling this script whether to load data or reset.
        // This method now only ensures UI is initialized correctly on scene start.
        UpdateUI();
        _playerController =  GetComponent<PlayerController>();
    }

    // Method called by GameManager after loading data to refresh all UI elements.
    public void UpdateUI()
    {
        UpdateHealthUI();
        UpdateStaminaUI();
        
        if (powerValueText != null)
        {
            powerValueText.text = "Power: " + PowerValue.ToString();
        }

       
    }

    void Update()
    {
        // Smoothly update UI sliders
        HeathSilder.value = Mathf.MoveTowards(HeathSilder.value, targetHealth, 25f * Time.unscaledDeltaTime);
        StaminaSlider.value = Mathf.MoveTowards(StaminaSlider.value, targetStamina, 25f * Time.unscaledDeltaTime);

        // Update health text to show percentage
        if (MaxHealth > 0)
        {
            float healthPercent = (HeathSilder.value / MaxHealth) * 100f;
            HeathText.text = $"{healthPercent:F0}%";
        }

        // Regenerate stamina
        if (StaminaPlayer < MaxStamina)
        {
            StaminaPlayer += 10f * Time.unscaledDeltaTime;
            if (StaminaPlayer > MaxStamina)
                StaminaPlayer = MaxStamina;
            targetStamina = StaminaPlayer;
        }
        if (hasWallJump == true)
        {
            _playerController.groundCheckRadius = 0.48f;
        }
        else
        {
            _playerController.groundCheckRadius = 0.05f;
        }
        
    }
    
    // --- Save/Load Integration ---

    public void ResetStats()
    {
        // This method is called when a scene starts without receiving transfer data.
        // First, check if a save file already exists on disk.
        if (SaveManager.Instance != null && SaveManager.Instance.SaveFileExists())
        {
            // If a file exists, load it. This ensures that even if the live transfer fails,
            // the most recent data from the disk is used instead of resetting progress.
            Debug.Log("ResetStats: Save file found on disk. Loading data instead of resetting.");
            SaveData existingData = SaveManager.Instance.LoadGame();
            if (existingData != null)
            {
                ApplySaveData(existingData);
            }
        }
        else
        {
            // If no save file exists, this is a true "New Game". Apply the default values.
            Debug.Log("ResetStats: No save file found. Applying default values for a new game.");
            ApplySaveData(new SaveData());
        }
    }

    public void ApplySaveData(SaveData data)
    {
        // Load max values
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

        // Load current values, with safety checks
        CurrentLives = data.currentLives;
        HeathPlayer = data.heathPlayer;

        // Safety check: Don't load into a dead or near-dead state.
        if (CurrentLives <= 0) CurrentLives = data.maxLives;
        if (HeathPlayer <= 0) HeathPlayer = data.maxHealth;
        
        // Restore stamina to full on load
        StaminaPlayer = MaxStamina;

        // Update the UI to reflect the loaded stats
        UpdateUI();
        Debug.Log("Save data applied to player stats.");
    }

    // --- The rest of the methods are the same... ---

    public void UseStamina(float energy)
    {
        StaminaPlayer -= energy;
        if (StaminaPlayer < 0) { StaminaPlayer = 0; }
        targetStamina = StaminaPlayer;
        OnStatsChanged?.Invoke();
    }

    public void TakeDamage(float damage)
    {
        HeathPlayer -= damage;
        if (HeathPlayer <= 0)
        {
            HeathPlayer = 0;
            //HandlePlayerDeath();
        }
        targetHealth = HeathPlayer;
        OnStatsChanged?.Invoke();
    }

    public void Heal(float amount)
    {
        HeathPlayer += amount;
        if (HeathPlayer > MaxHealth)
        {
            HeathPlayer = MaxHealth;
        }
        targetHealth = HeathPlayer;
        OnStatsChanged?.Invoke();
    }

    //private void HandlePlayerDeath()
    //{
    //    CurrentLives--;
    //    if (CurrentLives > 0)
    //    {
    //        HeathPlayer = MaxHealth;
    //        targetHealth = MaxHealth;
    //        Debug.Log("Life lost. Lives remaining: " + CurrentLives);
    //    }
    //    else
    //    {
    //        Debug.Log("Game Over!");
    //    }
    //}

    public void UpgradeHealth(float amount)
    {
        MaxHealth += amount;
        HeathPlayer = MaxHealth;
        healthUpgradeLevel++;
        UpdateHealthUI();
        OnStatsChanged?.Invoke();
    }

    public void UpgradeStamina(float amount)
    {
        MaxStamina += amount;
        StaminaPlayer = MaxStamina;
        staminaUpgradeLevel++;
        UpdateStaminaUI();
        OnStatsChanged?.Invoke();
    }

    public void AddLife()
    {
        MaxLives++;
        CurrentLives++;
        livesUpgradeLevel++;
        OnStatsChanged?.Invoke();
    }

    public void DecreaseLife()
    {
        CurrentLives--;
        OnStatsChanged?.Invoke(); // Notify that lives have changed.
    }
    
    public void UnlockWallJump()
    {
        hasWallJump = true;
        OnStatsChanged?.Invoke();
    }

    public void AddPowerValue(int amount)
    {
        PowerValue += amount;
        if (powerValueText != null)
        {
            powerValueText.text = "Power: " + PowerValue.ToString();
        }
        OnStatsChanged?.Invoke();
    }

    public bool UsePowerValue(int amount)
    {
        if (PowerValue >= amount)
        {
            PowerValue -= amount;
            if (powerValueText != null)
            {
                powerValueText.text = "Power: " + PowerValue.ToString();
            }
            OnStatsChanged?.Invoke();
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


