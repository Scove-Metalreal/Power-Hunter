using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    [Header("Movement Points")]
    [Tooltip("Drag the first target object here.")]
    public Transform pointA;
    [Tooltip("Drag the second target object here.")]
    public Transform pointB;

    [Header("Movement Settings")]
    [Tooltip("The speed at which the object moves between points.")]
    [Range(0.1f, 10f)]
    public float moveSpeed = 2.0f;

    private Transform nextPoint;
    private bool movingToB = true;

    void Start()
    {
        // Ensure points are assigned
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Movement points are not assigned. Please drag target objects to the pointA and pointB fields in the Inspector.");
            enabled = false; // Disable the script
            return;
        }

        // Initialize the starting position and the first target
        transform.position = pointA.position;
        nextPoint = pointB;
    }

    void Update()
    {
        // Ensure points are still valid
        if (pointA == null || pointB == null) return;

        // Move the object towards the next target point
        transform.position = Vector3.MoveTowards(transform.position, nextPoint.position, moveSpeed * Time.deltaTime);

        // Check if the object has reached the current target point
        if (Vector3.Distance(transform.position, nextPoint.position) < 0.01f)
        {
            // Switch the target point
            movingToB = !movingToB;
            nextPoint = movingToB ? pointB : pointA;
        }
    }

    // Optional: Draw gizmos in the editor to visualize the points
    void OnDrawGizmos()
    {
        // Do not draw gizmos if points are not assigned
        if (pointA == null || pointB == null) return;

        // Draw spheres at the points and a line between them
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pointA.position, 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointB.position, 0.3f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pointA.position, pointB.position);
    }
}
