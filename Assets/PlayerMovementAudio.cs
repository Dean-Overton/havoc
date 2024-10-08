using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAudio : MonoBehaviour
{
    public string footstepSoundName = "Footstep ";
    public float footstepSoundDelay = 0.5f;

    public void PlayFootstepSound()
    {
        if (isPlayingFootstepSound)
            return;
        
        isPlayingFootstepSound = true;
        StartCoroutine(FootstepSoundLoop());
    }
    public bool isPlayingFootstepSound = false;
    IEnumerator FootstepSoundLoop()
    {
        // Play footstep sound
        AudioManager.instance.Play(footstepSoundName);

        // Wait for the footstep sound to finish
        yield return new WaitForSeconds(footstepSoundDelay);
        
        isPlayingFootstepSound = false;
    }
}
