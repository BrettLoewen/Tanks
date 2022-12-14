using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    #region Variables

    [SerializeField] private Vector2 moveInput; //Stores the input data for moving the tank
    [SerializeField] private Vector2 aimInput; //Stores the input data for aiming the tank's turret

    #endregion //end Variables

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
}
