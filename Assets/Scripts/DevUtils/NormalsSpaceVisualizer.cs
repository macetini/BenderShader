using UnityEngine;

namespace DevUtils
{
    /// <summary>
    /// Debug class used for normal visualization. (Debug use only)
    /// </summary>
    public class NormalsSpaceVisualizer : MonoBehaviour
    {
        public float offset = 0.01f;

        /// <summary>
        /// Normal visualizer scale - the bigger the number the longer the normal line visual.
        /// </summary>
        public float scale = 0.1f;

        void OnDrawGizmos()
        {
            MeshFilter filter = GetComponent<MeshFilter>();
            if (filter)
            {
                Mesh mesh = filter.sharedMesh;
                if (mesh)
                {
                    ShowNormals(mesh);
                }
            }
        }

        void ShowNormals(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector4[] tangents = mesh.tangents;
            for (int i = 0; i < vertices.Length; i++)
            {
                ShowTangentSpace(
                    transform.TransformPoint(vertices[i]),
                    transform.TransformDirection(normals[i]),
                    transform.TransformDirection(tangents[i]),
                    tangents[i].w
                );
            }
        }

        void ShowTangentSpace(Vector3 vertex, Vector3 normal, Vector3 tangent, float binormalSign)
        {
            vertex += normal * offset;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(vertex, vertex + normal * scale);
        }
    }
}