using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Graphmesh {
    public class Graphmesh : MonoBehaviour {
        /// <summary> NodeGraph to use for generating the mesh </summary>
        public NodeGraph nodeGraph;
        /// <summary> Automatically generated and managed. Reuses node outputs. </summary>
        public NodeCache outputCache = new NodeCache();

        [ContextMenu("Generate")]
        public void Generate() {
            //We traverse backwards

            OutputModel[] outputNodes = nodeGraph.nodes.FindAll(x => x.GetType() == typeof(OutputModel)).ConvertAll(x => x as OutputModel).ToArray();

            GraphmeshNode.currentOutputCache = outputCache;

            List<Model> models = new List<Model>();
            for (int i = 0; i < outputNodes.Length; i++) {
                List<Model> newModels = outputNodes[i].GetModels();
                if (newModels != null) models.AddRange(newModels);
            }
            if (models == null) {
                Debug.LogWarning("Error in mesh generation");
            } else {
                if (models.Count == 1) ShowModel(gameObject, models[0]);
                else ShowModels(models);
            }
        }

        private void ShowModel(GameObject target, Model model) {
            MeshFilter filter = target.GetOrAddComponent<MeshFilter>();
            filter.mesh = model.mesh;
            MeshRenderer renderer = target.GetOrAddComponent<MeshRenderer>();
            renderer.materials = model.materials;
            if (model.colType == Model.ColliderType.Mesh) {
                MeshCollider mcol = target.GetOrAddComponent<MeshCollider>();
                mcol.sharedMesh = model.meshCol;
                mcol.convex = model.meshColConvex;
            }
        }
        private void ShowModels(List<Model> models) {
            ClearMesh();
            for (int i = 0; i < models.Count; i++) {
                GameObject go = new GameObject("mesh output " + i);
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                ShowModel(go, models[i]);
            }
        }

        private void ClearMesh() {
            int children = transform.childCount;
            for (int i = children - 1; i >= 0; i--) {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            DestroyImmediate(GetComponent<MeshRenderer>());
            DestroyImmediate(GetComponent<MeshFilter>());
            DestroyImmediate(GetComponent<Collider>());
        }

        void OnEnable() { }
    }

    public static class ExtensionMethods {
        /// <summary> Returns a copy of the mesh </summary>
        public static Mesh Copy(this Mesh mesh) {
            return new Mesh() {
                vertices = mesh.vertices,
                    triangles = mesh.triangles,
                    uv = mesh.uv,
                    uv2 = mesh.uv2,
                    normals = mesh.normals,
                    tangents = mesh.tangents,
                    colors = mesh.colors
            };
        }

        /// <summary> Returns a copy of meshes </summary>
        public static Mesh[] Copy(this Mesh[] mesh) {
            Mesh[] meshes = new Mesh[mesh.Length];
            for (int i = 0; i < meshes.Length; i++) {
                meshes[i] = new Mesh() {
                vertices = mesh[i].vertices,
                triangles = mesh[i].triangles,
                uv = mesh[i].uv,
                uv2 = mesh[i].uv2,
                normals = mesh[i].normals,
                tangents = mesh[i].tangents,
                colors = mesh[i].colors
                };
            }
            return meshes;
        }

        public static T GetOrAddComponent<T>(this GameObject target) where T : Component {
            T t = target.GetComponent<T>();
            if (t == null) t = target.AddComponent<T>();
            return t;
        }
    }
}