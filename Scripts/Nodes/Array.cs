using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class Array : GraphmeshNode {

        [Input(false)] public List<Model> model;
        [Input(false)] public Bezier3DSpline spline;
        [Output(false)] public List<Model> output;

        public int count;
        public Vector3 posOffset;
        public Vector3 rotOffset;
        public enum LengthFitMethod { RoundUp, RoundDown, ScaleUp, ScaleDown, Cut }

        public override object GenerateOutput(NodePort port) {
            Bezier3DSpline spline = GetPortByFieldName("spline").GetInputValue<Bezier3DSpline>();

            List<Model> inputModels = GetModelList(GetInputByFieldName("model"));
            List<Model> output = new List<Model>();

            if (spline != null) count = GetCount(spline);

            //Loop through input models
            for (int i = 0; i < inputModels.Count; i++) {
                CombineInstance[] finalCombines = new CombineInstance[inputModels[i].mesh.subMeshCount];

                //Loop through model submeshes
                for (int g = 0; g < inputModels[i].mesh.subMeshCount; g++) {
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
                            mesh = inputModels[i].mesh,
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
                output.Add(new Model(finalMesh, inputModels[i].materials));
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