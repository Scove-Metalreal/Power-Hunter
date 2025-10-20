using UnityEngine;

[System.Serializable]
public class SaveData
{
    // Player Position & Scene
    public string lastScene;
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    // Player Stats
    public float maxHealth;
    public float maxStamina;
    public int maxLives;
    public int powerValue;

    // Upgrade Levels
    public int healthUpgradeLevel;
    public int staminaUpgradeLevel;
    public int livesUpgradeLevel;

    // Abilities
    public bool hasWallJump;
    public int jumpCooldownLevel;
    public int dashCooldownLevel;

    // Default values for a new game
    public SaveData()
    {
        // Default position can be set to the start of the first level
        lastScene = "Level 1";
        playerPositionX = 0;
        playerPositionY = 0;
        playerPositionZ = 0;

        maxHealth = 100f;
        maxStamina = 100f;
        maxLives = 3;
        powerValue = 0;

        healthUpgradeLevel = 0;
        staminaUpgradeLevel = 0;
        livesUpgradeLevel = 0;

        hasWallJump = false;
        jumpCooldownLevel = 0;
        dashCooldownLevel = 0;
    }
}