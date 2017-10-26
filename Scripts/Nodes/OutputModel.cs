using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class OutputModel : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public List<Model> model;

        public List<Model> GetModels() {

            NodePort modelPort = GetInputByFieldName("model");
            return GetValue(modelPort) as List<Model>;
        }

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            List<Model> models = GetModelList(GetInputByFieldName("model"));
            for (int i = 0; i < models.Count; i++) {
                models[i].mesh.RecalculateBounds();
            }

            return models;
        }
    }
}