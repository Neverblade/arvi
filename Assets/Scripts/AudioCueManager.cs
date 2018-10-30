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
    public List<AudioCueInfo> audioCueInfoList = new List<AudioCueInfo>();
    public List<GameObject> audioCueObjList = new List<GameObject>();

    private Vector3 storedPosition;

    private void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                if (EventSystem.current.currentSelectedGameObject == null) {
                    Transform camTransform = Camera.main.transform;
                    storedPosition = camTransform.position + camTransform.forward * DROP_DISTANCE_FROM_CAMERA;
                    AddAudioCue(storedPosition);
                }
            }
        }
    }

    public void OnSimulatorDropAudioCue() {
        Transform camTransform = Camera.main.transform;
        storedPosition = camTransform.position + camTransform.forward * DROP_DISTANCE_FROM_CAMERA;

        AddAudioCue(storedPosition);
    }

    public void AddAudioCue(Vector3 position) {
        AudioCueInfo audioCueInfo = new AudioCueInfo();
        audioCueInfo.id = "sample"; // HARD-CODED
        audioCueInfo.position = position;
        audioCueInfoList.Add(audioCueInfo);

        GameObject audioCue = AudioCueFromInfo(audioCueInfo);
        audioCueObjList.Add(audioCue);
    }

    public GameObject AudioCueFromInfo(AudioCueInfo info) {
        return Instantiate(audioCuePrefab, info.position, Quaternion.identity);
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
                GameObject audioCue = AudioCueFromInfo(audioCueInfo);
                audioCueObjList.Add(audioCue);
            }
        }
    }
}