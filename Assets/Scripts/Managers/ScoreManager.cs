using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class will manage the player's score and resolving matches.
 * 
 * This is a Singleton, only one exists in a scene and can be accessed through Instance
 * 
 * This is attached to a Text UI element 
 */



[RequireComponent(typeof(Text))] 
public class ScoreManager : Singleton<ScoreManager>
{
    private MatchablePool pool;
    private MatchableGrid grid;

    [SerializeField]
    private Transform collectionPoint;
    
    // UI element for displaying the score
    private Text scoreText;

    // actual score, with getter 
    private int score;

    public int Score
    {
        get 
        {
            return score;
        }
    
    }

    // get references during Awake
    protected override void Init()
    {
        scoreText = GetComponent<Text>();
    }

    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
        grid = (MatchableGrid) MatchableGrid.Instance;
    }

    // add an amount to the score and update the UI Text
    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score : " + score;
    }

    // coroutine for resolving a match
    public IEnumerator ResolveMatch(Match toResolve, bool isResultOfPowerup = false)
    {
        Matchable powerup = null;
        Matchable matchable;

        Transform target = collectionPoint;

        // if larger match is made, create a powerup
        if (!isResultOfPowerup && toResolve.Count > 3)
        {
            powerup = pool.UpgradeMatchable(toResolve.ToBeUpgraded, toResolve.Type);

            toResolve.RemoveMatchable(powerup);

            target = powerup.transform;

            powerup.SortingOrder = 3;
        }


            // iterate through every matchable in a match
            for (int i = 0; i != toResolve.Count; ++i)
            {
                matchable = toResolve.Matchables[i];

                // remove the matchables from the grid 
                grid.RemoveItemAt(matchable.position);

                // move them off to the side of the screen 
                // and wait for the last one to finish
                if (i == toResolve.Count - 1)
                    yield return StartCoroutine(matchable.Resolve(target));

                else
                    StartCoroutine(matchable.Resolve(target));

            }

        // update the player's score
        // This algorithm Will allow bigger matches to be worth more points
        AddScore(toResolve.Count * toResolve.Count);

        // if there was a powerup, reset the sorting order
        if (powerup != null)
        {
            powerup.SortingOrder = 1;
        }

        yield return null;    
    }
}
