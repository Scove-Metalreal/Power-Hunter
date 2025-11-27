using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealingZone : MonoBehaviour
{
    [Tooltip("How many health points are restored per second.")]
    public float healPerSecond = 20f;

    private PlayerStat playerToHeal;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerHurtbox>() != null)
        {
            playerToHeal = collision.GetComponentInParent<PlayerStat>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerHurtbox>() != null)
        {
            playerToHeal = null;
        }
    }

    private void Update()
    {
        // Nếu người chơi ở trong vùng, hồi máu cho họ theo thời gian
        if (playerToHeal != null)
        {
            // Gọi hàm Heal mới trong PlayerStat để đảm bảo logic được quản lý tập trung
            playerToHeal.Heal(healPerSecond * Time.deltaTime);
        }
    }
}
