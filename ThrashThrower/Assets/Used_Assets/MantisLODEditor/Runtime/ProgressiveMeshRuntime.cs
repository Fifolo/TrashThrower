/*--------------------------------------------------------
   ProgressiveMeshRuntime.cs

   Created by MINGFEN WANG on 13-12-26.
   Copyright (c) 2013 MINGFEN WANG. All rights reserved.
   http://www.mesh-online.net/
   --------------------------------------------------------*/
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;

namespace MantisLODEditor
{
    public class Lod_Mesh
    {
        public Mesh mesh;
        public int triangle_count;
    }
    public class ProgressiveMeshRuntime : MonoBehaviour
    {
        // Drag a reference or assign it with code
        public ProgressiveMesh progressiveMesh = null;

        // Optional fields
        public Text fpsHint = null;
        public Text lodHint = null;
        public Text triangleHint = null;

        [HideInInspector]
        public bool optimize_on_the_fly = true;
        // Clamp lod to [minLod, maxLod]
        [HideInInspector]
        public int[] mesh_lod_range = null;
        [HideInInspector]
        public bool never_cull = true;
        [HideInInspector]
        public int lod_strategy = 1;
        [HideInInspector]
        public float cull_ratio = 0.1f;
        [HideInInspector]
        public float disappear_distance = 250.0f;
        [HideInInspector]
        public float updateInterval = 0.25f;

        private Component[] allFilters = null;
        private Component[] allRenderers = null;
        private Mesh[] shared_meshes = null;
        private string[] mesh_uuids = null;
        private int current_lod = -1;

        private Component[] allBasicRenderers = null;

        // How often to check lod changes, default four times per second.
        // You may increase the value to balance the load if you have hundreds of 3d models in the scene.
        private float currentTimeToInterval = 0.0f;
        private bool culled = false;
        private bool working = false;

#if UNITY_EDITOR
        [MenuItem("Window/Mantis LOD Editor/Component/Runtime/Progressive Mesh Runtime")]
        public static void AddComponent()
        {
            GameObject SelectedObject = Selection.activeGameObject;
            if (SelectedObject)
            {
                // Register root object for undo.
                Undo.RegisterCreatedObjectUndo(SelectedObject.AddComponent(typeof(ProgressiveMeshRuntime)), "Add Progressive Mesh Runtime");
            }
        }
        [MenuItem("Window/Mantis LOD Editor/Component/Runtime/Progressive Mesh Runtime", true)]
        static bool ValidateAddComponent()
        {
            // Return false if no gameobject is selected.
            return Selection.activeGameObject != null;
        }
#endif
        void Awake()
        {
            get_all_meshes();
        }
        // Use this for initialization
        void Start()
        {
        }
        private float ratio_of_screen()
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (Component child in allBasicRenderers)
            {
                Renderer rend = (Renderer)child;
                Vector3 center = rend.bounds.center;
                float radius = rend.bounds.extents.magnitude;
                Vector3[] six_points = new Vector3[6];
                six_points[0] = Camera.main.WorldToScreenPoint(new Vector3(center.x - radius, center.y, center.z));
                six_points[1] = Camera.main.WorldToScreenPoint(new Vector3(center.x + radius, center.y, center.z));
                six_points[2] = Camera.main.WorldToScreenPoint(new Vector3(center.x, center.y - radius, center.z));
                six_points[3] = Camera.main.WorldToScreenPoint(new Vector3(center.x, center.y + radius, center.z));
                six_points[4] = Camera.main.WorldToScreenPoint(new Vector3(center.x, center.y, center.z - radius));
                six_points[5] = Camera.main.WorldToScreenPoint(new Vector3(center.x, center.y, center.z + radius));
                foreach (Vector3 v in six_points)
                {
                    if (v.x < min.x) min.x = v.x;
                    if (v.y < min.y) min.y = v.y;
                    if (v.x > max.x) max.x = v.x;
                    if (v.y > max.y) max.y = v.y;
                }
            }
            float ratio_width = (max.x - min.x) / Camera.main.pixelWidth;
            float ratio_height = (max.y - min.y) / Camera.main.pixelHeight;
            float ratio = (ratio_width > ratio_height) ? ratio_width : ratio_height;
            if (ratio > 1.0f) ratio = 1.0f;

