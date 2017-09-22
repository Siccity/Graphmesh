using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Graphmesh {
    public class FollowSpline : GraphmeshNode {

        public enum Axis {
            x = 0,
            y = 1,
            z = 2
        }
        public Axis axis = Axis.z;

        protected override void Init() {
            name = "Follow Spline";

            inputs = new NodePort[2];
            inputs[0] = CreateNodeInput("Model", typeof(List<Model>));
            inputs[1] = CreateNodeInput("Spline", typeof(Bezier3DSpline));

            outputs = new NodePort[1];
            outputs[0] = CreateNodeOutput("Model", typeof(List<Model>));

        }

        public override object GenerateOutput(int outputIndex, object[][] inputs) {
            List<Model> input = UnpackModels(0, inputs);
            Bezier3DSpline spline = inputs[1][0] as Bezier3DSpline;

            if (spline == null) Debug.Log("Spline is null");

            for (int i = 0; i < input.Count; i++) {
                FollowCurve(input[i],spline);
            }
            return input;
        }

        private void FollowCurve(Model model, Bezier3DSpline spline) {
            int axis = (int)this.axis;
            Vector3[] verts = model.mesh.vertices;
            Vector3[] norms = model.mesh.normals;
            Vector4[] tan = model.mesh.tangents;
            for (int i = 0; i < model.mesh.vertexCount; i++) {
                float dist = verts[i][axis];
                Vector3 offset = verts[i];
                offset[axis] = 0;

                Quaternion orientation = spline.GetOrientationLocalFast(dist);

                verts[i] = spline.GetPointLocal(dist) + (orientation * offset);
                norms[i] = orientation * norms[i];
                float w = tan[i].w;
                tan[i] = orientation * new Vector3(tan[i].x, tan[i].y, tan[i].z);
                tan[i].w = w;
            }
            model.mesh.SetVertices(new List<Vector3>(verts));
            model.mesh.SetNormals(new List<Vector3>(norms));
            model.mesh.SetTangents(new List<Vector4>(tan));
        }
    }
}
