using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class Array : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public List<Model> model;
        [Output] public List<Model> output;

        public bool fitLength;
        [Input] public float length;
        [Input] public int count = 2;
        public Vector3 posOffset;
        public Vector3 rotOffset;
        public enum LengthFitMethod { RoundUp, RoundDown, ScaleUp, ScaleDown, Cut }

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            List<Model> inputModels = GetModelList(GetInputByFieldName("model"));
            List<Model> output = new List<Model>();

            float length = this.length;
            if (fitLength) {
                length = GetInputByFieldName("length").GetInputSum(this.length);
                count = GetCount(length);
            }

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

                        if (fitLength) {
                            float segmentWidth = posOffset.magnitude; //eg 4
                            float lengthToFit = length; //eg 11.12041
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

        int GetCount(float length) {
            int fittedCount = LengthToCount(length);
            return fittedCount;
            //return Mathf.Clamp(fittedCount + count, 0, GetMaxCount());
        }

        int LengthToCount(float length) {
            return Mathf.FloorToInt(length / posOffset.magnitude);
        }
    }
}