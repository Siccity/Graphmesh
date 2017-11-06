using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Graphmesh {
    public class FollowSpline : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public ModelGroup input;
        [Input] public Bezier3DSpline spline;
        [Output] public ModelGroup output;
        public enum Axis { x = 0, y = 1, z = 2 }
        public Axis axis = Axis.z;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            ModelGroup[] input = GetInputValues<ModelGroup>("input", this.input);
            Bezier3DSpline spline = GetInputValue<Bezier3DSpline>("spline", this.spline);
            ModelGroup output = new ModelGroup();
            if (input == null) return output;

            // Loop through input model groups
            for (int mg = 0; mg < input.Length; mg++) {
                if (input[mg] == null) continue;
                // Loop through group models
                for (int i = 0; i < input[mg].Count; i++) {
                    Mesh mesh = input[mg][i].mesh.Copy();
                    if (spline != null) FollowCurve(mesh, spline);
                    output.Add(new Model(input[mg][i]) { mesh = mesh });
                }
            }
            return output;
        }

        private void FollowCurve(Mesh mesh, Bezier3DSpline spline) {
            int axis = (int) this.axis;
            Vector3[] verts = mesh.vertices;
            Vector3[] norms = mesh.normals;
            Vector4[] tan = mesh.tangents;
            for (int i = 0; i < mesh.vertexCount; i++) {
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
            mesh.SetVertices(new List<Vector3>(verts));
            mesh.SetNormals(new List<Vector3>(norms));
            mesh.SetTangents(new List<Vector4>(tan));
        }
    }
}