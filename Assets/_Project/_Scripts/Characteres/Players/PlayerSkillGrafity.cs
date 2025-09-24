using Unity.Android.Gradle.Manifest;
using Unity.Mathematics;
using UnityEngine;

public class PlayerSkillGrafity : MonoBehaviour
{
    private PlayerController playerController;
    public int GrafityValueSkill = 50;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
        {
            Physics2D.gravity = new Vector2(-9.8f, 0);
            
            playerController.CanMove = false;
            transform.rotation = Quaternion.Euler(0, 0, -90);
            playerController.GrafityLeft = true;
            playerController.GrafityDown = false;
            playerController.GrafityUp = false;
            playerController.GrafityRight = false;
            playerController.isGround = false;
            GetComponent<Rigidbody2D>().gravityScale = GrafityValueSkill;

        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
        {
            Physics2D.gravity = new Vector2(0, -9.8f);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            playerController.CanMove = false;
            playerController.GrafityDown = true;
            playerController.GrafityLeft = false;

            playerController.GrafityUp = false;
            playerController.GrafityRight = false;
            playerController.isGround = false;
            GetComponent<Rigidbody2D>().gravityScale = GrafityValueSkill;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W))
        {
            Physics2D.gravity = new Vector2(0, 9.8f);
            transform.rotation = Quaternion.Euler(0, 0, 180);
            playerController.CanMove = false;
            playerController.GrafityUp = true;
            playerController.GrafityLeft = false;
            playerController.GrafityDown = false;

            playerController.GrafityRight = false;
            playerController.isGround = false;
            GetComponent<Rigidbody2D>().gravityScale = GrafityValueSkill;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
        {
            Physics2D.gravity = new Vector2(9.8f, 0);
            transform.rotation = Quaternion.Euler(0, 0, 90);
            playerController.CanMove = false;
            playerController.GrafityRight = true;
            playerController.GrafityLeft = false;
            playerController.GrafityDown = false;
            playerController.GrafityUp = false;
            playerController.isGround = false;
            GetComponent<Rigidbody2D>().gravityScale = GrafityValueSkill;
        }

        
    }
    
}