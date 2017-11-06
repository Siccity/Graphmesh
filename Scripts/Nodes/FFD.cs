using UnityEngine;
using XNode;

namespace Graphmesh {
    public class FFD : GraphmeshNode {

        [Input] public ModelGroup input;
        [Input] public FFDBox ffdBox;
        public bool reference = true;
        [Input] public Vector3 v_000 = new Vector3(0, 0, 0);
        [Input] public Vector3 v_001 = new Vector3(0, 0, 1);
        [Input] public Vector3 v_010 = new Vector3(0, 1, 0);
        [Input] public Vector3 v_011 = new Vector3(0, 1, 1);
        [Input] public Vector3 v_100 = new Vector3(1, 0, 0);
        [Input] public Vector3 v_101 = new Vector3(1, 0, 1);
        [Input] public Vector3 v_110 = new Vector3(1, 1, 0);
        [Input] public Vector3 v_111 = new Vector3(1, 1, 1);
        [Output] public ModelGroup output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            //Get inputs
            ModelGroup[] input = GetInputValues<ModelGroup>("input", this.input);

            Vector3 v_000, v_001, v_010, v_011, v_100, v_101, v_110, v_111;

            if (reference) {
                FFDBox ffdBox = GetInputValue<FFDBox>("ffdBox", this.ffdBox);
                FFDBox.FFDBoxSettings ffdBoxSettings = FFDBox.FFDBoxSettings.Box;
                if (ffdBox != null) ffdBoxSettings = ffdBox.settings;
                v_000 = ffdBoxSettings.v_000;
                v_001 = ffdBoxSettings.v_001;
                v_010 = ffdBoxSettings.v_010;
                v_011 = ffdBoxSettings.v_011;
                v_100 = ffdBoxSettings.v_100;
                v_101 = ffdBoxSettings.v_101;
                v_110 = ffdBoxSettings.v_110;
                v_111 = ffdBoxSettings.v_111;
            } else {
                v_000 = GetInputValue<Vector3>("v_000", this.v_000);
                v_001 = GetInputValue<Vector3>("v_001", this.v_001);
                v_010 = GetInputValue<Vector3>("v_010", this.v_010);
                v_011 = GetInputValue<Vector3>("v_011", this.v_011);
                v_100 = GetInputValue<Vector3>("v_100", this.v_100);
                v_101 = GetInputValue<Vector3>("v_101", this.v_101);
                v_110 = GetInputValue<Vector3>("v_110", this.v_110);
                v_111 = GetInputValue<Vector3>("v_111", this.v_111);
            }

            ModelGroup output = new ModelGroup();
            if (input == null) return output;

            for (int i = 0; i < input.Length; i++) {
                for (int k = 0; k < input[i].Count; k++) {
                    Model model = input[i][k];
                    Mesh mesh = model.mesh.Copy();
                    mesh.RecalculateBounds();
                    Bounds bounds = mesh.bounds;

                    Vector3[] verts = mesh.vertices;

                    for (int v = 0; v < verts.Length; v++) {
                        // Get weights
                        Vector3 weight = Vector3.zero;
                        weight.x = Mathf.InverseLerp(bounds.min.x, bounds.max.x, verts[v].x);
                        weight.y = Mathf.InverseLerp(bounds.min.y, bounds.max.y, verts[v].y);
                        weight.z = Mathf.InverseLerp(bounds.min.z, bounds.max.z, verts[v].z);

                        // Get positions for x axis
                        Vector3 vx_00 = Vector3.Lerp(v_000, v_100, weight.x);
                        Vector3 vx_01 = Vector3.Lerp(v_001, v_101, weight.x);
                        Vector3 vx_10 = Vector3.Lerp(v_010, v_110, weight.x);
                        Vector3 vx_11 = Vector3.Lerp(v_011, v_111, weight.x);

                        // Get positions for y axis
                        Vector3 vxy_0 = Vector3.Lerp(vx_00, vx_10, weight.y);
                        Vector3 vxy_1 = Vector3.Lerp(vx_01, vx_11, weight.y);

                        // Get position for z axis
                        Vector3 vxyz = Vector3.Lerp(vxy_0, vxy_1, weight.z);

                        // Apply
                        verts[v] = vxyz;
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