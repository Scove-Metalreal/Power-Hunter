using UnityEngine;

public class Intro : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAnimIntro()
    {
        Time.timeScale = 0f;

    }
    public void EndAnimIntro()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}
