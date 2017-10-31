using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Graphmesh {
    [CustomPropertyDrawer(typeof(FFDBox))]
    public class FFDPropertyDrawer : PropertyDrawer {

        private static SerializedProperty property;

        [InitializeOnLoadMethod]
        private static void OnLoad() {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            FFDPropertyDrawer.property = property;
            //EditorGUI.PropertyField(position, property, true);
            position.width *= 0.5f;
            EditorGUI.LabelField(position, label);
            /*position.x += position.width;
            bool edit = active.Contains(property);
            EditorGUI.BeginChangeCheck();
            edit = EditorGUI.Toggle(position, "edit", edit);
            if (EditorGUI.EndChangeCheck()) {
                Debug.Log("edit" + edit);

                if (edit) active.Add(property);
                else active.Remove(property);
                Debug.Log(active.Contains(property));
            }*/
        }

        private static void OnSceneGUI(SceneView sceneView) {
            if (Selection.activeGameObject != null && property != null) {
                Vector3 l_000 = property.FindPropertyRelative("l_000").vector3Value;
                Vector3 l_001 = property.FindPropertyRelative("l_001").vector3Value;
                Vector3 l_010 = property.FindPropertyRelative("l_010").vector3Value;
                Vector3 l_011 = property.FindPropertyRelative("l_011").vector3Value;
                Vector3 l_100 = property.FindPropertyRelative("l_100").vector3Value;
                Vector3 l_101 = property.FindPropertyRelative("l_101").vector3Value;
                Vector3 l_110 = property.FindPropertyRelative("l_110").vector3Value;
                Vector3 l_111 = property.FindPropertyRelative("l_111").vector3Value;
                Handles.DrawLine(l_000, l_001);
                Handles.DrawLine(l_000, l_010);
                Handles.DrawLine(l_000, l_100);
                Handles.DrawLine(l_111, l_101);
                Handles.DrawLine(l_111, l_110);
                Handles.DrawLine(l_111, l_011);

                Handles.DrawLine(l_101, l_001);
                Handles.DrawLine(l_110, l_100);
                Handles.DrawLine(l_011, l_010);
                Handles.DrawLine(l_100, l_101);
                Handles.DrawLine(l_010, l_110);
                Handles.DrawLine(l_001, l_011);

                property.FindPropertyRelative("l_000").vector3Value = Handles.PositionHandle(l_000, Quaternion.identity);
                property.FindPropertyRelative("l_001").vector3Value = Handles.PositionHandle(l_001, Quaternion.identity);
                property.FindPropertyRelative("l_010").vector3Value = Handles.PositionHandle(l_010, Quaternion.identity);
                property.FindPropertyRelative("l_011").vector3Value = Handles.PositionHandle(l_011, Quaternion.identity);
                property.FindPropertyRelative("l_100").vector3Value = Handles.PositionHandle(l_100, Quaternion.identity);
                property.FindPropertyRelative("l_101").vector3Value = Handles.PositionHandle(l_101, Quaternion.identity);
                property.FindPropertyRelative("l_110").vector3Value = Handles.PositionHandle(l_110, Quaternion.identity);
                property.FindPropertyRelative("l_111").vector3Value = Handles.PositionHandle(l_111, Quaternion.identity);
            }
        }
    }
}