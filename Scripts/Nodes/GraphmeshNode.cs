using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    /// <summary> Base node for the Graphmesh system </summary>
    public abstract class GraphmeshNode : Node {
        /// <summary> Override this before requesting model to use cache </summary>
        public static NodeCache currentOutputCache;

        /// <summary> Returns cached object if there is any. Otherwise returns null </summary>
        public override object GetValue(NodePort port) {
            //Try to return a cached object
            object output = currentOutputCache.GetCachedObject(this, port.fieldName);
            if (output != null) return output;
            //If no cached object could be returned, generate one
            else return null;
        }

        /*public List<Model> GetModelList(NodePort port) {
            List<Model> models = new List<Model>();
            for (int i = 0; i < port.ConnectionCount; i++) {
                NodePort connectedPort = port.GetConnection(i);
                GraphmeshNode inputNode = connectedPort.node as GraphmeshNode;
                List<Model> tempModels = new List<Model>();
                GameObject tempGo;
                if (inputNode.TryGetValue<List<Model>>(connectedPort, out tempModels)) {
                    models.AddRange(tempModels);
                } else if (inputNode.TryGetValue<GameObject>(connectedPort, out tempGo)) {
                    models.AddRange(Model.FromGameObject(tempGo));
                }
            }
            return models;
        }*/
    }
}