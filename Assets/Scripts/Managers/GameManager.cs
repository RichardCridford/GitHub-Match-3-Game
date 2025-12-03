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

    [SerializeField]
    private Fader loadingScreen;

    // the dimensions of the matchable grid, set in the inspector
    [SerializeField] private Vector2Int dimensions = Vector2Int.one;

    // a UI Text Object for displaying the contents of the grid data
    // for testing and debugging purposes only 
    [SerializeField] private Text gridOutput;

    
    private void Start()
    {
        // get references to other important game objects
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;
        cursor = Cursor.Instance;
        audioMixer = AudioMixer.Instance;

        //Setup the scene
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        // disable user input
        cursor.enabled = false;

        //unhide loading screen
        loadingScreen.Hide(false);
        
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
    }

    public void NoMoreMoves()
    {
        // game over?
        //print("NO MORE MOVES!\nGAME OVER!");

        // reward the player?
        grid.MatchEverything();
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
}
