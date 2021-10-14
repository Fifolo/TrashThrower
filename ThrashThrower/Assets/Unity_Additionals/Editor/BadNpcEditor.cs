using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BadNpcData))]
public class BadNpcEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BadNpcData badNpcData = target as BadNpcData;
        badNpcData.dropsTrash = GUILayout.Toggle(badNpcData.dropsTrash, "DropsTrash");

        if (badNpcData.dropsTrash)
        {
            badNpcData.dropRate = EditorGUILayout.Slider("Drop rate",badNpcData.dropRate, 20f, 0.5f);
            badNpcData.dropChance = EditorGUILayout.Slider("Drop chance", badNpcData.dropChance, 0f, 1f);
            badNpcData.initialWaitTime = EditorGUILayout.Slider("Initial wait time", badNpcData.initialWaitTime, 1f, 5f);
        }
    }
}
