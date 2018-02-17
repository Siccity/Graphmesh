using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Graphmesh {
    public class FollowSpline : GraphmeshNode {

        [Input(ShowBackingValue.Never)] public ModelGroup input;
        [Input] public Bezier3DSpline spline;
        [Output] public ModelGroup output;
        public enum Axis { x = 0, y = 1, z = 2 }
        public Axis axis = Axis.z;

        public override object GetValue(NodePort port) {
            object o = base.GetValue(port);
            if (o != null) return o;

            ModelGroup[] input = GetInputValues<ModelGroup>("input", this.input);
            Bezier3DSpline spline = GetInputValue<Bezier3DSpline>("spline", this.spline);
            ModelGroup output = new ModelGroup();
            if (input == null) return output;

            // Loop through input model groups
            for (int mg = 0; mg < input.Length; mg++) {
                if (input[mg] == null) continue;
                // Loop through group models
                for (int i = 0; i < input[mg].Count; i++) {
                    Mesh mesh = input[mg][i].mesh.Copy();
                    if (spline != null) FollowCurve(mesh, spline);
                    output.Add(new Model(input[mg][i]) { mesh = mesh });
                }
            }
            return output;
        }

        private void FollowCurve(Mesh mesh, Bezier3DSpline spline) {
            int axis = (int) this.axis;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

#if false
            Vector3[] verts = mesh.vertices;
            Vector3[] norms = mesh.normals;
            Vector4[] tan = mesh.tangents;
            for (int i = 0; i < mesh.vertexCount; i++) {
                float dist = verts[i][axis];
                Vector3 offset = verts[i];
                offset[axis] = 0;

                Quaternion orientation = spline.GetOrientationLocalFast(dist);

                verts[i] = spline.GetPointLocal(dist) + (orientation * offset);
                norms[i] = orientation * norms[i];
                float w = tan[i].w;
                tan[i] = orientation * new Vector3(tan[i].x, tan[i].y, tan[i].z);
                tan[i].w = w;
            }
            mesh.SetVertices(new List<Vector3>(verts));
            mesh.SetNormals(new List<Vector3>(norms));
            mesh.SetTangents(new List<Vector4>(tan));
#else
            var splineRotation = spline.transform.rotation;
            Quaternion orientation = Quaternion.identity;
            Quaternion[] orientations = new Quaternion[spline.CurveCount];
            for (int i = 0; i < spline.CurveCount; i++)
            {
                spline.GetOrientationRaw(i, out orientations[i]);
            }

            var verts = mesh.vertices;
            var norms = mesh.normals;
            var tan = mesh.tangents;

            for (int i = 0; i < verts.Length; i++)
            {
                float dist = verts[i][axis];
                float curveTime;
                int index;

                var curve = spline.GetCurveIndexTime(dist, out index, out curveTime);
                Quaternion orientation1;
                GetOrientation(curve, orientations, ref splineRotation, index, curveTime, out orientation1);

                Vector3 offset = verts[i];
                offset[axis] = 0;

                curve.GetPoint(curveTime, out verts[i]);

                MultiplyOrientation(ref orientation1, ref norms[i], ref tan[i], ref offset, ref verts[i]);
            }
            mesh.vertices = verts;
            mesh.normals = norms;
            mesh.tangents = tan;
#endif
            sw.Stop();
            Debug.LogFormat("FollowSpline Inner loop {0} ticks, vertex count {1}", sw.ElapsedTicks, mesh.vertexCount);
        }

        #region Inner Loop helper methods, readability sacrificed for performance
        private static void GetOrientation(Bezier3DCurve curve, Quaternion[] orientations, ref Quaternion splineRotation, int index, float curveTime, out Quaternion orientation)
        {
            var nextIndex = index + 1;
            if (nextIndex == orientations.Length)
                nextIndex = index;

            Quaternion lerpedRotation;
            Quaternion rot;
            Lerp(ref orientations[index], ref orientations[nextIndex], curveTime, out lerpedRotation);
            Multiply(ref splineRotation, ref lerpedRotation, out rot);

            Vector3 rotatedUp;
            MultiplyUp(ref rot, out rotatedUp);

            Vector3 forward;
            curve.GetForward(curveTime, out forward);
            if (forward.x * forward.x + forward.y * forward.y + forward.z * forward.z == 0)
            {
                orientation = Quaternion.identity;
                return;
            }
            ProjectOnPlane(ref forward, ref rotatedUp);
            QuaternionLookRotation(ref forward, ref rotatedUp, out orientation);
        }

        private static void QuaternionLookRotation(ref Vector3 forward, ref Vector3 up, out Quaternion result)
        {
            Vector3 vector2;

            Cross(ref up, ref forward, out vector2);
            Vector3 vector3;
            Cross(ref forward, ref vector2, out vector3);


            float num8 = (vector2.x + vector3.y) + forward.z;
            if (num8 > 0f)
            {
                var num = Mathf.Sqrt(num8 + 1f);
                result.w = num * 0.5f;
                num = 0.5f / num;
                result.x = (vector3.z - forward.y) * num;
                result.y = (forward.x - vector2.z) * num;
                result.z = (vector2.y - vector3.x) * num;
                return;
            }
            if ((vector2.x >= vector3.y) && (vector2.x >= forward.z))
            {
                var num7 = Mathf.Sqrt(((1f + vector2.x) - vector3.y) - forward.z);
                var num4 = 0.5f / num7;
                result.x = 0.5f * num7;
                result.y = (vector2.y + vector3.x) * num4;
                result.z = (vector2.z + forward.x) * num4;
                result.w = (vector3.z - forward.y) * num4;
                return;
            }
            if (vector3.y > forward.z)
            {
                var num6 = Mathf.Sqrt(((1f + vector3.y) - vector2.x) - forward.z);
                var num3 = 0.5f / num6;
                result.x = (vector3.x + vector2.y) * num3;
                result.y = 0.5f * num6;
                result.z = (forward.y + vector3.z) * num3;
                result.w = (forward.x - vector2.z) * num3;
                return;
            }
            var num5 = Mathf.Sqrt(((1f + forward.z) - vector2.x) - vector3.y);
            var num2 = 0.5f / num5;
            result.x = (forward.x + vector2.z) * num2;
            result.y = (forward.y + vector3.z) * num2;
            result.z = 0.5f * num5;
            result.w = (vector2.y - vector3.x) * num2;
        }

        private static void Cross(ref Vector3 lhs, ref Vector3 rhs, out Vector3 result)
        {
            result.x = lhs.y * rhs.z - lhs.z * rhs.y;
            result.y = lhs.z * rhs.x - lhs.x * rhs.z;
            result.z = lhs.x * rhs.y - lhs.y * rhs.x;
        }

        static void Lerp(ref Quaternion a, ref Quaternion b, float t, out Quaternion result)
        {
            float t_ = 1 - t;
            result.x = t_* a.x + t* b.x;
            result.y = t_* a.y + t* b.y;
            result.z = t_* a.z + t* b.z;
            result.w = t_* a.w + t* b.w;

            var invLen = 1f / Mathf.Sqrt(result.x * result.x + result.y * result.y + result.z * result.z + result.w * result.w);
            result.x *= invLen;
            result.y *= invLen;
            result.z *= invLen;
            result.w *= invLen;
        }

        static void Multiply(ref Quaternion lhs, ref Quaternion rhs, out Quaternion result)
        {
            result.x = lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y;
            result.y = lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z;
            result.z = lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x;
            result.w = lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z;
        }

        static void MultiplyUp(ref Quaternion rotation, out Vector3 result)
        {
            float num = rotation.x * 2f;
            float num2 = rotation.y * 2f;
            float num3 = rotation.z * 2f;
            result.x = (rotation.x * num2 - rotation.w * num3);
            result.y = (1f - (rotation.x * num + rotation.z * num3));
            result.z = (rotation.y * num3 + rotation.w * num);
        }

        static void MultiplyOrientation(ref Quaternion rotation, ref Vector3 offset, ref Vector4 tan, ref Vector3 point, ref Vector3 result)
        {
            float num = rotation.x * 2f;
            float num2 = rotation.y * 2f;
            float num3 = rotation.z * 2f;
            float num4 = rotation.x * num;
            float num5 = rotation.y * num2;
            float num6 = rotation.z * num3;
            float num7 = rotation.x * num2;
            float num8 = rotation.x * num3;
            float num9 = rotation.y * num3;
            float num10 = rotation.w * num;
            float num11 = rotation.w * num2;
            float num12 = rotation.w * num3;

            float mulxx = (1f - (num5 + num6));
            float mulxy = (num7 - num12);
            float mulxz = (num8 + num11);

            float mulyx = (num7 + num12);
            float mulyy = (1f - (num4 + num6));
            float mulyz = (num9 - num10);

            float mulzx = (num8 - num11);
            float mulzy = (num9 + num10);
            float mulzz = (1f - (num4 + num5));

            result.x += mulxx * point.x + mulxy * point.y + mulxz * point.z;
            result.y += mulyx * point.x + mulyy * point.y + mulyz * point.z;
            result.z += mulzx * point.x + mulzy * point.y + mulzz * point.z;

            float px = offset.x;
            float py = offset.y;
            float pz = offset.z;
            offset.x = mulxx * px + mulxy * py + mulxz * pz;
            offset.y = mulyx * px + mulyy * py + mulyz * pz;
            offset.z = mulzx * px + mulzy * py + mulzz * pz;

            px = tan.x;
            py = tan.y;
            pz = tan.z;
            tan.x = mulxx * px + mulxy * py + mulxz * pz;
            tan.y = mulyx * px + mulyy * py + mulyz * pz;
            tan.z = mulzx * px + mulzy * py + mulzz * pz;
        }

        private static void ProjectOnPlane(ref Vector3 vector, ref Vector3 planeNormal)
        {
            float num = planeNormal.x * planeNormal.x + planeNormal.y * planeNormal.y + planeNormal.z * planeNormal.z;
            if (num > Mathf.Epsilon)
            {
                var scalar = (vector.x * planeNormal.x + vector.y * planeNormal.y + vector.z * planeNormal.z) / num;
                vector.x -= planeNormal.x * scalar;
                vector.y -= planeNormal.y * scalar;
                vector.z -= planeNormal.z * scalar;

                var invLen = 1f / Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
                vector.x *= invLen;
                vector.y *= invLen;
                vector.z *= invLen;
            }
        }
        #endregion
    }
}