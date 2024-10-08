using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    string soundToPlayName = "";
    string soundToStopName = "";
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get a reference to the target object (ResettableObject)
        AudioManager myScript = (AudioManager)target;

        EditorGUILayout.BeginHorizontal();
        soundToPlayName = EditorGUILayout.TextField(soundToPlayName);
        // Add button with string input field next to it
        if (GUILayout.Button("Play", GUILayout.Width(100)))
        {
            if (soundToPlayName == "")
            {
                Debug.LogWarning("Please enter a sound name to play.");
                return;
            }

            // Check game is playing
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Game must be in play mode.");
                return;
            }

            myScript.Play(soundToPlayName);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        soundToStopName = EditorGUILayout.TextField(soundToStopName);
        // Add button with string input field next to it
        if (GUILayout.Button("Stop", GUILayout.Width(100)))
        {
            if (soundToPlayName == "")
            {
                Debug.LogWarning("Please enter a sound name to stop.");
                return;
            }

            // Check game is playing
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Game must be in play mode.");
                return;
            }

            myScript.StopPlaying(soundToPlayName);
        }
        EditorGUILayout.EndHorizontal();
    }
}
