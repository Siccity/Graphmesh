using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMesh : MonoBehaviour {
	private MeshFilter filter { get { return _filter != null ? _filter : _filter = GetComponent<MeshFilter>(); } }
	private MeshFilter _filter;

	public bool showNormals = true;
	public bool showTangents = true;
	public float renderDistance = 10f;

	[ContextMenu("Print mesh info")]
	public void PrintMeshInfo() {
		Debug.Log("Verts: " + filter.sharedMesh.vertexCount + ", Norms: " + filter.sharedMesh.normals.Length + " Uvs: " + filter.sharedMesh.uv.Length + " Tris: " + filter.sharedMesh.triangles.Length + " Submeshes: " + filter.sharedMesh.subMeshCount);
	}

	// Update is called once per frame
	void OnDrawGizmos() {
		if (!filter) return;

		Vector3[] verts = filter.sharedMesh.vertices;
		Vector4[] tangents = filter.sharedMesh.tangents;
		Vector3[] norms = filter.sharedMesh.normals;
		for (int i = 0; i < verts.Length; i++) {
			verts[i] = transform.TransformPoint(verts[i]);
			if (Vector3.Distance(verts[i], Camera.current.transform.position) > renderDistance) continue;

			if (showNormals) {
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(verts[i], verts[i] + transform.rotation * norms[i] * 0.1f);
			}

			if (showTangents) {
				if (tangents[i].w > 0) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawLine(verts[i], verts[i] + transform.rotation * tangents[i] * 0.1f);
				} else {
					Gizmos.color = Color.red;
					Gizmos.DrawLine(verts[i], verts[i] + transform.rotation * -tangents[i] * 0.1f);
				}
			}
		}
	}
}
