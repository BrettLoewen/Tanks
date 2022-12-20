using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCursor : MonoBehaviour
{
    #region Variables

    private Player player;  //Stores a reference to the player that controls this cursor

    [SerializeField] private Image image;                       //Used to set the cursor's color
    [SerializeField] private TextMeshProUGUI playerNumberText;  //The cursor's display text
    [SerializeField] private float speed;                       //The speed at which the cursor moves

    private bool cursorIsOverButton = false;    //Stores whether or not the cursor is over the play button
    private CursorButton currentButton;         //Stores the button that cursor is currently hovering over

    private PlayerInputHandler inputHandler;    //The input handler that controls the cursor
    private Vector2 moveInput;                  //The movement input from the player

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
        //If the player wants to leave, destroy their input handler to remove them from the game
        if(inputHandler.GetLeaveInput())
        {
            Destroy(inputHandler.gameObject);
            return;
        }

        //Setup the cursor's display according to its player data
        image.color = player.playerColor;                   //Set the cursor's color to the player's color
        playerNumberText.text = "P" + player.playerNumber;  //Set the cursor's text (Ex. P1)

        //Move the cursor according to input from its player
        Move();

        //Handle checking for buttons and clicking on them
        HandleButtons();
    }//end Update

    /// <summary>
    /// When the cursor is destroyed, make sure it doesn't make the play button count incorrect
    /// </summary>
    private void OnDestroy()
    {
        //If the cursor is over the play button when it gets destroyed, tell the main menu
        if(cursorIsOverButton)
        {
            currentButton.RemoveCursor();
        }
    }//end OnDestroy

    #endregion //end Unity Control Methods

    #region

    /// <summary>
    /// Setup the cursor so it references the correct player
    /// </summary>
    /// <param name="_player">The player that controls this cursor</param>
    public void Setup(Player _player)
    {
        //Store a reference to the player that controls this cursor
        player = _player;

        //Set the input handler to the player's input handler
        inputHandler = _player.inputHandler;
    }//end Setup

    /// <summary>
    /// Return the stored reference to the player that controls this cursor
    /// </summary>
    /// <returns>Returns the Player that controls this PlayerCursor</returns>
    public Player GetPlayer()
    {
        return player;
    }//end GetPlayer

    /// <summary>
    /// Move the cursor according to input from its player
    /// </summary>
    private void Move()
    {
        //Get move input from the player
        moveInput = inputHandler.GetMoveInput();

        //Determine where to move the cursor to according to player input and move it accordingly
        Vector3 moveDir = new Vector3(moveInput.x, moveInput.y, 0f).normalized;
        transform.position += moveDir * speed * Time.deltaTime;
    }//end Move

    /// <summary>
    /// Determine whether or the cursor is over a button. If it is, tell the button that a cursor is hovering over it.<br></br>
    /// If the player presses Select while hovering over a button, tell the button it was clicked
    /// </summary>
    private void HandleButtons()
    {
        //Store whether or not the cursor was over a button last frame
        bool tempInBounds = cursorIsOverButton;
        CursorButton tempButton = currentButton;

        //Determine whether or not the cursor is over a button this frame, and if so, store a reference to that button
        currentButton = CursorButton.IsOverCursorButton(this);
        cursorIsOverButton = currentButton != null;

        //
        if (currentButton != null && tempButton != null && tempButton != currentButton)
        {
            tempButton.RemoveCursor();
        }

        //If the cursor is not over a button
        if (currentButton == null)
        {
            //If the cursor was over a button last frame, we need to remove ourselves from it, so store the reference
            if (tempButton != null)
            {
                currentButton = tempButton;
            }
            //If the cursor was not over a button last frame, stop
            else
            {
                return;
            }
        }

        //If the cursor starts being over a button, tell the button
        if (tempInBounds == false && cursorIsOverButton == true)
        {
            currentButton.AddCursor();
        }
        //If the cursor stops being over a button, tell the button
        else if (tempInBounds == true && cursorIsOverButton == false)
        {
            currentButton.RemoveCursor();
        }

        //If the cursor is over a button and the player pressed Select, tell the button
        if (cursorIsOverButton && inputHandler.GetSelectInput())
        {
            currentButton.OnClick(this);
        }
    }//end HandleButtons

    #endregion
}
