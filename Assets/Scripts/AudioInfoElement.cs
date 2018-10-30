using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioInfoElement : MonoBehaviour {

    public Sprite playSprite;
    public Sprite pauseSprite;

    private Image buttonImage;
    private bool playing = false;

    private void Start() {
        buttonImage = transform.Find("PlayButton").GetComponent<Image>();
    }

    /**
     * Play/pause button clicked.
     */
    public void OnClick() {
        playing = !playing;
        if (playing) {
            buttonImage.sprite = playSprite;
        } else {
            buttonImage.sprite = pauseSprite;
        }
    }
}
