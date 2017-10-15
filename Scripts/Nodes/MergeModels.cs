using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class MergeModels : GraphmeshNode {

        [Input(false)] public List<Model> input = new List<Model>();
        [Output(false)] public List<Model> output = new List<Model>();

        protected override void Init() {
            name = "Array Modifier";
        }

        public override object GenerateOutput(int outputIndex, object[][] inputs) {

            List<Model> input = UnpackModels(0, inputs);
            return new List<Model>() { Model.CombineModels(input) };

        }
    }
}