using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SM = ScriptManager;

public class AudioCueMode : Mode {

    public GameObject audioCuePanel;
    public Mode newMapMode;

    public override void CleanupMode() {
        audioCuePanel.SetActive(false);

        // Clean up elements
        SM.instance.elements.Clear();

        // Clean up event handler
        TapSwipeDetector.OnSwipe -= OnSwipeDefault;
        TapSwipeDetector.OnTap -= OnTapDefault;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode() {
        audioCuePanel.SetActive(true);

        // Set up elements
        List<SM.Element> elements = new List<SM.Element>();
        elements.Add(new SM.Element("Audio cue list", OnSelectAudioCueList));
        elements.Add(new SM.Element("Cancel", OnSelectCancel));
        SM.instance.elements = elements;
        SM.instance.index = 0;

        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnSwipeDefault;
        TapSwipeDetector.OnTap += OnTapDefault;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output current element name
        OutputElementName(SM.instance.elements[SM.instance.index]);
    }

    public void OnSelectAudioCueList() {
        print("Selecting audio cue. Moving to New Map Mode.");
        SM.instance.SwitchModes(newMapMode);
    }

    public void OnSelectCancel() {
        print("Cancelling. Moving to New Map Mode.");
        SM.instance.SwitchModes(newMapMode);
    }
}
