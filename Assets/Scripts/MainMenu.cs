using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    #region Variables

    public Transform playerUIParent;
    public Transform playerCursorParent;
    [SerializeField] private CBColorPicker[] colorPickers;

    #endregion //end Variables

    #region Unity Control Methods

    // Awake is called before Start before the first frame update
    void Awake()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if(gameManager == null)
        {
            SceneManager.LoadSceneAsync("PersistentScene", LoadSceneMode.Additive);
        }
        else
        {
            gameManager.ResetForMenu();
        }
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

    #region

    public void StartGame()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.LoadScene("SampleScene", "MainMenu");
    }

    //
    public CBColorPicker GetFirstAvailableColorPicker()
    {
        //Create a variable to hold the first color picker found that has no current player
        CBColorPicker availableColorPicker = null;

        //Loop through the array of color pickers
        for (int i = 0; i < colorPickers.Length; i++)
        {
            //If the color picker has no current player, it is avaiable
            if(colorPickers[i].HasCurrentPlayer() == false)
            {
                //Store the available color picker and stop the loop
                availableColorPicker = colorPickers[i];
                break;
            }
        }

        //Return the stored available color picker
        return availableColorPicker;
    }

    #endregion
}
