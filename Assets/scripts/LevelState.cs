using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelState 
{
    public float gold = 0;
    TileBehavior tileBehavior;
    Main main; 

    public LevelState(Main main, TileBehavior tileBehavior) {

        this.tileBehavior = tileBehavior;
        this.main = main; 
    }   

    public void RestoreSaved() { 
    
    }

    public void updateGold(float goldAmount) {
        gold += goldAmount;
        main.goldTxt.text = "Gold: " + gold; 
    }


}
