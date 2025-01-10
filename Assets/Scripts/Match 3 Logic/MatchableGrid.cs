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
    public IEnumerator PopulateGrid(bool allowMatches = false, bool initialPopulation = false)
    {
        // list of new matchables added during population
        List<Matchable> newMatchables = new List<Matchable>();

        Matchable newMatchable;
        Vector3 onscreenPosition;

        // Work through each grid position
        for (int y = 0; y != Dimensions.y; ++y)
            for (int x = 0; x != Dimensions.x; ++x)
                if(IsEmpty(x, y))
                {
                    // get a matchable from the pool
                    newMatchable = pool.GetRandomMatchable();
                   
                    // position the matchable off screen
                    newMatchable.transform.position = new Vector3 (x,y) + offscreenOffset;

                     // activate the matchable
                    newMatchable.gameObject.SetActive(true);

                    // tell the matchable where it is on the grid
                    newMatchable.position = new Vector2Int(x, y);
             

                    // place the matchable in the grid
                    PutItemAt(newMatchable, x, y);

                    // add the matchable to the list
                    newMatchables.Add(newMatchable);

                
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
                }

       // move each matchable to its onscreen position, yielding until the last has finished
       for(int i = 0; i != newMatchables.Count; ++i)
       {
            // calculate the future on screen position of the matchable
            onscreenPosition = transform.position + new Vector3(newMatchables[i].position.x, newMatchables[i].position.y);

            // move the matchable to its onscreen position
            if (i == newMatchables.Count - 1)
                yield return StartCoroutine(newMatchables[i].MoveToPosition(onscreenPosition));

            else
                StartCoroutine(newMatchables[i].MoveToPosition(onscreenPosition));

            // pause for 1/10th second for cool effect
            if (initialPopulation)
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
        {
            yield return StartCoroutine(Swap(copies));

            if(ScanForMatches())
                StartCoroutine(FillAndScanGrid());
        }
        else
            // After the match, start to fill in the space left over
            StartCoroutine(FillAndScanGrid());
         
    }

    private IEnumerator FillAndScanGrid()
    {
        CollapseGrid();
        yield return StartCoroutine(PopulateGrid(true));

        // scan the grid for chain reactions
        if (ScanForMatches())
            // rescursive routine  
            StartCoroutine(FillAndScanGrid());

    }

    // coroutine that swaps 2 matchables in the grid
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

    private Match GetMatch(Matchable toMatch)
    {
        Match match = new Match(toMatch);

        Match   horizontalMatch,
                verticalMatch;

        // first get the horizontal matches to the left and right
        horizontalMatch = GetMatchesInDirection(toMatch, Vector2Int.left);
        horizontalMatch.Merge(GetMatchesInDirection(toMatch, Vector2Int.right));

        horizontalMatch.orientation = Orientation.horizontal;

        if (horizontalMatch.Count > 1)
        {
            match.Merge(horizontalMatch);
            // scan for vertical branches
        }
            


        // then get vertical matches up and down
        verticalMatch = GetMatchesInDirection(toMatch, Vector2Int.up);
        verticalMatch.Merge(GetMatchesInDirection(toMatch, Vector2Int.down));

        verticalMatch.orientation = Orientation.vertical;


        if (verticalMatch.Count > 1)
        {
            match.Merge(verticalMatch);
            // scan for horizontal branches
        }
            
            
        


        if (match.Count == 1)
            return null;

        return match;
    }

    private void GetBranches(Match tree, Match branchToSearch, Orientation perpendicular)
    {

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

    private void CollapseGrid()
    {
        /* go through each column left ot right,
         * search from bottom up to find an empty space,
         * then look above the empty space, and up through the rest of the column,
         * until you find a non empty space.
         * Move the matchable at the non empty space into the empty space,
         * then continue looking for emty spaces
         * */ 
        for (int x = 0; x != Dimensions.x; ++x)
            for (int yEmpty = 0; yEmpty != Dimensions.y - 1; ++yEmpty)
                if (IsEmpty(x, yEmpty))
                    for (int yNotEmpty = yEmpty + 1; yNotEmpty != Dimensions.y; ++yNotEmpty)
                        if (!IsEmpty(x, yNotEmpty) && GetItemAt(x, yNotEmpty).Idle)
                        {
                            MoveMatchableToPosition(GetItemAt(x, yNotEmpty), x, yEmpty);
                            break;
                        
                        }
    }

    private void MoveMatchableToPosition(Matchable toMove, int x, int y)
    {
      
        // move the matchable to its new position in the grid
        MoveItemTo(toMove.position, new Vector2Int(x, y));

        // update the matchable's internal grid position
        toMove.position = new Vector2Int(x, y);

        // start animation to move it on screen
        StartCoroutine(toMove.MoveToPosition(transform.position + new Vector3(x, y)));
    
    }

    // scane the grid for any matches and resolve them 
    private bool ScanForMatches()
    {
        bool madeAMatch = false;
        Matchable toMatch;
        Match match;

        // iterate through the grid, looking for non-empty and idle matchables
        
        for(int y = 0; y != Dimensions.y; ++y)
            for (int x = 0; x != Dimensions.y; ++x)
                if(!IsEmpty(x, y))
                {
                    toMatch = GetItemAt(x, y);

                    if (!toMatch.Idle)
                        continue;
                   // try to match and resolve
                   match = GetMatch(toMatch);

                    if (match != null)
                    {
                        madeAMatch = true;
                        StartCoroutine(score.ResolveMatch(match));
                    }
                        


                }
        return madeAMatch;

    }
}

