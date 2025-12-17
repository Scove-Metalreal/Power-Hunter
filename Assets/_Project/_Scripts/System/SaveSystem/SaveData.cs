using UnityEngine;

// Enum to represent the four possible gravity directions
public enum GravityDirection
{
    Down,
    Up,
    Left,
    Right
}

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
    public float heathPlayer; // Current Health
    public float maxStamina;
    public int maxLives;
    public int currentLives; // Current Lives
    public int powerValue;

    // Player Gravity
    public GravityDirection gravityDirection;

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
        lastScene = "Level 1";
        playerPositionX = 0;
        playerPositionY = 0;
        playerPositionZ = 0;

        maxHealth = 50f;
        heathPlayer = 50f; // Default to max
        maxStamina = 50f;
        maxLives = 3;
        currentLives = 3; // Default to max
        powerValue = 0;

        gravityDirection = GravityDirection.Down; // Default gravity direction

        healthUpgradeLevel = 0;
        staminaUpgradeLevel = 0;
        livesUpgradeLevel = 0;

        hasWallJump = false;
        jumpCooldownLevel = 0;
        dashCooldownLevel = 0;
    }
}