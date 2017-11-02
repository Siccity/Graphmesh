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
            if (!currentOutputCache.IsCached(port.node, port.fieldName)) return null;
            object output = null;
            if (port.ValueType == typeof(int)) output = currentOutputCache.GetCachedInt(this, port.fieldName);
            else if (port.ValueType == typeof(float)) output = currentOutputCache.GetCachedFloat(this, port.fieldName);
            else if (port.ValueType == typeof(bool)) output = currentOutputCache.GetCachedBool(this, port.fieldName);
            else if (port.ValueType == typeof(string)) output = currentOutputCache.GetCachedString(this, port.fieldName);
            else if (port.ValueType.IsSubclassOf(typeof(Object)) || port.ValueType == typeof(Object)) output = currentOutputCache.GetCachedObject(this, port.fieldName);
            //Try to return a cached object
            if (output != null) return output;
            //If no cached object could be returned, generate one
            else return null;
        }
    }
}