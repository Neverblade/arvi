using UnityEngine;
using System.Collections.Generic;
using MM = ModeManager;
using PM = PlacenoteManager;

public class LocalizeMode : Mode
{
    public Mode mapListMode;

    public override void CleanupMode()
    {
        // Clean up elements
        MM.instance.elements.Clear();

        // Clean up event handlers
        TapSwipeDetector.OnTap -= OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode()
    {
        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element("Cancel", OnSelectCancel));
        MM.instance.elements = elements;
        MM.instance.index = 0;

        // Set up event handlers
        TapSwipeDetector.OnTap += OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        //Calling Placenote to localize
        PM.instance.StartLocalize();

        // Output current element name
        OutputCurrentElement();
    }

    public void OnSelectCancel()
    {
        Debug.Log("Cancelling. Moving to MainList Mode.");
        MM.instance.SwitchModes(mapListMode);
    }

}
