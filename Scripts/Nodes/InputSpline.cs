using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class InputSpline : GraphmeshNode {

        [Input] public Bezier3DSpline input;
        [Output] public Bezier3DSpline output;
        public override Type[] ExposedInputs { get { return new Type[1] { typeof(Bezier3DSpline) }; } }

        protected override void Init() {
            name = "Input Spline";
        }

        public override object GenerateOutput(int outputIndex, object[][] inputs) {
            if (inputs == null || inputs.Length == 0) return null;
            Bezier3DSpline spline = inputs[0][0] as Bezier3DSpline;
            if (spline == null) return null;
            return spline;
        }
    }
}