using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Graphmesh {
    public class SplineData : GraphmeshNode {

        [Input] public Bezier3DSpline spline;
        [Output] public float length;
        [Output] public float pointCount;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            Bezier3DSpline spline = GetInputValue<Bezier3DSpline>("spline", this.spline);
            if (spline == null) return 0;

            if (port.fieldName == "length") {
                return spline.totalLength;
            } else if (port.fieldName == "pointCount") {
                return spline.KnotCount;
            } else return null;
        }
    }
}