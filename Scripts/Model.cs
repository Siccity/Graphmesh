using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Graphmesh {
    public class Model {
        public Mesh mesh;
        public Material[] materials;

        public Model(Mesh mesh, Material[] materials) {
            this.mesh = mesh;
            this.materials = materials;
        }

        public static List<Model> FromGameObject(GameObject go) {
            List<Model> output = new List<Model>();

            MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++) {
                MeshRenderer renderer = renderers[i];
                MeshFilter filter = renderer.GetComponent<MeshFilter>();
                if (filter == null) continue;
                Mesh mesh = BakeMeshTransform(go.transform, filter);
                Material[] materials = renderer.sharedMaterials;
                output.Add(new Model(mesh, materials));
            }
            return output;
        }

        private static Mesh BakeMeshTransform(Transform root, MeshFilter filter) {
            Mesh mesh = filter.sharedMesh.Copy();
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector4> tangent = new List<Vector4>();
            mesh.GetVertices(verts);
            mesh.GetNormals(norms);
            mesh.GetTangents(tangent);

            for (int i = 0; i < verts.Count; i++) {
                //Vert position
                Vector3 world = filter.transform.TransformPoint(verts[i]); //Transform from filter local to world
                Vector3 local = root.InverseTransformPoint(world); //Transform from world to root local 
                verts[i] = local;

                //Vert normal
                world = filter.transform.TransformDirection(norms[i]);
                local = root.InverseTransformDirection(world);
                norms[i] = local;

                //Vert tangent
                float w = tangent[i].w;
                world = filter.transform.TransformDirection(tangent[i]);
                local = root.InverseTransformDirection(world);
                tangent[i] = new Vector4(local.x, local.y, local.z, w);
            }
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetTangents(tangent);
            return mesh;
        }

        public static Model CombineModels(List<Model> models) {
            Material[] submeshMaterials = models.SelectMany(x => x.materials).Distinct().ToArray();
            Mesh[] submeshes = new Mesh[submeshMaterials.Length];

            for (int i = 0; i < submeshes.Length; i++) {
                List<CombineInstance> submeshCombines = new List<CombineInstance>();
                for (int k = 0; k < models.Count; k++) {
                    int submeshIndex = models[k].GetMaterialSubmeshIndex(submeshMaterials[i]);
                    if (submeshIndex != -1) {
                        CombineInstance combine = new CombineInstance();
                        combine.mesh = models[k].mesh;
                        combine.transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                        combine.subMeshIndex = submeshIndex;
                        submeshCombines.Add(combine);
                    }
                }
                Mesh submesh = new Mesh();
                submesh.CombineMeshes(submeshCombines.ToArray(), true);
                submeshes[i] = submesh;
            }

            CombineInstance[] finalCombines = submeshes.Select(x => new CombineInstance() {
                mesh = x,
                    transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one)
            }).ToArray();

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(finalCombines, false);
            return new Model(mesh, submeshMaterials);
        }

        public int GetMaterialSubmeshIndex(Material material) {
            for (int i = 0; i < materials.Length; i++) {
                if (materials[i] == material) return i;
            }
            return -1;
        }
    }
}