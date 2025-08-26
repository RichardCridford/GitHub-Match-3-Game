using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        
        //put in a loading screne here   
        
        // pool the matchables 
        // size of the pool is based on a worst case scenario of all the matchables matching at the same time
        // (so double the amount we need)
        pool.PoolObjects(dimensions.x * dimensions.y * 2);

        //create a grid 
        grid.InitializeGrid(dimensions);

        // fade out loading screen

        // start background music
        audioMixer.PlayMusic();

        // enable user input
        cursor.enabled = true;

        yield return null;

        StartCoroutine(grid.PopulateGrid(false, true));

        

        // check for grid lock and offer the player a hint if they need it.
        grid.CheckPossibleMoves();
       }

    public void NoMoreMoves()
    {
        // game over?
        //print("NO MORE MOVES!\nGAME OVER!");

        // reward the player?
        grid.MatchEverything();
    }
}
