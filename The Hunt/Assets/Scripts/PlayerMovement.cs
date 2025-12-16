using UnityEngine;

public interface IPlayerControllerControl
{
    void SetCanMove(bool value);
    void SetCanLook(bool value);
}
public class PlayerMovement : MonoBehaviour, IPlayerControllerControl
{

    public float MoveSmoothTime;
    public float GravityStrenght;
    public float JumpStrenght;
    public float walkSpeed;
    public float RunSpeed;

    private CharacterController Controller;
    private Vector3 CurrentMoveVelocity;
    private Vector3 MoveDampVelocity;

    private Vector3 CurrentForceVelocity;

    public bool canMove = true;


    PlayerLook look;
    
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Controller = GetComponent<CharacterController>();

        look = GetComponent<PlayerLook>();
    }



    public void SetCanMove(bool value)
    {
        canMove = value;
     //   Controller.enabled = value;
        if (!canMove)
            StopMovement();
    }

    public void SetCanLook(bool value)
    {
        look.canLook = value;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) 
        {
            CurrentForceVelocity = Vector3.zero;
            CurrentMoveVelocity = Vector3.zero;
            Controller.Move(Vector3.zero);
            return;
        
        }
        
        Vector3 PlayerInput = new Vector3 { x = Input.GetAxisRaw("Horizontal"), y = 0f, z = Input.GetAxisRaw("Vertical") };

        if(PlayerInput.magnitude > 1f)
        {
            PlayerInput.Normalize();
        }

        Vector3 MoveVector = transform.TransformDirection(PlayerInput);
        float CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? RunSpeed : walkSpeed;

        CurrentMoveVelocity = Vector3.SmoothDamp(CurrentMoveVelocity, MoveVector * CurrentSpeed, ref MoveDampVelocity, MoveSmoothTime);

        Controller.Move(CurrentMoveVelocity * Time.deltaTime);

        Ray groundCheckRay = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(groundCheckRay, 1.1f))
        {
            CurrentForceVelocity.y = -2f;

            if (Input.GetKey(KeyCode.Space))
            {
                CurrentForceVelocity.y = JumpStrenght;
            }

        }
        else
        {
            CurrentForceVelocity.y -= GravityStrenght * Time.deltaTime;
        }

        Controller.Move(CurrentForceVelocity * Time.deltaTime);



    }

    public void StopMovement()
    {
        CurrentMoveVelocity = Vector3.zero;
        CurrentForceVelocity = Vector3.zero;
    }
}
