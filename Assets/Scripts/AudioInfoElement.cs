using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioInfoElement : MonoBehaviour {

    public Sprite playSprite;
    public Sprite pauseSprite;

    [HideInInspector] public string id;
    [HideInInspector] public AudioClip clip;
    [HideInInspector] public AudioCueManager manager;
    [HideInInspector] public Image buttonImage;

    private bool playing = false;

    private void Start() {
        buttonImage = transform.Find("PlayButton").GetComponent<Image>();
        buttonImage.sprite = playSprite;
        transform.Find("AudioIdLabel").GetComponent<Text>().text = id;
    }

    /**
     * Play/pause button clicked.
     */
    public void OnClick() {
        if (!playing) {
            TurnOn();
        } else {
            TurnOff();
        }
    }

    public void TurnOn() {
        playing = true;
        buttonImage.sprite = pauseSprite;
        manager.PlayAudioClip(this);
    }

    public void TurnOff() {
        playing = false;
        buttonImage.sprite = playSprite;
        manager.StopAudioClip();
    }

    /**
     * Selecting or deselecting the element.
     */
    public void OnValueChanged(bool value) {
        manager.selectedAudioInfoElement = this;
    }
}
