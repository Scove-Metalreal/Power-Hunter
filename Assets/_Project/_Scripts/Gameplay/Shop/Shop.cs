using UnityEngine;

public class Shop : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pressEUI; // UI text "Press E to open shop"
    public GameObject shopMenuUI; // The main shop panel UI

    [Header("Upgrade Settings")]
    public int baseHealthCost = 100;
    public float healthIncreasePerLevel = 20f;

    public int baseStaminaCost = 100;
    public float staminaIncreasePerLevel = 20f;

    public int baseLivesCost = 500;
    // Lives increase by 1 per upgrade, no need for a separate variable.

    [Header("Cost Progression")]
    public float costMultiplier = 1.5f;

    private PlayerStat playerStat; // Store a reference to the player's stats
    private bool canOpenShop = false;

    private void Start()
    {
        // Ensure all UI is hidden at the start
        if (pressEUI != null) pressEUI.SetActive(false);
        if (shopMenuUI != null) shopMenuUI.SetActive(false);
    }

    private void Update()
    {
        // Check if the player is in range and presses the interact key
        if (canOpenShop && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShopMenu();
        }
    }

    public void ToggleShopMenu()
    {
        if (shopMenuUI == null) return;

        bool isOpening = !shopMenuUI.activeSelf;
        shopMenuUI.SetActive(isOpening);

        // Pause or resume the game
        Time.timeScale = isOpening ? 0f : 1f;

        // Show or hide the cursor
        Cursor.visible = isOpening;
        Cursor.lockState = isOpening ? CursorLockMode.Confined : CursorLockMode.Locked;

        // Hide the "Press E" prompt when the shop is open
        if (pressEUI != null)
        {
            pressEUI.SetActive(!isOpening);
        }
    }

    #region --- Upgrade Logic ---

    // Can be called by UI buttons to purchase the health upgrade
    public void PurchaseHealthUpgrade()
    {
        if (playerStat == null) return;

        int cost = CalculateCost(baseHealthCost, playerStat.healthUpgradeLevel);

        if (playerStat.UsePowerValue(cost))
        {
            playerStat.UpgradeHealth(healthIncreasePerLevel);
            Debug.Log($"Successfully upgraded Health for {cost} PowerValue. New level: {playerStat.healthUpgradeLevel}");
            
            // Immediately save the game to commit the transaction
            GameManager.Instance.SaveGameState();
        }
        else
        {
            Debug.Log($"Not enough PowerValue for Health upgrade. Need {cost}, have {playerStat.PowerValue}.");
        }
    }

    // Can be called by UI buttons to purchase the stamina upgrade
    public void PurchaseStaminaUpgrade()
    {
        if (playerStat == null) return;

        int cost = CalculateCost(baseStaminaCost, playerStat.staminaUpgradeLevel);

        if (playerStat.UsePowerValue(cost))
        {
            playerStat.UpgradeStamina(staminaIncreasePerLevel);
            Debug.Log($"Successfully upgraded Stamina for {cost} PowerValue. New level: {playerStat.staminaUpgradeLevel}");

            // Immediately save the game to commit the transaction
            GameManager.Instance.SaveGameState();
        }
        else
        {

            Debug.Log($"Not enough PowerValue for Stamina upgrade. Need {cost}, have {playerStat.PowerValue}.");
        }
    }

    // Can be called by UI buttons to purchase the life upgrade
    public void PurchaseLifeUpgrade()
    {
        if (playerStat == null) return;

        int cost = CalculateCost(baseLivesCost, playerStat.livesUpgradeLevel);

        if (playerStat.UsePowerValue(cost))
        {
            playerStat.AddLife();
            Debug.Log($"Successfully upgraded Lives for {cost} PowerValue. New level: {playerStat.livesUpgradeLevel}");

            // Immediately save the game to commit the transaction
            GameManager.Instance.SaveGameState();
        }
        else
        {
            Debug.Log($"Not enough PowerValue for Lives upgrade. Need {cost}, have {playerStat.PowerValue}.");
        }
    }

    // Helper method to calculate the cost based on the current level
    public int CalculateCost(int baseCost, int level)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level));
    }

    #endregion

    #region --- Triggers ---

    // When player enters the shop's trigger area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Get the PlayerStat component to interact with
            playerStat = collision.GetComponent<PlayerStat>();
            if (playerStat != null)
            {
                canOpenShop = true;
                if (pressEUI != null) pressEUI.SetActive(true);
            }
        }
    }

    // When player leaves the shop's trigger area
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canOpenShop = false;
            playerStat = null; // Clear the reference

            if (pressEUI != null) pressEUI.SetActive(false);

            // If the player walks away, close the shop menu
            if (shopMenuUI != null && shopMenuUI.activeSelf)
            {
                ToggleShopMenu();
            }
        }
    }

    #endregion
}
