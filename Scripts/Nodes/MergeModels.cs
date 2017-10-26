using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class MergeModels : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public List<Model> input = new List<Model>();
        [Output] public List<Model> output = new List<Model>();

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            List<Model> inputModels = GetModelList(GetInputByFieldName("input"));
            return new List<Model>() { Model.CombineModels(inputModels) };
        }
    }
}