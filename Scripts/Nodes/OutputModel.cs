using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Graphmesh {
    [CreateNodeMenu("Output/Output")]
    public class OutputModel : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public ModelGroup input;

        public List<Model> GetModels() {
            NodePort modelPort = GetInputPort("input");
            return GetValue(modelPort) as List<Model>;
        }

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            // Get inputs
            ModelGroup[] input = GetInputValues<ModelGroup>("input", this.input);
            ModelGroup output = new ModelGroup();

            if (input == null) return output;
            // Loop through input model groups
            for (int mg = 0; mg < input.Length; mg++) {
                if (input[mg] == null) continue;
                // Loop through group models
                for (int i = 0; i < input[mg].Count; i++) {
                    input[mg][i].mesh.RecalculateBounds();
                    output.Add(input[mg][i]);
                }
            }
            return output;
        }
    }
}