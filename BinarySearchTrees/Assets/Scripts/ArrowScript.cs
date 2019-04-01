using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour {

	private Transform fromNode, toNode;
	private static Color DEFAULT_COLOR = Color.black, VISUALIZATION_COLOR = Color.red;

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
		UpdateLineRenderer();
	}

	void UpdateLineRenderer()
	{
		if (fromNode == null) return;

		if(toNode == null)
		{
			Reset();
			return;
		}

		LineRenderer lr = GetComponent<LineRenderer>();
		//lr.useWorldSpace = true;
		lr.positionCount = 2;
		lr.SetPosition(0, fromNode.position);
		lr.SetPosition(1, toNode.position);
	}

	public void SetDefaultColor()
	{
		LineRenderer lr = GetComponent<LineRenderer>();
		lr.startColor = DEFAULT_COLOR;
		lr.endColor = DEFAULT_COLOR;
	}

	public void SetVisualizationColor()
	{
		LineRenderer lr = GetComponent<LineRenderer>();
		lr.startColor = VISUALIZATION_COLOR;
		lr.endColor = VISUALIZATION_COLOR;
	}

	public void Reset()
	{
		LineRenderer lr = GetComponent<LineRenderer>();
		if (lr == null) return;

		lr.positionCount = 0;
	}
}