            return ratio;
        }
        private float ratio_of_distance(float distance0)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (Component child in allBasicRenderers)
            {
                Renderer rend = (Renderer)child;
                Vector3 center = rend.bounds.center;
                float radius = rend.bounds.extents.magnitude;
                Vector3[] six_points = new Vector3[6];
                six_points[0] = new Vector3(center.x - radius, center.y, center.z);
                six_points[1] = new Vector3(center.x + radius, center.y, center.z);
                six_points[2] = new Vector3(center.x, center.y - radius, center.z);
                six_points[3] = new Vector3(center.x, center.y + radius, center.z);
                six_points[4] = new Vector3(center.x, center.y, center.z - radius);
                six_points[5] = new Vector3(center.x, center.y, center.z + radius);
                foreach (Vector3 v in six_points)
                {
                    if (v.x < min.x) min.x = v.x;
                    if (v.y < min.y) min.y = v.y;
                    if (v.z < min.z) min.z = v.z;
                    if (v.x > max.x) max.x = v.x;
                    if (v.y > max.y) max.y = v.y;
                    if (v.z > max.z) max.z = v.z;
                }
            }
            Vector3 average_position = (min + max) * 0.5f;
            float distance = Vector3.Distance(Camera.main.transform.position, average_position);
            float ratio = 1.0f - distance / distance0;
            if (ratio < 0.0f) ratio = 0.0f;

