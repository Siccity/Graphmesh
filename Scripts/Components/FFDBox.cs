using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class FFDBox : MonoBehaviour {
        public Vector3 l_000 = new Vector3(0,0,0);
        public Vector3 l_001 = new Vector3(0,0,1);
        public Vector3 l_010 = new Vector3(0,1,0);
        public Vector3 l_011 = new Vector3(0,1,1);
        public Vector3 l_100 = new Vector3(1,0,0);
        public Vector3 l_101 = new Vector3(1,0,1);
        public Vector3 l_110 = new Vector3(1,1,0);
        public Vector3 l_111 = new Vector3(1,1,1);
    }
}