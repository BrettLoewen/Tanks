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
    public RectTransform playButton;
    public Vector2 playButtonBounds = new Vector2(400f, 75f);
    public int cursorsOnPlayButton;

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
        if(cursorsOnPlayButton > 0)
        {
            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }//end Update

    #endregion //end Unity Control Methods

    #region

    public void StartGame()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.LoadScene("SampleScene", "MainMenu");
    }

    #endregion
}
