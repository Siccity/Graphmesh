using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace Graphmesh {
    public class Extrude : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public ModelGroup input;
        [Input] public float distance;
        [Output] public ModelGroup output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            //Get inputs
            ModelGroup[] input = GetInputValues<ModelGroup>("input", this.input);
            float distance = GetInputValue<float>("distance", this.distance);

            ModelGroup output = new ModelGroup();

            if (input == null) return output;
            // Loop through input model groups
            for (int mg = 0; mg < input.Length; mg++) {
                if (input[mg] == null) continue;
                // Loop through group models
                for (int i = 0; i < input[mg].Count; i++) {
                    Mesh mesh = input[mg][i].mesh.Copy();

                    Vector3[] prevVerts = mesh.vertices;
                    List<Vector3> verts = new List<Vector3>();
                    mesh.GetVertices(verts);
                    verts.CopyTo(prevVerts, 0);
                    List<Vector3> norms = new List<Vector3>();
                    mesh.GetNormals(norms);
                    while (norms.Count < verts.Count) norms.Add(Vector3.up);

                    //Move existing verts
                    for (int k = 0; k < verts.Count; k++) {
                        if (norms.Count > k) verts[k] += norms[k] * distance;
                        else verts[k] += Vector3.up * distance;
                    }

                    //Add sides
                    List<int> edges = GetEdges(mesh.triangles);
                    List<int> tris = new List<int>();
                    mesh.GetTriangles(tris, 0);
                    for (int k = 0; k < edges.Count; k += 2) {
                        Vector3 up = norms[k];
                        Vector3 ab = (prevVerts[edges[k]] - prevVerts[edges[k + 1]]).normalized;
                        verts.Add(verts[edges[k]]);
                        verts.Add(verts[edges[k + 1]]);
                        verts.Add(prevVerts[edges[k]]);
                        verts.Add(prevVerts[edges[k + 1]]);
                        tris.Add(verts.Count - 1);
                        tris.Add(verts.Count - 4);
                        tris.Add(verts.Count - 2);
                        tris.Add(verts.Count - 1);
                        tris.Add(verts.Count - 3);
                        tris.Add(verts.Count - 4);
                        Vector3 norm = Vector3.Cross(verts[verts.Count - 1] - verts[verts.Count - 2], verts[verts.Count - 3] - verts[verts.Count - 1]);
                        norm = norm.normalized;
                        norms.Add(norm);
                        norms.Add(norm);
                        norms.Add(norm);
                        norms.Add(norm);

                    }

                    mesh.SetVertices(verts);
                    mesh.SetNormals(norms);
                    mesh.SetTriangles(tris, 0);
                    output.Add(new Model(input[mg][i]) { mesh = mesh });
                }
            }
            return output;
        }

        /// <summary> Returns edges with only 1 connected face </summary>
        public List<int> GetEdges(int[] tris) {
            List<int> edges = new List<int>();
            for (int i = 0; i < tris.Length; i += 3) {
                bool solidA = false, solidB = false, solidC = false;
                for (int k = 0; k < tris.Length; k += 3) {
                    if (!solidA) {
                        if ((tris[i + 0] == tris[k + 1] && tris[i + 1] == tris[k + 0])) solidA = true;
                        if ((tris[i + 0] == tris[k + 2] && tris[i + 1] == tris[k + 1])) solidA = true;
                        if ((tris[i + 0] == tris[k + 0] && tris[i + 1] == tris[k + 2])) solidA = true;
                    }
                    if (!solidB) {
                        if ((tris[i + 1] == tris[k + 1] && tris[i + 2] == tris[k + 0])) solidB = true;
                        if ((tris[i + 1] == tris[k + 2] && tris[i + 2] == tris[k + 1])) solidB = true;
                        if ((tris[i + 1] == tris[k + 0] && tris[i + 2] == tris[k + 2])) solidB = true;
                    }
                    if (!solidC) {
                        if ((tris[i + 2] == tris[k + 1] && tris[i + 0] == tris[k + 0])) solidC = true;
                        if ((tris[i + 2] == tris[k + 2] && tris[i + 0] == tris[k + 1])) solidC = true;
                        if ((tris[i + 2] == tris[k + 0] && tris[i + 0] == tris[k + 2])) solidC = true;
                    }
                    if (solidA && solidB && solidC) break;
                }
                if (!solidA) {
                    edges.Add(tris[i + 0]);
                    edges.Add(tris[i + 1]);
                }
                if (!solidB) {
                    edges.Add(tris[i + 1]);
                    edges.Add(tris[i + 2]);
                }
                if (!solidC) {
                    edges.Add(tris[i + 2]);
                    edges.Add(tris[i + 0]);
                }
            }
            return edges;
        }
    }
}