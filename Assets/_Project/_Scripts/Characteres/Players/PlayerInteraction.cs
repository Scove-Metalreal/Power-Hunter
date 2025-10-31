using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("An empty GameObject in front of the player where objects will be held.")]
    public Transform holdPoint;

    [Header("Throwing")]
    [Tooltip("The angle of the throw in degrees. 0 is straight, 45 is a high arc.")]
    public float throwAngle = 30f;
    [Tooltip("The minimum force of a throw.")]
    public float minThrowForce = 5f;
    [Tooltip("The maximum force of a throw at full charge.")]
    public float maxThrowForce = 20f;
    [Tooltip("The maximum time in seconds to charge a throw for full power.")]
    public float maxChargeTime = 2f;
    [Tooltip("The UI canvas group for the charge bar.")]
    public CanvasGroup chargeBarCanvas;
    [Tooltip("The UI Image component for the charge bar's fill.")]
    public Image chargeBarFill;

    // --- Private State ---
    private List<ThrowableObject> nearbyThrowables = new List<ThrowableObject>();
    private ThrowableObject closestThrowable;
    private ThrowableObject heldObject;
    private bool isChargingThrow = false;
    private float chargeTime = 0f;

    private void Start()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
        if (holdPoint == null) Debug.LogError("Hold Point is not set!", gameObject);
        if (chargeBarCanvas != null) chargeBarCanvas.alpha = 0; // Hide charge bar initially
    }

    private void Update()
    {
        if (heldObject == null)
        {
            UpdateClosestObject();
        }

        HandlePickupAndDrop();
        HandleThrowing();
    }

    private void HandlePickupAndDrop()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null && closestThrowable != null)
            {
                PickupObject(closestThrowable);
            }
            else if (heldObject != null)
            {
                DropObject();
            }
        }
    }

    private void HandleThrowing()
    {
        if (heldObject == null) return;

        if (Input.GetMouseButtonDown(1))
        {
            isChargingThrow = true;
            chargeTime = 0f;
            if (chargeBarCanvas != null) chargeBarCanvas.alpha = 1;
        }

        if (Input.GetMouseButton(1) && isChargingThrow)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Min(chargeTime, maxChargeTime);
            if (chargeBarFill != null) chargeBarFill.fillAmount = chargeTime / maxChargeTime;
        }

                if (Input.GetMouseButtonUp(1) && isChargingThrow)
                {
                    float chargePercentage = chargeTime / maxChargeTime;
                    float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargePercentage);
        
                    // --- Calculate Angled Throw Direction ---
                    // 1. Determine horizontal direction based on player flip state
                    float horizontalDirection = (transform.localScale.x < 0) ? -1f : 1f;
                    
                    // 2. Convert angle from degrees to radians for Sin/Cos
                    float angleInRadians = throwAngle * Mathf.Deg2Rad;
        
                    // 3. Create the final vector with both horizontal and vertical components
                    Vector2 throwDirection = new Vector2(
                        horizontalDirection * Mathf.Cos(angleInRadians),
                        Mathf.Sin(angleInRadians)
                    );
        
                    ThrowableObject objectToThrow = heldObject;
                    heldObject = null;
                    objectToThrow.OnThrow(throwDirection, throwForce);
        
                    isChargingThrow = false;
                    chargeTime = 0f;
                    if (chargeBarCanvas != null) chargeBarCanvas.alpha = 0;
                    if (chargeBarFill != null) chargeBarFill.fillAmount = 0;
                }    }

    private void PickupObject(ThrowableObject throwable)
    {
        heldObject = throwable;
        heldObject.OnPickup(holdPoint);
        if (closestThrowable != null) closestThrowable.HidePrompt();
        closestThrowable = null;
        nearbyThrowables.Clear();
    }

    private void DropObject()
    {
        if (heldObject == null) return;
        heldObject.OnDrop();
        heldObject = null;
    }

    private void UpdateClosestObject()
    {
        nearbyThrowables.RemoveAll(item => item == null);
        ThrowableObject newClosest = null;
        if (nearbyThrowables.Count > 0)
        {
            newClosest = nearbyThrowables.OrderBy(t => Vector2.Distance(transform.position, t.transform.position)).FirstOrDefault();
        }
        if (newClosest != closestThrowable)
        {
            if (closestThrowable != null) closestThrowable.HidePrompt();
            if (newClosest != null) newClosest.ShowPrompt();
            closestThrowable = newClosest;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (heldObject != null) return;
        ThrowableObject throwable = other.GetComponent<ThrowableObject>();
        if (throwable != null && !nearbyThrowables.Contains(throwable))
        {
            nearbyThrowables.Add(throwable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ThrowableObject throwable = other.GetComponent<ThrowableObject>();
        if (throwable != null && nearbyThrowables.Contains(throwable))
        {
            if (throwable == closestThrowable)
            {
                throwable.HidePrompt();
                closestThrowable = null;
            }
            nearbyThrowables.Remove(throwable);
        }
    }
}
