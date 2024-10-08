using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicManager))]
public class MusicManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        MusicManager myScript = (MusicManager)target;

        // SHow the current music type
        EditorGUILayout.LabelField("Current Music Type", myScript.currentMusicType.ToString());

        // Add button to change music type
        if (GUILayout.Button("Change Music Type"))
        {
            // Check game is playing
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Game must be in play mode.");
                return;
            }
            myScript.currentMusicType = (MusicType)Random.Range(0, System.Enum.GetValues(typeof(MusicType)).Length);
        }
    }
}
