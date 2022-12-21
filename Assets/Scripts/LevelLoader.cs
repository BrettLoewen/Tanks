using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader: MonoBehaviour
{
    #region Variables

    [SerializeField] private string[] levelNames;   //Contains the name of each level
    private Queue<string> randomlyOrderedLevels = new Queue<string>();

    private string mainMenuString = "MainMenu";
    private string activeScene;

    [SerializeField] private Animator transitionAnimator;
    private string startTransitionString = "startTransition";
    private string endTransitionString = "endTransition";

    private WaitForSeconds transitionDuration = new WaitForSeconds(1f);

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

    #region Scene Transitioning

    //
    public IEnumerator StartTransition()
    {
        transitionAnimator.SetTrigger(startTransitionString);
        yield return transitionDuration;
    }//end StartTransition

    //
    public IEnumerator EndTransition()
    {
        transitionAnimator.SetTrigger(endTransitionString);
        yield return transitionDuration;
    }//end EndTransition

    #endregion //end Scene Transitioning

    #region Scene Loading

    private IEnumerator LoadScene(string sceneToLoad, string sceneToUnload)
    {
        activeScene = sceneToLoad;

        yield return StartCoroutine(UnloadScene(sceneToUnload));

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while(operation.isDone == false)
        {
            yield return null;
        }
    }

    private IEnumerator UnloadScene(string sceneToUnload)
    {
        if (sceneToUnload == null || sceneToUnload.Equals(""))
        {
            sceneToUnload = mainMenuString;
        }

        AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneToUnload);

        while (operation.isDone == false)
        {
            yield return null;
        }
    }

    public IEnumerator LoadMenu()
    {
        yield return StartCoroutine(LoadScene(mainMenuString, activeScene));
    }

    public IEnumerator LoadNextLevel()
    {
        if(randomlyOrderedLevels.Count == 0)
        {
            GenerateRandomLevelQueue();
        }

        string nextLevel = randomlyOrderedLevels.Dequeue();

        yield return StartCoroutine(LoadScene(nextLevel, activeScene));
    }

    #endregion //end Scene Management

    private void GenerateRandomLevelQueue()
    {
        randomlyOrderedLevels.Clear();
        List<string> levels = new List<string>();

        for (int i = 0; i < levelNames.Length; i++)
        {
            levels.Add(levelNames[i]);
        }

        while(levels.Count > 0)
        {
            int randIndex = Random.Range(0, levels.Count);

            string nextLevel = levels[randIndex];
                
            levels.RemoveAt(randIndex);

            randomlyOrderedLevels.Enqueue(nextLevel);
        }
    }
}