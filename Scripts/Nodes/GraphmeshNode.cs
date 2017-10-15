using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    /// <summary> Base node for the Graphmesh system </summary>
    public abstract class GraphmeshNode : Node {
        /// <summary> Override this before requesting model to use cache </summary>
        public static NodeCache currentOutputCache;

        public override object GetValue(NodePort port) {
            //Try to return a cached object
            object output = currentOutputCache.GetCachedObject(this, port.fieldName);
            if (output != null) return output;

            //If no cached objects are found, get all inputs and try to generate one.
            /*object[][] inputData = new object[InputCount][];
            for (int i = 0; i < InputCount; i++) {
                NodePort input = base.inputs[i];
                if (input == null || !input.IsConnected) {
                    inputData[i] = null;
                }
                inputData[i] = new object[input.ConnectionCount];
                for (int k = 0; k < input.ConnectionCount; k++) {
                    NodePort connectedPort = input.GetConnection(k);
                    GraphmeshNode node = connectedPort.node as GraphmeshNode;
                    inputData[i][k] = node.GetValue(connectedPort);
                }
            }*/
            output = GenerateOutput(port);
            return output;
        }

        public virtual bool TryGetValue<T>(NodePort port, out T value) {
            object obj = GetValue(port);
            if (obj is T) value = (T)obj;
            else value = default(T);
            return value != null;
        }

        /// <summary> Called when no object is found in cache. Generates a new output. </summary>        
        /// <param name="port">Requested port</param>
        /// <param name="inputs">All up-to-date inputs</param>
        public abstract object GenerateOutput(NodePort port);

        public List<Model> GetModelList(NodePort port) {
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
        }
    }
}