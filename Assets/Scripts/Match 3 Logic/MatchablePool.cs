using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * This is a pool of matachables which will instantiate during load time. 
 * Remember to activate each game object before requesting a new one. 
 * This class is a Singleton that can be accessed through the Instance.
 * 
 * This class also handles the types, sprites and colours of new matchables.
 * This type can be randomised or incremented.
 */

public class MatchablePool : ObjectPool<Matchable>
{
    [SerializeField] private int howManyTypes;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Color[] colors;

    [SerializeField] private Sprite match4Powerup;
    [SerializeField] private Sprite match5Powerup;
    [SerializeField] private Sprite crossPowerup;

    public void RandomizeType(Matchable toRandomize)
    {
        int random = Random.Range (0, howManyTypes);
        toRandomize.SetType(random, sprites[random], colors[random]);
    
    }

    // Get a matchable from the pool and randomise its type. 
    public Matchable GetRandomMatchable()
    {
        Matchable randomMatchable = GetPooledObject();

        RandomizeType(randomMatchable);

        return randomMatchable;
    
    }
    
    // Increment the type of matchable and return its new type.
    public int NextType(Matchable matchable)
    {
        int nextType = (matchable.Type + 1) % howManyTypes;

        matchable.SetType(nextType, sprites[nextType], colors[nextType]);

        return nextType;
    }

    public Matchable UpgradeMatchable(Matchable toBeUpgraded, MatchType type)
    {
        if(type == MatchType.cross)
            return toBeUpgraded.Upgrade(crossPowerup);


        if (type == MatchType.match4)
        
            return toBeUpgraded.Upgrade(match4Powerup);

        if (type == MatchType.match5)

            return toBeUpgraded.Upgrade(match5Powerup);

        Debug.LogWarning("Tried to upgrade a matchable with an invalid match type");

        return toBeUpgraded;
    }
    
    // Manually set the type of matchable, used for testing obscure cases.
    public void ChangeType(Matchable toChange, int type)
    {
        toChange.SetType(type, sprites[type], colors[type]);
    }
}
