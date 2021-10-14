using System;
using System.Collections.Generic;
using UnityEngine;

namespace MantisLOD
{
    class My_Half_edge : IComparable
    {
        public bool alive;
        public int pqIndex;
        public My_Half_vertex vertex;
        public int index;
        public My_Half_face face;
        public My_Half_edge next;
        public float cost;
        public My_Half_edge()
        {
            alive = true;
        }
        public int CompareTo(object obj)
        {
            return cost.CompareTo((obj as My_Half_edge).cost);
        }
    }

    class My_Half_vertex
    {
        public bool alive;
        public bool on_boundary;
        public bool on_symmetry;
        public Vector3 position = new Vector3();
        public List<My_Half_edge> edges = new List<My_Half_edge>();
        public My_Half_vertex()
        {
            alive = true;
            on_boundary = false;
            on_symmetry = false;
        }
    }

    class My_Half_face
    {
        public bool alive;
        public int mat;
        public My_Half_edge edge;
        public Vector3 n = new Vector3();
        public My_Half_face()
        {
            alive = true;
        }
    }

    class My_Half_edge_index
    {
        public My_Half_edge edge;
        public int index_from;
        public int index_to;
    }

    class My_Half_trace
    {
        public bool safe;
        public My_Half_vertex v_from;
        public My_Half_vertex v_to;
        public List<My_Half_face> erased_faces = new List<My_Half_face>();
        public List<My_Half_edge_index> updated_edge_indices = new List<My_Half_edge_index>();
        public My_Half_trace()
        {
            safe = true;
        }
    }

    abstract class BinaryHeap
    {
        public BinaryHeap()
        {
            collection = new List<My_Half_edge>();
            collection.Add(new My_Half_edge());
        }
        public BinaryHeap(int capacity)
        {
            collection = new List<My_Half_edge>(capacity);
            collection.Add(new My_Half_edge());
        }

        public void Push(My_Half_edge item)
        {
            collection.Add(item);
            item.pqIndex = LastNodeIndex;
            BubbleUp(LastNodeIndex);
        }

        public My_Half_edge Pop()
        {
            if (LastNodeIndex == 0)
                return null;

            My_Half_edge result = collection[rootIndex];
            collection[rootIndex].pqIndex = -1;
            collection[rootIndex] = collection[LastNodeIndex];
            collection[rootIndex].pqIndex = rootIndex;
            BubbleDown(rootIndex);
            collection.RemoveAt(LastNodeIndex);

            return result;
        }

        public bool Remove(int index)
        {
            if (LastNodeIndex == 0)
                return false;

            collection[index].pqIndex = -1;
            collection[index] = collection[LastNodeIndex];
            collection[index].pqIndex = index;
            BubbleDown(index);
            collection.RemoveAt(LastNodeIndex);

            return true;
        }

        public int Size()
        {
            return collection.Count - 1;
        }

        public My_Half_edge Top()
        {
            if (LastNodeIndex == 0)
                return null;

            return collection[rootIndex];
        }

        private const int rootIndex = 1;

        private readonly List<My_Half_edge> collection;

        private int LastNodeIndex
        {
            get
            {
                return collection.Count - 1;
            }
        }

        protected abstract bool Compare(My_Half_edge current, My_Half_edge other);

        private void BubbleUp(int index)
        {
            int currentIndex = index;
            int parentIndex = index / 2;
            My_Half_edge current = collection[currentIndex];

            while (currentIndex > rootIndex)
            {
                if (Compare(collection[parentIndex], current))
                {
                    collection[currentIndex] = collection[parentIndex];
                    collection[currentIndex].pqIndex = currentIndex;
                    currentIndex = parentIndex;
                    parentIndex /= 2;
                }
                else
                {
                    break;
                }
            }
            collection[currentIndex] = current;
            collection[currentIndex].pqIndex = currentIndex;
        }

        private void BubbleDown(int index)
        {
            int currentIndex = index;
            int leftChildIndex = index * 2;
            My_Half_edge current = collection[currentIndex];

            while (leftChildIndex <= LastNodeIndex)
            {
                if (leftChildIndex < LastNodeIndex && Compare(collection[leftChildIndex], collection[leftChildIndex + 1]))
                {
                    leftChildIndex++;
                }

                if (Compare(current, collection[leftChildIndex]))
                {
                    collection[currentIndex] = collection[leftChildIndex];
                    collection[currentIndex].pqIndex = currentIndex;
                    currentIndex = leftChildIndex;
                    leftChildIndex *= 2;
                }
                else
                {
                    break;
                }
            }
            collection[currentIndex] = current;
            collection[currentIndex].pqIndex = currentIndex;
        }
    }

