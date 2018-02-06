using System;
using Graphmesh;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(ExposedInput))]
public class ExposedInputEditor : NodeEditor {

	public override void OnBodyGUI() {
		ExposedInput node = target as ExposedInput;
		EditorGUILayout.PropertyField(serializedObject.FindProperty("label"));
		Type type = node.GetOutputType();
		string typeName = type != null ? type.ToString() : "Not set";
		NodeEditorGUILayout.PortField(new GUIContent(typeName, typeName), node.GetOutputPort("value"));
	}
}