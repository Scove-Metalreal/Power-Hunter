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
    public TextMeshProUGUI powerValueText; // Reference to the TextMeshPro UI for power value

    void Start()
    {
        // The GameManager is responsible for telling this script whether to load data or reset.
        // This method now only ensures UI is initialized correctly on scene start.
        UpdateUI();
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
    }
    
    // --- Save/Load Integration ---

    public void ResetStats()
    {
        // Use the SaveData constructor to get default values
        SaveData defaultData = new SaveData();

        MaxHealth = defaultData.maxHealth;
        MaxStamina = defaultData.maxStamina;
        MaxLives = defaultData.maxLives;
        PowerValue = defaultData.powerValue;
        healthUpgradeLevel = defaultData.healthUpgradeLevel;
        staminaUpgradeLevel = defaultData.staminaUpgradeLevel;
        livesUpgradeLevel = defaultData.livesUpgradeLevel;
        hasWallJump = defaultData.hasWallJump;
        jumpCooldownLevel = defaultData.jumpCooldownLevel;
        dashCooldownLevel = defaultData.dashCooldownLevel;

        // Set current stats to max
        HeathPlayer = MaxHealth;
        StaminaPlayer = MaxStamina;
        CurrentLives = MaxLives;

        // Update the UI to reflect the reset stats
        UpdateUI();
        Debug.Log("Player stats reset to default for a new game.");
    }

    public void ApplySaveData(SaveData data)
    {
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

        // Set current stats to max
        HeathPlayer = MaxHealth;
        StaminaPlayer = MaxStamina;
        CurrentLives = MaxLives;

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
    }

    public void Heal(float amount)
    {
        HeathPlayer += amount;
        if (HeathPlayer > MaxHealth)
        {
            HeathPlayer = MaxHealth;
        }
        targetHealth = HeathPlayer;
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
        if (powerValueText != null)
        {
            powerValueText.text = "Power: " + PowerValue.ToString();
        }
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

