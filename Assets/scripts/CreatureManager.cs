using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager
{ 

    public List<Imp> imps;
    public List<Fighter> fightersFriendly;
    public List<Fighter> fightersEnemy;

    TileBehavior tileBehavior; 

    public CreatureManager()
    {
        imps = new List<Imp>();

       // imps.Add(new Imp(new Vector2(0,0)));
       // imps.Add(new Imp(new Vector2(0.25f,0.25f)));
       // imps.Add(new Imp(new Vector2(0.2f, 0.2f)));
       // imps.Add(new Imp(new Vector2(0.15f, 0.2f)));
       // imps.Add(new Imp(new Vector2(0.15f, 0.15f)));

        fightersFriendly = new List<Fighter>();
        fightersEnemy = new List<Fighter>();

        
        for (int i = 0; i < 60; i++)
        {
            float newX = Random.Range(-.9f, .9f);
            float newY = Random.Range(-.9f, .9f); 

            if(i%2==0)
                fightersFriendly.Add(new Fighter(this, new Vector3(newX, newY, -3), 0, .5f));
            else
                fightersEnemy.Add(new Fighter(this, new Vector3(newX, newY, -3), 1, .5f));

        }
        
        /*
        fightersFriendly.Add(new Fighter(this, new Vector3(0, 0, -3), 0, 1f));
        fightersFriendly.Add(new Fighter(this, new Vector3(0.25f, 0, -3), 0, 1f));
        fightersFriendly.Add(new Fighter(this, new Vector3(0, 0.25f, -3), 0, 1f));
        fightersFriendly.Add(new Fighter(this, new Vector3(0.5f, 0.5f, -3), 0, 1f));

        fightersEnemy.Add(new Fighter(this, new Vector3(-1, -1, -3), 1, 0.5f));
        fightersEnemy.Add(new Fighter(this, new Vector3(-1, -.5f, -3), 1, 0.5f));
        fightersEnemy.Add(new Fighter(this, new Vector3(-.8f, -.8f, -3), 1, 0.5f));
        */
    }

    public void SetTileBehavior(TileBehavior tileBehavior)
    {
        this.tileBehavior = tileBehavior;

        //fightersEnemy[0].InitiateRandomWalk(tileBehavior, 35); 
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

        foreach (Imp imp in imps)
        {
            imp.Update(); 

        }

        foreach (Fighter friendly in fightersFriendly)
        {
            if (!friendly.wagingWar)
                friendly.SeekAndDestroy(fightersEnemy, tileBehavior);

            friendly.Update(tileBehavior); 
        }

        foreach (Fighter enemy in fightersEnemy)
        {
            if (!enemy.wagingWar)
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
