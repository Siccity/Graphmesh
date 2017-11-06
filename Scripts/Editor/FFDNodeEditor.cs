using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Graphmesh {
    [CustomNodeEditor(typeof(FFD))]
    public class FFDNodeEditor : NodeEditor {
        public override void OnBodyGUI() {
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("input"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("reference"));
            bool reference = serializedObject.FindProperty("reference").boolValue;
            if (reference) {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("ffdBox"));
            } else {
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("v_000"));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("v_001"));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("v_010"));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("v_011"));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("v_100"));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("v_101"));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("v_110"));
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("v_111"));
            }
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("output"));
        }
    }
}