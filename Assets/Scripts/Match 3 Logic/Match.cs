using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Orientation
{
    none,
    horizontal,
    vertical,
    both

}

/*
 * This is a collection of Matchables that have been matched.
 */

// Doesn't need to be a monobehaviour because it's not being attached to a game object
public class Match
{
    // the number of matchables that are considered part of this match but aren't added to the list
    private int unlisted = 0 ;

    // is this match horizontal or vertical?
    public Orientation orientation = Orientation.none;

    // the internal list of matched matchables
    private List<Matchable> matchables;

    private Matchable toBeUpgraded = null;
    
    // getters for the list and the list count
    public List<Matchable> Matchables
    {
        get
        {
            return matchables;
        }
    }

    // getter for number of matchables part of this match
    public int Count
    {
        get 
        {
            return Matchables.Count + unlisted;
        }
    
    }

    // check if a matchable is already in this match
    public bool Contains(Matchable toCompare)
    {
        return Matchables.Contains(toCompare);
    }

    // constructor initialises the list 
    public Match()
    {
        matchables = new List<Matchable>(5);
    
    }

    // overload, also adds a match
    public Match(Matchable original) : this()
    {
        AddMatchable(original);
        toBeUpgraded = original; 
    }

    // get the matchable to be upgraded
    public Matchable ToBeUpgraded
    {
        get
        {
            if (toBeUpgraded != null)
                return toBeUpgraded;

            return matchables[Random.Range(0 , matchables.Count)];
        }
    }

    // add a matchable to the list
    public void AddMatchable(Matchable toAdd)
    {
        matchables.Add(toAdd);
    }

    // add a matchable to the count without adding it to the list
    public void AddUnlisted()
    {
        unlisted++;
    }

    // remove Matchable from the list
    public void RemoveMatchable(Matchable toBeRemoved)
    {
        matchables.Remove(toBeRemoved);
    
    }

    // merge another match into this one
    public void Merge(Match toMerge)
    {
        matchables.AddRange(toMerge.Matchables);
    
    }

    public override string ToString()
    {
        string s = "Match of type" + matchables[0].Type + " : ";

        foreach (Matchable m in matchables)
            s += "(" + m.position.x + ", " + m.position.y + ") ";

        return s;
    
    }


    
}
