using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class MergeModels : GraphmeshNode {

        [Input(false)] public List<Model> input = new List<Model>();
        [Output(false)] public List<Model> output = new List<Model>();

        protected override void Init() {
            name = "Array Modifier";
        }

        public override object GenerateOutput(NodePort port) {
            List<Model> inputModels = GetModelList(GetInputByFieldName("input"));
            return new List<Model>() { Model.CombineModels(input) };
        }
    }
}