using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCursor : MonoBehaviour
{
    #region Variables

    [SerializeField] private Image image;                       //Used to set the cursor's color
    [SerializeField] private TextMeshProUGUI playerNumberText;  //The cursor's display text
    bool cursorInPlayButtonBounds = false;                      //Stores whether or not the cursor is over the play button
    private MainMenu menu;                                      //Stores a reference to the main menu

    [SerializeField] private float speed;   //The speed at which the cursor moves

    private PlayerInputHandler inputHandler;    //The input handler that controls the cursor
    private Vector2 moveInput;                  //The movement input from the player

    #endregion //end Variables

    #region Unity Control Methods

    // Awake is called before Start before the first frame update
    void Awake()
    {
        //Get the MainMenu
        menu = FindObjectOfType<MainMenu>();
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

        //Get move input from the player
        moveInput = inputHandler.GetMoveInput();

        //Determine where to move the cursor to according to player input and move it accordingly
        Vector3 moveDir = new Vector3(moveInput.x, moveInput.y, 0f).normalized;
        transform.position += moveDir * speed * Time.deltaTime;

        //Determine whether or not the cursor is over the play button
        bool tempInBounds = cursorInPlayButtonBounds;
        cursorInPlayButtonBounds =
                transform.position.x >= menu.playButton.position.x - menu.playButtonBounds.x &&
                transform.position.x <= menu.playButton.position.x + menu.playButtonBounds.x &&
                transform.position.y >= menu.playButton.position.y - menu.playButtonBounds.y &&
                transform.position.y <= menu.playButton.position.y + menu.playButtonBounds.y;

        //If the cursor starts being over the play button, tell the main menu
        if (tempInBounds == false && cursorInPlayButtonBounds == true)
        {
            menu.cursorsOnPlayButton++;
        }
        //If the cursor stops being over the play button, tell the main menu
        else if (tempInBounds == true && cursorInPlayButtonBounds == false)
        {
            menu.cursorsOnPlayButton--;
        }

        //If the cursor is over the play button and the player pressed Select, start the game
        if(cursorInPlayButtonBounds && inputHandler.GetSelectInput())
        {
            menu.StartGame();
        }
    }//end Update

    /// <summary>
    /// When the cursor is destroyed, make sure it doesn't make the play button count incorrect
    /// </summary>
    private void OnDestroy()
    {
        //If the cursor is over the play button when it gets destroyed, tell the main menu
        if(cursorInPlayButtonBounds)
        {
            menu.cursorsOnPlayButton--;
        }
    }//end OnDestroy

    #endregion //end Unity Control Methods

    #region

    /// <summary>
    /// Setup the cursor so it references the correct player
    /// </summary>
    /// <param name="player">The player that controls this cursor</param>
    public void Setup(Player player)
    {
        inputHandler = player.inputHandler;                 //Set the input handler to the player's input handler
        image.color = player.playerColor;                   //Set the cursor's color to the player's color
        playerNumberText.text = "P" + player.playerNumber;  //Set the cursor's text (Ex. P1)
    }//end Setup

    #endregion
}
