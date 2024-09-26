using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Build;
using UnityEngine;

/*
 * This class will manage the grid of matchables by inheriting grid mechanics from Grid System
 * It's also a Singleton which can be accessed through Instance
 */

public class MatchableGrid : GridSystem<Matchable>
{
    // The pool of Matchables with which to populate the grid
    private MatchablePool pool;

    private ScoreManager score;

    // A distance offscreen where the matchables will spawn
    [SerializeField] private Vector3 offscreenOffset;

    // Get a reference to the pool on start
    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;

        score = ScoreManager.Instance;
    }

    
    // Populate the grid with matchables from the pool
    // Optionally allow or not allow matches when populating
    public IEnumerator PopulateGrid(bool allowMatches = false)
    {
        Matchable newMatchable;

    // Work through each grid position
        for(int y = 0; y != Dimensions.y; ++y)
            for (int x = 0; x != Dimensions.x; ++x)
            {
                // get a matchable from the pool
                newMatchable = pool.GetRandomMatchable();
                Vector3 onscreenPosition;

                // calculate the future on screen position of the matchable
                onscreenPosition = transform.position + new Vector3(x, y);

                // position the matchable off screen
                newMatchable.transform.position = onscreenPosition + offscreenOffset;

                 // activate the matchable
                newMatchable.gameObject.SetActive(true);

                // tell the matchable where it is on the grid
                newMatchable.position = new Vector2Int(x, y);
             

                // place the matchable in the grid
                PutItemAt(newMatchable, x, y);

                
                // what was the initial type of the new matchable?
                int initialType = newMatchable.Type;

                while (!allowMatches && IsPartOfAMatch(newMatchable))
                {

                    // change the matchable's type until it isn't a match anymore
                    if (pool.NextType(newMatchable) == initialType)
                    {
                        Debug.LogWarning("Failed to find a matchable that didn't match at (" + x + ", " + y + ")");
                        Debug.Break();
                        break;

                    }
                }

                // move the matchable to its onscreen position
                StartCoroutine(newMatchable.MoveToPosition(onscreenPosition));
                
                yield return new WaitForSeconds(0.1f);

            }
    }

    
    // Check that the matchable being populated into the grid is part of a match or not
    private bool IsPartOfAMatch(Matchable toMatch)
    {
        int horizontalMatches   = 0,
            verticalMatches     = 0;

        // first look to the left
        horizontalMatches += CountMatchesInDirection(toMatch, Vector2Int.left);

        // then look to the right
        horizontalMatches += CountMatchesInDirection(toMatch, Vector2Int.right);

        if (horizontalMatches > 1)
            return true;

        // look up
        verticalMatches += CountMatchesInDirection(toMatch, Vector2Int.up);

        // look down
        verticalMatches += CountMatchesInDirection(toMatch, Vector2Int.down);

        if (verticalMatches > 1)
            return true;


        return false;
    }

    // Count the number of matches on the grid starting from the matchable to match in the direction indicated
    private int CountMatchesInDirection(Matchable toMatch, Vector2Int direction)
    {
        int matches = 0;

        Vector2Int position = toMatch.position + direction;

        while (CheckBounds(position) && !IsEmpty(position) && GetItemAt(position).Type == toMatch.Type)
        {
            ++matches;
            position += direction;
        }
        return matches;       
    }



    public IEnumerator TrySwap(Matchable[] toBeSwapped)       
    {
        // Make a local copy of what we're swapping so cursor doesn't overwrite
        Matchable[] copies = new Matchable[2];
        copies[0] = toBeSwapped[0];
        copies[1] = toBeSwapped[1];

        // yield until matchables animate swapping
        yield return StartCoroutine(Swap(copies));

        // check for a valid match
        Match[] matches = new Match[2];

        matches[0] = GetMatch(copies[0]);
        matches[1] = GetMatch(copies[1]);


        // if we made valid matches, resolve them
        if (matches[0] != null)
            StartCoroutine(score.ResolveMatch(matches[0]));
        

        if (matches[1] != null)
            StartCoroutine(score.ResolveMatch(matches[1]));



        // if no match, swap them back
        if (matches[0] == null && matches[1] == null)
            StartCoroutine(Swap(copies));
        
    }

    private Match GetMatch(Matchable toMatch)
    {
        Match match = new Match(toMatch);

        Match   horizontalMatch,
                verticalMatch;

        // first get the horizontal matches to the left and right
        horizontalMatch = GetMatchesInDirection(toMatch, Vector2Int.left);
        horizontalMatch.Merge(GetMatchesInDirection(toMatch, Vector2Int.right));

        if (horizontalMatch.Count > 1)
            match.Merge(horizontalMatch);


        // then get vertical matches up and down
        verticalMatch = GetMatchesInDirection(toMatch, Vector2Int.up);
        verticalMatch.Merge(GetMatchesInDirection(toMatch, Vector2Int.down));


        if (verticalMatch.Count > 1)
           match.Merge(verticalMatch);
        


        if (match.Count == 1)
            return null;

        return match;
    }

    // Add each matching matchable in the direction to a match and return it
    private Match GetMatchesInDirection(Matchable toMatch, Vector2Int direction)
    {
        Match match = new Match();
        Vector2Int position = toMatch.position + direction;
        Matchable next;

        while (CheckBounds(position) && !IsEmpty(position))
        {
            next = GetItemAt(position);

            if (next.Type == toMatch.Type && next.Idle)
            {
                match.AddMatchable(next);
                position += direction;
            }
            else
                break;
            
        }
        return match;
    }


    private IEnumerator Swap(Matchable[] toBeSwapped)
    {
        // swap them in the grid data structure
        SwapItemsAt(toBeSwapped[0].position, toBeSwapped[1].position);

        // tell the matchables their new positions

        Vector2Int temp = toBeSwapped[0].position;
        toBeSwapped[0].position = toBeSwapped[1].position;
        toBeSwapped[1].position = temp;


        // get the world positions of both
        Vector3[] worldPosition = new Vector3[2];
        worldPosition[0] = toBeSwapped[0].transform.position;
        worldPosition[1] = toBeSwapped[1].transform.position;

        // move them to new position on screen
                    StartCoroutine(toBeSwapped[0].MoveToPosition(worldPosition[1]));
       yield return StartCoroutine(toBeSwapped[1].MoveToPosition(worldPosition[0]));


    }
}
