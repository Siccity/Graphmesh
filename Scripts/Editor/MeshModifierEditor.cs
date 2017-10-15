using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Graphmesh {
    [InitializeOnLoad]
    [CustomEditor(typeof(MeshModifier))]
    public class MeshModifierEditor : Editor {

        public static bool displayDefaultInspector;

        static MeshModifierEditor() {
            Bezier3DSplineEditor.onUpdateSpline -= OnUpdateSpline;
            Bezier3DSplineEditor.onUpdateSpline += OnUpdateSpline;
            NodeEditor.onUpdateNode -= OnUpdateNode;
            NodeEditor.onUpdateNode += OnUpdateNode;
        }

        public override void OnInspectorGUI() {

            MeshModifier modifier = target as MeshModifier;

            modifier.nodeGraph = EditorGUILayout.ObjectField("Node Graph", modifier.nodeGraph, typeof(GraphmeshNodeGraph), true) as GraphmeshNodeGraph;

            //Display exposed inputs
            if (modifier.nodeGraph != null) {
                //Get all exposed inputs
                ExposedInput[] inputNodes = modifier.nodeGraph.nodes.Where(x => x is ExposedInput).Select(x => x as ExposedInput).ToArray();

                for (int i = 0; i < inputNodes.Length; i++) {
                    ExposedInput inputNode = inputNodes[i];
                    System.Type inputType = inputNode.GetOutputType();

                    //If inputType is null, it means the ExposedInput node isn't connected. Don't display it.
                    if (inputType == null) continue;

                    Object obj = modifier.outputCache.GetCachedObject(inputNode, "value") as Object;
                    EditorGUI.BeginChangeCheck();
                    obj = EditorGUILayout.ObjectField(inputNode.label, obj, inputNode.GetOutputType(), true);

                    if (EditorGUI.EndChangeCheck()) {
                        modifier.outputCache.Cache(inputNode, obj, "value");
                    }
                }
            }
            GUILayout.Space(20);

            displayDefaultInspector = EditorGUILayout.Toggle("Default Inspector", displayDefaultInspector);
            if (displayDefaultInspector) base.OnInspectorGUI();
        }

        List<GraphmeshNode> GetGraphMeshNodes(NodeGraph graph) {
            List<GraphmeshNode> output = new List<GraphmeshNode>();
            for (int i = 0; i < graph.nodes.Count; i++) {
                if (graph.nodes[i] is GraphmeshNode) output.Add(graph.nodes[i] as GraphmeshNode);
            }
            return output;
        }

        static void OnUpdateSpline(Bezier3DSpline spline) {
            MeshModifier[] meshModifiers = FindObjectsOfType<MeshModifier>();
            for (int i = 0; i < meshModifiers.Length; i++) {
                if (meshModifiers[i].outputCache.Contains(spline)) {
                    meshModifiers[i].Generate();
                }
            }
        }

        static void OnUpdateNode(Node node) {
            MeshModifier[] meshModifiers = FindObjectsOfType<MeshModifier>();
            for (int i = 0; i < meshModifiers.Length; i++) {
                if (meshModifiers[i].nodeGraph == node.graph) {
                    meshModifiers[i].Generate();
                }
            }
        }
    }
}