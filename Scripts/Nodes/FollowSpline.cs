using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class FollowSpline : GraphmeshNode {

        [Input(false)] public List<Model> model;
        [Input(false)] public Bezier3DSpline spline;
        [Output(false)] public List<Model> output;
        public enum Axis {
            x = 0,
                y = 1,
                z = 2
        }
        public Axis axis = Axis.z;

        protected override void Init() {
            name = "Follow Spline";
        }

        public override object GenerateOutput(NodePort port) {
            List<Model> input = GetModelList(GetInputByFieldName("model"));
            NodePort splinePort = GetPortByFieldName("spline");
            Bezier3DSpline spline = splinePort.Connection.GetValue() as Bezier3DSpline;

            if (spline == null) Debug.Log("Spline is null");

            for (int i = 0; i < input.Count; i++) {
                FollowCurve(input[i], spline);
            }
            return input;
        }

        private void FollowCurve(Model model, Bezier3DSpline spline) {
            int axis = (int) this.axis;
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