using UnityEngine;
using System.Collections.Generic;

namespace Graphmesh {
    public class Array : GraphmeshNode {

        [Input] public List<Model> model;
        [Input] public Bezier3DSpline spline;
        [Output] public List<Model> output;

        public int count;
        public Vector3 posOffset;
        public Vector3 rotOffset;
        public enum LengthFitMethod { RoundUp, RoundDown, ScaleUp, ScaleDown, Cut}
        protected override void Init() {
            name = "Array Modifier";
        }

        public override object GenerateOutput(int outputIndex, object[][] inputs) {

            List<Model> input = UnpackModels(0, inputs);
            Bezier3DSpline spline = null;
            bool fit = true;
            if (inputs[1] != null && inputs[1].Length != 0) {
                spline = inputs[1][0] as Bezier3DSpline;
            }
            List<Model> output = new List<Model>();

            if (spline != null) count = GetCount(spline);

            //Loop through input models
            for (int i = 0; i < input.Count; i++) {
                CombineInstance[] finalCombines = new CombineInstance[input[i].mesh.subMeshCount];

                //Loop through model submeshes
                for (int g = 0; g < input[i].mesh.subMeshCount; g++) {
                    //Create a CombineInstance array for the submesh
                    CombineInstance[] submeshCombines = new CombineInstance[count];

                    //Perform the array modifier
                    for (int k = 0; k < count; k++) {
                        Vector3 pos = posOffset * k;
                        Vector3 rot = rotOffset * k;
                        Vector3 scale = Vector3.one;

                        if (spline != null) {
                            float segmentWidth = posOffset.magnitude; //eg 4
                            float lengthToFit = spline.totalLength; //eg 11.12041
                            float fitScale = lengthToFit / (segmentWidth * count); //eg 1.390052
                            Vector3 fitAxis = posOffset / segmentWidth; //eg (0,0,1)
                            Vector3 fitScaleOffset = fitAxis * fitScale; //eg (0,0,1.390052)

                            scale.x *= Mathf.Lerp(1f, fitScaleOffset.x, fitAxis.x);
                            scale.y *= Mathf.Lerp(1f, fitScaleOffset.y, fitAxis.y);
                            scale.z *= Mathf.Lerp(1f, fitScaleOffset.z, fitAxis.z);
                            pos.x *= Mathf.Lerp(1f, fitScaleOffset.x, fitAxis.x);
                            pos.y *= Mathf.Lerp(1f, fitScaleOffset.y, fitAxis.y);
                            pos.z *= Mathf.Lerp(1f, fitScaleOffset.z, fitAxis.z);

                        }
                        //scale += (scaleOffset * k);

                        //Setup CombineInstance
                        submeshCombines[k] = new CombineInstance() {
                            mesh = input[i].mesh,
                            transform = Matrix4x4.TRS(pos, Quaternion.Euler(rot), scale),
                            subMeshIndex = g
                        };
                    }
                    Mesh arrayedSubmesh = new Mesh();
                    arrayedSubmesh.CombineMeshes(submeshCombines, true);
                    finalCombines[g] = new CombineInstance() {
                        mesh = arrayedSubmesh,
                        transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one),
                    };
                }
                Mesh finalMesh = new Mesh();
                finalMesh.CombineMeshes(finalCombines, false);
                output.Add(new Model(finalMesh, input[i].materials));
            }
            return output;
        }

        int GetCount(Bezier3DSpline spline) {
            int fittedCount = LengthToCount(spline.totalLength);
            return fittedCount;
            //return Mathf.Clamp(fittedCount + count, 0, GetMaxCount());
        }

        int LengthToCount(float length) {
            return Mathf.FloorToInt(length / posOffset.magnitude);
        }
    }
}
