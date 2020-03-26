using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager
{ 

    public List<Imp> imps;
    public List<Fighter> fightersFriendly;
    public List<Fighter> fightersEnemy;

    TileBehavior tileBehavior;
    public Main main;
    long framecount = 0; 

    public CreatureManager()
    {
        imps = new List<Imp>();

        imps.Add(new Imp(new Vector2(2.4f,-2.45f)));
        imps.Add(new Imp(new Vector2(2.5f,-2.4f)));
        imps.Add(new Imp(new Vector2(2.45f, -2.5f)));
 

        fightersFriendly = new List<Fighter>();
        fightersEnemy = new List<Fighter>();

        float[] pos1 = new float[] { -2.4f,-2.4f};
        float[] pos2 = new float[] { -2.4f, -2.3f };

        for (int i = 0; i < 0; i++)
        {
           // float newX = Random.Range(-1.8f, 0f);
           // float newY = Random.Range(-1.8f, 0f); 
            
            if(i%2==0)
                fightersFriendly.Add(new Fighter(this, new Vector3(pos1[0], pos1[1], -3), 0, .5f));
            else
                fightersEnemy.Add(new Fighter(this, new Vector3(pos2[0], pos2[1], -3), 1, .5f));
                
        }
        
    }

    public void Spawn(Vector2 location, int polarity) {
        if(polarity==0)
            fightersFriendly.Add(new Fighter(this, new Vector3(location.x, location.y, -3), polarity, .5f));
        else if(polarity==1)
            fightersEnemy.Add(new Fighter(this, new Vector3(location.x, location.y, -3), polarity, .5f));

    }

    public void SpawnImp(Vector2 location)
    {

        imps.Add(new Imp(location));

    }

    public void SetTileBehavior(TileBehavior tileBehavior)
    {
        this.tileBehavior = tileBehavior;
    }

    public List<Imp> GetIdleImps()
    {
        return imps; 
    }

    public void CancelImpMovements(string objectTag)
    {
        foreach (Imp imp in imps)
        {
            imp.CancelMovementToTagged(objectTag);
        }
    }

    public void UpdateAllCreatures()
    {
        framecount++; 

        foreach (Imp imp in imps)
        {
            imp.Update(); 

        }
        
        foreach (Fighter friendly in fightersFriendly)
        {
            if (framecount%10==0)
                friendly.SeekAndDestroy(fightersEnemy, tileBehavior);

            friendly.Update(tileBehavior); 
        }

        foreach (Fighter enemy in fightersEnemy)
        {
            if (framecount%10==0)
                enemy.SeekAndDestroy(fightersFriendly, tileBehavior); 

            enemy.Update(tileBehavior);
        }
        
    }

    public void EraseCreature(Fighter creature)
    {
        if (fightersEnemy.Contains(creature))
        {
            fightersEnemy.Remove(creature);
            creature.RemoveSelf();
        }
        else if (fightersFriendly.Contains(creature))
        {
            fightersFriendly.Remove(creature);
            creature.RemoveSelf();
        }
    }

}
