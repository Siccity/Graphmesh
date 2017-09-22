using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class InputSpline : GraphmeshNode {

        public override Type[] ExposedInputs { get { return new Type[1] { typeof(Bezier3DSpline) }; } }

        protected override void Init() {
            name = "Input Spline";
            inputs = new NodePort[1];
            inputs[0] = CreateNodeInput("Spline", typeof(Bezier3DSpline));

            outputs = new NodePort[1];
            outputs[0] = CreateNodeOutput("Spline", typeof(Bezier3DSpline));
        }

        public override object GenerateOutput(int outputIndex, object[][] inputs) {
            if (inputs == null || inputs.Length == 0) return null;
            Bezier3DSpline spline = inputs[0][0] as Bezier3DSpline;
            if (spline == null) return null;
            return spline;
        }
    }
}