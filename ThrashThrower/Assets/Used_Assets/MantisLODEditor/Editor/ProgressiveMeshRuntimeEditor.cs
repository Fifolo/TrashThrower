using UnityEditor;
using UnityEngine;

namespace MantisLODEditor
{
    [CustomEditor(typeof(ProgressiveMeshRuntime))]
    public class ProgressiveMeshRuntimeEditor : Editor
    {
        private ProgressiveMesh save_progressiveMesh = null;
        private bool show_advanced_options = true;

        override public void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var runtime = target as ProgressiveMeshRuntime;
            if (runtime)
            {
                // when the property changed
                if (runtime.progressiveMesh != save_progressiveMesh)
                {
                    // clear the property
                    if (runtime.progressiveMesh == null)
                    {
                        runtime.reset_all_parameters();
                    }
                    else
                    {
                        // diffent property or mesh lod range not exists
                        if (save_progressiveMesh != null || runtime.mesh_lod_range == null || runtime.mesh_lod_range.Length == 0)
                        {
                            runtime.reset_all_parameters();
                            int max_lod_count = runtime.progressiveMesh.triangles[0];
                            int mesh_count = runtime.progressiveMesh.triangles[1];
                            runtime.mesh_lod_range = new int[mesh_count * 2];
                            for (int i = 0; i < mesh_count; i++)
                            {
                                runtime.mesh_lod_range[i * 2] = 0;
                                runtime.mesh_lod_range[i * 2 + 1] = max_lod_count - 1;
                            }
                        }
                    }
                    save_progressiveMesh = runtime.progressiveMesh;
                }
                // show advanced options
                show_advanced_options = EditorGUILayout.Foldout(show_advanced_options, show_advanced_options ? "Hide Advanced Options" : "Show Advanced Options");
                if (show_advanced_options)
                {
                    GUIStyle helpStyle = new GUIStyle(GUI.skin.box);
                    helpStyle.wordWrap = true;
                    helpStyle.alignment = TextAnchor.UpperLeft;
                    helpStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
                    GUILayout.Label(
                        "If the gameObject is an instance of a prefab, you must drag the gameObject from the hierarchy window onto the source prefab to save the changes when finished editing. Otherwise, all the changes will lose after reloading the scene!"
                        , helpStyle
                        , GUILayout.ExpandWidth(true));
                    runtime.optimize_on_the_fly = EditorGUILayout.Toggle("Optimize On The Fly", runtime.optimize_on_the_fly);
                    GUILayout.Label(
                        "You should always turn on this option in most cases, but when running in editor XR (Mock HMD Loader), the character may blink when switching LODs when this option is enabled. I don't know if it is a Unity bug or it also happens on real XR devices. If this happens, please turn off this option."
                        , helpStyle
                        , GUILayout.ExpandWidth(true));
                    runtime.updateInterval = EditorGUILayout.FloatField("Update Interval", runtime.updateInterval);
                    GUILayout.Label(
                        "How often to check LOD changes."
                        , helpStyle
                        , GUILayout.ExpandWidth(true));
                    // clamp to valid range
                    if (runtime.updateInterval < 0.1f) runtime.updateInterval = 0.1f;
                    if (runtime.updateInterval > 5.0f) runtime.updateInterval = 5.0f;
                    EditorGUILayout.Space();
                    runtime.never_cull = EditorGUILayout.Toggle("Never Cull", runtime.never_cull);
                    GUILayout.Label(
                        "The gameObject is alway visible or be culled when far away."
                        , helpStyle
                        , GUILayout.ExpandWidth(true));
                    runtime.cull_ratio = EditorGUILayout.FloatField("Cull Ratio", runtime.cull_ratio);
                    if (!runtime.never_cull)
                    {
                        GUILayout.Label(
                            "How far away will the gameObject be culled."
                            , helpStyle
                            , GUILayout.ExpandWidth(true));
                    }
                    // clamp to valid range
                    if (runtime.cull_ratio < 0.0f) runtime.cull_ratio = 0.0f;
                    if (runtime.cull_ratio > 1.0f) runtime.cull_ratio = 1.0f;
                    string[] options = new string[] { "Cull By Size", "Cull By Distance" };
                    runtime.lod_strategy = GUILayout.SelectionGrid(runtime.lod_strategy, options, 1, EditorStyles.radioButton);
                    if (runtime.lod_strategy == 1)
                    {
                        runtime.disappear_distance = EditorGUILayout.FloatField("Disappear Distance", runtime.disappear_distance);
                        GUILayout.Label(
                            "How far away will the gameObject look like a tiny point."
                            , helpStyle
                            , GUILayout.ExpandWidth(true));
                        // clamp to valid range
                        if (runtime.disappear_distance < 0.0f) runtime.disappear_distance = 0.0f;
                    }
                    // mesh lod range exists
                    if (runtime.mesh_lod_range != null && runtime.mesh_lod_range.Length != 0)
                    {
                        for (int i = 0; i < runtime.mesh_lod_range.Length; i++)
                        {
                            if (i % 2 == 0)
                            {
                                EditorGUILayout.Space();
                                // mesh name
                                EditorGUILayout.LabelField("Mesh: " + runtime.progressiveMesh.uuids[i / 2]);
                                // min lod
                                runtime.mesh_lod_range[i] = EditorGUILayout.IntField("Min LOD", runtime.mesh_lod_range[i]);
                                EditorGUILayout.LabelField("Triangles Count: " + runtime.get_triangles_count_from_progressive_mesh(runtime.mesh_lod_range[i], i / 2).ToString());
                                GUILayout.Label(
                                    "Adjust to avoid too much detailed."
                                    , helpStyle
                                    , GUILayout.ExpandWidth(true));
                            }
                            else
                            {
                                // max lod
                                runtime.mesh_lod_range[i] = EditorGUILayout.IntField("Max LOD", runtime.mesh_lod_range[i]);
                                EditorGUILayout.LabelField("Triangles Count: " + runtime.get_triangles_count_from_progressive_mesh(runtime.mesh_lod_range[i], i / 2).ToString());
                                GUILayout.Label(
                                    "Adjust to avoid too much simplified."
                                    , helpStyle
                                    , GUILayout.ExpandWidth(true));
                            }
                            // clamp to valid range
                            int max_lod_count = runtime.progressiveMesh.triangles[0];
                            if (runtime.mesh_lod_range[i] < 0) runtime.mesh_lod_range[i] = 0;
                            if (runtime.mesh_lod_range[i] > max_lod_count - 1) runtime.mesh_lod_range[i] = max_lod_count - 1;
                        }
                    }
                }
            }
        }
    }
}
