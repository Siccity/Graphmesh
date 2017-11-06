using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace Graphmesh {
    public class FillSpline : GraphmeshNode {

        [Input] public Bezier3DSpline spline;
        [Input] public Material material;
        [Input] public int resolution = 10;
        [Output] public ModelGroup output;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            Bezier3DSpline spline = GetInputValue<Bezier3DSpline>("spline", this.spline);
            Material material = GetInputValue<Material>("material", this.material);
            float resolution = GetInputValue<float>("resolution", this.resolution);

            if (spline != null) {
                List<Vector2> points = new List<Vector2>();
                for (int i = 0; i < spline.KnotCount; i++) {
                    Vector3 pos = spline.GetKnot(i).position;
                    points.Add(new Vector2(pos.x, pos.z));

                    Bezier3DCurve curve = spline.GetCurve(i);
                    if (!curve.isLinear) {
                        for (int k = 1; k < resolution; k++) {
                            float t = (float) k / resolution;
                            pos = curve.GetPoint(t);
                            points.Add(new Vector2(pos.x, pos.z));
                        }
                    }
                }
                int[] tris = Triangulate(points);
                List<Vector3> verts = points.Select(x => new Vector3(x.x, 0, x.y)).ToList();
                List<Vector3> norms = points.Select(x => Vector3.up).ToList();

                Mesh mesh = new Mesh();
                mesh.vertices = verts.ToArray();
                mesh.triangles = tris;
                mesh.normals = norms.ToArray();
                ModelGroup output = new ModelGroup();
                Material[] mats = new Material[] { material };
                output.Add(new Model(mesh, mats));
                return output;
            } else return null;
        }

        public int[] Triangulate(List<Vector2> points) {
            List<int> indices = new List<int>();

            int n = points.Count;
            if (n < 3)
                return indices.ToArray();

            int[] V = new int[n];
            if (Area(points) > 0) {
                for (int v = 0; v < n; v++)
                    V[v] = v;
            } else {
                for (int v = 0; v < n; v++)
                    V[v] = (n - 1) - v;
            }

            int nv = n;
            int count = 2 * nv;
            for (int m = 0, v = nv - 1; nv > 2;) {
                if ((count--) <= 0)
                    return indices.ToArray();

                int u = v;
                if (nv <= u)
                    u = 0;
                v = u + 1;
                if (nv <= v)
                    v = 0;
                int w = v + 1;
                if (nv <= w)
                    w = 0;

                if (Snip(u, v, w, nv, V, points)) {
                    int a, b, c, s, t;
                    a = V[u];
                    b = V[v];
                    c = V[w];
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(c);
                    m++;
                    for (s = v, t = v + 1; t < nv; s++, t++)
                        V[s] = V[t];
                    nv--;
                    count = 2 * nv;
                }
            }

            indices.Reverse();
            return indices.ToArray();
        }

        private float Area(List<Vector2> points) {
            int n = points.Count;
            float A = 0.0f;
            for (int p = n - 1, q = 0; q < n; p = q++) {
                Vector2 pval = points[p];
                Vector2 qval = points[q];
                A += pval.x * qval.y - qval.x * pval.y;
            }
            return (A * 0.5f);
        }

        private bool Snip(int u, int v, int w, int n, int[] V, List<Vector2> points) {
            int p;
            Vector2 A = points[V[u]];
            Vector2 B = points[V[v]];
            Vector2 C = points[V[w]];
            if (Mathf.Epsilon >(((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
                return false;
            for (p = 0; p < n; p++) {
                if ((p == u) || (p == v) || (p == w))
                    continue;
                Vector2 P = points[V[p]];
                if (InsideTriangle(A, B, C, P))
                    return false;
            }
            return true;
        }

        private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P) {
            float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            float cCROSSap, bCROSScp, aCROSSbp;

            ax = C.x - B.x;
            ay = C.y - B.y;
            bx = A.x - C.x;
            by = A.y - C.y;
            cx = B.x - A.x;
            cy = B.y - A.y;
            apx = P.x - A.x;
            apy = P.y - A.y;
            bpx = P.x - B.x;
            bpy = P.y - B.y;
            cpx = P.x - C.x;
            cpy = P.y - C.y;

            aCROSSbp = ax * bpy - ay * bpx;
            cCROSSap = cx * apy - cy * apx;
            bCROSScp = bx * cpy - by * cpx;

            return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
        }
    }
}