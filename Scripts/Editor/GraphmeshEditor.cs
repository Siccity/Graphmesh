using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Graphmesh {
    [InitializeOnLoad]
    [CustomEditor(typeof(Graphmesh))]
    public class GraphmeshEditor : Editor {

        public static bool displayDefaultInspector;

        static GraphmeshEditor() {
            Bezier3DSplineEditor.onUpdateSpline -= OnUpdateSpline;
            Bezier3DSplineEditor.onUpdateSpline += OnUpdateSpline;
            NodeEditor.onUpdateNode -= OnUpdateNode;
            NodeEditor.onUpdateNode += OnUpdateNode;
            FFDBoxEditor.onUpdateFFD -= OnUpdateFFD;
            FFDBoxEditor.onUpdateFFD += OnUpdateFFD;
        }

        public override void OnInspectorGUI() {

            Graphmesh graphmesh = target as Graphmesh;

            graphmesh.nodeGraph = EditorGUILayout.ObjectField("Node Graph", graphmesh.nodeGraph, typeof(GraphmeshGraph), true) as GraphmeshGraph;

            GUILayout.Space(20);

            //Display exposed inputs
            if (graphmesh.nodeGraph != null) {
                //Get all exposed inputs
                ExposedInput[] inputNodes = graphmesh.nodeGraph.nodes.Where(x => x is ExposedInput).Select(x => x as ExposedInput).ToArray();

                for (int i = 0; i < inputNodes.Length; i++) {
                    //We do this by modifying one node field directly and then copying the result to the other nodes
                    EditorGUI.BeginChangeCheck();
                    ExposedInput inputNode = inputNodes[i];
                    NodePort port = inputNode.GetOutputPort("value");
                    NodePort targetPort = port.Connection;
                    if (targetPort == null) continue;
                    Node targetNode = targetPort.node;
                    SerializedObject targetSo = new SerializedObject(targetNode);
                    SerializedProperty targetProperty = targetSo.FindProperty(targetPort.fieldName);
                    if (inputNode.GetOutputType().IsSubclassOf(typeof(Object))) {
                        Object obj = graphmesh.outputCache.GetCachedObject(inputNode, "value");
                        obj = EditorGUILayout.ObjectField(inputNode.label, obj, inputNode.GetOutputType(), true);
                        if (EditorGUI.EndChangeCheck()) {
                            graphmesh.outputCache.Cache(inputNode, obj, "value");
                            if (NodeEditor.onUpdateNode != null) NodeEditor.onUpdateNode(inputNode);
                        }

                    } else {
                        EditorGUILayout.PropertyField(targetProperty, new GUIContent(inputNode.label), true);
                        targetSo.ApplyModifiedProperties();

                        if (EditorGUI.EndChangeCheck()) {

                            object newValue = targetNode.GetType().GetField(targetProperty.propertyPath).GetValue(targetNode);

                            if (newValue is int) graphmesh.outputCache.Cache(inputNode, (int) newValue, "value");
                            else if (newValue is string) graphmesh.outputCache.Cache(inputNode, (string) newValue, "value");
                            else if (newValue is float) graphmesh.outputCache.Cache(inputNode, (float) newValue, "value");
                            else if (newValue is bool) graphmesh.outputCache.Cache(inputNode, (bool) newValue, "value");

                            if (NodeEditor.onUpdateNode != null) NodeEditor.onUpdateNode(inputNode);
                        }
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
            Graphmesh[] meshModifiers = FindObjectsOfType<Graphmesh>();
            for (int i = 0; i < meshModifiers.Length; i++) {
                if (meshModifiers[i].outputCache.Contains(spline)) {
                    meshModifiers[i].Generate();
                }
            }
        }

        static void OnUpdateFFD(FFDBox ffd) {
            Graphmesh[] meshModifiers = FindObjectsOfType<Graphmesh>();
            for (int i = 0; i < meshModifiers.Length; i++) {
                if (meshModifiers[i].outputCache.Contains(ffd)) {
                    meshModifiers[i].Generate();
                }
            }
        }

        static void OnUpdateNode(Node node) {
            Graphmesh[] meshModifiers = FindObjectsOfType<Graphmesh>();
            for (int i = 0; i < meshModifiers.Length; i++) {
                if (meshModifiers[i].nodeGraph == node.graph) {
                    meshModifiers[i].Generate();
                }
            }
        }
    }
}