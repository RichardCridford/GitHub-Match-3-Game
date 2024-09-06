using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class will manage the grid of matchables by inheriting grid mechanics from Grid System
 * It's also a Singleton which can be accessed through Instance
 */

public class MatchableGrid : GridSystem<Matchable>
{
    // The pool of Matchables with which to populate the grid
    private MatchablePool pool;

    // A distance offscreen where the matchables will spawn
    [SerializeField] private Vector3 offscreenOffset;

    // Get a reference to the pool on start
    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
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
                int type = newMatchable.Type;

                while (!allowMatches && IsPartOfAMatch(newMatchable))
                {
                    // change the matchable's type until it isn't a match anymore
                    if (pool.NextType(newMatchable) == type)
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

    /*
     * TODO : Write this function
     * 
     */

    private bool IsPartOfAMatch(Matchable matchable)
    {

        return false;
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

        //TODO : complete match validation! 

        // if no match, swap them back
        StartCoroutine(Swap(copies));

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

