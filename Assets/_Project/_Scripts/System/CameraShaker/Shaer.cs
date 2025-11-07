using FirstGearGames.SmoothCameraShaker;
using UnityEngine;

public class Shaer : MonoBehaviour
{
    public ShakeData explosionShakeDate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            CameraShakerHandler.Shake(explosionShakeDate);
        }
    }
}
