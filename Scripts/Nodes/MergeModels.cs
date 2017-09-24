using UnityEngine;
using System.Collections.Generic;

namespace Graphmesh {
    public class MergeModels : GraphmeshNode {

        protected override void Init() {
            name = "Array Modifier";
        }
        [Input] public List<Model> input = new List<Model>();
        [Output] public List<Model> output = new List<Model>();

        public override object GenerateOutput(int outputIndex, object[][] inputs) {

            List<Model> input = UnpackModels(0, inputs);
            return new List<Model>() { Model.CombineModels(input) };
  
        }
    }
}
