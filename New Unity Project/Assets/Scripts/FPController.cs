using UnityEngine;

[RequireComponent(typeof(GravityPhysics))]
public class FPController : ControllerBase
{
    [SerializeField]
    private Camera playerCam;

    [SerializeField]
    private float jumpForce = 0.1f;

    private float currentJumpForce = 0f;

    protected bool isJumping;

    private GravityPhysics gravity;

    // Start is called before the first frame update
    void Start()
    {
        gravity = GetComponent<GravityPhysics>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    public override Vector3 ApplyPhysics(Vector3 currentPosition)
    {
        return PlayerInput(currentPosition);
    }

    public Vector3 PlayerInput(Vector3 position)
    {
        Rotate();

        Move(ref position);
        Jump(ref position);

        return position;
    }

    private void Move(ref Vector3 position)
    {
        float yPosition = position.y;
        position += transform.right * Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
        position += transform.forward * Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        position.y = yPosition;
    }

    private void Jump(ref Vector3 position)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentJumpForce = jumpForce;
            gravity.DisableGravity();
            Debug.Log($"GravityDisabled");
        }

        currentJumpForce = Mathf.Clamp(currentJumpForce - (Time.deltaTime / 10), 0f, jumpForce);
        position += Vector3.up * currentJumpForce;

        if(currentJumpForce <= 0f && !gravity.IsEnabled())
        {
            Debug.Log($"GravityEnabled");
            gravity.EnableGravity();
        }
    }

    private void Rotate()
    {
        //so only the camera rotates up and down
        playerCam.transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), 0.0f, 0.0f);
        transform.eulerAngles += new Vector3(0.0f, Input.GetAxis("Mouse X"), 0.0f);
    }
}