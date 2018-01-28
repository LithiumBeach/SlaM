using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using patterns;

public class SoundManager : SingletonBehavior<SoundManager> {

    public AudioSource DynamicSource;
    public AudioClip[] AudioClips;
    public enum AudioClipKeys{
        Hover,
        Select,
        Deselect,
        Correct
    }

    public void PlayAudio(int clipKey) {
        if (clipKey >= AudioClips.Length)
        {
            Debug.LogError("Requested audio key is not within the clip list!");
            return;
        } else if (!DynamicSource)
        {
            Debug.LogError("SoundManager does not have a dynamic audio source!");
            return;
        }

        DynamicSource.Stop();
        Debug.Log(string.Format("Playing clip {0}", (AudioClipKeys)clipKey));
        DynamicSource.clip = AudioClips[clipKey];
        DynamicSource.Play();
        
    }
}
