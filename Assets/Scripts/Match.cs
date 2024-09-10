using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Doesn't need to be a monobehaviour because it's not being attached to a game object
public class Match
{
    private List<Matchable> matchables;
    public List<Matchable> Matchables
    {
        get
        {
            return matchables;
        }
    }

    public int Count
    {
        get 
        {
            return Matchables.Count;
        }
    
    }

    public Match()
    {
        matchables = new List<Matchable>(5);
    
    }
    public Match(Matchable original) : this()
    {
        Addmatchable(original);
    
    }
    public void AddMatchable(Matchable toAdd)
    {
        matchables.Add(toAdd);
    }

    public override string ToString()
    {
        string s = "Match of type" + matchables[0].Type + " : ";

        foreach (Matchable m in matchables)
            s += "(" + m.position.x + ", " + m.position.y + ") ";

        return s;
    
    }


    
}
