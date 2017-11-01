using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Graphmesh {
    [CustomEditor (typeof (FFDBox))]
    public class FFDBoxEditor : Editor {

        public static Action<FFDBox> onUpdateFFD;

        void OnSceneGUI () {
            FFDBox ffd = target as FFDBox;

            Vector3 l_000 = ffd.transform.TransformPoint (ffd.l_000);
            Vector3 l_001 = ffd.transform.TransformPoint (ffd.l_001);
            Vector3 l_010 = ffd.transform.TransformPoint (ffd.l_010);
            Vector3 l_011 = ffd.transform.TransformPoint (ffd.l_011);
            Vector3 l_100 = ffd.transform.TransformPoint (ffd.l_100);
            Vector3 l_101 = ffd.transform.TransformPoint (ffd.l_101);
            Vector3 l_110 = ffd.transform.TransformPoint (ffd.l_110);
            Vector3 l_111 = ffd.transform.TransformPoint (ffd.l_111);

            Handles.DrawLine (l_000, l_001);
            Handles.DrawLine (l_000, l_010);
            Handles.DrawLine (l_000, l_100);
            Handles.DrawLine (l_111, l_101);
            Handles.DrawLine (l_111, l_110);
            Handles.DrawLine (l_111, l_011);

            Handles.DrawLine (l_101, l_001);
            Handles.DrawLine (l_110, l_100);
            Handles.DrawLine (l_011, l_010);
            Handles.DrawLine (l_100, l_101);
            Handles.DrawLine (l_010, l_110);
            Handles.DrawLine (l_001, l_011);

            EditorGUI.BeginChangeCheck ();
            l_000 = Handles.PositionHandle (l_000, ffd.transform.rotation);
            l_001 = Handles.PositionHandle (l_001, ffd.transform.rotation);
            l_010 = Handles.PositionHandle (l_010, ffd.transform.rotation);
            l_011 = Handles.PositionHandle (l_011, ffd.transform.rotation);
            l_100 = Handles.PositionHandle (l_100, ffd.transform.rotation);
            l_101 = Handles.PositionHandle (l_101, ffd.transform.rotation);
            l_110 = Handles.PositionHandle (l_110, ffd.transform.rotation);
            l_111 = Handles.PositionHandle (l_111, ffd.transform.rotation);

            ffd.l_000 = ffd.transform.InverseTransformPoint(l_000);
            ffd.l_001 = ffd.transform.InverseTransformPoint(l_001);
            ffd.l_010 = ffd.transform.InverseTransformPoint(l_010);
            ffd.l_011 = ffd.transform.InverseTransformPoint(l_011);
            ffd.l_100 = ffd.transform.InverseTransformPoint(l_100);
            ffd.l_101 = ffd.transform.InverseTransformPoint(l_101);
            ffd.l_110 = ffd.transform.InverseTransformPoint(l_110);
            ffd.l_111 = ffd.transform.InverseTransformPoint(l_111);
            if (EditorGUI.EndChangeCheck ()) {
                if (onUpdateFFD != null) onUpdateFFD (ffd);
            }
        }
    }
}