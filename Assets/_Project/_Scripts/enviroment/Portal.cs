using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using FirstGearGames.SmoothCameraShaker;

public class Portal : MonoBehaviour
{
    [Tooltip("The name of the scene to load.")]
    public string sceneToLoad;

    [Tooltip("How long the player must stay in the portal to teleport.")]
    public float timeToTeleport = 3f;

    [Tooltip("GameObject with an Image/Animator for the screen overlay effect. This object will be enabled/disabled by the script.")]
    public GameObject portalEffectObject;

    [Header("Camera Shake")]
    [Tooltip("ShakeData to use as a template for the portal effect. If null, a default one will be created.")]
    public ShakeData portalShakeData;
    [Tooltip("Maximum magnitude for the camera shake.")]
    public float maxShakeMagnitude = 1.5f;

    private bool playerInPortal = false;
    private float timeInPortal = 0f;
    private Coroutine portalCoroutine;
    private float initialCameraSize;
    private ShakerInstance _shakerInstance;
    private ShakeData _runtimeShakeData;

    void Start()
    {
        if (portalEffectObject != null)
        {
            portalEffectObject.SetActive(false);
        }
        if (Camera.main != null)
        {
            initialCameraSize = Camera.main.orthographicSize;
        }

        if (portalShakeData == null)
        {
            portalShakeData = ScriptableObject.CreateInstance<ShakeData>();
        }
    
        _runtimeShakeData = portalShakeData.CreateInstance();
    
        _runtimeShakeData.SetTotalDuration(-1f);
        _runtimeShakeData.SetFadeInDuration(0.5f);
        _runtimeShakeData.SetFadeOutDuration(0.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInPortal = true;
            if (portalCoroutine == null)
            {
                portalCoroutine = StartCoroutine(PortalSequence());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInPortal = false;
            if (portalCoroutine != null)
            {
                StopCoroutine(portalCoroutine);
                portalCoroutine = null;
            }
            ResetPortalEffect();
        }
    }

    private IEnumerator PortalSequence()
    {
        timeInPortal = 0f;

        _shakerInstance = CameraShakerHandler.Shake(_runtimeShakeData);
        if (_shakerInstance != null)
        {
            _shakerInstance.MultiplyMagnitude(0f, 0f, false);
        }

        if (portalEffectObject != null)
        {
            portalEffectObject.SetActive(true);
        }

        while (timeInPortal < timeToTeleport)
        {
            if (!playerInPortal)
            {
                ResetPortalEffect();
                yield break;
            }

            timeInPortal += Time.deltaTime;
            float progress = Mathf.Clamp01(timeInPortal / timeToTeleport);

            if (Camera.main != null)
            {
                Camera.main.orthographicSize = Mathf.Lerp(initialCameraSize, initialCameraSize * 0.8f, progress);
            }

            if (_shakerInstance != null)
            {
                _shakerInstance.MultiplyMagnitude(progress * maxShakeMagnitude, 0.1f, false);
            }

            yield return null;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private void ResetPortalEffect()
    {
        timeInPortal = 0f;
        if (portalEffectObject != null)
        {
            portalEffectObject.SetActive(false);
        }
        if (Camera.main != null)
        {
            Camera.main.orthographicSize = initialCameraSize;
        }

        if (_shakerInstance != null)
        {
            _shakerInstance.FadeOut(0.5f);
            _shakerInstance = null;
        }
    }

    private void OnDisable()
    {
        if (playerInPortal)
        {
            ResetPortalEffect();
        }
    }
}
