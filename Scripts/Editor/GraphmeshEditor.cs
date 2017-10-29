using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
        }

        public override void OnInspectorGUI() {

            Graphmesh graphmesh = target as Graphmesh;

            graphmesh.nodeGraph = EditorGUILayout.ObjectField("Node Graph", graphmesh.nodeGraph, typeof(GraphmeshNodeGraph), true) as GraphmeshNodeGraph;

            GUILayout.Space(20);

            //Display exposed inputs
            if (graphmesh.nodeGraph != null) {
                //Get all exposed inputs
                ExposedInput[] inputNodes = graphmesh.nodeGraph.nodes.Where(x => x is ExposedInput).Select(x => x as ExposedInput).ToArray();

                for (int i = 0; i < inputNodes.Length; i++) {
                    ExposedInput inputNode = inputNodes[i];
                    System.Type type = inputNode.GetOutputType();
                    string label = inputNode.label;

                    EditorGUI.BeginChangeCheck();
                    if (type == null) continue;
                    else if (type == typeof(int)) {
                        int value = graphmesh.outputCache.GetCachedInt(inputNode, "value");
                        value = EditorGUILayout.IntField(label, value);
                        if (EditorGUI.EndChangeCheck()) {
                            graphmesh.outputCache.Cache(inputNode, value, "value");
                            NodeEditor.onUpdateNode(inputNode);
                        }
                    }
                    else if (type == typeof(float)) {
                        float value = graphmesh.outputCache.GetCachedFloat(inputNode, "value");
                        value = EditorGUILayout.FloatField(label, value);
                        if (EditorGUI.EndChangeCheck()) {
                            graphmesh.outputCache.Cache(inputNode, value, "value");
                            NodeEditor.onUpdateNode(inputNode);
                        }
                    }
                    else if (type == typeof(string)) {
                        string value = graphmesh.outputCache.GetCachedString(inputNode, "value");
                        value = EditorGUILayout.TextField(label, value);
                        if (EditorGUI.EndChangeCheck()) {
                            graphmesh.outputCache.Cache(inputNode, value, "value");
                            NodeEditor.onUpdateNode(inputNode);
                        }
                    }
                    else if (type == typeof(bool)) {
                        bool value = graphmesh.outputCache.GetCachedBool(inputNode, "value");
                        value = EditorGUILayout.Toggle(label, value);
                        if (EditorGUI.EndChangeCheck()) {
                            graphmesh.outputCache.Cache(inputNode, value, "value");
                            NodeEditor.onUpdateNode(inputNode);
                        }
                    }
                    else if (type.IsSubclassOf(typeof(Object)) || type == typeof(Object)) {
                        Object value = graphmesh.outputCache.GetCachedObject(inputNode, "value");
                        value = EditorGUILayout.ObjectField(label, value, type, true);
                        if (EditorGUI.EndChangeCheck()) {
                            graphmesh.outputCache.Cache(inputNode, value, "value");
                            NodeEditor.onUpdateNode(inputNode);
                        }
                    }
                    else {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.LabelField(label, "Unsupported type ("+type.ToString()+")");
                        EditorGUI.EndDisabledGroup();
                        EditorGUI.EndChangeCheck();
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