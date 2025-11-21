using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    private Rigidbody _rb;
    private float _distToGround;
    private bool _jumpRequest; 

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        // Calculate distance from center to feet automatically
        _distToGround = GetComponent<Collider>().bounds.extents.y;
    }
    
    void Update()
    {
        // "GetButtonDown" works best in Update. It catches the frame the button is hit.
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            _jumpRequest = true; // Remember that we want to jump
        }
    }
    
    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed;
        _rb.linearVelocity = new Vector3(movement.x, _rb.linearVelocity.y, movement.z);

        // If the flag is raised, perform the jump
        if (_jumpRequest)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpRequest = false; // Lower the flag
        }
    }

    bool IsGrounded()
    {
        // Raycast length is distance to feet + small buffer (0.1f)
        return Physics.Raycast(transform.position, Vector3.down, _distToGround + 0.1f);
    }
}