using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Graphmesh {
    public class TRS : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public List<Model> input;
        [Header("Offsets")]
        [Input] public Vector3 t;
        [Input] public Vector3 r;
        [Input] public Vector3 s = Vector3.one;
        [Output] public List<Model> output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            List<Model> inputs = GetModelList(GetInputByFieldName("input"));
            if (inputs == null) return null;

            Matrix4x4 trs = Matrix4x4.TRS(t, Quaternion.Euler(r), s);

            for (int i = 0; i < inputs.Count; i++) {
                Vector3[] verts = inputs[i].mesh.vertices;
                List<Vector3> norms = new List<Vector3>();
                inputs[i].mesh.GetNormals(norms);
                while (norms.Count < verts.Length) norms.Add(Vector3.up);

                for (int v = 0; v < verts.Length; v++) {
                    verts[v] = trs.MultiplyPoint(verts[v]);
                    norms[v] = trs.MultiplyVector(norms[v]);
                }
                inputs[i].mesh.SetVertices(verts.ToList());
                inputs[i].mesh.SetNormals(norms);
            }
            return inputs;
        }
    }
}