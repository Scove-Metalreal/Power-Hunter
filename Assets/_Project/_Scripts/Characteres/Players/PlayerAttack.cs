using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private Transform spearSpawm;
    public GameObject spearHitBoxPre;
    public PlayerController playerController;
    public Transform Spear;
    [Header("Attack")]
    public float AttackTime = 0.6f;
    public float AttackCooldown = 1f;
    public bool isAttacking = false;
    private float AttackTimeLeft;
    private float AttackCooldownTime;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }


    void Update()
    {

        SpearAttack();
    }
    void SpearAttack()
    {
        if (AttackCooldownTime > 0)
        {
            AttackCooldownTime -= Time.deltaTime;
        }
        if (Input.GetMouseButtonDown(0) && AttackCooldownTime <= 0 && isAttacking == false)
        {

            isAttacking = true;
            AttackTimeLeft = AttackTime;
            AttackCooldownTime = AttackCooldown;
            playerController.animator.SetTrigger("isAttack");
        }
        if (isAttacking == true)
        {
            if (AttackTimeLeft > 0)
            {
                AttackTimeLeft -= Time.deltaTime;
            }
            else
            {
                isAttacking = false;
                playerController.animator.SetTrigger("isAttack");
            }
        }
    }
    public void StartAnimAttack()
    {
        var spawmSpearHB = Instantiate(spearHitBoxPre, spearSpawm.position, Quaternion.identity);
        //Vector3 currentSpawmScale = spearHitBoxPre.transform.localScale;
        //if (playerController.Direction == -1)
        //{
        //    spawmSpearHB.transform.localScale = new Vector3(-Mathf.Abs(currentSpawmScale.x), currentSpawmScale.y, currentSpawmScale.z);
        //}
        //if (playerController.Direction == 1)
        //{
        //    spawmSpearHB.transform.localScale = new Vector3(Mathf.Abs(currentSpawmScale.x), currentSpawmScale.y, currentSpawmScale.z);
        //}
        spawmSpearHB.transform.SetParent(Spear);
        Destroy(spawmSpearHB, 1.2f);
    }
}