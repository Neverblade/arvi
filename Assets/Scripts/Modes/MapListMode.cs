using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MM = ModeManager;

public class MapListMode : Mode
{
    public GameObject mapListPanel;
    public Mode localizeMode;
    public Mode mainMenuMode;

    public override void CleanupMode()
    {
        mapListPanel.SetActive(false);

        // Clean up elements
        MM.instance.elements.Clear();

        // Clean up event handler
        TapSwipeDetector.OnSwipe -= OnSwipeDefault;
        TapSwipeDetector.OnTap -= OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode()
    {
        mapListPanel.SetActive(true);

        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element("load map list", OnSelectMapList));
        elements.Add(new MM.Element("Cancel", OnSelectCancel));
        MM.instance.elements = elements;
        MM.instance.index = 0;

        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnSwipeDefault;  //need to override to implement vertical swipe when connect to placenote 
        TapSwipeDetector.OnTap += OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output current element name
        OutputCurrentElement();
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
