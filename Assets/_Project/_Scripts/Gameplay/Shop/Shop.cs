using UnityEngine;
using UnityEngine.UI; // Cần cho Button

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

    [Tooltip("Chi phí một lần để mở khóa Wall Jump")]
    public int wallJumpCost = 750;

    [Header("Cost Progression")]
    public float costMultiplier = 1.5f;

    [Header("Button References")]
    [Tooltip("Kéo Button nâng cấp Wall Jump vào đây")]
    public Button wallJumpButton;

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

        // Nếu đang mở shop, cập nhật trạng thái các nút
        if (isOpening)
        {
            UpdateShopButtons();
        }

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

    // <<< HÀM MỚI >>>
    // Cập nhật trạng thái của các nút trong shop (ví dụ: vô hiệu hóa nếu đã mua)
    private void UpdateShopButtons()
    {
        if (playerStat == null) return;

        // Vô hiệu hóa nút Wall Jump nếu người chơi đã có kỹ năng này
        if (wallJumpButton != null)
        {
            if (playerStat.hasWallJump)
            {
                wallJumpButton.interactable = false;
                // Tùy chọn: thay đổi text của nút để hiển thị "Đã Mua"
                // wallJumpButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Đã Mua";
            }
            else
            {
                wallJumpButton.interactable = true;
            }
        }
    }

    #region --- Upgrade Logic ---

    public void PurchaseHealthUpgrade()
    {
        if (playerStat == null) return;
        int cost = CalculateCost(baseHealthCost, playerStat.healthUpgradeLevel);
        if (playerStat.UsePowerValue(cost))
        {
            playerStat.UpgradeHealth(healthIncreasePerLevel);
            GameManager.Instance.SaveGameState();
        }
    }

    public void PurchaseStaminaUpgrade()
    {
        if (playerStat == null) return;
        int cost = CalculateCost(baseStaminaCost, playerStat.staminaUpgradeLevel);
        if (playerStat.UsePowerValue(cost))
        {
            playerStat.UpgradeStamina(staminaIncreasePerLevel);
            GameManager.Instance.SaveGameState();
        }
    }

    public void PurchaseLifeUpgrade()
    {
        if (playerStat == null) return;
        int cost = CalculateCost(baseLivesCost, playerStat.livesUpgradeLevel);
        if (playerStat.UsePowerValue(cost))
        {
            playerStat.AddLife();
            GameManager.Instance.SaveGameState();
        }
    }

    // <<< HÀM MỚI >>>
    // Được gọi bởi nút UI để mua kỹ năng nhảy tường
    public void PurchaseWallJump()
    {
        if (playerStat == null) return;

        // Kiểm tra nếu đã có kỹ năng rồi thì không làm gì cả
        if (playerStat.hasWallJump)
        {
            Debug.Log("Player already has Wall Jump.");
            return;
        }

        // Thử trừ tiền và thực hiện nâng cấp
        if (playerStat.UsePowerValue(wallJumpCost))
        {
            playerStat.UnlockWallJump();
            Debug.Log($"Successfully unlocked Wall Jump for {wallJumpCost} PowerValue.");
            
            // Cập nhật lại trạng thái nút ngay lập tức
            UpdateShopButtons();

            // Lưu game
            GameManager.Instance.SaveGameState();
        }
        else
        {
            Debug.Log($"Not enough PowerValue for Wall Jump. Need {wallJumpCost}, have {playerStat.PowerValue}.");
        }
    }

    public int CalculateCost(int baseCost, int level)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level));
    }

    #endregion

    #region --- Triggers ---

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerStat = collision.GetComponent<PlayerStat>();
            if (playerStat != null)
            {
                canOpenShop = true;
                if (pressEUI != null) pressEUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canOpenShop = false;
            playerStat = null;
            if (pressEUI != null) pressEUI.SetActive(false);
            if (shopMenuUI != null && shopMenuUI.activeSelf)
            {
                ToggleShopMenu();
            }
        }
    }

    #endregion
}
