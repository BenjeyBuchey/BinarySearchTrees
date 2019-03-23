using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour {

	private Transform fromNode, toNode;
	private List<Vector2> linePoints = new List<Vector2>();

	public Transform ToNode
	{
		get
		{
			return toNode;
		}

		set
		{
			toNode = value;
		}
	}

	// Use this for initialization
	void Start () {
		fromNode = gameObject.transform.parent;
		UpdateLineRenderer();
	}
	
	// Update is called once per frame
	void Update () {
		//transform.position = fromNode.position;
		//transform.LookAt(toNode);
		//float mag = (fromNode.position - toNode.position).magnitude;
		//transform.localScale = new Vector3(mag, mag, 0);
		UpdateLineRenderer();
	}

	void UpdateLineRenderer()
	{
		if (fromNode == null || toNode == null) return;

		LineRenderer lr = GetComponent<LineRenderer>();
		//lr.useWorldSpace = true;
		lr.positionCount = 2;
		lr.SetPosition(0, fromNode.position);
		lr.SetPosition(1, toNode.position);
	}
}
