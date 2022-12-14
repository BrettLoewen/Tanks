using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    #region Variables

    [SerializeField] private Vector2 moveInput; //Stores the input data for movement

    #endregion //end Variables

    #region Unity Control Methods

    // Awake is called before Start before the first frame update
    void Awake()
    {
        
    }//end Awake

    // Start is called before the first frame update
    void Start()
    {
        
    }//end Start

    // Update is called once per frame
    void Update()
    {
        
    }//end Update

    #endregion //end Unity Control Methods

    #region Move

    /// <summary>
    /// Used to receive input events from a PlayerInput component
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
}
