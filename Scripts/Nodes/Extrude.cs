using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Graphmesh {
    public class Extrude : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public List<Model> input;
        [Input] public float distance;
        [Output] public List<Model> output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            List<Model> input = GetModelList(GetInputByFieldName("input"));
            float distance = 0;
            object obj = GetInputByFieldName("distance").GetInputValue();
            if (obj != null) distance = (float)obj;
            else distance = this.distance;
            for (int i = 0; i < input.Count; i++) {
                Vector3[] prevVerts = input[i].mesh.vertices;
                List<Vector3> verts = new List<Vector3>();
                input[i].mesh.GetVertices(verts);
                verts.CopyTo(prevVerts,0);
                List<Vector3> norms = new List<Vector3>();
                input[i].mesh.GetNormals(norms);
                while (norms.Count < verts.Count) norms.Add(Vector3.up);

                //Move existing verts
                for (int k = 0; k < verts.Count; k++) {
                    if (norms.Count > k) verts[k] += norms[k] * distance;
                    else verts[k] += Vector3.up * distance;
                }

                //Add sides
                List<int> edges = GetEdges(input[i].mesh.triangles);
                List<int> tris = new List<int>();
                input[i].mesh.GetTriangles(tris,0);
                for (int k = 0; k < edges.Count; k += 2) {
                    Vector3 up = norms[k];
                    Vector3 ab = (prevVerts[edges[k]] - prevVerts[edges[k+1]]).normalized;
                    verts.Add(verts[edges[k]]);
                    verts.Add(verts[edges[k+1]]);
                    verts.Add(prevVerts[edges[k]]);
                    verts.Add(prevVerts[edges[k+1]]);
                    tris.Add(verts.Count - 1);
                    tris.Add(verts.Count - 4);
                    tris.Add(verts.Count - 2);
                    tris.Add(verts.Count - 1);
                    tris.Add(verts.Count - 3);
                    tris.Add(verts.Count - 4);
                    Vector3 norm = Vector3.Cross(verts[verts.Count - 1]- verts[verts.Count - 2], verts[verts.Count - 3] - verts[verts.Count - 1]);
                    norm = norm.normalized;
                    norms.Add(norm);
                    norms.Add(norm);
                    norms.Add(norm);
                    norms.Add(norm);


                }

                input[i].mesh.SetVertices(verts);
                input[i].mesh.SetNormals(norms);
                input[i].mesh.SetTriangles(tris,0);
            }
            return input;
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