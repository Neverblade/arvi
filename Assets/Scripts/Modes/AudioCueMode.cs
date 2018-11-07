﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SM = ModeManager;

public class AudioCueMode : Mode {

    public GameObject audioCuePanel;
    public Mode newMapMode;

    public override void CleanupMode() {
        audioCuePanel.SetActive(false);

        // Clean up elements
        SM.instance.elements.Clear();

        // Clean up event handler
        TapSwipeDetector.OnSwipe -= OnSwipeDefault;
        TapSwipeDetector.OnTap -= OutputCurrentElement;
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
        TapSwipeDetector.OnTap += OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output current element name
        OutputCurrentElement();
    }

    public void OnSelectAudioCueList() {
        Debug.Log("Selecting audio cue. Moving to New Map Mode.");
        SM.instance.SwitchModes(newMapMode);
    }

    public void OnSelectCancel() {
        Debug.Log("Cancelling. Moving to New Map Mode.");
        SM.instance.SwitchModes(newMapMode);
    }
}
