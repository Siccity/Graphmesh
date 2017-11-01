using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class FFDBox : MonoBehaviour {
        public FFDBoxSettings settings = FFDBoxSettings.Box;

        [System.Serializable]
        public struct FFDBoxSettings {
            public Vector3 v_000;
            public Vector3 v_001;
            public Vector3 v_010;
            public Vector3 v_011;
            public Vector3 v_100;
            public Vector3 v_101;
            public Vector3 v_110;
            public Vector3 v_111;

            public static FFDBoxSettings Box {
                get {
                    return new FFDBoxSettings {
                        v_001 = new Vector3(0, 0, 1),
                            v_010 = new Vector3(0, 1, 0),
                            v_011 = new Vector3(0, 1, 1),
                            v_100 = new Vector3(1, 0, 0),
                            v_000 = new Vector3(0, 0, 0),
                            v_101 = new Vector3(1, 0, 1),
                            v_110 = new Vector3(1, 1, 0),
                            v_111 = new Vector3(1, 1, 1)
                    };
                }
            }
        }
    }
}