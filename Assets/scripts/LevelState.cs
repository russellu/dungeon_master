using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelState 
{
    public float gold = 0;
    public float stone = 0; 
    TileBehavior tileBehavior;
    Main main; 

    public LevelState(Main main, TileBehavior tileBehavior) {

        this.tileBehavior = tileBehavior;
        this.main = main; 
    }   

    public void RestoreSaved() { 
    
    }

    public void updateStone(float stoneAmount) {
        stone += 10;
        main.stoneTxt.text = "Stone: " + stone; 
    }

    public void updateGold(float goldAmount) {
        gold += 10;
        main.goldTxt.text = "Gold: " + gold; 
    }


}
