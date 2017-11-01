using UnityEngine;

namespace Graphmesh {
    public class FFD : GraphmeshNode {

        [Input] public ModelGroup input;
        [Input] public FFDBox ffdBox;
        [Output] public ModelGroup output;
        [Output] public string s;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            //Get inputs
            ModelGroup[] input = GetInputsByFieldName("input", this.input);
            FFDBox ffdBox = GetInputByFieldName<FFDBox>("ffdBox", this.ffdBox);
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

                    if (ffdBox != null) {

                        for (int v = 0; v < verts.Length; v++) {
                            // Get weights
                            weights[v].x = Mathf.InverseLerp(bounds.min.x, bounds.max.x, verts[v].x);
                            weights[v].y = Mathf.InverseLerp(bounds.min.y, bounds.max.y, verts[v].y);
                            weights[v].z = Mathf.InverseLerp(bounds.min.z, bounds.max.z, verts[v].z);

                            // Get positions for x axis
                            Vector3 vx_00 = Vector3.Lerp(ffdBox.l_000, ffdBox.l_100, weights[v].x);
                            Vector3 vx_01 = Vector3.Lerp(ffdBox.l_001, ffdBox.l_101, weights[v].x);
                            Vector3 vx_10 = Vector3.Lerp(ffdBox.l_010, ffdBox.l_110, weights[v].x);
                            Vector3 vx_11 = Vector3.Lerp(ffdBox.l_011, ffdBox.l_111, weights[v].x);

                            // Get positions for y axis
                            Vector3 vxy_0 = Vector3.Lerp(vx_00, vx_10, weights[v].y);
                            Vector3 vxy_1 = Vector3.Lerp(vx_01, vx_11, weights[v].y);

                            // Get position for z axis
                            Vector3 vxyz = Vector3.Lerp(vxy_0, vxy_1, weights[v].z);

                            // Apply
                            verts[v] = vxyz;
                        }
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