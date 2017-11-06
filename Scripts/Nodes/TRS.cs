using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace Graphmesh {
    public class TRS : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public ModelGroup input;
        [Input] public Vector3 t;
        [Input] public Vector3 r;
        [Input] public Vector3 s = Vector3.one;
        [Output] public ModelGroup output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            // Get inputs
            ModelGroup[] input = GetInputValues<ModelGroup>("input", this.input);
            Vector3 t = GetInputValue<Vector3>("t", this.t);
            Vector3 r = GetInputValue<Vector3>("r", this.r);
            Vector3 s = GetInputValue<Vector3>("s", this.s);

            Matrix4x4 trs = Matrix4x4.TRS(t, Quaternion.Euler(r), s);

            ModelGroup output = new ModelGroup();

            // Loop through input model groups
            for (int mg = 0; mg < input.Length; mg++) {
                if (input[mg] == null) continue;
                // Loop through group models
                for (int i = 0; i < input[mg].Count; i++) {
                    Mesh mesh = input[mg][i].mesh.Copy();
                    Vector3[] verts = mesh.vertices;
                    List<Vector3> norms = new List<Vector3>();
                    mesh.GetNormals(norms);
                    while (norms.Count < verts.Length) norms.Add(Vector3.up);

                    for (int v = 0; v < verts.Length; v++) {
                        verts[v] = trs.MultiplyPoint(verts[v]);
                        norms[v] = trs.MultiplyVector(norms[v]);
                    }
                    mesh.SetVertices(verts.ToList());
                    mesh.SetNormals(norms);
                    output.Add(new Model(input[mg][i]) { mesh = mesh });
                }
            }
            return output;
        }
    }
}