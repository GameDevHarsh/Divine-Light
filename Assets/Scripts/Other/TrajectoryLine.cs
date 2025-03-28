using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TrajectoryLine : MonoBehaviour
{
    // Public Variables: General Settings
    [Header("General Settings")]
    public float stepSize = 0.1f; // Distance between points on the line
    public int maxReflections = 3; // Maximum number of reflections

    // Public Variables: Layer Masks
    [Header("Layer Masks")]
    public LayerMask mirrorLayer; // Layer to identify mirrors
    public LayerMask obstacleLayer; // Layer to identify obstacles

    // Public Variables: Prefabs
    [Header("Prefabs")]
    public Light2D lightPrefab; // Prefab of Light2D to be instantiated
    public GameObject boxColliderPrefab; // Prefab of BoxCollider2D

    // Public Variables: Collider Settings
    [Header("Collider Settings")]
    public float colliderWidth = 1f; // Width of the BoxCollider2D

    // Private Variables
    private LineRenderer lineRenderer; // LineRenderer for trajectory visualization
    private List<BoxCollider2D> colliders = new List<BoxCollider2D>(); // List of active colliders
    private List<Light2D> lights = new List<Light2D>(); // List of active Light2D instances

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        DrawTrajectory();
    }

    // Main Function: Draw the trajectory
    void DrawTrajectory()
    {
        if (lineRenderer == null)
            return;

        Vector3[] points = CalculateTrajectory();
        UpdateLightShapes(points);
        UpdateColliders(points);

        if (points.Length > 0)
        {
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
        else
        {
            lineRenderer.positionCount = 0; // Clear the line if no points
        }
    }

    // Calculate the trajectory points
    Vector3[] CalculateTrajectory()
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 position = transform.position;
        Vector3 direction = transform.forward * 10f; // Initial velocity (assumed to be along Z)

        points.Add(position); // Add the starting point

        for (int reflection = 0; reflection <= maxReflections; reflection++)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, direction, Mathf.Infinity, mirrorLayer | obstacleLayer);

            if (hit.collider != null)
            {
                if (((1 << hit.collider.gameObject.layer) & mirrorLayer) != 0)
                {
                    points.Add(hit.point); // Add the hit point
                    direction = Vector2.Reflect(direction, hit.normal); // Reflect direction
                    position = (Vector3)hit.point + direction * 0.1f; // Move position slightly
                }
                else if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0)
                {
                    points.Add(hit.point); // Add the hit point

                    // Special handling for IceDoor
                    if (hit.collider.gameObject.CompareTag("IceDoor"))
                    {
                        hit.collider.GetComponent<Animator>().enabled = true;
                    }
                    break; // Trajectory ends on obstacle
                }
                else
                {
                    break; // Stop if hit is not a mirror or obstacle
                }
            }
            else
            {
                position += direction * stepSize; // Extend the trajectory
                points.Add(position);
            }
        }

        return points.ToArray();
    }

    // Update Light2D shapes along the trajectory
    void UpdateLightShapes(Vector3[] points)
    {
        // Ensure enough lights for each segment
        while (lights.Count < points.Length - 1)
        {
            Light2D newLight = Instantiate(lightPrefab, transform);
            lights.Add(newLight);
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 start = points[i];
            Vector3 end = points[i + 1];
            Light2D light = lights[i];

            // Set light position
            light.transform.position = start;

            // Calculate direction, distance, and rotation
            Vector3 direction = end - start;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            light.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Define light shape
            float yOffset = angle < 0 ? 0.5f : -0.5f;
            List<Vector3> lightPath = new List<Vector3>
            {
                new Vector3(0, yOffset, 0),
                new Vector3(distance, yOffset, 0),
                new Vector3(distance, yOffset + 1, 0),
                new Vector3(0, yOffset + 1, 0)
            };
            light.SetShapePath(lightPath.ToArray());
            light.gameObject.SetActive(true);
        }

        // Deactivate excess lights
        for (int i = points.Length - 1; i < lights.Count; i++)
        {
            lights[i].gameObject.SetActive(false);
        }
    }

    // Update colliders along the trajectory
    void UpdateColliders(Vector3[] points)
    {
        float backOffset = 1f; // Offset at the start of the segment
        float frontOffset = 0.5f; // Offset at the end of the segment

        // Ensure enough colliders for each segment
        while (colliders.Count < points.Length - 1)
        {
            GameObject newColliderObject = Instantiate(boxColliderPrefab, transform);
            BoxCollider2D newCollider = newColliderObject.GetComponent<BoxCollider2D>();
            colliders.Add(newCollider);
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 start = points[i];
            Vector3 end = points[i + 1];
            BoxCollider2D collider = colliders[i];

            Vector3 direction = (end - start).normalized;
            Vector3 adjustedStart = start - direction * backOffset;
            Vector3 adjustedEnd = end + direction * frontOffset;
            float length = Vector3.Distance(adjustedStart, adjustedEnd);
            Vector3 midpoint = (adjustedStart + adjustedEnd) / 2;

            collider.transform.position = midpoint;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            collider.transform.rotation = Quaternion.Euler(0, 0, angle);
            collider.size = new Vector2(length, colliderWidth);

            collider.gameObject.SetActive(true);
        }

        // Deactivate excess colliders
        for (int i = points.Length - 1; i < colliders.Count; i++)
        {
            colliders[i].gameObject.SetActive(false);
        }
    }
}
