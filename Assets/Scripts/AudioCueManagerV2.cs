using System.Collections.Generic;
using UnityEngine;

public class AudioCueManagerV2 : MonoBehaviour {

    public static AudioCueManagerV2 instance;

    public GameObject audioCuePrefab;

    private GameObject candidateAudioCue;
    private List<AudioCueInfo> audioCueInfoList = new List<AudioCueInfo>();
    private List<GameObject> audioCueObjList = new List<GameObject>();
    private Audio storedAudio;

    private void Start() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(this);
        }
    }

    #region Candidate Audio Cue

    /**
     * Places a candidate audio cue at the given position.
     * Candidate audio cues are not saved in the metadata,
     * and don't automatically play clips.
     */
    public void PlaceCandidateAudioCue(Vector3 position) {
        candidateAudioCue = Instantiate(audioCuePrefab, position, Quaternion.identity);
        AudioSource audioSource = candidateAudioCue.GetComponent<AudioSource>();
        audioSource.Stop();
    }

    /**
     * Changes the audio clip for the candidate audio cue.
     * Must be called at least once before the audio cue is confirmed.
     */
    public void ChangeCandidateAudioCueClip(Audio audio) {
        AudioSource audioSource = candidateAudioCue.GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = audio.clip;
        storedAudio = audio;
    }

    /**
     * Starts playing audio for the candidate audio cue.
     */
    public void PlayCandidateAudioCueSound() {
        AudioSource audioSource = candidateAudioCue.GetComponent<AudioSource>();
        audioSource.Play();
    }

    /**
     * Stops playing audio for the candidate audio cue.
     */
    public void PauseCandidateAudioCueSound() {
        AudioSource audioSource = candidateAudioCue.GetComponent<AudioSource>();
        audioSource.Stop();
    }

    /**
     * Adds the candidate audio cue to the map's metadata.
     * 
     */
    public void ConfirmCandidateAudioCue() {
        AudioCueInfo audioCueInfo = new AudioCueInfo();
        audioCueInfo.id = storedAudio.id;
        audioCueInfo.position = candidateAudioCue.transform.position;

        audioCueInfoList.Add(audioCueInfo);
        audioCueObjList.Add(candidateAudioCue);

        storedAudio = null;
        candidateAudioCue = null;
    }

    /**
     * Removes the candidate audio cue from the scene.
     * Happens when the user cancels the add.
     */
    public void RemoveCandidateAudioCue() {
        Destroy(candidateAudioCue);
        storedAudio = null;
        candidateAudioCue = null;
    }

    #endregion
}
