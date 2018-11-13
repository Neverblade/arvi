using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Audio {
    public string id;
    public AudioClip clip;
}

/**
 * Stores audio clips along with their associated metadata.
 * Provides methods of retrieval.
 */
public class AudioLibrary : MonoBehaviour {

    public static AudioLibrary instance;

    public Audio[] library;

    private void Start() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(this);
        }
    }

    /**
     * Given an id, return the associated audio clip.
     * Return null if not found.
     */
    public AudioClip GetAudioClip(string id) {
        foreach (Audio audio in library) {
            if (id.Equals(audio.id)) {
                return audio.clip;
            }
        }

        return null;
    }
}
