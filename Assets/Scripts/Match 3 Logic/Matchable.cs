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
    private Cursor cursor;
    
    private int type;

    public int Type
    {
        get
        {
            return type;
        }
    }

    private SpriteRenderer spriteRenderer;

    // where is this matchable in the grid?
    public Vector2Int position;

    private void Awake()
    {
        cursor = Cursor.Instance;
        pool = (MatchablePool)MatchablePool.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetType(int type, Sprite sprite, Color color)
    {
        this.type = type;
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;

    }

    public IEnumerator Resolve(Transform collectionPoint)
    {
        // draw above others in the grid
        spriteRenderer.sortingOrder = 2;
        
        
        // move off the grid to a collection point
        yield return StartCoroutine (MoveToPosition(collectionPoint.position));

        // reset
        spriteRenderer.sortingOrder = 1;

        // return back to the pool
        pool.ReturnObjectToPool(this);

    }

    private void OnMouseDown()
    {
        cursor.SelectFirst(this);
    }

    private void OnMouseUp()
    {
        // if the player clicks without dragging, the selection is cancelled
        cursor.SelectFirst(null);
    }

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
