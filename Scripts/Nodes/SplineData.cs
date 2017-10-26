using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class SplineData : GraphmeshNode {

        [Input] public Bezier3DSpline spline;
        [Output] public float length;
        [Output] public float pointCount;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            Bezier3DSpline spline = this.spline;
			NodePort splineInput = GetInputByFieldName("spline").Connection;
			if (splineInput != null) spline = splineInput.GetOutputValue() as Bezier3DSpline;

            if (port.fieldName == "length") {
				if (spline != null) return spline.totalLength;
				else return 0;
			}
			else if (port.fieldName == "pointCount") {
				if (spline != null) return spline.KnotCount;
				else return 0;
			}
			else return null;
        }
    }
}