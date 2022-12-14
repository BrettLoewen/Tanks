using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    #region Variables

    [SerializeField] private int playerNumber = 1;
    [SerializeField] private PlayerInputHandler inputHandler;

    [SerializeField] private float speed = 12f;
    [SerializeField] private float turnSmoothTime;
    private float turnSmoothVelocity;

    [SerializeField] private AudioSource movementAudio;
    [SerializeField] private AudioClip engineIdleClip;
    [SerializeField] private AudioClip engineDrivingClip;
    [SerializeField] private float pitchRange = 0.2f;
    private float originalPitch;

    private Vector2 moveInput;  //The input telling the tank how to move

    private Rigidbody rb;   //The tank's rigidbody

    #endregion //end Variables

    #region Unity Control Methods

    //Awake is called before Start before the first frame update
    void Awake()
    {
        //Get and store the tank's rigidbody
        rb = GetComponent<Rigidbody>();
    }//end Awake

    //Start is called before the first frame update
    void Start()
    {
        //Get and store the original pitch of the movement audio source
        originalPitch = movementAudio.pitch;
    }//end Start

    //OnEnable is called whenever the gameobject is enabled
    private void OnEnable()
    {
        //If the tank is enabled, turn on physics
        rb.isKinematic = false;
    }//end OnEnable

    //OnDisable is caleld whenever the gameobject is disabled
    private void OnDisable()
    {
        //If the tank is disabled, turn off physics
        rb.isKinematic = true;
    }//end OnDisable

    //Update is called once per frame
    void Update()
    {
        //Get the input telling the tank how to move
        moveInput = inputHandler.GetMoveInput();

        //Play the correct engine audio
        EngineAudio();
    }//end Update

    //FixedUpdate is called a set amount of times every second
    private void FixedUpdate()
    {
        //Move and turn the tank according to its movement input
        Move();
    }//end FixedUpdate

    #endregion //end Unity Control Methods

    #region

    /// <summary>
    /// Play the correct audio clip based on whether or not the tank is moving
    /// </summary>
    private void EngineAudio()
    {
        //If the tank is not receiving input telling it to move
        if(Mathf.Abs(moveInput.magnitude) < 0.1f)
        {
            //If the audio source was playing the driving clip, play the idle clip
            if(movementAudio.clip == engineDrivingClip)
            {
                movementAudio.clip = engineIdleClip;
                movementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
                movementAudio.Play();
            }
        }
        //If the tank is receiving input telling it to move
        else
        {
            //If the audio source was playing the idle clip, play the driving clip
            if (movementAudio.clip == engineIdleClip)
            {
                movementAudio.clip = engineDrivingClip;
                movementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
                movementAudio.Play();
            }
        }
    }//end EngineAudio

    /// <summary>
    /// Move and rotate the tank according to the movement input gathered in Update from the PlayerInputHandler
    /// </summary>
    private void Move()
    {
        //Make a movement direction vector using the move input
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        //Only move and rotate the tank if there is sufficient input
        if(direction.magnitude >= 0.1f)
        {
            //Get and smooth the angle for the movement direction relative to the camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //Rotate the player to the angle
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            //Turn the move angle into a new movement direction
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            //Move the character
            rb.MovePosition(rb.position + (moveDir.normalized * speed * Time.deltaTime));
        }
    }//end Move

    #endregion
}
