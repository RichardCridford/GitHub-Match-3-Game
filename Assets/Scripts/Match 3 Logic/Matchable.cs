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

    // get required references during awake.
    private void Awake()
    {
        cursor = Cursor.Instance;
        pool = (MatchablePool)MatchablePool.Instance;
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
        // draw above others in the grid
        spriteRenderer.sortingOrder = 2;
        
        
        // move off the grid to a collection point
        yield return StartCoroutine (MoveToPosition(collectionPoint.position));

        // reset
        spriteRenderer.sortingOrder = 1;

        // return back to the pool
        pool.ReturnObjectToPool(this);

    }

    public void Upgrade(Sprite powerupSprite)
    {
        spriteRenderer.sprite = powerupSprite;
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
