using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Graphmesh {
    public class MakeModel : GraphmeshNode {

        [Input] public Mesh mesh;
        [Input] public Material material;
        [Output] public ModelGroup output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            Mesh mesh = GetInputValue("mesh", this.mesh);
            Material material = GetInputValue("material", this.material);

            if (mesh == null) return new ModelGroup();
            //Fixme: Support for more than one material
            Model model = new Model(mesh.Copy(), new Material[] { material });
            return new ModelGroup() { model };
        }
    }
}