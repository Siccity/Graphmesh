using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Graphmesh {
    public class GenerateMeshCollider : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public List<Model> input;
        public bool convex;
        [Output] public List<Model> output;
        
        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            List<Model> models = GetModelList(GetInputByFieldName("input"));
            for (int i = 0; i < models.Count; i++) {
                MeshCollider meshCol = new MeshCollider();
                models[i].colType = Model.ColliderType.Mesh;
                models[i].meshCol = models[i].mesh;
                models[i].meshColConvex = convex;
            }

            return models;
        }
    }
}