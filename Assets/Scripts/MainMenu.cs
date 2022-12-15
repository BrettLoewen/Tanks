using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Variables



    #endregion //end Variables

    #region Unity Control Methods

    // Awake is called before Start before the first frame update
    void Awake()
    {
        SceneManager.LoadSceneAsync("PersistentScene", LoadSceneMode.Additive);
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

    #endregion
}
