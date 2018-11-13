using UnityEngine;
using UnityEngine.UI;

/**
 * A script attached to audio cue elements in the audio cue list.
 * Controls the play button.
 */
public class AudioInfoElementV2 : MonoBehaviour {

    public Sprite playSprite;
    public Sprite pauseSprite;

    private Image buttonImage;
    private bool playing = false;

    private void Start() {
        buttonImage = transform.Find("PlayButton").GetComponent<Image>();
        buttonImage.sprite = playSprite;
    }

    public void SetId(string id) {
        transform.Find("AudioIdLabel").GetComponent<Text>().text = id;
    }

    public void ToggleSprite() {
        playing = !playing;
        if (playing) {
            buttonImage.sprite = pauseSprite;
        } else {
            buttonImage.sprite = playSprite;
        }
    }
}
