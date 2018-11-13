using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MM = ModeManager;

public class AudioCueMode : Mode {

    public GameObject audioCuePanel;
    public GameObject audioInfoElementPrefab;
    public Mode newMapMode;

    private RectTransform contentPanel;
    private int index;

    public override void CleanupMode() {
        audioCuePanel.SetActive(false);

        // Clean up elements
        MM.instance.elements.Clear();

        // Clean up event handler
        TapSwipeDetector.OnSwipe -= OnSwipeDefault;
        TapSwipeDetector.OnSwipe -= OnVerticalSwipe;
        TapSwipeDetector.OnTap -= OutputCurrentElement;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode() {
        audioCuePanel.SetActive(true);

        // Set up list
        contentPanel = (RectTransform) audioCuePanel.transform
            .Find("AudioListPanel")
            .Find("Viewport")
            .Find("Content");
        AddAudioCueListElement(AudioLibrary.instance.library[index].id);

        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element("Audio cue list", OnSelectAudioCueList));
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

    /**
     * For iterating between elements on the audio cue list.
     */
    public void OnVerticalSwipe(SwipeData swipeData) {
        // Only trigger on up/down swipes
        if (swipeData.Direction != SwipeDirection.Down
            && swipeData.Direction != SwipeDirection.Up) {
            return;
        }

        // Remove current element(s)
        foreach (Transform child in contentPanel.transform) {
            Destroy(child.gameObject);
        }

        // Update index
        index += swipeData.Direction == SwipeDirection.Up ? 1 : -1;
        int librarySize = AudioLibrary.instance.library.Length;
        index = (index + librarySize) % librarySize;

        // Update list
        AddAudioCueListElement(AudioLibrary.instance.library[index].id);
    }

    public void OnSelectAudioCueList() {
        Debug.Log("Selecting audio cue. Moving to New Map Mode.");
        MM.instance.SwitchModes(newMapMode);
    }

    public void OnSelectCancel() {
        Debug.Log("Cancelling. Moving to New Map Mode.");
        MM.instance.SwitchModes(newMapMode);
    }

    /**
     * Places an audio cue list element with the given id on the list.
     */
    private void AddAudioCueListElement(string id) {
        GameObject element = Instantiate(audioInfoElementPrefab);
        AudioInfoElementV2 audioInfoElement = element.GetComponent<AudioInfoElementV2>();
        audioInfoElement.SetId(id);
        element.transform.SetParent(contentPanel);
    }
}
