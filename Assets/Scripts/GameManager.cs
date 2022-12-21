using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables

    public static GameManager Instance;

    [SerializeField] private PlayerManager playerManager;

    private LevelManager levelManager;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private int maxRoundsPerLevel = 3;
    private int currentRoundsOnLevel;
    bool levelSetupComplete;

    [SerializeField] private int numRoundsToWin = 10;
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float endDelay = 3f;
    private CameraControl cameraControl;
    [SerializeField] private GameObject tankPrefab;

    [SerializeField] private CanvasGroup background;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Transform pointDisplayParent;
    [SerializeField] private PlayerPointDisplay pointDisplayPrefab;
    [SerializeField] private AnimationCurve pointIncreaseCurve;
    [SerializeField] private GameObject pauseMenuText;
    
    public static bool IsPaused { get; private set; }
    private Player pausedPlayer;

    [SerializeField] private GameObject deviceLostPopup;
    [SerializeField] private TextMeshProUGUI deviceLostText;
    public static bool InGame { get; private set; }

    private int roundNumber;
    private WaitForSeconds startWait;
    private WaitForSeconds endWait;
    private Player roundWinner;
    private Player gameWinner;
    private bool waitingForRoundEnd;

    #endregion //end Variables

    #region Unity Control Methods

    // Awake is called before Start before the first frame update
    void Awake()
    {
        //Setup a singleton reference for the GameManager
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("There are more than 1 GameManagers in the scene!");
        }
    }//end Awake

    // Start is called before the first frame update
    void Start()
    {
        ClearPlayerPointDisplays();
        pauseMenuText.SetActive(false);
        waitingForRoundEnd = false;

        deviceLostPopup.SetActive(false);
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
        LevelManager tempLevelManager = levelManager;
        
        //
        levelManager = FindObjectOfType<LevelManager>();

        //If a new level was loaded, do the setup that needs to happen every level
        if(tempLevelManager != levelManager)
        {
            //
            cameraControl = levelManager.cameraControl;

            //
            currentRoundsOnLevel = 0;
        }

        //True if the game just started, false if the game has already been setup before
        bool gameJustStarted = !InGame;

        //If the game just started, do the setup that needs to happen every game
        if(gameJustStarted)
        {
            //
            startWait = new WaitForSeconds(startDelay);
            endWait = new WaitForSeconds(endDelay);

            //
            SpawnAllTanks();

            //
            InGame = true;
        }
        else
        {
            //
            int i = 0;
            foreach (Player player in playerManager.players)
            {
                //
                player.spawnPoint = levelManager.spawnPoints[i];

                //
                cameraControl.targets.Add(player.instance.transform);

                i++;
            }
        }

        //
        levelSetupComplete = true;

        //If the game just started, initiate the game loop
        if (gameJustStarted)
        {
            StartCoroutine(GameLoop());
        }
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

        currentRoundsOnLevel++;

        if(gameWinner != null)
        {
            ClearPlayerPointDisplays();

            winnerText.text = "";

            InGame = false;

            yield return StartCoroutine(levelLoader.LoadMenu());
        }
        else
        {
            if(currentRoundsOnLevel >= maxRoundsPerLevel)
            {
                levelSetupComplete = false;
                yield return StartCoroutine(levelLoader.LoadNextLevel());
            }

            StartCoroutine(GameLoop());
        }
    }//end GameLoop

    //
    private IEnumerator RoundStarting()
    {
        //
        while(levelSetupComplete == false)
        {
            yield return null;
        }

        ResetAllTanks();
        DisableTankControl();

        cameraControl.SetStartPositionAndSize();

        roundNumber++;
        winnerText.text = "ROUND " + roundNumber;

        ClearPlayerPointDisplays();

        yield return StartCoroutine(levelLoader.EndTransition());

        yield return startWait;
    }//end RoundStarting

    //
    private IEnumerator RoundPlaying()
    {
        //
        EnableTankControl();

        winnerText.text = string.Empty;

        while (!OnePlayerLeft())
        {
            yield return null;
        }
    }//end RoundPlaying

    //
    private IEnumerator RoundEnding()
    {
        //
        waitingForRoundEnd = true;

        //
        yield return new WaitForSeconds(1f);

        //
        DisableTankControl();

        //
        roundWinner = null;
        roundWinner = GetRoundWinner();

        if (roundWinner != null)
        {
            roundWinner.wins++;
        }

        gameWinner = GetGameWinner();

        //
        string message = EndMessage();
        winnerText.text = message;

        //
        PlayerPointDisplay winnerDisplay = SetupPlayerPointDisplays();

        //
        if (winnerDisplay != null)
        {
            yield return StartCoroutine(winnerDisplay.IncreasePoints(pointIncreaseCurve));
        }

        yield return endWait;

        yield return StartCoroutine(levelLoader.StartTransition());

        waitingForRoundEnd = false;
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

        /*message += "\n\n\n\n";

        foreach (Player player in playerManager.players)
        {
            message += player.coloredPlayerText + ": " + player.wins + " WINS\n";
        }*/

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
            Destroy(player.instance);
            player.ResetForMenu();
            playerManager.SetupPlayerForMenu(player);
        }

        roundNumber = 0;

        StartCoroutine(levelLoader.EndTransition());
    }//end ResetForMenu

    #endregion //end GameLoop

    #region Pausing / Device Management

    //
    public void Pause(Player player)
    {
        //Don't pause while waiting for the round to end
        if(waitingForRoundEnd)
        {
            return;
        }

        //
        if(pausedPlayer == null)
        {
            pausedPlayer = player;
            SetupPlayerPointDisplays();
            background.alpha = 1f;
            pauseMenuText.SetActive(true);
            winnerText.text = player.coloredPlayerText + " PAUSED";

            IsPaused = true;

            Time.timeScale = 0f;
        }
        //
        else if(pausedPlayer == player)
        {
            pausedPlayer = null;
            ClearPlayerPointDisplays();
            background.alpha = 0f;
            pauseMenuText.SetActive(false);
            winnerText.text = "";

            IsPaused = false;

            Time.timeScale = 1f;
        }
    }//end Pause

    //
    public void LostDevice(Player player)
    {
        //If we are on the main menu, ignore the loss
        if(InGame == false)
        {
            return;
        }

        //If the game is not paused, pause it
        if(IsPaused == false)
        {
            Pause(player);
        }
        //If the game is already paused
        else
        {
            //If the game is paused by a player other than the one that lost its device
            if(pausedPlayer != player)
            {
                //Unpause with the paused player and pause again with the player who lost its device
                Pause(pausedPlayer);
                Pause(player);
            }
            //If the player who lost its device already paused the game, leave it paused
        }

        //
        deviceLostPopup.SetActive(true);
        deviceLostText.text = player.coloredPlayerText + " Disconnected";
    }

    //
    public void RegainedDevice()
    {
        deviceLostPopup.SetActive(false);
    }

    #endregion //end Pausing / Device Management

    #region UI

    //
    private PlayerPointDisplay SetupPlayerPointDisplays()
    {
        //
        PlayerPointDisplay winnerDisplay = null;

        //
        foreach (Player player in playerManager.players)
        {
            PlayerPointDisplay display = Instantiate(pointDisplayPrefab, pointDisplayParent);
            display.Setup(player, numRoundsToWin);

            if (player.Equals(roundWinner))
            {
                winnerDisplay = display;
            }
        }

        background.alpha = 1f;

        //
        return winnerDisplay;
    }//end SetupPlayerPointDisplays

    //
    private void ClearPlayerPointDisplays()
    {
        //
        foreach (Transform t in pointDisplayParent)
        {
            Destroy(t.gameObject);
        }

        background.alpha = 0f;
    }//end ClearPlayerPointDisplays

    #endregion //end UI
}
