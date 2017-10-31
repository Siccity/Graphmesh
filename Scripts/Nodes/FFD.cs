using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class FFD : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public ModelGroup input;
        Vector3 topLeftFront;
        Vector3 topLeftBack;
        Vector3 topRightFront;
        Vector3 topRightBack;
        public float xTop, xBtm;
        [Output] public ModelGroup output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            //Get inputs
            ModelGroup[] input = GetInputsByFieldName("input", this.input);
            ModelGroup output = new ModelGroup();
            if (input == null) return output;

            for (int i = 0; i < input.Length; i++) {
                for (int k = 0; k < input[i].Count; k++) {
                    Model model = input[i][k];
                    Mesh mesh = model.mesh.Copy();
                    mesh.RecalculateBounds();
                    Bounds bounds = mesh.bounds;
                    Vector3[] verts = mesh.vertices;
                    Vector3[] weights = new Vector3[verts.Length];

                    for (int v = 0; v < verts.Length; v++) {
                        weights[v].x = Mathf.InverseLerp(bounds.min.x, bounds.max.x, verts[v].x);
                        weights[v].y = Mathf.InverseLerp(bounds.min.y, bounds.max.y, verts[v].y);
                        weights[v].z = Mathf.InverseLerp(bounds.min.z, bounds.max.z, verts[v].z);

                        // X top
                        float vxtop = Mathf.Lerp(bounds.min.x - topLeftFront.x, bounds.max.x + xTop, weights[v].x);
                        // X btm
                        float vxbtm = Mathf.Lerp(bounds.min.x - xBtm, bounds.max.x + xBtm, weights[v].x);
                        verts[v].x = Mathf.Lerp(vxbtm, vxtop, weights[v].y);
                        
                    }

                    mesh.vertices = verts;
                    model.mesh = mesh;
                    output.Add(model);
                }
            }
            return output;
        }
    }
}