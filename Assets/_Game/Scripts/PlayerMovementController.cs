using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private CharacterController controller;

    [SerializeField] private Vector3 forward;
    [SerializeField] private Vector3 right;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (right * horizontal + forward * vertical).normalized;

        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }
}
