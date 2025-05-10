using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairSystem : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private GameObject crosshairPrefab;
    [SerializeField] private float hoverHeight = 0.1f; // Height above ground
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color invalidTargetColor = Color.red;

    private Camera mainCamera;
    private GameObject crosshairInstance;
    private Renderer crosshairRenderer;
    private RaycastHit hitInfo;
    
    // Public property to access the current target position
    public Vector3 TargetPosition { get; private set; }
    public bool HasValidTarget { get; private set; }

    private void Awake()
    {
        mainCamera = Camera.main;
        
        // Create the crosshair instance
        if (crosshairPrefab != null)
        {
            crosshairInstance = Instantiate(crosshairPrefab);
            crosshairRenderer = crosshairInstance.GetComponentInChildren<Renderer>();
            
            // Make sure the crosshair doesn't collide with anything
            Collider[] colliders = crosshairInstance.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
        }
        else
        {
            Debug.LogError("Crosshair prefab is not assigned!");
        }
    }

    private void Update()
    {
        UpdateCrosshairPosition();
    }

    private void UpdateCrosshairPosition()
    {
        if (crosshairInstance == null || mainCamera == null)
            return;

        // Create a ray from the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        // Default position if no hit
        HasValidTarget = false;
        
        // Check if the ray hits something in the ground layer
        if (Physics.Raycast(ray, out hitInfo, maxDistance, groundLayer))
        {
            // Set the crosshair position slightly above the hit point
            TargetPosition = hitInfo.point;
            crosshairInstance.transform.position = hitInfo.point + new Vector3(0, hoverHeight, 0);
            
            // Mark as valid target
            HasValidTarget = true;
            
            // Set the crosshair's default color
            if (crosshairRenderer != null)
            {
                SetCrosshairColor(defaultColor);
            }
        }
        else
        {
            // If no hit, position the crosshair at a distance along the ray
            TargetPosition = ray.GetPoint(maxDistance);
            crosshairInstance.transform.position = ray.GetPoint(maxDistance);
            
            // Set the crosshair's invalid color
            if (crosshairRenderer != null)
            {
                SetCrosshairColor(invalidTargetColor);
            }
        }
        
        // Make the crosshair face the camera (billboard effect)
        crosshairInstance.transform.rotation = Quaternion.LookRotation(
            crosshairInstance.transform.position - mainCamera.transform.position
        );
    }
    
    private void SetCrosshairColor(Color color)
    {
        // If the renderer has materials, set their color
        if (crosshairRenderer != null && crosshairRenderer.materials.Length > 0)
        {
            foreach (Material mat in crosshairRenderer.materials)
            {
                // Attempt to set color for different shader properties
                if (mat.HasProperty("_Color"))
                {
                    mat.color = color;
                }
                else if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", color);
                }
                else if (mat.HasProperty("_EmissionColor"))
                {
                    mat.SetColor("_EmissionColor", color);
                }
            }
        }
    }
    
    // Get the direction from player to crosshair
    public Vector3 GetDirectionFromPlayer(Transform playerTransform)
    {
        Vector3 direction = TargetPosition - playerTransform.position;
        direction.y = 0; // Keep the direction on the horizontal plane
        return direction.normalized;
    }
    
    // For debugging purposes
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && HasValidTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, TargetPosition);
            Gizmos.DrawWireSphere(TargetPosition, 0.3f);
        }
    }
}
