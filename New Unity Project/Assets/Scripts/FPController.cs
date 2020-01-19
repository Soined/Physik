using UnityEngine;

[RequireComponent(typeof(GravityPhysics))]
public class FPController : ControllerBase
{
    [SerializeField]
    private Camera playerCam;

    [SerializeField]
    private float jumpForce = 0.1f;

    [SerializeField]
    private int maxJumps = 2;

    private int currentJumps = 0;

    private float currentJumpForce = 0f;

    protected bool isJumping;

    private bool controllsEnabled = true;

    private GravityPhysics gravity;

    private Vector3 startPosition;
    private Quaternion startRotation;

    // Start is called before the first frame update
    void Start()
    {
        gravity = GetComponent<GravityPhysics>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
    private void Update()
    {
        //Have to do it this way to get a proper nullCheck on the standingOn 
        if(PhysicsObject.collisionInfo.grounded)
        {
            currentJumps = maxJumps;
            if (PhysicsObject.collisionInfo.standingOn.tag == "KillGround") Die();
        }

        if(Input.GetKeyDown(KeyCode.R) && controllsEnabled)
        {
            Die();
        }
    }

    public override Vector3 ApplyPhysics(Vector3 currentPosition)
    {
        return PlayerInput(currentPosition);
    }

    public Vector3 PlayerInput(Vector3 position)
    {
        if (!controllsEnabled) return position;

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
        if (Input.GetKeyDown(KeyCode.Space) && CanJump())
        {
            currentJumpForce = jumpForce;
            currentJumps--;
            gravity.DisableGravity();
            Debug.Log($"GravityDisabled");
        }

        if(PhysicsObject.collisionInfo.collisionOnTop)
        {
            currentJumpForce = 0f;
        }

        currentJumpForce = Mathf.Clamp(currentJumpForce - (Time.deltaTime / 6), 0f, jumpForce);
        position += Vector3.up * currentJumpForce;

        if(currentJumpForce <= 0f && !gravity.IsEnabled())
        {
            Debug.Log($"GravityEnabled");
            gravity.EnableGravity();
        }
    }
    private bool CanJump()
    {
        //Might add more here later so it gets its own function already
        if (currentJumps > 0) return true;
        else return false;
    }

    private void Rotate()
    {
        //so only the camera rotates up and down
        playerCam.transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), 0.0f, 0.0f);
        transform.eulerAngles += new Vector3(0.0f, Input.GetAxis("Mouse X"), 0.0f);
    }

    public void Die()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    public void DisableControlls()
    {
        controllsEnabled = false;
    }
    public void EnableControlls()
    {
        controllsEnabled = true;
    }
}