using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class MergeModels : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public ModelGroup input;
        [Output] public ModelGroup output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            // Get inputs
            ModelGroup[] input = GetInputsByFieldName<ModelGroup>("input", this.input);

            ModelGroup output = new ModelGroup();

            // Loop through input model groups
            for (int mg = 0; mg < input.Length; mg++) {
                if (input[mg] == null) continue;
                // Loop through group models
                for (int i = 0; i < input[mg].Count; i++) {
                    output.Add(input[mg][i]);
                }
            }
            return new ModelGroup { Model.CombineModels(output) };
        }
    }
}