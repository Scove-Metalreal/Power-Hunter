using Unity.Mathematics;
using UnityEngine;

public class PlayerSkillGrafity : MonoBehaviour
{
    private PlayerController playerController;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A)) {
            Physics2D.gravity = new Vector2(-9.8f, 0);
            transform.rotation = Quaternion.Euler(0, 0, -90);
            playerController.GrafityLeft = true;
            playerController.GrafityDown = false;
            playerController.GrafityUp = false;
            playerController.GrafityRight = false;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) {
            Physics2D.gravity = new Vector2(0, -9.8f);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            playerController.GrafityDown = true;
            playerController.GrafityLeft = false;
            
            playerController.GrafityUp = false;
            playerController.GrafityRight = false;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W)) {
            Physics2D.gravity = new Vector2(0, 9.8f);
            transform.rotation = Quaternion.Euler(0, 0, 180);
            playerController.GrafityUp = true;
            playerController.GrafityLeft = false;
            playerController.GrafityDown = false;
            
            playerController.GrafityRight = false;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D)) {
            Physics2D.gravity = new Vector2(9.8f, 0);
            transform.rotation = Quaternion.Euler(0, 0, 90);
            playerController.GrafityRight = true;
            playerController.GrafityLeft = false;
            playerController.GrafityDown = false;
            playerController.GrafityUp = false;
            
        }
    }
}
