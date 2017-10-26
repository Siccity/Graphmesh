using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Graphmesh {
    public class FlipNormals : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public List<Model> input;
        [Output] public List<Model> output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            List<Model> input = GetModelList(GetInputByFieldName("input"));
            if (input != null) {
                for (int i = 0; i < input.Count; i++) {
                    Vector3[] norms = input[i].mesh.normals;
                    for (int k = 0; k < norms.Length; k++) {
                        norms[k] = -norms[k];
                    }
                    for (int s = 0; s < input[i].mesh.subMeshCount; s++) {
                        int[] tris = input[i].mesh.GetTriangles(s);
                        for (int k = 0; k < tris.Length; k += 3) {
                            int t = tris[k];
                            tris[k] = tris[k + 1];
                            tris[k + 1] = t;
                        }
                        input[i].mesh.SetNormals(norms.ToList());
                        input[i].mesh.SetTriangles(tris, s);
                    }

                }
                return input;
            }
            else return null;
        }
    }
}