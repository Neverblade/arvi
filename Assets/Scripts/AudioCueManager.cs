using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

[System.Serializable]
public class AudioCueInfo {
    public string id;
    public Vector3 position;
}

[System.Serializable]
public class AudioCueList {
    public AudioCueInfo[] audioCues;
}

public class AudioCueManager : MonoBehaviour {

    public static float DROP_DISTANCE_FROM_CAMERA = 0;
    public static string AUDIO_CUE_LIST_NAME = "audioCueList";

    public GameObject audioCuePrefab;
    public GameObject audioInfoElementPrefab;
    public GameObject audioListPanel;
    public RectTransform audioListContentPanel;

    [HideInInspector] public List<AudioCueInfo> audioCueInfoList = new List<AudioCueInfo>();
    [HideInInspector] public List<GameObject> audioCueObjList = new List<GameObject>();
    [HideInInspector] public AudioInfoElement selectedAudioInfoElement;
    [HideInInspector] public Vector3 storedPosition;

    private AudioSource audioSource;
    private AudioLibrary audioLibrary;
    private AudioInfoElement playingAudioInfoElement;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        audioLibrary = GetComponent<AudioLibrary>();
        audioListPanel.SetActive(false);
        PopulateAudioList();
    }

    private void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                if (EventSystem.current.currentSelectedGameObject == null) {
                    Transform camTransform = Camera.main.transform;
                    storedPosition = camTransform.position + camTransform.forward * DROP_DISTANCE_FROM_CAMERA;
                    audioListPanel.SetActive(true);
                }
            }
        }
    }

    #region Audio Cues in the Scene

    public void AddAudioCue(Vector3 position, AudioInfoElement audioInfo) {
        AudioCueInfo audioCueInfo = new AudioCueInfo();
        audioCueInfo.id = audioInfo.id;
        audioCueInfo.position = position;
        audioCueInfoList.Add(audioCueInfo);

        GameObject audioCue = CreateAudioCueFromInfo(audioCueInfo);
        audioCueObjList.Add(audioCue);
    }

    public GameObject CreateAudioCueFromInfo(AudioCueInfo info) {
        GameObject audioCueObj = Instantiate(audioCuePrefab, info.position, Quaternion.identity);
        audioCueObj.GetComponent<AudioSource>().clip = audioLibrary.GetAudioClip(info.id);
        audioCueObj.GetComponent<AudioSource>().Play();
        return audioCueObj;
    }

    public void ClearAudioCues() {
        foreach (GameObject obj in audioCueObjList) {
            Destroy(obj);
        }
        audioCueObjList.Clear();
        audioCueInfoList.Clear();
    }

    public JObject AudioCuesToJSON() {
        AudioCueList audioCueList = new AudioCueList();
        audioCueList.audioCues = new AudioCueInfo[audioCueInfoList.Count];
        for (int i = 0; i < audioCueInfoList.Count; i++) {
            audioCueList.audioCues[i] = audioCueInfoList[i];
        }

        return JObject.FromObject(audioCueList);
    }

    public void LoadAudioCuesJSON(JToken mapMetadata) {
        ClearAudioCues();
        if (mapMetadata is JObject && mapMetadata[AUDIO_CUE_LIST_NAME] is JObject) {
            AudioCueList audioCueList = mapMetadata[AUDIO_CUE_LIST_NAME].ToObject<AudioCueList>();
            if (audioCueList.audioCues == null) {
                Debug.Log("No audio cues dropped.");
                return;
            }

            foreach (AudioCueInfo audioCueInfo in audioCueList.audioCues) {
                audioCueInfoList.Add(audioCueInfo);
                GameObject audioCue = CreateAudioCueFromInfo(audioCueInfo);
                audioCueObjList.Add(audioCue);
            }
        }
    }

    #endregion

    #region Audio Cue List UI

    /**
     * Fills a panel with AudioCueElements with info from the library.
     */
    public void PopulateAudioList() {
        foreach (AudioLibrary.Audio audio in audioLibrary.library) {
            GameObject element = Instantiate(audioInfoElementPrefab);
            AudioInfoElement audioInfoElement = element.GetComponent<AudioInfoElement>();
            audioInfoElement.id = audio.id;
            audioInfoElement.clip = audio.clip;
            audioInfoElement.manager = this;
            element.transform.SetParent(audioListContentPanel.transform);
        }
    }

    /**
     * Plays the given audio clip in the UI.
     */
    public void PlayAudioClip(AudioInfoElement element) {
        StopAudioClip();
        if (element.clip != null) {
            audioSource.clip = element.clip;
            audioSource.Play();
            playingAudioInfoElement = element;
        } else {
            Debug.LogWarning("Provided clip was null.");
        }
    }

    /**
     * Stops playing whatever clip is running in the UI.
     */
    public void StopAudioClip() {
        audioSource.Stop();
        audioSource.clip = null;
        if (playingAudioInfoElement != null) {
            AudioInfoElement element = playingAudioInfoElement;
            playingAudioInfoElement = null;
            element.TurnOff();
        }
    }

    #endregion
}