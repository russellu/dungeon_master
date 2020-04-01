using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Rooms 
{

    public Button roomButton;
    public Button creatureButton;
    public Button spellButton;
    public Button trapButton; 

    public Image[] imagePanels;

    private TileBehavior tileBehavior; 

    public Rooms(TileBehavior tileBehavior) {

        this.tileBehavior = tileBehavior; 
        roomButton = GameObject.Find("Rooms").GetComponent<Button>();
        roomButton.onClick.AddListener(OnRoomClick);

        creatureButton = GameObject.Find("Creatures").GetComponent<Button>();
        creatureButton.onClick.AddListener(OnCreatureClick); 

        imagePanels = new Image[7];

        ShowCreatureTab();
        PortalOpenEnableCreatures(); 
    }

    private void OnRoomClick() {
        ShowRoomTab(); 
    }

    private void OnCreatureClick() {
        ShowCreatureTab(); 
    }


    private void ShowCreatureTab() {

        PortalOpenEnableCreatures(); 
       // Image im1 = GameObject.Find("c1").GetComponent<Image>();
       // im1.sprite = Resources.Load<Sprite>("LevelItems/icon_imp");

    }

    private void ShowRoomTab() {
        string[] roomLabels = { "room_lair","room_hatchery","room_treasury","room_training","room_library","room_workshop",
            "room_temple"};
        for (int i = 1; i < roomLabels.Length+1; i++) {
            Image im_i = GameObject.Find("c"+i).GetComponent<Image>();
            im_i.sprite = Resources.Load<Sprite>("LevelItems/"+roomLabels[i-1]);

            Button button = GameObject.Find("c" + i).GetComponent<Button>();
            button.onClick.AddListener(delegate { OnClickRoomButton(i); }); 
        }
    }

    private void OnClickRoomButton(int index) {
        Debug.Log("ONCLICKROOMBUTTON");
        tileBehavior.MarkAvailableRoomTiles(); 
    }


    

    public void PortalOpenEnableCreatures() {


        string[] creatureLabels = { "creature_imp","creature_fly", "creature_spider","creature_goblin",   
        "creature_thief","creature_zombie","creature_wizard"};

        for (int i = 1; i < creatureLabels.Length + 1; i++)
        {
            Image im_i = GameObject.Find("c" + i).GetComponent<Image>();
            im_i.sprite = Resources.Load<Sprite>("LevelItems/" + creatureLabels[i - 1]);

            Button button = GameObject.Find("c" + i).GetComponent<Button>();
            button.onClick.AddListener(delegate { OnClickRoomButton(i); });
        }

        //ShowCreatureTab(); 
    
    }

}
