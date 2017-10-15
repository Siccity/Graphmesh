using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphmesh {
    public class ExposedInput : GraphmeshNode {

        public string label = "Unnamed";
        [Output(false)] public object value;

        public System.Type GetOutputType() {
            NodePort port = GetOutputByFieldName("value");
            if (port.IsConnected) {
                if (port.Connection.type == typeof(List<Model>)) return typeof(GameObject);
                return port.Connection.type;
            } else return null;
        }

        private List<Model> ModelsFromGameObjects(Transform root) {
            List<Model> output = new List<Model>();

            MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++) {
                MeshRenderer renderer = renderers[i];
                MeshFilter filter = renderer.GetComponent<MeshFilter>();
                if (filter == null) continue;
                Mesh mesh = BakeMeshTransform(root, filter);
                Material[] materials = renderer.sharedMaterials;
                output.Add(new Model(mesh, materials));
            }
            return output;
        }

        private Mesh BakeMeshTransform(Transform root, MeshFilter filter) {
            Mesh mesh = filter.sharedMesh.Copy();
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector4> tangent = new List<Vector4>();
            mesh.GetVertices(verts);
            mesh.GetNormals(norms);
            mesh.GetTangents(tangent);

            for (int i = 0; i < verts.Count; i++) {
                //Vert position
                Vector3 world = filter.transform.TransformPoint(verts[i]); //Transform from filter local to world
                Vector3 local = root.InverseTransformPoint(world); //Transform from world to root local 
                verts[i] = local;

                //Vert normal
                world = filter.transform.TransformDirection(norms[i]);
                local = root.InverseTransformDirection(world);
                norms[i] = local;

                //Vert tangent
                float w = tangent[i].w;
                world = filter.transform.TransformDirection(tangent[i]);
                local = root.InverseTransformDirection(world);
                tangent[i] = new Vector4(local.x, local.y, local.z, w);
            }
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetTangents(tangent);
            return mesh;
        }

        public override object GenerateOutput(NodePort port) {
            return null;
        }
    }
}