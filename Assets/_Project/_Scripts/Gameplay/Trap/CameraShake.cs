using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public ShakeData explosionShakeDate;
    public IEnumerator Shake()
    {
        CameraShakerHandler.Shake(explosionShakeDate);
        yield return null;
       
    }
}
