using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CursorButton: MonoBehaviour
{
    #region Variables

    static List<CursorButton> AllCursorButtons = new List<CursorButton>();  //Stores a reference to every CursorButton
    static private float cursorSizeModifier = 4f;                           //Divide the size of the cursor by this to get its bounds
    static private float buttonSizeModifier = 2f;                           //Divide the size of the button by this to get its bounds

    private int cursorsOnButtonCount;   //The number of cursors currently hovering over the button

    private Image image;                                //Store the button's image component
    [SerializeField] protected Color normalColor;       //The color of the image when a cursor is not over the button
    [SerializeField] private Color highlightedColor;    //The color of the image when a cursor is over the button

    [SerializeField] private UnityEvent onClickEvent;

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

    //OnEnable is called whenever the gameobject is enabled
    protected void OnEnable()
    {
        //Add this button to the static list containing all buttons
        AllCursorButtons.Add(this);

        //Setup some values
        cursorsOnButtonCount = 0;
        image = GetComponent<Image>();
        image.color = normalColor;
    }//end OnEnable

    //OnDisable is caleld whenever the gameobject is disabled
    protected void OnDisable()
    {
        //Remove this button from the static list containing all buttons
        AllCursorButtons.Remove(this);
    }//end OnDisable

    // Update is called once per frame
    protected virtual void Update()
    {
        //If there is at least 1 cursor hovering over this button, highlight the button 
        if(cursorsOnButtonCount > 0)
        {
            image.color = highlightedColor;
        }
        //If there are no cursors hovering over this button, do not highlight the button
        else
        {
            image.color = normalColor;
            cursorsOnButtonCount = 0;
        }
    }//end Update

    #endregion //end Unity Control Methods

    #region

    /// <summary>
    /// Check each CursorButton to see if the passed PlayerCursor is within its bounds until one is found or all have been checked.<br></br>
    /// If the PlayerCursor is found to be within the bounds of a CursorButton, return that CursorButton.
    /// </summary>
    /// <param name="cursor">The PlayerCursor to use to check for overlaps</param>
    /// <returns>Returns the CursorButton that the PlayerCursor is hovering over if one is found, and null otherwise</returns>
    public static CursorButton IsOverCursorButton(PlayerCursor cursor)
    {
        //Will store the button the cursor is over if one is found
        CursorButton foundButton = null;

        //Get the position and size of the cursor
        Vector2 cursorSize = cursor.GetComponent<RectTransform>().sizeDelta / cursorSizeModifier;
        Vector2 cursorPos = Camera.main.WorldToScreenPoint(cursor.transform.position);

        //Loop through every CursorButton
        foreach (CursorButton button in AllCursorButtons)
        {
            //Get the position and size of the button
            Vector2 buttonSize = button.GetComponent<RectTransform>().sizeDelta / buttonSizeModifier;
            Vector2 buttonPos = Camera.main.WorldToScreenPoint(button.transform.position);

            //Determine whether or not the cursor is within the button
            bool cursorInBounds = cursorPos.x + cursorSize.x >= buttonPos.x - buttonSize.x &&
               cursorPos.x - cursorSize.x <= buttonPos.x + buttonSize.x &&
               cursorPos.y + cursorSize.y >= buttonPos.y - buttonSize.y &&
               cursorPos.y - cursorSize.y <= buttonPos.y + buttonSize.y;

            //If the cursor is in the bounds of the button, store the reference to that button and stop looping
            if(cursorInBounds)
            {
                foundButton = button;
            }
        }

        //Return the value stored in foundButton
        return foundButton;
    }//end IsOverCursorButton

    /// <summary>
    /// Increase the stored number of cursors that are hovering over this button
    /// </summary>
    public void AddCursor()
    {
        cursorsOnButtonCount++;
    }//end AddCursor

    /// <summary>
    /// Decrease the stored number of cursors that are hovering over this button
    /// </summary>
    public void RemoveCursor()
    {
        cursorsOnButtonCount--;
    }//end AddCursor

    /// <summary>
    /// Provide a virtual method the PlayerCursor can call when it clicks on a CursorButton and child classes can implement
    /// </summary>
    /// <param name="cursor">The PlayerCursor that clicked on this button</param>
    public virtual void OnClick(PlayerCursor cursor)
    {
        if(onClickEvent != null)
        {
            onClickEvent.Invoke();
        }
    }//end OnClick

    #endregion
}