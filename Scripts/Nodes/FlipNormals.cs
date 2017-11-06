using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace Graphmesh {
    public class FlipNormals : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public ModelGroup input;
        [Output] public ModelGroup output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            //Get inputs
            ModelGroup[] input = GetInputValues<ModelGroup>("input", this.input);

            ModelGroup output = new ModelGroup();

            // Loop through input model groups
            for (int mg = 0; mg < input.Length; mg++) {
                if (input[mg] == null) continue;
                // Loop through group models
                for (int i = 0; i < input[mg].Count; i++) {
                    Mesh mesh = input[mg][i].mesh.Copy();

                    Vector3[] norms = mesh.normals;
                    for (int k = 0; k < norms.Length; k++) {
                        norms[k] = -norms[k];
                    }
                    for (int s = 0; s < mesh.subMeshCount; s++) {
                        int[] tris = mesh.GetTriangles(s);
                        for (int k = 0; k < tris.Length; k += 3) {
                            int t = tris[k];
                            tris[k] = tris[k + 1];
                            tris[k + 1] = t;
                        }
                        mesh.SetNormals(norms.ToList());
                        mesh.SetTriangles(tris, s);
                    }

                    output.Add(new Model(input[mg][i]) { mesh = mesh });
                }
            }
            return output;
        }
    }
}