using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * This class will setup the scene and initialize objects
 * 
 * This class inherits from Singleton so any other script can access it easily through GameManager.Instance
 */

public class GameManager : Singleton<GameManager>
{
    private MatchablePool pool;
    private MatchableGrid grid;
    private Cursor cursor;
    private AudioMixer audioMixer;
    private ScoreManager score;

    [SerializeField]
    private Fader loadingScreen,
                    darkener;

    [SerializeField]
    private Text finalScoreText;

    [SerializeField]
    private Movable resultsPage;

    [SerializeField]
    private bool levelisTimed;

    [SerializeField]
    private LevelTimer timer;

    [SerializeField]
    private float timeLimit;

    // the dimensions of the matchable grid, set in the inspector
    [SerializeField] private Vector2Int dimensions = Vector2Int.one;

    // a UI Text Object for displaying the contents of the grid data
    // for testing and debugging purposes only 
    [SerializeField] private Text gridOutput;
    [SerializeField] private bool debugMode;

    
    private void Start()
    {
        // get references to other important game objects
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;
        cursor = Cursor.Instance;
        audioMixer = AudioMixer.Instance;
        score = ScoreManager.Instance;

        //Setup the scene
        StartCoroutine(Setup());
    }

    // Remember to comment this out before building!
    //private void Update()
    //{
        //if (debugMode && Input.GetButtonDown("Jump"))
            //NoMoreMoves();
    //}

    private IEnumerator Setup()
    {
        // disable user input
        cursor.enabled = false;

        //unhide loading screen
        loadingScreen.Hide(false);

        // if level is timed, set the timer 
        if(levelisTimed)
            timer.SetTimer(timeLimit);

        
        // pool the matchables 
        // size of the pool is based on a worst case scenario of all the matchables matching at the same time
        // (so double the amount we need)
        pool.PoolObjects(dimensions.x * dimensions.y * 2);

        //create a grid 
        grid.InitializeGrid(dimensions);

        // fade out loading screen
        StartCoroutine(loadingScreen.Fade(0));

        // start background music
        audioMixer.PlayMusic();

        
        // populate the grid
        yield return StartCoroutine(grid.PopulateGrid(false, true));

        
        // check for grid lock and offer the player a hint if they need it.
        grid.CheckPossibleMoves();

        // enable user input
        cursor.enabled = true;

        if (levelisTimed)
            StartCoroutine(timer.Countdown()); 
    }

    public void NoMoreMoves()
    {
        // if the level is timed, reward the player for running out of moves
        if (levelisTimed)
            grid.MatchEverything();
        
        // In survival mode, punish the player for running out of moves
        else
            GameOver();
        
        // game over?
        //print("NO MORE MOVES!\nGAME OVER!");

        // reward the player?
        //grid.MatchEverything();
    }

    public void GameOver()
    { 
        // get and update the final score for the results page
        finalScoreText.text = score.Score.ToString();

        // disable the cursor
        cursor.enabled = false;

        // unhide the darkener and fade in
        darkener.Hide(false);
        StartCoroutine(darkener.Fade(0.75f));

        // move the results page onto the centre of the screen by dividing the screen width and height by 2 
        StartCoroutine(resultsPage.MoveToPosition(new Vector2(Screen.width /2, Screen.height / 2)));
    
    }

    private IEnumerator Quit()
    {
        yield return StartCoroutine(loadingScreen.Fade(1));
        SceneManager.LoadScene("Main Menu");
    }
    public void QuitButtonPressed()
    {
        StartCoroutine(Quit());
    }

    private IEnumerator Retry()
    {
        // Fade out the darkener, and moe the results page off screen
        StartCoroutine(resultsPage.MoveToPosition(new Vector2(Screen.width / 2, Screen.height / 2) + Vector2.down * 1000));
        yield return StartCoroutine(darkener.Fade(0));
        darkener.Hide(true);


        // Reset the cursor, game grid and score
        // if level is timed, set the timer 
        if (levelisTimed)
            timer.SetTimer(timeLimit);
        cursor.Reset();
        score.Reset();

        yield return StartCoroutine(grid.Reset());

        // let the player start playing again
        cursor.enabled = true;

        // if level is timed, start timer
        if (levelisTimed)
            StartCoroutine(timer.Countdown());
    }
    public void RetryButtonPressed()
    {
        StartCoroutine(Retry());
    }
}
