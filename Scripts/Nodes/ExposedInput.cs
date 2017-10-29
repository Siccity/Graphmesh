using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class ExposedInput : GraphmeshNode {

        public string label = "Unnamed";
        [Output] public object value;

        public override object GetValue(NodePort port) {
            System.Type type = GetOutputType();
            if (type == typeof(int)) return GraphmeshNode.currentOutputCache.GetCachedInt(this, "value");
            else if (type == typeof(float)) return GraphmeshNode.currentOutputCache.GetCachedFloat(this, "value");
            else if (type == typeof(bool)) return GraphmeshNode.currentOutputCache.GetCachedBool(this, "value");
            else if (type == typeof(string)) return GraphmeshNode.currentOutputCache.GetCachedString(this, "value");
            else if (type.IsSubclassOf(typeof(Object)) || type == typeof(Object)) return GraphmeshNode.currentOutputCache.GetCachedObject(this, "value");
            else return null;


        }

        public System.Type GetOutputType() {
            NodePort port = GetOutputByFieldName("value");
            if (port != null && port.IsConnected) return port.Connection.type;
            else return null;
        }
    }
}