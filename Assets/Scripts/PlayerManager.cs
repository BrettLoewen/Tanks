using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    #region Variables

    public List<Player> players = new List<Player>();

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

        switch(players.Count)
        {
            case 0:
                player.playerColor = Color.blue;
                break;
            case 1:
                player.playerColor = Color.red;
                break;
            default:
                player.playerColor = Color.black;
                break;
        }

        players.Add(player);
    }

    #endregion
}

[System.Serializable]
public class Player
{
    public PlayerInputHandler inputHandler; //

    public Color playerColor;                           //
    public Transform spawnPoint;                        //
    [HideInInspector] public int playerNumber;          //
    [HideInInspector] public string coloredPlayerText;  //
    [HideInInspector] public GameObject instance;       //
    [HideInInspector] public int wins;                  //

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

        //
        coloredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">PLAYER" + playerNumber + "</color>";

        //
        MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();

        //
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = playerColor;
        }
    }//end Setup

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
        instance.SetActive(false);
        instance.SetActive(true);
    }//end Reset
}//end Player