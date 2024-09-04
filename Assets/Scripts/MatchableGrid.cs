using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchableGrid : GridSystem<Matchable>
{
    private MatchablePool pool;

    [SerializeField] private Vector3 offscreenOffset;


    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
    }

    public IEnumerator PopulateGrid(bool allowMatches = false)
    {
        Matchable newMatchable;

        for(int y = 0; y != Dimensions.y; ++y)
            for (int x = 0; x != Dimensions.x; ++x)
            {
                // get a matchable from the pool
                newMatchable = pool.GetRandomMatchable();
                Vector3 onscreenPosition;

                // position the matchable on screen
                //newMatchable.transform.position = transform.position + new Vector3(x, y);

                onscreenPosition = transform.position + new Vector3(x, y);
                newMatchable.transform.position = onscreenPosition + offscreenOffset;

                // activate the matchable
                newMatchable.gameObject.SetActive(true);
             

                // place the matchable in the grid
                PutItemAt(newMatchable, x, y);

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
}

