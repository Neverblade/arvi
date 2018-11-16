using UnityEngine;
using System.Collections.Generic;
using MM = ModeManager;
using PM = PlacenoteManager;
using AM = AudioCueManagerV2;

public class LocalizeMode : Mode
{
    public GameObject localizePanel;
    public Mode mapListMode;

    public override void CleanupMode()
    {
        localizePanel.SetActive(false);

        // Clean up elements
        MM.instance.elements.Clear();
        UnhighlightElement(localizePanel);

        // Clean up event handlers
        TapSwipeDetector.OnTap -= OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode()
    {
        localizePanel.SetActive(true);

        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element("Cancel", OnSelectCancel));
        MM.instance.elements = elements;
        MM.instance.index = 0;
        MM.instance.currentPanel = localizePanel;
        HighlightElement(localizePanel);

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
        LibPlacenote.Instance.StopSession();
        FeaturesVisualizer.clearPointcloud();
        AM.instance.ClearAudioCues();
        MM.instance.SwitchModes(mapListMode);
    }

}
