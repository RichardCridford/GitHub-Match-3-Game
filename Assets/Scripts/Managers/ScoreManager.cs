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
    public IEnumerator ResolveMatch(Match toResolve, MatchType powerupUsed = MatchType.invalid)
    {
        Matchable powerupFormed = null;
        Matchable matchable;

        Transform target = collectionPoint;

        // if no powerup was used to trigger this match and a larger match is made, create a powerup
        if (powerupUsed == MatchType.invalid && toResolve.Count > 3)
        {
            powerupFormed = pool.UpgradeMatchable(toResolve.ToBeUpgraded, toResolve.Type);

            toResolve.RemoveMatchable(powerupFormed);

            target = powerupFormed.transform;

            powerupFormed.SortingOrder = 3;
        }


            // iterate through every matchable in a match
            for (int i = 0; i != toResolve.Count; ++i)
            {
                matchable = toResolve.Matchables[i];

                // only allow games used as powerups to resolve gems
                // it can stay on the grid until the player decides to use it
                if (powerupUsed != MatchType.match5 && matchable.isGem)
                    continue;
            


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
        if (powerupFormed != null)
        {
            powerupFormed.SortingOrder = 1;
        }

        yield return null;    
    }
}
