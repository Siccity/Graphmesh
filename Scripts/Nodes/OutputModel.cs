using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class OutputModel : GraphmeshNode {

        [Input(false)] public List<Model> model;

        public List<Model> GetModels() {

            NodePort modelPort = GetInputByFieldName("model");
            return GetValue(modelPort) as List<Model>;
        }

        public override object GenerateOutput(NodePort port) {
            List<Model> models = GetModelList(GetInputByFieldName("model"));
            for (int i = 0; i < models.Count; i++) {
                models[i].mesh.RecalculateBounds();
            }

            return models;
        }
    }
}