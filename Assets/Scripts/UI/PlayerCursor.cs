using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCursor : MonoBehaviour
{
    #region Variables

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI playerNumberText;
    bool cursorInPlayButtonBounds = false;
    private MainMenu menu;

    [SerializeField] private float speed;

    private PlayerInputHandler inputHandler;
    private Vector2 moveInput;

    #endregion //end Variables

    #region Unity Control Methods

    // Awake is called before Start before the first frame update
    void Awake()
    {
        menu = FindObjectOfType<MainMenu>();
    }//end Awake

    // Start is called before the first frame update
    void Start()
    {
        
    }//end Start

    // Update is called once per frame
    void Update()
    {
        //
        if(inputHandler == null)
        {
            Debug.Log("Hi");
            return;
        }
        if(inputHandler.GetLeaveInput())
        {

            Destroy(inputHandler.gameObject);
            return;
        }

        //
        moveInput = inputHandler.GetMoveInput();

        //
        Vector3 moveDir = new Vector3(moveInput.x, moveInput.y, 0f).normalized;
        transform.position += moveDir * speed * Time.deltaTime;

        bool tempInBounds = cursorInPlayButtonBounds;
        cursorInPlayButtonBounds =
                transform.position.x >= menu.playButton.position.x - menu.playButtonBounds.x &&
                transform.position.x <= menu.playButton.position.x + menu.playButtonBounds.x &&
                transform.position.y >= menu.playButton.position.y - menu.playButtonBounds.y &&
                transform.position.y <= menu.playButton.position.y + menu.playButtonBounds.y;

        if (tempInBounds == false && cursorInPlayButtonBounds == true)
        {
            menu.cursorsOnPlayButton++;
        }
        else if (tempInBounds == true && cursorInPlayButtonBounds == false)
        {
            menu.cursorsOnPlayButton--;
        }

        if(cursorInPlayButtonBounds && inputHandler.GetSelectInput())
        {
            menu.StartGame();
        }
    }//end Update

    //
    private void OnDestroy()
    {
        if(cursorInPlayButtonBounds)
        {
            menu.cursorsOnPlayButton--;
        }
    }

    #endregion //end Unity Control Methods

    #region

    //
    public void Setup(Player player)
    {
        inputHandler = player.inputHandler;
        image.color = player.playerColor;
        playerNumberText.text = "P" + player.playerNumber;
    }

    #endregion
}
