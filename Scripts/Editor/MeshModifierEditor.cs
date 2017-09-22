using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Graphmesh {
    [InitializeOnLoad]
    [CustomEditor(typeof(MeshModifier))]
    public class MeshModifierEditor : Editor {

        static MeshModifierEditor() {
            Bezier3DSplineEditor.onUpdateSpline -= OnUpdateSpline;
            Bezier3DSplineEditor.onUpdateSpline += OnUpdateSpline;
            //NodeEditor.onNodeGuiChange -= OnUpdateNode;
            //NodeEditor.onNodeGuiChange += OnUpdateNode;
        }
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            MeshModifier modifier = target as MeshModifier;

            if (modifier.nodeGraph != null) {
                List<GraphmeshNode> inputNodes = GetGraphMeshNodes(modifier.nodeGraph);

                for (int i = 0; i < inputNodes.Count; i++) {
                    GraphmeshNode node = inputNodes[i];
                    for (int k = 0; k < node.ExposedInputs.Length; k++) {

                        Object obj = modifier.inputCache.GetCachedObject(node, k) as Object;
                        EditorGUI.BeginChangeCheck();
                        obj = EditorGUILayout.ObjectField(node.name + " " + k, obj, node.ExposedInputs[k], true);
                        if (obj is Component) {
                            Component c = obj as Component;
                            GameObject go = c.gameObject;
                            if (go == modifier.gameObject) {
                            }
                        }

                        if (EditorGUI.EndChangeCheck()) {
                            modifier.inputCache.Cache(node, obj, k);
                        }
                    }
                }
            }
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
                if (meshModifiers[i].inputCache.Contains(spline)) {
                    meshModifiers[i].Generate();
                }
            }
        }

        /*static void OnUpdateNode(NodeGraphAsset graph, Node node) {
            MeshModifier[] meshModifiers = FindObjectsOfType<MeshModifier>();
            for (int i = 0; i < meshModifiers.Length; i++) {
                if (meshModifiers[i].graph == graph) {
                    meshModifiers[i].Generate();
                }
            }
        }*/
    }
}