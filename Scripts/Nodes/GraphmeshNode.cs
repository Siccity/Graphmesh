using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Graphmesh {
    public abstract class GraphmeshNode : Node {
        /// <summary> Override this before requesting model to use cache </summary>
        public static NodeCache currentInputCache, currentOutputCache;
        
        /// <summary> How many inputs to expose. A value of 2 exposes input 0 and 1 </summary>
        public virtual Type[] ExposedInputs { get { return new Type[0]; } }

        public object GetOutputValue(int outputIndex) {
            //Try to return a cached object
            object output = currentOutputCache.GetCachedObject(this, outputIndex);
            if (output != null) return output;
            //If no cached objects are found, get all inputs and try to generate one.
            else {
                object[][] inputs = new object[InputCount][];
                for (int i = 0; i < InputCount; i++) {
                    if (i < ExposedInputs.Length) {
                        inputs[i] = new object[1] { currentInputCache.GetCachedObject(this, i) };
                    }
                    else {
                        NodePort input = GetInput(i);
                        if (input == null || !input.IsConnected) {
                            inputs[i] = null;
                        }
                        inputs[i] = new object[input.ConnectionCount];
                        for (int k = 0; k < input.ConnectionCount; k++) {
                            GraphmeshNode node = input.GetConnection(k).node as GraphmeshNode;
                            int inputOutputIndex = node.GetOutputId(input.GetConnection(k));
                            if (inputOutputIndex == -1) Debug.Log("output not found");
                            else inputs[i][k] = node.GetOutputValue(inputOutputIndex);
                        }
                    }
                }
                output = GenerateOutput(outputIndex, inputs);
                return output;
                
            }
            
        }

        /// <summary> Called when no object is found in cache. Generates a new output. </summary>        
        /// <param name="i">Index of output to be generated</param>
        /// <param name="inputs">All up-to-date inputs</param>
        public abstract object GenerateOutput(int outputIndex, object[][] inputs);

        protected List<Model> UnpackModels(int index, object[][] input) {
            List<Model> result = new List<Model>();
            if (input == null || input.Length <= index) return result;
            else {
                if (input[index] == null) return result;
                for (int i = 0; i < input[index].Length; i++) {
                    if (input[index][i] is List<Model>) {
                        List<Model> models = input[index][i] as List<Model>;
                        result.AddRange(models);
                    }
                }
                return result;
            }
        }
    }
}
