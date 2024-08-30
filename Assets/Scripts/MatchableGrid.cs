using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchableGrid : GridSystem<Matchable>
{
    private MatchablePool pool;

    private void Start()
    {
        pool = (MatchablePool) MatchablePool.Instance;
    }

    public IEnumerator PopulateGrid()
    {
        Matchable newMatchable;

        for(int y = 0; y != Dimensions.y; ++y)
            for (int x = 0; x != Dimensions.x; ++x)
            {
                // get a matchable from the pool
                newMatchable = pool.GetPooledObject();

                // position the matchable on screen
                newMatchable.transform.position = transform.position + new Vector3(x, y);

                // activate the matchable
                newMatchable.gameObject.SetActive(true);

                // place the matchable in the grid
                PutItemAt(newMatchable, x, y);

                yield return new WaitForSeconds(0.1f);

            }
    }
}

