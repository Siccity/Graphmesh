using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Graphmesh {
    [System.Serializable]
    public class NodeCache {
        [SerializeField] private List<Item> items = new List<Item>();

        public bool IsCached(Node node, string portName) {
            for (int i = 0; i < items.Count; i++) {
                if (items[i].node == node && items[i].portName == portName) return true;
            }
            return false;
        }
        public void Cache(Node node, Object value, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) items.Add(new Item(node, value, portName));
            else item.value = value;
        }

        public void Cache(Node node, string value, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) items.Add(new Item(node, value, portName));
            else item.stringValue = value;
        }

        public void Cache(Node node, int value, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) items.Add(new Item(node, value, portName));
            else item.intValue = value;
        }

        public void Cache(Node node, bool value, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) items.Add(new Item(node, value, portName));
            else item.intValue = value ? 1 : 0;
        }

        public void Cache(Node node, float value, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) items.Add(new Item(node, value, portName));
            else item.floatValue = value;
        }

        public Object GetCachedObject(Node node, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) return null;
            return item.value;
        }

        public int GetCachedInt(Node node, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) return 0;
            return item.intValue;
        }

        public string GetCachedString(Node node, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) return "";
            return item.stringValue;
        }

        public bool GetCachedBool(Node node, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) return false;
            return item.intValue == 1;
        }

        public float GetCachedFloat(Node node, string portName) {
            Item item = GetItem(node, portName);
            if (item == null) return 0f;
            return item.floatValue;
        }

        public bool Contains(Object obj) {
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

        [System.Serializable]
        public class Item {
            public Node node;
            public string portName;
            public Object value;
            public int intValue;
            public float floatValue;
            public string stringValue;

            public Item(Node node, Object value, string portName) {
                this.node = node;
                this.value = value;
                this.portName = portName;
            }

            public Item(Node node, int value, string portName) {
                this.node = node;
                this.intValue = value;
                this.portName = portName;
            }

            public Item(Node node, float value, string portName) {
                this.node = node;
                this.floatValue = value;
                this.portName = portName;
            }

            public Item(Node node, bool value, string portName) {
                this.node = node;
                this.intValue = value ? 1 : 0;
                this.portName = portName;
            }

            public Item(Node node, string value, string portName) {
                this.node = node;
                this.stringValue = value;
                this.portName = portName;
            }
        }
    }
}