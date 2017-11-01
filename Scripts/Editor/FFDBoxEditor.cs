using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Graphmesh {
    [CustomEditor(typeof(FFDBox))]
    public class FFDBoxEditor : Editor {

        public static Action<FFDBox> onUpdateFFD;

        void OnSceneGUI() {
            FFDBox ffd = target as FFDBox;
            Handles.DrawLine(ffd.l_000, ffd.l_001);
            Handles.DrawLine(ffd.l_000, ffd.l_010);
            Handles.DrawLine(ffd.l_000, ffd.l_100);
            Handles.DrawLine(ffd.l_111, ffd.l_101);
            Handles.DrawLine(ffd.l_111, ffd.l_110);
            Handles.DrawLine(ffd.l_111, ffd.l_011);

            Handles.DrawLine(ffd.l_101, ffd.l_001);
            Handles.DrawLine(ffd.l_110, ffd.l_100);
            Handles.DrawLine(ffd.l_011, ffd.l_010);
            Handles.DrawLine(ffd.l_100, ffd.l_101);
            Handles.DrawLine(ffd.l_010, ffd.l_110);
            Handles.DrawLine(ffd.l_001, ffd.l_011);

            EditorGUI.BeginChangeCheck();
            ffd.l_000 = Handles.PositionHandle(ffd.l_000, Quaternion.identity);
            ffd.l_001 = Handles.PositionHandle(ffd.l_001, Quaternion.identity);
            ffd.l_010 = Handles.PositionHandle(ffd.l_010, Quaternion.identity);
            ffd.l_011 = Handles.PositionHandle(ffd.l_011, Quaternion.identity);
            ffd.l_100 = Handles.PositionHandle(ffd.l_100, Quaternion.identity);
            ffd.l_101 = Handles.PositionHandle(ffd.l_101, Quaternion.identity);
            ffd.l_110 = Handles.PositionHandle(ffd.l_110, Quaternion.identity);
            ffd.l_111 = Handles.PositionHandle(ffd.l_111, Quaternion.identity);
            if (EditorGUI.EndChangeCheck()) {
                if (onUpdateFFD != null) onUpdateFFD(ffd);
            }
        }
    }
}