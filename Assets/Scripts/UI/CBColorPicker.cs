using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CBColorPicker: CursorButton
{
    #region Variables

    private Player currentPlayer;   //Stores the player whose color is controlled by this color picker

    [SerializeField] private Image playerIndicator;                 //Indicates which player's color, if any, is being controlled by this picker
    [SerializeField] private TextMeshProUGUI playerIndicatorText;   //Indicates which player's color, if any, is being controlled by this picker

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
    protected override void Update()
    {
        base.Update();

        //If the color picker is controlling a player's color
        if(currentPlayer != null)
        {
            //Make the indicator appear
            playerIndicator.gameObject.SetActive(true);

            //Set the color and number of the indicator to match the player
            playerIndicator.color = normalColor;
            playerIndicatorText.text = "P" + currentPlayer.playerNumber;
        }
        //If the color picker is not controlling a player's color
        else
        {
            //Hide the indicator
            playerIndicator.gameObject.SetActive(false);
        }
    }//end Update

    #endregion //end Unity Control Methods

    #region

    /// <summary>
    /// Called when a PlayerCursor clicks on a CursorButton. Will set the color of the player that clicks on it
    /// </summary>
    /// <param name="cursor">The PlayerCursor that clicked on this button</param>
    public override void OnClick(PlayerCursor cursor)
    {
        //
        SetPlayer(cursor.GetPlayer());
    }//end OnClick

    //
    public void SetPlayer(Player player)
    {
        //If the passed player is null, reset the color picker
        if(player == null)
        {
            //Hide the player indicator and delete any reference to a player
            playerIndicator.gameObject.SetActive(false);
            currentPlayer = null;
            return;
        }

        //Can only control a player's color if this picker is not already controlling a different player's color
        if (currentPlayer == null)
        {
            //Get the player from the passed cursor
            currentPlayer = player;

            //If the player's color was being controlled by a different picker, reset the previous color picker
            if (currentPlayer.colorPicker != null)
            {
                currentPlayer.colorPicker.currentPlayer = null;
            }

            //Set the player's color to this picker's color
            currentPlayer.SetColor(normalColor, this);
        }
    }

    //
    public bool HasCurrentPlayer()
    {
        return currentPlayer != null;
    }

    #endregion
}