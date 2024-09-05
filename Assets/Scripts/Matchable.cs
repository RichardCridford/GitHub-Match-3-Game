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
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetType(int type, Sprite sprite, Color color)
    {
        this.type = type;
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;

    }

    private void OnMouseDown()
    {
        cursor.SelectFirst(this);
    }

    private void OnMouseUp()
    {
        print("Mouse Up at (" + position.x + ", " + position.y + ")");
    }

    private void OnMouseEnter()
    {
        print("Mouse Enter at (" + position.x + ", " + position.y + ")");
    }


    public override string ToString()
    {
        return gameObject.name;
    }
}
