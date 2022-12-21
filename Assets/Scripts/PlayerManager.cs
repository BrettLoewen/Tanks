using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    #region Variables

    public List<Player> players = new List<Player>();
    //public Color[] playerColors = new Color[6];

    [SerializeField] private PlayerUI playerUIPrefab;
    [SerializeField] private PlayerCursor playerCursorPrefab;

    private List<PlayerUI> playerUIs = new List<PlayerUI>();
    private List<PlayerCursor> playerCursors = new List<PlayerCursor>();

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

    #region

    //
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Player player = new Player();

        player.inputHandler = playerInput.GetComponent<PlayerInputHandler>();
        player.inputHandler.player = player;
        player.playerNumber = players.Count + 1;
        FindObjectOfType<MainMenu>().GetFirstAvailableColorPicker().SetPlayer(player);
        //player.SetupColoredPlayerText();

        players.Add(player);

        SetupPlayerForMenu(player);
    }//end OnPlayerJoined

    //
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        int playerToRemove = -1;

        foreach (Player player in players)
        {
            if(player.inputHandler == null || player.inputHandler.GetComponent<PlayerInput>().Equals(playerInput))
            {
                playerToRemove = player.playerNumber - 1;
            }
        }

        if(playerToRemove == -1)
        {
            Debug.LogError("Couldn't find a player to remove");
            return;
        }

        if(players[playerToRemove].playerUI == null)
        {
            return;
        }

        Destroy(players[playerToRemove].playerUI.gameObject);
        Destroy(players[playerToRemove].cursor.gameObject);
        players[playerToRemove].colorPicker.SetPlayer(null);

        players.RemoveAt(playerToRemove);

        foreach (Player player in players)
        {
            if (player.playerNumber - 1 >= playerToRemove)
            {
                player.playerNumber--;
                player.SetupColoredPlayerText();
                player.playerUI.SetPlayer(player);
                player.cursor.Setup(player);
            }
        }
    }//end OnPlayerLeft

    //
    public void SetupPlayerForMenu(Player player)
    {
        MainMenu menu = FindObjectOfType<MainMenu>();

        PlayerUI playerUI = Instantiate(playerUIPrefab, menu.playerUIParent);
        playerUI.SetPlayer(player);
        player.playerUI = playerUI;

        PlayerCursor cursor = Instantiate(playerCursorPrefab, menu.playerCursorParent);
        cursor.Setup(player);
        player.cursor = cursor;
    }//end SetupPlayerForMenu

    #endregion
}

[System.Serializable]
public class Player
{
    public PlayerInputHandler inputHandler; //

    public Color playerColor;                           //
    public Transform spawnPoint;                        //
    public int playerNumber;                            //
    [HideInInspector] public string coloredPlayerText;  //
    public GameObject instance;       //
    [HideInInspector] public int wins;                  //

    public PlayerUI playerUI;
    public PlayerCursor cursor;
    public CBColorPicker colorPicker;

    private TankMovement tankMovement;      //
    private TankShooting tankShooting;      //
    private GameObject canvasGameObject;    //

    public void Setup()
    {
        //
        tankMovement = instance.GetComponent<TankMovement>();
        tankShooting = instance.GetComponent<TankShooting>();
        canvasGameObject = instance.GetComponentInChildren<Canvas>().gameObject;

        tankMovement.inputHandler = inputHandler;
        tankShooting.inputHandler = inputHandler;

        tankShooting.player = this;
        tankShooting.GetComponent<TankHealth>().SetPlayer(this);

        //
        SetupColoredPlayerText();

        //
        MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();

        //
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = playerColor;
        }
    }//end Setup

    //
    public void SetupColoredPlayerText()
    {
        coloredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">PLAYER" + playerNumber + "</color>";
    }//end SetupColoredPlayerText

    //
    public void SetColor(Color color, CBColorPicker _colorPicker)
    {
        playerColor = color;
        SetupColoredPlayerText();

        colorPicker = _colorPicker;
    }//end SetColor

    //
    public void DisableControl()
    {
        //
        tankMovement.enabled = false;
        tankShooting.enabled = false;

        canvasGameObject.SetActive(false);
    }//end DisableControl

    //
    public void EnableControl()
    {
        //
        tankMovement.enabled = true;
        tankShooting.enabled = true;

        canvasGameObject.SetActive(true);
    }//end EnableControl

    //
    public void Reset()
    {
        //
        instance.transform.position = spawnPoint.position;
        instance.transform.rotation = spawnPoint.rotation;

        //
        tankShooting.Reset();

        //
        instance.SetActive(false);
        instance.SetActive(true);
    }//end Reset

    //
    public void ResetForMenu()
    {
        instance = null;
        tankMovement = null;
        tankShooting = null;
        canvasGameObject = null;

        wins = 0;
    }
}//end Player