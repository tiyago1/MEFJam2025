using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    // Components
    private Rigidbody rb;
    private Camera mainCamera;
    private Animator animator;

    // Movement state
    private Vector3 moveDirection;
    private bool isDashing;
    private bool canDash = true;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Calculate movement direction based on camera orientation for isometric feel
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        // Project vectors onto XZ plane
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Calculate move direction
        moveDirection = forward * vertical + right * horizontal;

        // Normalize movement direction if magnitude > 1
        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }

        // Handle dash when space is pressed
        if (Input.GetKeyDown(KeyCode.Space) && canDash && isGrounded)
        {
            StartCoroutine(Dash());
        }

        // Update animation parameters if animator exists
        if (animator != null)
        {
            animator.SetFloat("Speed", moveDirection.magnitude);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsDashing", isDashing);
        }
    }

    private void FixedUpdate()
    {
        // Move the player
        if (!isDashing)
        {
            // Apply movement
            Vector3 movement = moveDirection * moveSpeed;
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

            // Rotate player to face movement direction
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // Get mouse position in world space
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(20f);
        }

        // Calculate dash direction (towards mouse position)
        Vector3 dashDirection = targetPoint - transform.position;
        dashDirection.y = 0; // Keep dash on the horizontal plane
        dashDirection.Normalize();

        // Apply dash force
        rb.velocity = Vector3.zero; // Reset velocity before dash
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);

        // Rotate player to face dash direction
        transform.rotation = Quaternion.LookRotation(dashDirection);

        // Wait a short time for dash to complete
        yield return new WaitForSeconds(0.25f);
        isDashing = false;

        // Apply cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check sphere
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}