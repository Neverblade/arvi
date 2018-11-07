﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SM = ScriptManager;

public class MainMenuMode : Mode {

    public GameObject mainMenuPanel;
    public Mode scanningMode;
    public Mode localizingMode;

    public override void CleanupMode() {
        mainMenuPanel.SetActive(false);

        // Clean up elements
        SM.instance.elements.Clear();

        // Clean up event handlers
        TapSwipeDetector.OnSwipe -= OnSwipeDefault;
        TapSwipeDetector.OnTap -= OnTapDefault;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode() {
        mainMenuPanel.SetActive(true);
        
        // Set up elements
        List<SM.Element> elements = new List<SM.Element>();
        elements.Add(new SM.Element("Scanning", OnSelectScanning));
        elements.Add(new SM.Element("Localizing", OnSelectLocalizing));
        SM.instance.elements = elements;

        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnSwipeDefault;
        TapSwipeDetector.OnTap += OnTapDefault;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;
    }

    public void OnSelectScanning() {
        print("Scanning. Moving to New Map Mode.");
        SM.instance.SwitchModes(scanningMode);
    }

    public void OnSelectLocalizing() {
        print("Localizing. Moving to Load Map Menu.");
        SM.instance.SwitchModes(localizingMode);
    }
}
