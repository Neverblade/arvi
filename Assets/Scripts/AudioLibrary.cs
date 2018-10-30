using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Stores audio clips along with their associated metadata.
 * Provides methods of retrieval.
 */
public class AudioLibrary : MonoBehaviour {

    [System.Serializable]
    public class Audio {
        public string id;
        public AudioClip clip;
    }

    public Audio[] library;

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
