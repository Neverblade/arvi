using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MM = ModeManager;
using PM = PlacenoteManager;

public class MapListMode : Mode {

    public static string MAP_LIST_NAME = "load map list";

    public GameObject mapListPanel;
    public RectTransform listContentParent;
    public GameObject listElement;
    public ToggleGroup toggleGroup;
    public Mode localizeMode;
    public Mode mainMenuMode;

    private MapInfoElement mapInfoElement;

    public override void CleanupMode() {
        mapListPanel.SetActive(false);

        // Clean up elements
        MM.instance.elements.Clear();

        // Clean up event handler
        TapSwipeDetector.OnSwipe -= OnHorizontalSwipe;
        TapSwipeDetector.OnSwipe -= OnVerticalSwipe;
        TapSwipeDetector.OnTap -= OnMapListTap;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode() {
        mapListPanel.SetActive(true);

        //clear current List
        foreach (Transform t in listContentParent.transform) {
            Destroy(t.gameObject);
        }

        //Load maps
        if(PM.instance.LoadMapList(SeedFirstListElement)){
            Debug.Log("Loading. Moving to Map List Mode.");
        } else {
            MM.instance.OutputText("ARVI is still loading, please wait.");
        }

        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element("load map list", OnSelectMapList));
        elements.Add(new MM.Element("Cancel", OnSelectCancel));
        MM.instance.elements = elements;
        MM.instance.index = 0;

        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnHorizontalSwipe;
        TapSwipeDetector.OnSwipe += OnVerticalSwipe;
        TapSwipeDetector.OnTap += OnMapListTap;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output current element name
        SpecialOutputElement();
    }

    /**
     * For iterating between UI elements, with some special casing.
     */
    public void OnHorizontalSwipe(SwipeData swipeData){
        // Only trigger on left/right swipes
        if (swipeData.Direction != SwipeDirection.Left
            && swipeData.Direction != SwipeDirection.Right)
        {
            return;
        }

        SwitchElements(swipeData.Direction);
        SpecialOutputElement();
    }

    private void SpecialOutputElement()
    {
        if (MM.instance.elements[MM.instance.index].name.Equals(MAP_LIST_NAME))
        {
            MM.instance.OutputText(MAP_LIST_NAME + ", " + PM.instance.selectedMapInfo.metadata.name);
        }
        else
        {
            OutputCurrentElement();
        }
    }

    /**
    * For iterating between map list elements
    */
    public void OnVerticalSwipe(SwipeData data) {
        if (data.Direction != SwipeDirection.Down
            && data.Direction != SwipeDirection.Up)
        {
            return;
        }
        foreach (Transform t in listContentParent.transform) {
            Destroy(t.gameObject);
        }

        if (data.Direction == SwipeDirection.Up) {
            PM.instance.mMapListIdx -= 1;
            PM.instance.mMapListIdx += PM.instance.mMapList.Count;
        }
        else if (data.Direction == SwipeDirection.Down) {
            PM.instance.mMapListIdx += 1;
        }
        PM.instance.mMapListIdx = PM.instance.mMapListIdx % PM.instance.mMapList.Count;
        AddMapToList(PM.instance.mMapList[PM.instance.mMapListIdx]);
        MM.instance.OutputText(PM.instance.selectedMapInfo.metadata.name);
    }


    public void OnMapListTap()
    {
        if (!MM.instance.elements[MM.instance.index].name.Equals(MAP_LIST_NAME))
        {
            OutputCurrentElement();
        }
        else
        {
            MM.instance.OutputText(PM.instance.selectedMapInfo.metadata.name + ", " + mapInfoElement.mLocationText.text);
        }
    }

    public void AddMapToList(LibPlacenote.MapInfo mapInfo) {
        GameObject newElement = Instantiate(listElement) as GameObject;
        mapInfoElement = newElement.GetComponent<MapInfoElement>();
        mapInfoElement.Initialize(mapInfo, toggleGroup, listContentParent, (value) => {
        });
        PM.instance.selectedMapInfo = mapInfo;
    }

    public void OnSelectMapList() {
        Debug.Log("Selecting map. Moving to Localize Mode.");
        MM.instance.SwitchModes(localizeMode);
    }

    public void OnSelectCancel() {
        Debug.Log("Cancelling. Moving to Main Menu Mode.");
        MM.instance.SwitchModes(mainMenuMode);
    }

    public void SeedFirstListElement() {
        if (PM.instance.mMapList.Count == 0) {
            MM.instance.OutputText("There are no scans in your area.");
        } else {
            AddMapToList(PM.instance.mMapList[PM.instance.mMapListIdx]);
        }
    }
}
