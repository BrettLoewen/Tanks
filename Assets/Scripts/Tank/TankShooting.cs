using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    #region Variables

    public PlayerInputHandler inputHandler;   //Use this to receive inputs
    private Vector2 aimInput;                                   //The received input showing the direction that the player is aiming

    [SerializeField] private Rigidbody shellPrefab;         //The prefab of the shell to be spawned and launched
    [SerializeField] private Transform shootPoint;          //The point where the shell will be spawned
    [SerializeField] private Slider aimSlider;              //Used to visualise how far the shell will be launched
    [SerializeField] private float minLaunchForce = 15f;    //The starting launch force for the shell
    [SerializeField] private float maxLaunchForce = 30f;    //The highest possible launch force for the shell
    private float currentLaunchForce;                       //The launch force to use when launching the shell
    [SerializeField] private float maxChargeTime = 0.75f;   //The amount of time to reach the maximum launch force
    private float chargeSpeed;                              //The speed at which the launch force increases while charging
    
    [SerializeField] private AudioSource shootingAudio;     //The audio source used to play shooting audio
    [SerializeField] private AudioClip chargingClip;        //The clip used to play charging audio
    [SerializeField] private AudioClip shootClip;           //The clip used to play shooting audio
    
    [SerializeField] private Transform turret;              //The turret of the tank, so it can be rotated
    [SerializeField] private Transform shootPointParent;    //The parent of the shell spawn point, so it can be rotated
    [SerializeField] private float turretTurnTime;          //The time it will take to rotate the turret to its new angle
    private float turretTurnSpeed;                          //Used to store calculation data for rotating the turret

    [SerializeField] private int maxNumShots = 3;               //The maximum number of shots the tank can have
    private float currentNumShots;                              //The current number of shots the tank has
    [SerializeField] private float maxShotRechargeTime = 1f;    //The time it takes to regain one shot after it has been fired
    private float shotRechargeSpeed;                            //The speed at which shots are regained
    [SerializeField] private Slider ammoSlider;                 //Used to visualise the tank's current number of shots
    [SerializeField] private Image ammoSliderFill;              //Used to set the color of the ammo slider

    private bool hasShot;   //Stores whether or not the tank has launched the currently charged shot

    #endregion //end Variables

    #region Unity Control Methods

    // Awake is called before Start before the first frame update
    void Awake()
    {
        
    }//end Awake

    // Start is called before the first frame update
    void Start()
    {
        //Initialize charge and recharge speeds
        chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
        shotRechargeSpeed = 1 / maxShotRechargeTime;
    }//end Start

    // OnEnable is called whenever the gameobject is enabled
    private void OnEnable()
    {
        //Initialize the values for launching
        currentLaunchForce = minLaunchForce;
        aimSlider.value = minLaunchForce;

        //Initialize the values for tracking shot counts
        currentNumShots = maxNumShots;
        ammoSlider.value = currentNumShots;
        ammoSlider.maxValue = maxNumShots;
    }//end OnEnable

    // Update is called once per frame
    void Update()
    {
        if(inputHandler.GetShootPrimaryStartInput())
        {
            Debug.Log("Shoot");
        }
        //Ensure the value for the aimSlider gets updated
        aimSlider.value = minLaunchForce;

        //Aim the turret according to aim input
        Aim();

        //If the tank is not at its maximum number of shots, recharge
        if(currentNumShots < maxNumShots)
        {
            currentNumShots += shotRechargeSpeed * Time.deltaTime;
        }
        //If the tank is at its maximum number of shots, don't let it get more
        else
        {
            currentNumShots = maxNumShots;
        }

        //Update the value of the ammo slider according to the number of shots the tank has left
        ammoSlider.value = currentNumShots;

        //If the tank is out of shots, make the slider red and do not allow it to shoot
        if (currentNumShots < 1f)
        {
            ammoSliderFill.color = Color.red;
            return;
        }
        //If the tank has shots left, make the slider green and allow it to shoot
        else
        {
            ammoSliderFill.color = Color.green;
        }

        //If we are at max charge but have not shot yet
        if (currentLaunchForce >= maxLaunchForce && !hasShot)
        {
            //Ensure the launch force does not exceed the max launch force and shoot the shell
            currentLaunchForce = maxLaunchForce;
            Shoot();
        }
        //If we just started pressing the shoot button
        else if(inputHandler.GetShootPrimaryStartInput())
        {
            //Start charging the shot
            hasShot = false;
            currentLaunchForce = minLaunchForce;

            //Play the charging audio
            shootingAudio.clip = chargingClip;
            shootingAudio.Play();
        }
        //If we are holding the shoot button
        else if(inputHandler.GetShootPrimaryHoldInput() && !hasShot)
        {
            //Continue charging the shot
            currentLaunchForce += chargeSpeed * Time.deltaTime;

            //Display the charge
            aimSlider.value = currentLaunchForce;
        }
        //If we just released the shoot button
        else if(inputHandler.GetShootPrimaryReleaseInput() && !hasShot)
        {
            //Shoot the shell with the current launch force
            Shoot();
        }
    }//end Update

    #endregion //end Unity Control Methods

    #region
    
    /// <summary>
    /// Get the aim input and use it to rotate the turret and shoot point so the tank shoots in the direction that the player is aiming
    /// </summary>
    private void Aim()
    {
        //Get the aim input from the PlayerInputHandler
        aimInput = inputHandler.GetAimInput();

        //Make a aim direction vector using the aim input
        Vector3 direction = new Vector3(aimInput.x, 0f, aimInput.y).normalized;

        //Only rotate the turret and UI if there is sufficient input
        if (direction.magnitude >= 0.1f)
        {
            //Get and smooth the angle for the aim direction relative to the camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(turret.eulerAngles.y, targetAngle, ref turretTurnSpeed, turretTurnTime);

            //Smoothly rotate the turret to the angle
            turret.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            //Rotate the shoot point to the angle (without smoothing so the player always shoots exactly where they are aiming)
            shootPointParent.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
    }//end Aim

    /// <summary>
    /// Spawn the shell projectile, launch it, play the shoot audio, and update variable values to record this launch
    /// </summary>
    private void Shoot()
    {
        //Record that the shell has been fired
        hasShot = true;

        //Reduce the number of shots left in the tank
        currentNumShots--;

        //Spawn the shell at the correct location
        Rigidbody shell = Instantiate(shellPrefab, shootPoint.position, shootPoint.rotation);

        //Give the shell its velocity
        shell.velocity = shootPoint.forward * currentLaunchForce;

        //Play the shoot audio
        shootingAudio.clip = shootClip;
        shootingAudio.Play();

        //Reset the current launch force
        currentLaunchForce = minLaunchForce;
    }//end Shoot

    #endregion
}
