using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Graphmesh {
    [CustomNodeEditor(typeof(FFD), "Graphmesh/FFD")]
    public class FFDNodeEditor : NodeEditor {
        protected override void OnBodyGUI() {
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
			var sp = serializedObject.FindProperty("output");
			if (sp != null) NodeEditorGUILayout.PropertyField(sp);
			else EditorGUILayout.LabelField("Na");
        }
    }
}