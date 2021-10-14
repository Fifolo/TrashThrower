/*--------------------------------------------------------
   ProgressiveMesh.cs

   Created by MINGFEN WANG on 13-12-26.
   Copyright (c) 2013 MINGFEN WANG. All rights reserved.
   http://www.mesh-online.net/
   --------------------------------------------------------*/
using System.Collections.Generic;
using UnityEngine;

namespace MantisLODEditor
{
    [System.Serializable]
    public class ProgressiveMesh : ScriptableObject
    {
        public int[] triangles;
        public string[] uuids;
        public Dictionary<string, Lod_Mesh[]> lod_meshes;
    }
}
