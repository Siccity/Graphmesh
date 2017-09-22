using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Graphmesh {
    public class MergeModels : GraphmeshNode {

        protected override void Init() {
            name = "Array Modifier";

            inputs = new NodePort[1];
            inputs[0] = CreateNodeInput("Model", typeof(List<Model>));

            outputs = new NodePort[1];
            outputs[0] = CreateNodeOutput("Model", typeof(List<Model>));
        }

        public override object GenerateOutput(int outputIndex, object[][] inputs) {

            List<Model> input = UnpackModels(0, inputs);
            return new List<Model>() { Model.CombineModels(input) };
  
        }
    }
}
