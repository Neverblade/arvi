using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MM = ModeManager;
using AM = AudioCueManagerV2;

public class AudioCueMode : Mode {

    public static string AUDIO_CUE_LIST_NAME = "Audio cue list";

    public GameObject audioCuePanel;
    public GameObject audioInfoElementPrefab;
    public Mode newMapMode;

    private RectTransform contentPanel;
    private int audioLibraryIndex;
    private AudioInfoElementV2 storedAudioInfoElement;
    private bool playingAudio;

    public override void CleanupMode() {
        audioCuePanel.SetActive(false);

        // Clean up elements
        MM.instance.elements.Clear();
        foreach (Transform child in contentPanel.transform) {
            Destroy(child.gameObject);
        }
        UnhighlightElement(audioCuePanel);

        // Clean up event handler
        TapSwipeDetector.OnSwipe -= OnHorizontalSwipe;
        TapSwipeDetector.OnSwipe -= OnVerticalSwipe;
        TapSwipeDetector.OnTap -= OnAudioCueListTap;
        TapSwipeDetector.OnDoubleTap -= OnDoubleTapDefault;
    }

    public override void SetupMode() {
        audioCuePanel.SetActive(true);

        // Set up list, seed w/ the first element
        contentPanel = (RectTransform) audioCuePanel.transform
                .Find("AudioListPanel")
                .Find("Viewport")
                .Find("Content");
        Audio audio = AudioLibrary.instance.library[audioLibraryIndex];
        AddAudioCueListElement(audio.id);
        AM.instance.PauseCandidateAudioCueSound();
        AM.instance.ChangeCandidateAudioCueClip(audio);

        // Set up elements
        List<MM.Element> elements = new List<MM.Element>();
        elements.Add(new MM.Element(AUDIO_CUE_LIST_NAME, OnSelectAudioCueList));
        elements.Add(new MM.Element("Cancel", OnSelectCancel));
        MM.instance.elements = elements;
        MM.instance.index = 0;
        MM.instance.listTransform = contentPanel;
        MM.instance.currentPanel = audioCuePanel;
        HighlightElement(audioCuePanel, contentPanel);

        // Set up event handlers
        TapSwipeDetector.OnSwipe += OnHorizontalSwipe;
        TapSwipeDetector.OnSwipe += OnVerticalSwipe;
        TapSwipeDetector.OnTap += OnAudioCueListTap;
        TapSwipeDetector.OnDoubleTap += OnDoubleTapDefault;

        // Output current element name
        SpecialOutputElement();
    }

    /**
     * For iterating between UI elements, with some special casing.
     */
    public void OnHorizontalSwipe(SwipeData swipeData) {
        // Only trigger on left/right swipes
        if (swipeData.Direction != SwipeDirection.Left
            && swipeData.Direction != SwipeDirection.Right) {
            return;
        }

        SwitchElements(swipeData.Direction);
        SpecialOutputElement();
    }

    /**
     * Outputs the element name, with special casing to output the audio cue
     * name when the audio cue list is selected.
     */
    private void SpecialOutputElement() {
        if (MM.instance.elements[MM.instance.index].name.Equals(AUDIO_CUE_LIST_NAME)) {
            Audio audio = AudioLibrary.instance.library[audioLibraryIndex];
            MM.OutputText(AUDIO_CUE_LIST_NAME + ", " + audio.id);
        } else {
            OutputCurrentElement();
        }
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
        audioLibraryIndex += swipeData.Direction == SwipeDirection.Up ? 1 : -1;
        int librarySize = AudioLibrary.instance.library.Length;
        audioLibraryIndex = (audioLibraryIndex + librarySize) % librarySize;

        // Update list and candidate audio cue
        playingAudio = false;
        Audio audio = AudioLibrary.instance.library[audioLibraryIndex];
        AddAudioCueListElement(audio.id);
        AM.instance.PauseCandidateAudioCueSound();
        AM.instance.ChangeCandidateAudioCueClip(audio);

        // Output current audio cue name
        MM.OutputText(audio.id);
    }

    /**
     * For pausing sound when switching elements.
     */
    public void OnHorizontalSwipe() {
        AM.instance.PauseCandidateAudioCueSound();
    }

    /**
     * For toggling sound on an audio cue element.
     */
    public void OnAudioCueListTap() {
        if (!MM.instance.elements[MM.instance.index].name.Equals(AUDIO_CUE_LIST_NAME)) {
            OutputCurrentElement();
        } else {
            playingAudio = !playingAudio;
            if (playingAudio) {
                storedAudioInfoElement.Play();
                AM.instance.PlayCandidateAudioCueSound();
            } else {
                storedAudioInfoElement.Pause();
                AM.instance.PauseCandidateAudioCueSound();
            }
        }
    }

    public void OnSelectAudioCueList() {
        Debug.Log("Selecting audio cue. Moving to New Map Mode.");
        AM.instance.PlayCandidateAudioCueSound();
        AM.instance.ConfirmCandidateAudioCue();
        MM.instance.SwitchModes(newMapMode);
    }

    public void OnSelectCancel() {
        Debug.Log("Cancelling. Moving to New Map Mode.");
        AM.instance.RemoveCandidateAudioCue();
        MM.instance.SwitchModes(newMapMode);
    }

    /**
     * Places an audio cue list element with the given id on the list.
     */
    private void AddAudioCueListElement(string id) {
        GameObject element = Instantiate(audioInfoElementPrefab);
        Color highlightColor;
        ColorUtility.TryParseHtmlString(HIGHLIGHT_COLOR_CODE, out highlightColor);
        element.GetComponent<Image>().color = MM.instance.index == 0 ? highlightColor : Color.white;
        storedAudioInfoElement = element.GetComponent<AudioInfoElementV2>();
        storedAudioInfoElement.SetId(id);
        element.transform.SetParent(contentPanel);
    }
}
