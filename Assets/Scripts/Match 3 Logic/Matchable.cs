using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * These are the things in the grid that the player will match by swapping around
 * 
 * They require a sprite to be drawn on the screen
 * 
 * The Matchable Type can only be set along with a sprite and colour
 */

[RequireComponent(typeof(SpriteRenderer))]

public class Matchable : Movable
{
    private MatchablePool pool;
    private MatchableGrid grid;
    private Cursor cursor;
    
    private int type;

    public int Type
    {
        get
        {
            return type;
        }
    }

    private MatchType powerup = MatchType.invalid;

    public bool isGem
    {
        get
        {
            return powerup == MatchType.match5;
        
        } 
    }



    private SpriteRenderer spriteRenderer;

    // where is this matchable in the grid?
    public Vector2Int position;

    // get required references during awake.
    private void Awake()
    {
        cursor = Cursor.Instance;
        pool = (MatchablePool)MatchablePool.Instance;
        grid = (MatchableGrid)MatchableGrid.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // set the type, sprite and colour all at once, performed by the pool
    public void SetType(int type, Sprite sprite, Color color)
    {
        this.type = type;
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;

    }

    public IEnumerator Resolve(Transform collectionPoint)
    {
        // if matchable is a powerup resolve it in that way
        if (powerup != MatchType.invalid)
        {
            // resolve a match4 powerup
            if (powerup == MatchType.match4)
            {
                // score everything adjacent to this
                grid.MatchAllAdjacent(this);
            }

            // resolve a cross powerup
            if (powerup == MatchType.cross)
            {
                grid.MatchRowAndColumn(this);
            
            }

            powerup = MatchType.invalid;

        }

        // if tis was called as the result of a powerup being upgraded, then don't move it off the grid
        if (collectionPoint == null)
            yield break;

        // draw above others in the grid
        spriteRenderer.sortingOrder = 2;
        
        
        // move off the grid to a collection point
        yield return StartCoroutine (MoveToTransform(collectionPoint));

        // reset
        spriteRenderer.sortingOrder = 1;

        // return back to the pool
        pool.ReturnObjectToPool(this);


    }
    // change the sprite of this matchable to be a powerup while retaining colour and type
    public Matchable Upgrade(MatchType powerupType , Sprite powerupSprite)
    {

        // if this is already a powerup, resolve it before upgrading
        if (powerup != MatchType.invalid)
        {
            idle = false;
            StartCoroutine(Resolve(null));
            idle = true;
        }
        if (powerupType == MatchType.match5)
        {
            // this powerup is it's own type and can be used with any colour matchable
            type = -1;
            // set sprite to default colour
            spriteRenderer.color = Color.white;
        }
        
        powerup = powerupType;
        spriteRenderer.sprite = powerupSprite;

        return this;
    }

    // set the sorting order of the sprite renderer so it will be drawn aboce or below others
    public int SortingOrder
    {
        set 
        {
            spriteRenderer.sortingOrder = value;
        }
    
    }

    // when the player clicks, select this as the first selected
    private void OnMouseDown()
    {
        cursor.SelectFirst(this);
    }

    private void OnMouseUp()
    {
        // if the player clicks without dragging, the selection is cancelled
        cursor.SelectFirst(null);
    }

    // when the player drags the mouse, select this as second selected
    private void OnMouseEnter()
    {
        // used for swapping matchables
        cursor.SelectSecond(this);
    }


    public override string ToString()
    {
        return gameObject.name;
    }


}
