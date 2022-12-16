using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private PlayerManager playerManager;
    private LevelManager levelManager;

    [SerializeField] private int numRoundsToWin = 10;
    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float endDelay = 3f;
    private CameraControl cameraControl;
    private TextMeshProUGUI messageText;
    [SerializeField] private GameObject tankPrefab;

    private int roundNumber;
    private WaitForSeconds startWait;
    private WaitForSeconds endWait;
    private Player roundWinner;
    private Player gameWinner;

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

    #region GameLoop

    //
    public void StartGame()
    {
        //
        levelManager = FindObjectOfType<LevelManager>();
        cameraControl = levelManager.cameraControl;
        messageText = levelManager.messageText;

        //
        startWait = new WaitForSeconds(startDelay);
        endWait = new WaitForSeconds(endDelay);

        //
        SpawnAllTanks();

        //
        StartCoroutine(GameLoop());
    }

    //
    private void SpawnAllTanks()
    {
        //
        int i = 1;
        foreach (Player player in playerManager.players)
        {
            //
            player.spawnPoint = levelManager.spawnPoints[i - 1];

            //
            player.instance = Instantiate(tankPrefab, player.spawnPoint.position, player.spawnPoint.rotation);

            //
            player.instance.GetComponentInChildren<UIDirectionControl>().SetCameraRig(cameraControl.transform);
            
            //
            player.playerNumber = i;
            player.Setup();
            
            //
            i++;

            //
            cameraControl.targets.Add(player.instance.transform);
        }
    }//end SpawnAllTanks

    //
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if(gameWinner != null)
        {
            LoadScene("MainMenu", "SampleScene");
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }//end GameLoop

    //
    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        cameraControl.SetStartPositionAndSize();

        roundNumber++;
        messageText.text = "ROUND " + roundNumber;

        yield return startWait;
    }//end RoundStarting

    //
    private IEnumerator RoundPlaying()
    {
        //
        EnableTankControl();

        messageText.text = string.Empty;

        while (!OnePlayerLeft())
        {
            yield return null;
        }
    }//end RoundPlaying

    //
    private IEnumerator RoundEnding()
    {
        //
        DisableTankControl();

        //
        roundWinner = null;
        roundWinner = GetRoundWinner();

        if(roundWinner != null)
        {
            roundWinner.wins++;
        }

        gameWinner = GetGameWinner();

        //
        string message = EndMessage();
        messageText.text = message;

        yield return endWait;
    }//end RoundEnding

    //
    private void ResetAllTanks()
    {
        foreach (Player player in playerManager.players)
        {
            player.Reset();
        }
    }//end ResetAllTanks

    //
    private void DisableTankControl()
    {
        foreach (Player player in playerManager.players)
        {
            player.DisableControl();
        }
    }//end DisableTankControl

    //
    private void EnableTankControl()
    {
        foreach (Player player in playerManager.players)
        {
            player.EnableControl();
        }
    }//end EnableTankControl

    //
    private bool OnePlayerLeft()
    {
        int numPlayersLeft = 0;

        foreach (Player player in playerManager.players)
        {
            if (player.instance.activeSelf)
            {
                numPlayersLeft++;
            }
        }

        return numPlayersLeft <= 1;
    }//end OnePlayerLeft

    //
    private Player GetRoundWinner()
    {
        foreach (Player player in playerManager.players)
        {
            if(player.instance.activeSelf)
            {
                return player;
            }
        }

        return null;
    }//end GetRoundWinner

    //
    private Player GetGameWinner()
    {
        foreach (Player player in playerManager.players)
        {
            if (player.wins >= numRoundsToWin)
            {
                return player;
            }
        }

        return null;
    }//end GetGameWinner

    //
    private string EndMessage()
    {
        string message = "DRAW!";

        if(roundWinner != null)
        {
            message = roundWinner.coloredPlayerText + " WINS THE ROUND!";
        }

        message += "\n\n\n\n";

        foreach (Player player in playerManager.players)
        {
            message += player.coloredPlayerText + ": " + player.wins + " WINS\n";
        }

        if(gameWinner != null)
        {
            message = gameWinner.coloredPlayerText + " WINS THE GAME!";
        }

        return message;
    }//end EndMessage

    //
    public void ResetForMenu()
    {
        foreach (Player player in playerManager.players)
        {
            player.ResetForMenu();
            playerManager.SetupPlayerForMenu(player);
        }
    }//end ResetForMenu

    #endregion //end GameLoop

    #region Scene Management

    public void LoadScene(string sceneName, string sceneToUnload)
    {
        UnloadScene(sceneToUnload);

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    #endregion //end Scene Management
}
