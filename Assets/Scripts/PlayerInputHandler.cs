using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    #region Variables
    static int count;

    public Player player;

    [SerializeField] private Vector2 moveInput; //Stores the input data for moving the tank
    [SerializeField] private Vector2 aimInput;  //Stores the input data for aiming the tank's turret

    private bool shootPrimaryStartFlag;     //
    private bool shootPrimaryHoldFlag;      //
    private bool shootPrimaryReleaseFlag;   //

    private bool dashFlag;      //

    private bool pauseFlag;     //

    private bool leaveFlag;     //
    private bool selectFlag;    //

    #endregion //end Variables

    #region Unity Control Methods

    private void Awake()
    {
        transform.parent = PlayerInputManager.instance.transform;
        count++;
        gameObject.name = "Player " + count;
    }

    // Update is called once every frame
    private void LateUpdate()
    {
        //Cancel flags if the input has finished
        shootPrimaryStartFlag = false;
        shootPrimaryReleaseFlag = false;
        dashFlag = false;
        leaveFlag = false;
        selectFlag = false;
        pauseFlag = false;
    }//end Update

    #endregion //end Unity Control Methods

    #region Move

    /// <summary>
    /// Used to receive movement input events from a PlayerInput component
    /// </summary>
    /// <param name="context">Contains data describing the input action that occured</param>
    public void ReceiveMoveInput(InputAction.CallbackContext context)
    {
        //Read the input data as a vector2 and store it
        moveInput = context.ReadValue<Vector2>();
    }//end ReceiveMoveInput

    /// <summary>
    /// Return the movement input data that was received and stored
    /// </summary>
    /// <returns>Return the movement input data that was received and stored</returns>
    public Vector2 GetMoveInput()
    {
        //Return the movement input data
        return moveInput;
    }//end GetMoveInput

    #endregion //end Move

    #region Aim

    /// <summary>
    /// Used to receive aiming input events from a PlayerInput component
    /// </summary>
    /// <param name="context">Contains data describing the input action that occured</param>
    public void ReceiveAimInput(InputAction.CallbackContext context)
    {
        //Read the input data as a vector2 and store it
        aimInput = context.ReadValue<Vector2>();
    }//end ReceiveAimInput

    /// <summary>
    /// Return the aim input data that was received and stored
    /// </summary>
    /// <returns>Return the aim input data that was received and stored</returns>
    public Vector2 GetAimInput()
    {
        //Return the aim input data
        return aimInput;
    }//end GetAimInput

    #endregion //end Aim

    #region Shoot Primary

    /// <summary>
    /// Used to receive shooting (primary) input events from a PlayerInput component
    /// </summary>
    /// <param name="context">Contains data describing the input action that occured</param>
    public void ReceiveShootPrimaryInput(InputAction.CallbackContext context)
    {
        //If the input was started (to prevent input from holding or releasing), store that the button was pressed
        if(context.started)
        {
            shootPrimaryStartFlag = true;
            shootPrimaryHoldFlag = true;
        }
        else if(context.canceled)
        {
            shootPrimaryHoldFlag = false;
            shootPrimaryReleaseFlag = true;
        }
    }//end ReceiveShootPrimaryInput

    /// <summary>
    /// Return the shooting (primary) (start) input data that was received and stored
    /// </summary>
    /// <returns>Returns true if the the shooting (primary) input just started and false otherwise</returns>
    public bool GetShootPrimaryStartInput()
    {
        //Return the value stored in the shoot primary start flag
        return GameManager.IsPaused ? false : shootPrimaryStartFlag;
    }//end GetShootPrimaryStartInput

    /// <summary>
    /// Return the shooting (primary) (holding) input data that was received and stored
    /// </summary>
    /// <returns>Returns true if the shooting (primary) input is being held down</returns>
    public bool GetShootPrimaryHoldInput()
    {
        //Return the value stored in the shoot primary hold flag
        return GameManager.IsPaused ? false : shootPrimaryHoldFlag;
    }//end GetShootPrimaryHoldInput

    /// <summary>
    /// Return the shooting (primary) (release) input data that was received and stored
    /// </summary>
    /// <returns>Returns true if the the shooting (primary) input was just released and false otherwise</returns>
    public bool GetShootPrimaryReleaseInput()
    {
        //Return the value stored in the shoot primary release flag
        return GameManager.IsPaused ? false : shootPrimaryReleaseFlag;
    }//end GetShootPrimaryReleaseInput

    #endregion //end Shoot Primary

    #region Leave

    /// <summary>
    /// Used to receive leave input events from a PlayerInput component
    /// </summary>
    /// <param name="context">Contains data describing the input action that occured</param>
    public void ReceiveLeaveInput(InputAction.CallbackContext context)
    {
        //If the leave input started, store it
        if(context.started)
        {
            leaveFlag = true;
        }
    }//end ReceiveLeaveInput

    /// <summary>
    /// Return the leave input data that was received and stored
    /// </summary>
    /// <returns>Returns true if the leave input started</returns>
    public bool GetLeaveInput()
    {
        return leaveFlag;
    }//end GetLeaveInput

    #endregion //end Leave

    #region Select

    /// <summary>
    /// Used to receive select input events from a PlayerInput component
    /// </summary>
    /// <param name="context">Contains data describing the input action that occured</param>
    public void ReceiveSelectInput(InputAction.CallbackContext context)
    {
        //If the select input started, store it
        if (context.started)
        {
            selectFlag = true;
        }
    }//end ReceiveSelectInput

    /// <summary>
    /// Return the select input data that was received and stored
    /// </summary>
    /// <returns>Returns true if the select input started</returns>
    public bool GetSelectInput()
    {
        return selectFlag;
    }//end GetSelectInput

    #endregion //end Select

    #region Dash

    /// <summary>
    /// Used to receive dash input events from a PlayerInput component
    /// </summary>
    /// <param name="context">Contains data describing the input action that occured</param>
    public void ReceiveDashInput(InputAction.CallbackContext context)
    {
        //If the dash input started, store it
        if (context.started)
        {
            dashFlag = true;
        }
    }//end ReceiveDash

    /// <summary>
    /// Return the dash input data that was received and stored
    /// </summary>
    /// <returns>Returns true if the dash input started</returns>
    public bool GetDashInput()
    {
        return dashFlag;
    }//end GetDashInput

    #endregion //end Dash

    #region Pause

    /// <summary>
    /// Used to receive pause input events from a PlayerInput component
    /// </summary>
    /// <param name="context">Contains data describing the input action that occured</param>
    public void ReceivePauseInput(InputAction.CallbackContext context)
    {
        //If the dash input started, store it
        if (context.started)
        {
            pauseFlag = true;
        }
    }//end ReceivePause

    /// <summary>
    /// Return the pause input data that was received and stored
    /// </summary>
    /// <returns>Returns true if the pause input started</returns>
    public bool GetPauseInput()
    {
        return pauseFlag;
    }//end GetPauseInput

    #endregion //end Pause

    #region Device Lost / Regained

    /// <summary>
    /// Receive the information that the player's device was lost and act accordingly
    /// </summary>
    public void ReceiveDeviceLost()
    {
        //If we are in a game, tell the game manager that the device was lost
        if(GameManager.InGame)
        {
            GameManager.Instance.LostDevice(player);
        }
        //If we are on the main menu, delete the lost player
        else
        {
            Destroy(gameObject);
        }
    }//end ReceiveDeviceLost

    /// <summary>
    /// Receive the information that the player's device was regained and tell the game manager
    /// </summary>
    public void ReceiveDeviceRegained()
    {
        GameManager.Instance.RegainedDevice();
    }//end ReceiveDeviceRegained

    #endregion //end Device Lost / Regained
}
