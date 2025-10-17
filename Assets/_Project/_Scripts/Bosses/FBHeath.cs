using UnityEngine;

public class FBHeath : MonoBehaviour
{
    private float Bossheath;
    public float damagePlayer = 5f;
    public GameObject light33;
    public GameObject light66;
    public GameObject light100;
    public GameObject winUIpanel;
    void Start()
    {
        Bossheath = 100f;
        light33.SetActive(false);
        light66.SetActive(false);
        light100.SetActive(true);
        winUIpanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("HItboxWeapon")) {
         Bossheath -= damagePlayer; 
            if(Bossheath >66 ||  Bossheath <= 100)
            {
                light33.SetActive(false);
                light66.SetActive(false);
                light100.SetActive(true);
            }
            if(Bossheath >33 ||  Bossheath <= 66)
            {
                light33.SetActive(false);
                light66.SetActive(true);
                light100.SetActive(false);
            }
            if(Bossheath >0 ||  Bossheath <= 33)
            {
                light33.SetActive(true);
                light66.SetActive(false);
                light100.SetActive(false);
            }
            if(Bossheath <= 0)
            {
                Time.timeScale = 0f;
                winUIpanel.SetActive(true);
            }
        }
    }
}
