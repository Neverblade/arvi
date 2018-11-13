using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MM = ModeManager;
using PM = PlacenoteManager;

public class MapListMode : Mode
{
    public GameObject mapListPanel;
    public RectTransform listContentParent;
    public GameObject listElement;
    public ToggleGroup toggleGroup;
    public Mode localizeMode;
    public Mode mainMenuMode;

    public override void CleanupMode()
    {
        mapListPanel.SetActive(false);

        // Clean up elements
        MM.instance.elements.Clear();

        // Clean up event handler
        TapSwipeDetector.OnSwipe -= OnSwipeDefault;
        TapSwipeDetector.OnSwipe -= OnVerticalSwipe;
        TapSwipeDetector.OnTap -= OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode()
    {
        mapListPanel.SetActive(true);

        //clear current List
        foreach (Transform t in listContentParent.transform)
        {
            Destroy(t.gameObject);
        }

        //Load maps
        if(PM.instance.LoadMapList()){
            Debug.Log("Loading. Moving to Map List Mode.");
        }else{
            MM.instance.OutputText("ARVI is still loading, please wait.");
        }
        //Show first element
        AddMapToList(PM.instance.mMapList[PM.instance.mMapListIdx]);

        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element("load map list", OnSelectMapList));
        elements.Add(new MM.Element("Cancel", OnSelectCancel));
        MM.instance.elements = elements;
        MM.instance.index = 0;

        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnSwipeDefault;
        TapSwipeDetector.OnSwipe += OnVerticalSwipe;
        TapSwipeDetector.OnTap += OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output current element name
        OutputCurrentElement();
    }

    public void OnVerticalSwipe(SwipeData data)
    {
        foreach (Transform t in listContentParent.transform)
        {
            Destroy(t.gameObject);
        }

        if (data.Direction == SwipeDirection.Up)
        {
            PM.instance.mMapListIdx -= 1;
            PM.instance.mMapListIdx += PM.instance.mMapList.Count;
        }
        else if (data.Direction == SwipeDirection.Down)
        {
            PM.instance.mMapListIdx += 1;
        }
        PM.instance.mMapListIdx = PM.instance.mMapListIdx % PM.instance.mMapList.Count;
        AddMapToList(PM.instance.mMapList[PM.instance.mMapListIdx]);
    }

    public void AddMapToList(LibPlacenote.MapInfo mapInfo)
    {
        GameObject newElement = Instantiate(listElement) as GameObject;
        MapInfoElement mapInfoElement = newElement.GetComponent<MapInfoElement>();
        mapInfoElement.Initialize(mapInfo, toggleGroup, listContentParent, (value) => {
        });
        PM.instance.selectedMapInfo = mapInfo;
    }
    public void OnSelectMapList()
    {
        Debug.Log("Selecting map. Moving to Localize Mode.");
        MM.instance.SwitchModes(localizeMode);
    }

    public void OnSelectCancel()
    {
        Debug.Log("Cancelling. Moving to Main Menu Mode.");
        MM.instance.SwitchModes(mainMenuMode);
    }

}
