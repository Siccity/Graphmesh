using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Graphmesh {
    [CustomNodeEditor(typeof(Array), "Graphmesh/Array")]
    public class ArrayEditor : NodeEditor {

        protected override void OnBodyGUI(out Dictionary<NodePort, Vector2> portPositions) {
            portPositions = new Dictionary<NodePort, Vector2>();
            Array array = target as Array;
            NodePort modelPort = array.GetInputByFieldName("model");
            Vector2 portPos;
            array.model = NodeEditorGUILayout.PortField("Model", array.model, typeof(List<Model>), modelPort, false, out portPos) as List<Model>;
            portPositions.Add(modelPort, portPos);

            NodePort outputPort = array.GetOutputByFieldName("output");
            NodeEditorGUILayout.PortField("Output", null, typeof(List<Model>), outputPort, false, out portPos);
            portPositions.Add(outputPort, portPos);

            array.fitLength = NodeEditorGUILayout.Toggle("Fit Length", array.fitLength);

            if (array.fitLength) {
                NodePort lengthPort = array.GetInputByFieldName("length");
				array.length = (float)NodeEditorGUILayout.PortField("Length", array.length, typeof(float), lengthPort, !lengthPort.IsConnected, out portPos);
                portPositions.Add(lengthPort, portPos);
            } else {
                NodePort countPort = array.GetInputByFieldName("count");
                array.count = (int)NodeEditorGUILayout.PortField("Count", array.count, typeof(int), countPort, !countPort.IsConnected, out portPos);
                portPositions.Add(countPort, portPos);
            }

			array.posOffset = NodeEditorGUILayout.Vector3Field("Pos Offset",array.posOffset);
			array.rotOffset = NodeEditorGUILayout.Vector3Field("Rot Offset",array.rotOffset);

			if (GUI.changed) NodeEditor.onUpdateNode(array);
        }
    }
}