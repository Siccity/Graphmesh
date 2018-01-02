using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(GraphmeshGraph))]
public class GraphmeshGraphEditor : NodeGraphEditor {

	public override string GetNodePath(System.Type type) {
		if (type.Namespace == "Graphmesh") return base.GetNodePath(type).Replace("Graphmesh/","");
		else return null;
	}
}
