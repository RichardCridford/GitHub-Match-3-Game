using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))] 
public class ScoreManager : Singleton<ScoreManager>
{
    private MatchableGrid grid;
    
    private Text scoreText;
    private int score;

    public int Score
    {
        get 
        {
            return score;
        }
    
    }

    protected override void Init()
    {
        scoreText = GetComponent<Text>();
    }

    private void Start()
    {
        grid = (MatchableGrid)MatchableGrid.Instance;
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score : " + score;
    }

    public IEnumerator ResolveMatch(Match toResolve)
    {
        foreach (Matchable matchable in toResolve.Matchables)
        {
            // remove the matchables from the grid 
            grid.RemoveItemAt(matchable.position);

            // move them off to the side of the screen 

        }

        // update the player's score
        // Will allow bigger matches to be worth more points
        AddScore(toResolve.Count * toResolve.Count);

        yield return null;    
    }
}
