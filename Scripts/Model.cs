using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Graphmesh {
    public class Model {
        public Mesh mesh;
        public Material[] materials;

        public Model(Mesh mesh, Material[] materials) {
            this.mesh = mesh;
            this.materials = materials;
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
            return new Model(mesh,submeshMaterials);
        }

        public int GetMaterialSubmeshIndex(Material material) {
            for (int i = 0; i < materials.Length; i++) {
                if (materials[i] == material) return i;
            }
            return -1;
        }
    }
}