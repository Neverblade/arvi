using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MM = ModeManager;
using PM = PlacenoteManager;

public class MapListMode : Mode {

    public static string MAP_LIST_NAME = "Map list";

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
        PM.instance.mMapList.Clear();
        foreach (Transform child in listContentParent.transform){
            Destroy(child.gameObject);
        }
        UnhighlightElement(mapListPanel);

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
        listContentParent.DetachChildren();
        //Load maps
        if (PM.instance.LoadMapList(SeedFirstListElement)){
            //Debug.Log("Loading. Moving to Map List Mode.");
        } else {
            MM.OutputText("ARVI is still loading, please stand by.");
        }
        //PM.instance.selectedMapInfo = PM.instance.mMapList[PM.instance.mMapListIdx];

        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element(MAP_LIST_NAME, OnSelectMapList));
        elements.Add(new MM.Element("Cancel", OnSelectCancel));
        MM.instance.elements = elements;
        MM.instance.currentPanel = mapListPanel;
        MM.instance.listTransform = listContentParent;
        MM.instance.index = 0;
        HighlightElement(mapListPanel, listContentParent);
       
        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnHorizontalSwipe;
        TapSwipeDetector.OnSwipe += OnVerticalSwipe;
        TapSwipeDetector.OnTap += OnMapListTap;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output intro
        MM.OutputText("Choose a map from the list.");
        MM.OutputText(PM.instance.selectedMapInfo.metadata.name);
    }

    /**
     * For iterating between UI elements, with some special casing.
     */
    public void OnHorizontalSwipe(SwipeData swipeData){
        // Only trigger on left/right swipes
        if (swipeData.Direction != SwipeDirection.Left && swipeData.Direction != SwipeDirection.Right) {
            return;
        }

        SwitchElements(swipeData.Direction);
        SpecialOutputElement();
    }

    private void SpecialOutputElement() {
        if (MM.instance.elements[MM.instance.index].name.Equals(MAP_LIST_NAME)) {
            MM.OutputText(MAP_LIST_NAME + ", " + PM.instance.selectedMapInfo.metadata.name);
        }
        else {
            OutputCurrentElement();
        }
    }

    /**
    * For iterating between map list elements
    */
    public void OnVerticalSwipe(SwipeData data) {
        if (data.Direction != SwipeDirection.Down && data.Direction != SwipeDirection.Up) {
            return;
        }

        if (data.Direction == SwipeDirection.Up) {
            if (PM.instance.mMapListStart == PM.instance.mMapListIdx && PM.instance.mMapListStart==0) {
                PM.instance.mMapListEnd = PM.instance.mMapList.Count - 1;
                PM.instance.mMapListStart = PM.instance.mMapListEnd - 9;
                UpdateList();
            } else if(PM.instance.mMapListStart == PM.instance.mMapListIdx && PM.instance.mMapListStart!=0){
                PM.instance.mMapListStart--;
                PM.instance.mMapListEnd--;
                UpdateList();
            }
            PM.instance.mMapListIdx -= 1;
            PM.instance.mMapListIdx += PM.instance.mMapList.Count;
        }
        else if (data.Direction == SwipeDirection.Down) {
            if (PM.instance.mMapListEnd == PM.instance.mMapListIdx && PM.instance.mMapListEnd<PM.instance.mMapList.Count-1) {
                PM.instance.mMapListStart++;
                PM.instance.mMapListEnd++;
                UpdateList();
            } else if(PM.instance.mMapListEnd == PM.instance.mMapListIdx && PM.instance.mMapListEnd>=PM.instance.mMapList.Count-1) {
                PM.instance.mMapListStart = 0;
                PM.instance.mMapListEnd = PM.instance.mMapListStart + 9;
                UpdateList();
            }
            PM.instance.mMapListIdx += 1;
        }
        PM.instance.mMapListIdx = PM.instance.mMapListIdx % PM.instance.mMapList.Count;

        HighlightElement(mapListPanel, listContentParent);
        PM.instance.selectedMapInfo = PM.instance.mMapList[PM.instance.mMapListIdx];
        MM.OutputText(PM.instance.selectedMapInfo.metadata.name);

    }


    public void OnMapListTap() {
        if (!MM.instance.elements[MM.instance.index].name.Equals(MAP_LIST_NAME)) {
            OutputCurrentElement();
        }
        else {
            MM.OutputText(PM.instance.selectedMapInfo.metadata.name + ", " + mapInfoElement.mLocationText.text);
        }
    }

    public void AddMapToList(LibPlacenote.MapInfo mapInfo) {
        GameObject newElement = Instantiate(listElement) as GameObject;
        mapInfoElement = newElement.GetComponent<MapInfoElement>();
        mapInfoElement.Initialize(mapInfo, toggleGroup, listContentParent, (value) => {
        });
    }

    public void OnSelectMapList() {
        //Debug.Log("Selecting map. Moving to Localize Mode.");
        MM.instance.SwitchModes(localizeMode);
    }

    public void OnSelectCancel() {
        //Debug.Log("Cancelling. Moving to Main Menu Mode.");
        MM.instance.SwitchModes(mainMenuMode);
    }

    public void SeedFirstListElement() {
        if (PM.instance.mMapList.Count == 0) {
            MM.OutputText("There are no scans in your area.");
        } else {
            if(PM.instance.mMapListEnd==0){
                PM.instance.mMapListEnd = 9;
            }
            for (int i = PM.instance.mMapListStart; i <= PM.instance.mMapListEnd;i++){
                AddMapToList(PM.instance.mMapList[i]);
            }
        }
    }

    public void UpdateList(){
        foreach (Transform t in listContentParent.transform){
            Destroy(t.gameObject);
        }
        MM.instance.listTransform = listContentParent;
        listContentParent.DetachChildren();
        for (int i = PM.instance.mMapListStart; i <= PM.instance.mMapListEnd; i++){
            AddMapToList(PM.instance.mMapList[i]);
        }
    }
}