            return ratio;
        }
        // Update is called once per frame
        void Update()
        {
            if (progressiveMesh)
            {
                currentTimeToInterval -= Time.deltaTime;
                // time out
                if (currentTimeToInterval <= 0.0f)
                {
                    // detect if the game object is visible
                    bool visable = false;
                    if (!culled)
                    {
                        allBasicRenderers = (Component[])(gameObject.GetComponentsInChildren(typeof(Renderer)));
                        foreach (Component child in allBasicRenderers)
                        {
                            if (((Renderer)child).isVisible) visable = true;
                            break;
                        }
                    }
                    // we only change levels when the game object had been culled by ourselves or is visable
                    if (culled || visable)
                    {
                        float ratio = 0.0f;
                        // we only calculate ratio of screen when the main camera is active in hierarchy
                        if (Camera.main != null && Camera.main.gameObject != null && Camera.main.gameObject.activeInHierarchy)
                        {
                            allBasicRenderers = (Component[])(gameObject.GetComponentsInChildren(typeof(Renderer)));
                            if (lod_strategy == 0) ratio = ratio_of_screen();
                            if (lod_strategy == 1) ratio = ratio_of_distance(disappear_distance);
                        }
                        // you may change cull condition here
                        if (never_cull == false && ratio < cull_ratio)
                        {
                            // cull the game object
                            if (!culled)
                            {
                                // cull me
                                allBasicRenderers = (Component[])(gameObject.GetComponentsInChildren(typeof(Renderer)));
                                foreach (Component child in allBasicRenderers)
                                {
                                    ((Renderer)child).enabled = false;
                                }
                                culled = true;
                            }
                        }
                        else
                        {
                            // show the game object
                            if (culled)
                            {
                                // show me
                                allBasicRenderers = (Component[])(gameObject.GetComponentsInChildren(typeof(Renderer)));
                                foreach (Component child in allBasicRenderers)
                                {
                                    ((Renderer)child).enabled = true;
                                }
                                culled = false;
                            }
                            // get lod count
                            int max_lod_count = progressiveMesh.triangles[0];
                            // set triangle list according to current lod
                            int lod = (int)((1.0f - ratio) * max_lod_count);
                            // clamp the value
                            if (lod > max_lod_count - 1) lod = max_lod_count - 1;
                            // lod changed
                            if (current_lod != lod)
                            {
                                int total_triangles_count = 0;
                                int counter = 0;
                                foreach (Component child in allFilters)
                                {
                                    string uuid = mesh_uuids[counter];
                                    int mesh_index = Array.IndexOf(progressiveMesh.uuids, uuid);
                                    // the mesh is in the progressive mesh list
                                    if (mesh_index != -1)
                                    {
                                        int clamp_lod = lod;
                                        // clamp to valid range
                                        if (clamp_lod < mesh_lod_range[mesh_index * 2]) clamp_lod = mesh_lod_range[mesh_index * 2];
                                        if (clamp_lod > mesh_lod_range[mesh_index * 2 + 1]) clamp_lod = mesh_lod_range[mesh_index * 2 + 1];
                                        // because other instances may have terminated, we need to check if lod meshes is null.
                                        if (progressiveMesh.lod_meshes != null && progressiveMesh.lod_meshes.ContainsKey(uuid))
                                        {
                                            Lod_Mesh lod_mesh = ((Lod_Mesh[])progressiveMesh.lod_meshes[uuid])[clamp_lod];
                                            ((MeshFilter)child).sharedMesh = lod_mesh.mesh;
                                            total_triangles_count += lod_mesh.triangle_count;
                                        }
                                    }
                                    counter++;
                                }
                                foreach (Component child in allRenderers)
                                {
                                    string uuid = mesh_uuids[counter];
                                    int mesh_index = Array.IndexOf(progressiveMesh.uuids, uuid);
                                    // the mesh is in the progressive mesh list
                                    if (mesh_index != -1)
                                    {
                                        int clamp_lod = lod;
                                        // clamp to valid range
                                        if (clamp_lod < mesh_lod_range[mesh_index * 2]) clamp_lod = mesh_lod_range[mesh_index * 2];
                                        if (clamp_lod > mesh_lod_range[mesh_index * 2 + 1]) clamp_lod = mesh_lod_range[mesh_index * 2 + 1];
                                        // because other instances may have terminated, we need to check if lod meshes is null.
                                        if (progressiveMesh.lod_meshes != null && progressiveMesh.lod_meshes.ContainsKey(uuid))
                                        {
                                            Lod_Mesh lod_mesh = ((Lod_Mesh[])progressiveMesh.lod_meshes[uuid])[clamp_lod];
                                            ((SkinnedMeshRenderer)child).sharedMesh = lod_mesh.mesh;
                                            total_triangles_count += lod_mesh.triangle_count;
                                        }
                                    }
                                    counter++;
                                }
                                // update read only status
                                if (lodHint) lodHint.text = "Level Of Detail: " + lod.ToString();
                                if (triangleHint) triangleHint.text = "Triangle Count: " + (total_triangles_count / 3).ToString();
                                current_lod = lod;
                            }
                        }
                    }
                    if (fpsHint)
                    {
                        int fps = Mathf.RoundToInt(1.0f / Time.smoothDeltaTime);
                        fpsHint.text = "FPS: " + fps.ToString();
                    }
                    //reset timer
                    currentTimeToInterval = updateInterval + (UnityEngine.Random.value + 0.5f) * currentTimeToInterval;
                }
            }
        }
        public int get_triangles_count_from_progressive_mesh(int lod0, int mesh_count0)
        {
            int counter = 0;
            int triangle_count = 0;
            // max lod count
            int max_lod_count = progressiveMesh.triangles[triangle_count];
            triangle_count++;
            for (int lod = 0; lod < max_lod_count; lod++)
            {
                // max mesh count
                int max_mesh_count = progressiveMesh.triangles[triangle_count];
                triangle_count++;
                for (int mesh_count = 0; mesh_count < max_mesh_count; mesh_count++)
                {
                    // max sub mesh count
                    int max_sub_mesh_count = progressiveMesh.triangles[triangle_count];
                    triangle_count++;
                    for (int mat = 0; mat < max_sub_mesh_count; mat++)
                    {
                        // max triangle count
                        int max_triangle_count = progressiveMesh.triangles[triangle_count];
                        triangle_count++;
                        // here it is
                        if (lod == lod0 && mesh_count == mesh_count0)
                        {
                            counter += max_triangle_count;
                        }
                        // triangle list count
                        triangle_count += max_triangle_count;
                    }
                }
            }
            return counter / 3;
        }
        private int[] get_triangles_from_progressive_mesh(int lod0, int mesh_count0, int mat0)
        {
            int triangle_count = 0;
            // max lod count
            int max_lod_count = progressiveMesh.triangles[triangle_count];
            triangle_count++;
            for (int lod = 0; lod < max_lod_count; lod++)
            {
                // max mesh count
                int max_mesh_count = progressiveMesh.triangles[triangle_count];
                triangle_count++;
                for (int mesh_count = 0; mesh_count < max_mesh_count; mesh_count++)
                {
                    // max sub mesh count
                    int max_sub_mesh_count = progressiveMesh.triangles[triangle_count];
                    triangle_count++;
                    for (int mat = 0; mat < max_sub_mesh_count; mat++)
                    {
                        // max triangle count
                        int max_triangle_count = progressiveMesh.triangles[triangle_count];
                        triangle_count++;
                        // here it is
                        if (lod == lod0 && mesh_count == mesh_count0 && mat == mat0)
                        {
                            int[] new_triangles = new int[max_triangle_count];
                            Array.Copy(progressiveMesh.triangles, triangle_count, new_triangles, 0, max_triangle_count);
                            return new_triangles;
                        }
                        // triangle list count
                        triangle_count += max_triangle_count;
                    }
                }
            }
            return null;
        }
        private void set_triangles(Mesh mesh, string uuid, int lod)
        {
            int mesh_index = Array.IndexOf(progressiveMesh.uuids, uuid);
            // the mesh is in the progressive mesh list
            if (mesh_index != -1)
            {
                for (int mat = 0; mat < mesh.subMeshCount; mat++)
                {
                    int[] triangle_list = get_triangles_from_progressive_mesh(lod, mesh_index, mat);
                    mesh.SetTriangles(triangle_list, mat);
                }
            }
        }
        private struct BlendWeightData
        {
            public string[] names;
            public float[][] weights;
            public Vector3[][][] deltaVerts;
            public Vector3[][][] deltaNormals;
            public Vector3[][][] deltaTangents;
        }
        private BlendWeightData get_blend_weight_data(Mesh mesh)
        {
            if (mesh.blendShapeCount == 0)
            {
                return new BlendWeightData();
            }
            BlendWeightData blend_weight_data;
            blend_weight_data.names = new string[mesh.blendShapeCount];
            blend_weight_data.weights = new float[mesh.blendShapeCount][];
            blend_weight_data.deltaVerts = new Vector3[mesh.blendShapeCount][][];
            blend_weight_data.deltaNormals = new Vector3[mesh.blendShapeCount][][];
            blend_weight_data.deltaTangents = new Vector3[mesh.blendShapeCount][][];
            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                blend_weight_data.names[i] = mesh.GetBlendShapeName(i);
                blend_weight_data.weights[i] = new float[mesh.GetBlendShapeFrameCount(i)];
                blend_weight_data.deltaVerts[i] = new Vector3[mesh.GetBlendShapeFrameCount(i)][];
                blend_weight_data.deltaNormals[i] = new Vector3[mesh.GetBlendShapeFrameCount(i)][];
                blend_weight_data.deltaTangents[i] = new Vector3[mesh.GetBlendShapeFrameCount(i)][];
                for (int j = 0; j < mesh.GetBlendShapeFrameCount(i); j++)
                {
                    blend_weight_data.deltaVerts[i][j] = new Vector3[mesh.vertexCount];
                    blend_weight_data.deltaNormals[i][j] = new Vector3[mesh.vertexCount];
                    blend_weight_data.deltaTangents[i][j] = new Vector3[mesh.vertexCount];
                    blend_weight_data.weights[i][j] = mesh.GetBlendShapeFrameWeight(i, j);
                    mesh.GetBlendShapeFrameVertices(i, j, blend_weight_data.deltaVerts[i][j], blend_weight_data.deltaNormals[i][j], blend_weight_data.deltaTangents[i][j]);
                }
            }
            return blend_weight_data;
        }
        private string get_uuid_from_mesh(Mesh mesh)
        {
            string uuid = mesh.name + "_" + mesh.vertexCount.ToString() + "_" + mesh.subMeshCount.ToString();
            for (int i = 0; i < mesh.subMeshCount; ++i)
            {
                uuid += "_" + mesh.GetIndexCount(i).ToString();
            }
            return uuid;
        }
        private void create_default_mesh_lod_range()
        {
            int max_lod_count = progressiveMesh.triangles[0];
            int mesh_count = progressiveMesh.triangles[1];
            mesh_lod_range = new int[mesh_count * 2];
            for (int i = 0; i < mesh_count; i++)
            {
                mesh_lod_range[i * 2] = 0;
                mesh_lod_range[i * 2 + 1] = max_lod_count - 1;
            }
        }
        private void get_all_meshes()
        {
            if (!working)
            {
                int max_lod_count = progressiveMesh.triangles[0];
                if (mesh_lod_range == null || mesh_lod_range.Length == 0)
                {
                    create_default_mesh_lod_range();
                }
                if (allFilters == null && allRenderers == null)
                {
                    allFilters = (Component[])(gameObject.GetComponentsInChildren(typeof(MeshFilter)));
                    allRenderers = (Component[])(gameObject.GetComponentsInChildren(typeof(SkinnedMeshRenderer)));
                }
                int mesh_count = allFilters.Length + allRenderers.Length;
                if (mesh_count > 0)
                {
                    shared_meshes = new Mesh[mesh_count];
                    mesh_uuids = new string[mesh_count];
                    int counter = 0;
                    foreach (Component child in allFilters)
                    {
                        string uuid = get_uuid_from_mesh(((MeshFilter)child).sharedMesh);
                        mesh_uuids[counter] = uuid;
                        // store original shared mesh
                        shared_meshes[counter] = ((MeshFilter)child).sharedMesh;
                        // mesh lods map does not exist
                        if (progressiveMesh.lod_meshes == null)
                        {
                            progressiveMesh.lod_meshes = new Dictionary<string, Lod_Mesh[]>();
                        }
                        // create mesh lods if it does not exist
                        if (!progressiveMesh.lod_meshes.ContainsKey(uuid))
                        {
                            int mesh_index = Array.IndexOf(progressiveMesh.uuids, uuid);
                            if (mesh_index != -1)
                            {
                                Lod_Mesh[] lod_meshes = new Lod_Mesh[max_lod_count];
                                for (int lod = 0; lod < max_lod_count; lod++)
                                {
                                    lod_meshes[lod] = new Lod_Mesh();
                                    lod_meshes[lod].mesh = Instantiate(((MeshFilter)child).sharedMesh);
                                    set_triangles(lod_meshes[lod].mesh, uuid, lod);
                                    if (optimize_on_the_fly)
                                    {
#if UNITY_EDITOR
                                        MeshUtility.Optimize(lod_meshes[lod].mesh);
#elif UNITY_2019_1_OR_NEWER
                                        lod_meshes[lod].mesh.Optimize();
#endif
                                    }
                                    lod_meshes[lod].triangle_count = lod_meshes[lod].mesh.triangles.Length;
                                }
                                progressiveMesh.lod_meshes.Add(uuid, lod_meshes);
                            }
                        }
                        counter++;
                    }
                    foreach (Component child in allRenderers)
                    {
                        string uuid = get_uuid_from_mesh(((SkinnedMeshRenderer)child).sharedMesh);
                        mesh_uuids[counter] = uuid;
                        // store original shared mesh
                        shared_meshes[counter] = ((SkinnedMeshRenderer)child).sharedMesh;
                        // mesh lods map does not exist
                        if (progressiveMesh.lod_meshes == null)
                        {
                            progressiveMesh.lod_meshes = new Dictionary<string, Lod_Mesh[]>();
                        }
                        // create mesh lods if it does not exist
                        if (!progressiveMesh.lod_meshes.ContainsKey(uuid))
                        {
                            int mesh_index = Array.IndexOf(progressiveMesh.uuids, uuid);
                            if (mesh_index != -1)
                            {
                                Lod_Mesh[] lod_meshes = new Lod_Mesh[max_lod_count];
                                for (int lod = 0; lod < max_lod_count; lod++)
                                {
                                    lod_meshes[lod] = new Lod_Mesh();
                                    lod_meshes[lod].mesh = Instantiate(((SkinnedMeshRenderer)child).sharedMesh);
                                    set_triangles(lod_meshes[lod].mesh, uuid, lod);
                                    if (optimize_on_the_fly)
                                    {
#if UNITY_EDITOR
                                        MeshUtility.Optimize(lod_meshes[lod].mesh);
#elif UNITY_2019_1_OR_NEWER
                                        lod_meshes[lod].mesh.Optimize();
#endif
                                    }
                                    lod_meshes[lod].triangle_count = lod_meshes[lod].mesh.triangles.Length;
                                }
                                progressiveMesh.lod_meshes.Add(uuid, lod_meshes);
                            }
                        }
                        counter++;
                    }
                }

                // get all renderers
                allBasicRenderers = (Component[])(gameObject.GetComponentsInChildren(typeof(Renderer)));
                // We use random value to spread the update moment in range [0, updateInterval]
                currentTimeToInterval = UnityEngine.Random.value * updateInterval;

                // init current lod
                current_lod = -1;

                working = true;
            }
        }
        public void reset_all_parameters()
        {
            optimize_on_the_fly = true;
            mesh_lod_range = null;
            never_cull = true;
            lod_strategy = 1;
            cull_ratio = 0.1f;
            disappear_distance = 250.0f;
            updateInterval = 0.25f;
        }
        private void clean_all()
        {
            if (working)
            {
                int mesh_count = allFilters.Length + allRenderers.Length;
                if (mesh_count > 0)
                {
                    int counter = 0;
                    foreach (Component child in allFilters)
                    {
                        // restore original shared mesh
                        ((MeshFilter)child).sharedMesh = shared_meshes[counter];
                        counter++;
                    }
                    foreach (Component child in allRenderers)
                    {
                        // restore original shared mesh
                        ((SkinnedMeshRenderer)child).sharedMesh = shared_meshes[counter];
                        counter++;
                    }
                }
                shared_meshes = null;
                allBasicRenderers = null;
                allFilters = null;
                allRenderers = null;
                progressiveMesh.lod_meshes = null;

                working = false;
            }
        }
        void OnEnable()
        {
            Awake();
            Start();
        }
        void OnDisable()
        {
            clean_all();
        }
        void OnDestroy()
        {
            clean_all();
        }
    }
}
