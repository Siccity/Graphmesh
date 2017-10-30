using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class FFD : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public ModelGroup input;
        [Output] public ModelGroup output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            //Get inputs
            ModelGroup[] input = GetInputsByFieldName("input", this.input);

            if (input == null) return input;

            for (int i = 0; i < input.Length; i++) {
                for (int k = 0; k < input[i].Count; k++) {
                    Model model = input[i][k];
                    Mesh mesh = model.mesh.Copy();
                    mesh.RecalculateBounds();
                    Bounds bounds = mesh.bounds;
                    Vector3[] verts = mesh.vertices;
                    Vector3[] weights = new Vector3[verts.Length];

                    for (int v = 0; v < verts.Length; v++) {
                        
                    }
                }
            }
            return input;
        }
    }
}
