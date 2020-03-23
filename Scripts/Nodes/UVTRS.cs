using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace Graphmesh {
	[CreateNodeMenu("Modifiers (UV)/TRS")]
	public class UVTRS : GraphmeshNode {

		[Input(ShowBackingValue.Never)] public ModelGroup input;
		[Input] public Vector2 t;
		[Input] public float r;
		[Input] public Vector2 s = Vector2.one;
		[Output] public ModelGroup output;

		public override object GetValue(NodePort port) {
			object o = base.GetValue(port);
			if (o != null) return o;

			// Get inputs
			ModelGroup[] input = GetInputValues("input", this.input);
			Vector2 t = GetInputValue("t", this.t);
			float r = GetInputValue("r", this.r);
			Vector2 s = GetInputValue("s", this.s);

			ModelGroup output = new ModelGroup();

			// Loop through input model groups
			for (int mg = 0; mg < input.Length; mg++) {
				if (input[mg] == null) continue;
				// Loop through group models
				for (int i = 0; i < input[mg].Count; i++) {
					Mesh mesh = input[mg][i].mesh.Copy();
					Vector2[] uvs = mesh.uv;
					for (int u = 0; u < uvs.Length; u++) {
						uvs[u] += t;
						// TODO rotation
						uvs[u] *= s;
					}
					mesh.uv = uvs;
					output.Add(new Model(input[mg][i]) { mesh = mesh });
				}
			}
			return output;
		}
	}
}