    class MinHeap : BinaryHeap
    {
        public MinHeap() { }
        public MinHeap(int capacity) : base(capacity) { }

        protected override bool Compare(My_Half_edge current, My_Half_edge other)
        {
            return other.CompareTo(current) < 0;
        }
    }

    class Vector3Comparer : IEqualityComparer<Vector3>
    {
        public bool Equals(Vector3 vec1, Vector3 vec2)
        {
            const float epsilon = 1e-7f;
            return vec1.x + epsilon >= vec2.x && vec1.x <= vec2.x + epsilon && vec1.y + epsilon >= vec2.y && vec1.y <= vec2.y + epsilon && vec1.z + epsilon >= vec2.z && vec1.z <= vec2.z + epsilon;
        }

        public int GetHashCode(Vector3 vec)
        {
            return vec.x.GetHashCode() ^ vec.y.GetHashCode() ^ vec.z.GetHashCode();
        }
    }

    class Progressive_Mesh
    {
        public Progressive_Mesh()
        {
            lock_boundary = true;
            lock_detail = false;
            lock_symmetry = false;
            lock_normal = false;
            lock_shape = false;
            face_count = 0;
        }
        public int get_trace_num()
        {
            return contract_trace.Count;
        }
        public void create_progressive_mesh(Vector3[] vertex_array, int vertex_count, int[] triangle_array, int triangle_count, Vector3[] normal_array, int normal_count, Color[] color_array, int color_count, Vector2[] uv_array, int uv_count, int protect_boundary, int protect_detail, int protect_symmetry, int protect_normal, int protect_shape, int use_detail_map, int detail_boost)
        {
            if (!(contract_trace.Count == 0))
                return;

            lock_boundary = protect_boundary == 1;
            lock_detail = protect_detail == 1;
            lock_symmetry = protect_symmetry == 1;
            lock_normal = protect_normal == 1;
            lock_shape = protect_shape == 1;
            this.use_detail_map = use_detail_map == 1;
            this.detail_boost = detail_boost;
            load_mesh_from_array(vertex_array, vertex_count, triangle_array, triangle_count, normal_array, normal_count, color_array, color_count, uv_array, uv_count);
            calculate_cost_of_edges();
            contract_edges();
            trace_to(0);
        }
        public void get_triangle_list(int goal, int[] triangle_array, ref int triangle_count)
        {
            if (contract_trace.Count == 0)
            {
                triangle_count = 0;
                return;
            }

            goal = Math.Max(Math.Min(goal, contract_trace.Count), 0);
            goal = trace_to(goal);

            int counter = contract_trace.Count;
            List<List<int>> sub_triangles = new List<List<int>>(mat_count);
            for (int i = 0; i < mat_count; i++)
            {
                sub_triangles.Add(new List<int>());
            }
            for (int i = contract_trace.Count - 1; i >= 0; i--)
            {
                if (counter == goal)
                    break;

                foreach (My_Half_face face in contract_trace[i].erased_faces)
                {
                    sub_triangles[face.edge.face.mat].Add(face.edge.index);
                    sub_triangles[face.edge.next.face.mat].Add(face.edge.next.index);
                    sub_triangles[face.edge.next.next.face.mat].Add(face.edge.next.next.index);
                }
                counter--;
            }
            counter = 0;
            for (int i = 0; i < mat_count; i++)
            {
                int len = sub_triangles[i].Count;
                triangle_array[counter] = len;
                counter++;
                if (len > 0)
                {
                    sub_triangles[i].CopyTo(triangle_array, counter);
                    counter += len;
                }
            }
            triangle_count = counter;
        }
        private void load_mesh_from_array(Vector3[] vertex_array, int vertex_count, int[] triangle_array, int triangle_count, Vector3[] normal_array, int normal_count, Color[] color_array, int color_count, Vector2[] uv_array, int uv_count)
        {
            Dictionary<Vector3, int> position_unique_vertices = new Dictionary<Vector3, int>(new Vector3Comparer());
            List<int> imap = new List<int>();
            for (int i = 0; i < vertex_count; i++)
            {
                if (!position_unique_vertices.ContainsKey(vertex_array[i]))
                {
                    My_Half_vertex v = new My_Half_vertex
                    {
                        position = vertex_array[i]
                    };

                    if (v.position.x > MAX.x)
                        MAX.x = v.position.x;
                    if (v.position.y > MAX.y)
                        MAX.y = v.position.y;
                    if (v.position.z > MAX.z)
                        MAX.z = v.position.z;
                    if (v.position.x < MIN.x)
                        MIN.x = v.position.x;
                    if (v.position.y < MIN.y)
                        MIN.y = v.position.y;
                    if (v.position.z < MIN.z)
                        MIN.z = v.position.z;

                    int index = vertices.Count;
                    position_unique_vertices.Add(vertex_array[i], index);
                    imap.Add(index);

                    vertices.Add(v);
                }
                else
                {
                    imap.Add(position_unique_vertices[vertex_array[i]]);
                }
            }
            int mat = 0;
            int counter = 0;
            while (counter < triangle_count)
            {
                int len = triangle_array[counter];
                counter++;
                for (int i = 0; i < len; i += 3)
                {
                    int index0 = imap[triangle_array[counter + i + 0]];
                    int index1 = imap[triangle_array[counter + i + 1]];
                    int index2 = imap[triangle_array[counter + i + 2]];
                    if (index0 == index1 || index1 == index2 || index2 == index0)
                    {
                        continue;
                    }

                    My_Half_face f = new My_Half_face();

                    My_Half_edge[] three_edges = new My_Half_edge[3];
                    three_edges[0] = new My_Half_edge();
                    three_edges[1] = new My_Half_edge();
                    three_edges[2] = new My_Half_edge();
                    for (int j = 0; j < 3; j++)
                    {
                        three_edges[j].next = three_edges[(j + 1) % 3];
                        three_edges[j].face = f;
                        int index = imap[triangle_array[counter + i + j]];
                        three_edges[j].vertex = vertices[index];
                        three_edges[j].index = triangle_array[counter + i + j];

                        vertices[index].edges.Add(three_edges[j]);
                        edges.Add(three_edges[j]);
                    }

                    f.edge = three_edges[0];
                    f.mat = mat;
                    faces.Add(f);
                }
                counter += len;
                mat++;
            }
            mat_count = mat;
            for (int i = 0; i < normal_count; i++)
            {
                Vector3 one_normal = normal_array[i];
                normals.Add(one_normal);
            }
            for (int i = 0; i < color_count; i++)
            {
                Vector4 one_color = color_array[i];
                colors.Add(one_color);
            }
            for (int i = 0; i < uv_count; i++)
            {
                Vector2 one_uv = uv_array[i];
                uvs.Add(one_uv);
            }
            max_square_length_of_mesh = (MAX - MIN).sqrMagnitude;
            face_count = faces.Count;
        }
        private void calculate_face_normal(My_Half_face one_face)
        {
            one_face.n = Vector3.Cross(one_face.edge.next.vertex.position - one_face.edge.vertex.position, one_face.edge.next.next.vertex.position - one_face.edge.vertex.position);
            one_face.n.Normalize();
        }
        private void calculate_face_normals()
        {
            int counter = 0;
            foreach (My_Half_face face in faces)
            {
                calculate_face_normal(face);
                counter++;
            }
        }
        private bool is_boundary_edge(My_Half_edge edge)
        {
            My_Half_vertex v_from;
            My_Half_vertex v_to;
            v_from = edge.vertex;
            v_to = edge.next.vertex;
            int counter = 0;
            foreach (My_Half_edge from_edge in v_from.edges)
            {
                foreach (My_Half_edge to_edge in v_to.edges)
                {
                    if (from_edge.face == to_edge.face)
                    {
                        counter++;
                        break;
                    }
                }
            }
            return (counter == 1);
        }
        private void detect_and_mark_boundaries()
        {
            int counter = 0;
            foreach (My_Half_edge edge in edges)
            {
                if (!is_boundary_edge(edge))
                    continue;
                edge.vertex.on_boundary = true;
                edge.next.vertex.on_boundary = true;
                counter++;
            }
        }
        private bool is_symmetry_edge(My_Half_edge edge)
        {
            My_Half_vertex v_from;
            My_Half_vertex v_to;
            v_from = edge.vertex;
            v_to = edge.next.vertex;
            List<int> index_maps_from = new List<int>();
            List<int> index_maps_to = new List<int>();
            foreach (My_Half_edge from_edge in v_from.edges)
            {
                if (from_edge.next.vertex == v_to)
                {
                    index_maps_from.Add(from_edge.next.next.index);
                }
            }
            foreach (My_Half_edge to_edge in v_to.edges)
            {
                if (to_edge.next.vertex == v_from)
                {
                    index_maps_to.Add(to_edge.next.next.index);
                }
            }

            if (index_maps_from.Count != index_maps_to.Count)
                return false;

            bool has_broken_symmetry = false;
            foreach (int from_index in index_maps_from)
            {
                if (!has_broken_symmetry)
                {
                    bool is_symmetry = false;
                    foreach (int to_index in index_maps_to)
                    {
                        if (from_index == to_index)
                            continue;
                        if (uvs.Count > 0 && uvs[from_index] == uvs[to_index])
                        {
                            is_symmetry = true;
                            break;
                        }
                    }
                    if (!is_symmetry)
                    {
                        has_broken_symmetry = true;
                    }
                }
            }
            return !has_broken_symmetry;
        }
        private void detect_and_mark_symmetries()
        {
            int counter = 0;
            foreach (My_Half_edge edge in edges)
            {
                if (!is_symmetry_edge(edge))
                    continue;
                edge.vertex.on_symmetry = true;
                edge.next.vertex.on_symmetry = true;
                counter++;
            }
        }
        private float cost_of_edge(My_Half_edge edge)
        {
            My_Half_vertex v_from;
            My_Half_vertex v_to;
            v_from = edge.vertex;
            v_to = edge.next.vertex;

            float square_length_of_edge = (v_to.position - v_from.position).sqrMagnitude;

            List<int> index_maps = new List<int>();
            List<int> mat_maps = new List<int>();
            List<My_Half_face> common_faces = new List<My_Half_face>();
            foreach (My_Half_edge from_edge in v_from.edges)
            {
                foreach (My_Half_edge to_edge in v_to.edges)
                {
                    if (from_edge.face == to_edge.face)
                    {
                        index_maps.Add(from_edge.index);
                        mat_maps.Add(from_edge.face.mat);
                        common_faces.Add(from_edge.face);
                        break;
                    }
                }
            }

            bool has_broken_normal = false;
            bool has_broken_color = false;
            bool has_broken_uv = false;
            bool has_broken_mat = false;
            float max_curvature = float.MinValue;
            float worst_triangle_shape = 0.0f;
            foreach (My_Half_edge from_edge in v_from.edges)
            {
                bool is_common_face = false;
                float min_curvature = float.MaxValue;
                foreach (My_Half_face common_face in common_faces)
                {
                    float curvature = (1.0f - Vector3.Dot(from_edge.face.n, common_face.n)) * 0.5f;
                    if (curvature < min_curvature)
                        min_curvature = curvature;
                    if (from_edge.face == common_face)
                    {
                        is_common_face = true;
                    }
                }
                if (min_curvature > max_curvature)
                    max_curvature = min_curvature;
                if (!is_common_face)
                {
                    if (lock_shape)
                    {
                        My_Half_vertex v1;
                        My_Half_vertex v2;
                        My_Half_vertex v3;
                        v1 = v_to;
                        v2 = from_edge.next.vertex;
                        v3 = from_edge.next.next.vertex;
                        Vector3 e1 = (v2.position - v1.position).normalized;
                        Vector3 e2 = (v3.position - v2.position).normalized;
                        Vector3 e3 = (v1.position - v3.position).normalized;
                        float cos1 = Vector3.Dot(e3, e1);
                        float cos2 = Vector3.Dot(e1, e2);
                        float cos3 = Vector3.Dot(e2, e3);
                        float min_cos = Math.Min(cos1, Math.Min(cos2, cos3));
                        float max_cos = Math.Max(cos1, Math.Max(cos2, cos3));
                        float triangle_shape = (max_cos - min_cos) * 0.5f;
                        if (triangle_shape > worst_triangle_shape)
                            worst_triangle_shape = triangle_shape;
                    }
                    if (lock_normal)
                    {
                        if (!has_broken_normal)
                        {
                            bool normal_joined = false;
                            foreach (int idx in index_maps)
                            {
                                if (normals.Count == 0 || (normals[from_edge.index] == normals[idx]))
                                {
                                    normal_joined = true;
                                    break;
                                }
                            }
                            if (!normal_joined)
                            {
                                has_broken_normal = true;
                            }
                        }
                    }
                    if (!use_detail_map)
                    {
                        if (!has_broken_color)
                        {
                            bool color_joined = false;
                            foreach (int idx in index_maps)
                            {
                                if (colors.Count == 0 || (colors[from_edge.index] == colors[idx]))
                                {
                                    color_joined = true;
                                    break;
                                }
                            }
                            if (!color_joined)
                            {
                                has_broken_color = true;
                            }
                        }
                    }
                    if (!has_broken_uv)
                    {
                        bool uv_joined = false;
                        foreach (int idx in index_maps)
                        {
                            if (uvs.Count == 0 || (uvs[from_edge.index] == uvs[idx]))
                            {
                                uv_joined = true;
                                break;
                            }
                        }
                        if (!uv_joined)
                        {
                            has_broken_uv = true;
                        }
                    }
                    if (!has_broken_mat)
                    {
                        bool mat_joined = false;
                        foreach (int idx in mat_maps)
                        {
                            if (from_edge.face.mat == idx)
                            {
                                mat_joined = true;
                                break;
                            }
                        }
                        if (!mat_joined)
                        {
                            has_broken_mat = true;
                        }
                    }
                }
            }
            float broken_normal_punishment = has_broken_normal ? max_square_length_of_mesh : 0.0f;
            float broken_color_punishment = has_broken_color ? max_square_length_of_mesh : 0.0f;
            float broken_uv_punishment = has_broken_uv ? max_square_length_of_mesh : 0.0f;
            float broken_mat_punishment = has_broken_mat ? max_square_length_of_mesh : 0.0f;

            float symmetry_uv_punishment = 0.0f;
            if (lock_symmetry)
            {
                symmetry_uv_punishment = (v_from.on_symmetry && !v_to.on_symmetry) ? max_square_length_of_mesh : 0.0f;
            }

            float boundary_punishment = 0.0f;
            float max_boundary_curvature = 0.0f;
            if (lock_boundary)
            {
                if (v_from.on_boundary || v_to.on_boundary)
                    boundary_punishment = max_square_length_of_mesh;
            }
            else
            {
                if (v_from.on_boundary)
                {
                    if (v_to.on_boundary)
                    {
                        foreach (My_Half_edge from_edge in v_from.edges)
                        {
                            if (is_boundary_edge(from_edge.next.next))
                            {
                                Vector3 p0 = from_edge.next.next.vertex.position;
                                Vector3 p1 = v_from.position;
                                Vector3 p2 = v_to.position;
                                float boundary_curvature = (1.0f - Vector3.Dot((p1 - p0).normalized, (p2 - p1).normalized)) * 0.5f;
                                if (boundary_curvature > max_boundary_curvature)
                                    max_boundary_curvature = boundary_curvature;
                            }
                        }
                    }
                    else
                    {
                        boundary_punishment = max_square_length_of_mesh;
                    }
                }
            }

            if (lock_detail)
            {
                max_curvature *= max_curvature;
            }
            double advanced_detail_weight = (use_detail_map && colors.Count > 0 ? 1.0 + detail_boost * colors[edge.index].x : 1.0);
            if (max_curvature < 1e-6f)
            {
                max_curvature = 1e-6f;
            }

            const double curvature_weight = 20.0;
            const double boundary_curvature_weight = 20.0;
            const double triangle_shape_weight = 1.0;
            return (float)(square_length_of_edge * advanced_detail_weight * ((max_curvature * curvature_weight + max_boundary_curvature * boundary_curvature_weight + worst_triangle_shape * triangle_shape_weight) / (curvature_weight + boundary_curvature_weight + triangle_shape_weight)) + boundary_punishment + broken_normal_punishment + broken_color_punishment + broken_uv_punishment + symmetry_uv_punishment + broken_mat_punishment);
        }
        private void calculate_cost_of_edges()
        {
            calculate_face_normals();
            detect_and_mark_boundaries();
            detect_and_mark_symmetries();
            foreach (My_Half_edge edge in edges)
            {
                edge.cost = cost_of_edge(edge);
                pq.Push(edge);
            }
        }
        private bool contract_edge(My_Half_edge edge)
        {
            My_Half_vertex v_from;
            My_Half_vertex v_to;
            v_from = edge.vertex;
            v_to = edge.next.vertex;
            List<KeyValuePair<int, int>> index_maps = new List<KeyValuePair<int, int>>();
            List<My_Half_face> common_faces = new List<My_Half_face>();
            foreach (My_Half_edge from_edge in v_from.edges)
            {
                foreach (My_Half_edge to_edge in v_to.edges)
                {
                    if (from_edge.face == to_edge.face)
                    {
                        if (from_edge.next.vertex == v_to)
                        {
                            index_maps.Add(new KeyValuePair<int, int>(from_edge.index, from_edge.next.index));
                        }
                        else if (from_edge.next.next.vertex == v_to)
                        {
                            index_maps.Add(new KeyValuePair<int, int>(from_edge.index, from_edge.next.next.index));
                        }
                        common_faces.Add(from_edge.face);
                        break;
                    }
                }
            }
            My_Half_trace one_contract_trace = new My_Half_trace();
            List<My_Half_edge> symmetric_difference_edges = new List<My_Half_edge>();
            foreach (My_Half_edge from_edge in v_from.edges)
            {
                bool is_common_face = false;
                foreach (My_Half_face common_face in common_faces)
                {
                    if (from_edge.face == common_face)
                    {
                        is_common_face = true;
                        break;
                    }
                }
                if (!is_common_face)
                {
                    from_edge.vertex = v_to;
                    int best_index = index_maps[index_maps.Count - 1].Value;
                    foreach (KeyValuePair<int, int> pair in index_maps)
                    {
                        if (uvs.Count == 0 || (uvs[from_edge.index] == uvs[pair.Key]))
                        {
                            best_index = pair.Value;
                            break;
                        }
                    }
                    My_Half_edge_index one_edge_index = new My_Half_edge_index
                    {
                        edge = from_edge,
                        index_from = from_edge.index,
                        index_to = best_index
                    };
                    one_contract_trace.updated_edge_indices.Add(one_edge_index);
                    from_edge.index = best_index;
                    symmetric_difference_edges.Add(from_edge);
                }
            }
            foreach (My_Half_edge to_edge in v_to.edges)
            {
                bool is_common_face = false;
                foreach (My_Half_face common_face in common_faces)
                {
                    if (to_edge.face == common_face)
                    {
                        is_common_face = true;
                        break;
                    }
                }
                if (!is_common_face)
                {
                    symmetric_difference_edges.Add(to_edge);
                }
            }
            v_to.edges = symmetric_difference_edges;

            foreach (My_Half_edge to_edge in v_to.edges)
            {
                to_edge.cost = cost_of_edge(to_edge);
                to_edge.next.cost = cost_of_edge(to_edge.next);
                to_edge.next.next.cost = cost_of_edge(to_edge.next.next);
                pq.Remove(to_edge.pqIndex);
                pq.Push(to_edge);
                pq.Remove(to_edge.next.pqIndex);
                pq.Push(to_edge.next);
                pq.Remove(to_edge.next.next.pqIndex);
                pq.Push(to_edge.next.next);
                calculate_face_normal(to_edge.face);
            }

            foreach (My_Half_face common_face in common_faces)
            {
                common_face.alive = false;
                face_count--;
                one_contract_trace.erased_faces.Add(common_face);
                common_face.edge.alive = false;
                common_face.edge.next.alive = false;
                common_face.edge.next.next.alive = false;
                pq.Remove(common_face.edge.pqIndex);
                pq.Remove(common_face.edge.next.pqIndex);
                pq.Remove(common_face.edge.next.next.pqIndex);
                My_Half_edge first = common_face.edge;
                My_Half_edge finder = first;
                do
                {
                    if (finder.vertex != v_from && finder.vertex != v_to)
                    {
                        finder.vertex.edges.Remove(finder);
                        break;
                    }
                    finder = finder.next;
                } while (finder != first);
            }
            v_from.alive = false;

            one_contract_trace.v_from = v_from;
            one_contract_trace.v_to = v_to;

            if (edge.cost >= max_square_length_of_mesh)
                one_contract_trace.safe = false;

            contract_trace.Add(one_contract_trace);

            return true;
        }
        private void contract_edges()
        {
            int save_face_count = face_count;
            while (face_count > 0 && pq.Top() != null)
            {
                contract_edge(pq.Top());
                if (save_face_count > face_count + 2500)
                {
                    save_face_count = face_count;
                }
            }
            current_trace_position = (int)(contract_trace.Count);
        }
        private int trace_to(int goal)
        {
            while (true)
            {
                if (current_trace_position == goal)
                    break;
                if (current_trace_position > goal)
                {
                    current_trace_position--;
                    foreach (My_Half_edge_index idx in contract_trace[current_trace_position].updated_edge_indices)
                    {
                        idx.edge.vertex = contract_trace[current_trace_position].v_from;
                        idx.edge.index = idx.index_from;
                    }
                }
                else
                {
                    if (!contract_trace[current_trace_position].safe)
                        break;
                    foreach (My_Half_edge_index idx in contract_trace[current_trace_position].updated_edge_indices)
                    {
                        idx.edge.vertex = contract_trace[current_trace_position].v_to;
                        idx.edge.index = idx.index_to;
                    }
                    current_trace_position++;
                }
            }
            return current_trace_position;
        }
        private readonly List<My_Half_vertex> vertices = new List<My_Half_vertex>();
        private readonly List<My_Half_face> faces = new List<My_Half_face>();
        private readonly List<My_Half_edge> edges = new List<My_Half_edge>();
        private readonly List<My_Half_trace> contract_trace = new List<My_Half_trace>();
        private readonly List<Vector3> normals = new List<Vector3>();
        private readonly List<Vector4> colors = new List<Vector4>();
        private readonly List<Vector2> uvs = new List<Vector2>();
        private int current_trace_position;
        private readonly MinHeap pq = new MinHeap();
        private Vector3 MAX = new Vector3();
        private Vector3 MIN = new Vector3();
        private float max_square_length_of_mesh;
        private int face_count;
        private int mat_count;
        private bool lock_boundary;
        private bool lock_detail;
        private bool lock_symmetry;
        private bool lock_normal;
        private bool lock_shape;
        private bool use_detail_map;
        private int detail_boost;
    }

