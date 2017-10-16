using System.Collections.Generic;
using UnityEngine;
using System;

namespace Graphmesh {
    [Serializable]
    public class NodeCache { 
        [SerializeField] private List<Item> items = new List<Item>();

        public void Cache(Node node, UnityEngine.Object value, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) items.Add(new Item(node, value, portName));
            else {
                item.value = value;
            }
        }

        public UnityEngine.Object GetCachedObject (Node node, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) {
                return null;
            }
            return item.value;
        }

        public bool Contains(UnityEngine.Object obj) {
            for (int i = 0; i < items.Count; i++) {
                if (items[i].value == obj) return true;
            }
            return false;
        }


        private Item GetItem(Node node, string portName) {
            for (int i = 0; i < items.Count; i++) {
                if (items[i].node == node && items[i].portName == portName) return items[i];
            }
            return null;
        }

        [Serializable]
        private class Item {
            public Item(Node node, UnityEngine.Object value, string portName) {
                this.node = node;
                this.value = value;
                this.portName = portName;
            }

            public Node node;
            public string portName;
            public UnityEngine.Object value;
        }
    }
}
