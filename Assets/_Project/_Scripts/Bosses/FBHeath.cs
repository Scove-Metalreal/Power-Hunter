using UnityEngine;
using System.Collections;

public class FBHeath : MonoBehaviour
{
    public float Bossheath = 1000f;
    public float damagePlayer = 5f;

    public GameObject light33;
    public GameObject light66;
    public GameObject light100;
    public GameObject winUIpanel;

    private SpriteRenderer sprite;
    private Color originalColor;
    private bool isFlashing = false;

    void Start()
    {
        Bossheath = 1000f;
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            originalColor = sprite.color;

        light33.SetActive(false);
        light66.SetActive(false);
        light100.SetActive(true);
        winUIpanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerHitbox"))
        {
            Bossheath -= damagePlayer;

            
            if (!isFlashing)
                StartCoroutine(RGBEffect());

            
            UpdateLight();

            
            if (Bossheath <= 0)
            {
                Time.timeScale = 0f;
                winUIpanel.SetActive(true);
            }
        }
    }

    private void UpdateLight()
    {
        if (Bossheath > 66 && Bossheath <= 100)
        {
            light33.SetActive(false);
            light66.SetActive(false);
            light100.SetActive(true);
        }
        else if (Bossheath > 33 && Bossheath <= 66)
        {
            light33.SetActive(false);
            light66.SetActive(true);
            light100.SetActive(false);
        }
        else if (Bossheath > 0 && Bossheath <= 33)
        {
            light33.SetActive(true);
            light66.SetActive(false);
            light100.SetActive(false);
        }
    }

   
    private IEnumerator RGBEffect()
    {
        isFlashing = true;
        float duration = 1.2f; 
        float timer = 0f;

        while (timer < duration)
        {
            
            float hue = Mathf.PingPong(Time.time * 2f, 1f);
            if (sprite != null)
                sprite.color = Color.HSVToRGB(hue, 1f, 1f);

            timer += Time.deltaTime;
            yield return null;
        }

        
        if (sprite != null)
            sprite.color = originalColor;

        isFlashing = false;
    }
}
