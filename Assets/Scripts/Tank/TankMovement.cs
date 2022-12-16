using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    #region Variables

    public PlayerInputHandler inputHandler;   //Stores the script which gathers and stores player input

    [SerializeField] private float speed = 12f;     //The movement speed of the tank
    [SerializeField] private float turnSmoothTime;  //The time over which the tank will rotate to face the direction of movement
    private float turnSmoothVelocity;               //Used tp store values for turn speed calculations

    [SerializeField] private float dashForce = 1000f;           //The force used to launch the tank during a dash
    [SerializeField] private float maxDashRechargeTime = 1f;    //The time it takes to recharge a dash
    private float currentDashRechargeTime;                      //The current charge of the dash
    [SerializeField] private ParticleSystem dashExplosion;      //The effect for when the dash triggers
    [SerializeField] private ParticleSystem dashExhaust;        //The effect for when the dash is charging

    [SerializeField] private AudioSource movementAudio;     //Used to play the engine audio
    [SerializeField] private AudioClip engineIdleClip;      //The audio clip for when the tank is not moving
    [SerializeField] private AudioClip engineDrivingClip;   //The aduio clip for when the tank is moving
    [SerializeField] private float pitchRange = 0.2f;       //A range for a random number modifier for pitch
    private float originalPitch;                            //Stores the original pitch to be used in the random number generation

    private Vector2 moveInput;  //The input telling the tank how to move
    private bool dashFlag;      //The input telling the tank whether or not to dash

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

        //
        currentDashRechargeTime = maxDashRechargeTime;
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
        dashFlag = inputHandler.GetDashInput();

        //Play the correct engine audio
        EngineAudio();
    }//end Update

    //FixedUpdate is called a set amount of times every second
    private void FixedUpdate()
    {
        //Move and turn the tank according to its movement input
        Move();

        //
        Dash();
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
                movementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange); //Random pitch to avoid audio phasing
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
                movementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange); //Random pitch to avoid audio phasing
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

    /// <summary>
    /// Allows the tank to dash forward according to player input
    /// </summary>
    private void Dash()
    {
        //If the dash is charged, disable the dash charging effect
        if(currentDashRechargeTime >= maxDashRechargeTime)
        {
            dashExhaust.Stop();
        }

        //If the player made a dash input, and the dash is charged, then dash
        if (dashFlag && currentDashRechargeTime >= maxDashRechargeTime)
        {
            rb.AddForce(transform.forward * dashForce); //Add force to make the tank to dash
            dashExplosion.Play();                       //Play the dash particle effect
            currentDashRechargeTime = 0f;               //Reset the dash charge
        }
        //If the dash is not fully charged, charge it
        else if(currentDashRechargeTime < maxDashRechargeTime)
        {
            currentDashRechargeTime += Time.deltaTime;  //Increase the current dash charge
            dashExhaust.Play();                         //Play the dash charging effect
        }
    }//end Dash

    #endregion
}
