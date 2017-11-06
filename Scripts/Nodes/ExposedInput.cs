using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Graphmesh {
    public class ExposedInput : GraphmeshNode {

        public string label = "Unnamed";
        [Output] public bool value;

        public override object GetValue(NodePort port) {
            System.Type type = GetOutputType();
            if (type == typeof(int)) return currentOutputCache.GetCachedInt(this, "value");
            else if (type == typeof(float)) return currentOutputCache.GetCachedFloat(this, "value");
            else if (type == typeof(bool)) return currentOutputCache.GetCachedBool(this, "value");
            else if (type == typeof(string)) return currentOutputCache.GetCachedString(this, "value");
            else if (type.IsSubclassOf(typeof(Object)) || type == typeof(Object)) return currentOutputCache.GetCachedObject(this, "value");
            else return null;
        }

        public System.Type GetOutputType() {
            NodePort port = GetOutputPort("value");
            if (port != null && port.IsConnected) return port.Connection.ValueType;
            else return null;
        }
    }
}