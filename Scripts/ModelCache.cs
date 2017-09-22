using System.Collections.Generic;
using UnityEngine;
using System;

namespace Graphmesh {
    [Serializable]
    public class NodeCache { 
        [SerializeField] private List<Item> items;

        public void Cache(Node node, UnityEngine.Object value, int portIndex) {
            Item item = GetItem(node, portIndex);
            if (item == null) items.Add(new Item(node, value, portIndex));
            else {
                item.value = value;
            }
        }

        public UnityEngine.Object GetCachedObject (Node node, int portIndex) {
            Item item = GetItem(node, portIndex);
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


        private Item GetItem(Node node, int portIndex) {
            for (int i = 0; i < items.Count; i++) {
                if (items[i].node == node && items[i].portIndex == portIndex) return items[i];
            }
            return null;
        }

        [Serializable]
        private class Item {
            public Item(Node node, UnityEngine.Object value, int portIndex) {
                this.node = node;
                this.value = value;
                this.portIndex = portIndex;
            }

            public Node node;
            public int portIndex;
            public UnityEngine.Object value;
        }
    }
}