    public static class MantisLODSimpler
    {
        public static int create_progressive_mesh(Vector3[] vertex_array, int vertex_count, int[] triangle_array, int triangle_count, Vector3[] normal_array, int normal_count, Color[] color_array, int color_count, Vector2[] uv_array, int uv_count, int protect_boundary, int protect_detail, int protect_symmetry, int protect_normal, int protect_shape, int use_detail_map, int detail_boost)
        {
            bool found = false;
            int index = -1;
            for (int i = 0; i < (int)(simplers.Count); i++)
            {
                if (simplers[i] == null)
                {
                    found = true;
                    simplers[i] = new Progressive_Mesh();
                    index = i;
                    break;
                }
            }
            if (!found)
            {
                simplers.Add(new Progressive_Mesh());
                index = (int)(simplers.Count) - 1;
            }
            Progressive_Mesh simpler = simplers[index];
            simpler.create_progressive_mesh(vertex_array, vertex_count, triangle_array, triangle_count, normal_array, normal_count, color_array, color_count, uv_array, uv_count, protect_boundary, protect_detail, protect_symmetry, protect_normal, protect_shape, use_detail_map, detail_boost);
            return index;
        }

        public static int get_triangle_list(int index, float goal, int[] triangle_array, ref int triangle_count)
        {
            if (index >= 0 && index < (int)(simplers.Count) && simplers[index] != null)
            {
                Progressive_Mesh simpler = simplers[index];
                int inner_goal = (int)(simpler.get_trace_num() * (1.0f - goal * 0.01f) + 0.5f);
                simpler.get_triangle_list(inner_goal, triangle_array, ref triangle_count);
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static int delete_progressive_mesh(int index)
        {
            if (index >= 0 && index < (int)(simplers.Count) && simplers[index] != null)
            {
                simplers[index] = null;
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private readonly static List<Progressive_Mesh> simplers = new List<Progressive_Mesh>();
    }
}
