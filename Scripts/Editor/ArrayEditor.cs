using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Graphmesh {
    [CustomNodeEditor(typeof(Array))]
    public class ArrayEditor : NodeEditor {

        public override void OnBodyGUI() {
            portPositions = new Dictionary<NodePort, Vector2>();
            Array arrayNode = target as Array;

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("input"), true);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("output"), true);

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("fitLength"), true);

            if (arrayNode.fitLength) {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("length"), true);
            } else {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("count"), true);
            }

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("posOffset"), true);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("rotOffset"), true);
        }
    }
}