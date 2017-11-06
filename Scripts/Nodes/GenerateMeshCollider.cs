using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Graphmesh {
    public class GenerateMeshCollider : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public ModelGroup input;
        [Input] public bool convex;
        [Output] public ModelGroup output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            // Get inputs
            ModelGroup[] input = GetInputValues<ModelGroup>("input", this.input);
            bool convex = GetInputValue<bool>("convex", this.convex);
            ModelGroup output = new ModelGroup();

            // Loop through input model groups
            for (int mg = 0; mg < input.Length; mg++) {
                if (input[mg] == null) continue;
                // Loop through group models
                for (int i = 0; i < input[mg].Count; i++) {
                    output.Add(new Model(input[mg][i]) {
                        colType = Model.ColliderType.Mesh,
                            meshCol = input[mg][i].mesh,
                            meshColConvex = convex
                    });
                }
            }
            return output;
        }
    }
}