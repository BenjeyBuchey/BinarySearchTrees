﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeScript : MonoBehaviour {

	private int level = 0, key = 0;
	private GameObject parentNode = null, leftNode = null, rightNode = null;
	// treescript. spawn nodes there.

	public int Level
	{
		get
		{
			return level;
		}

		set
		{
			level = value;
		}
	}

	public int Key
	{
		get
		{
			return key;
		}

		set
		{
			key = value;
		}
	}

	public GameObject ParentNode
	{
		get
		{
			return parentNode;
		}

		set
		{
			parentNode = value;
		}
	}

	public GameObject LeftNode
	{
		get
		{
			return leftNode;
		}

		set
		{
			leftNode = value;
		}
	}

	public GameObject RightNode
	{
		get
		{
			return rightNode;
		}

		set
		{
			rightNode = value;
		}
	}


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		SetPosition();
		SetArrows();
	}

	public void SetChildNodeLeft(GameObject node, int childKey)
	{
		leftNode = node;
		InitChildNode(leftNode, childKey);

		//GameObject arrow = gameObject.transform.Find("ArrowLeft").gameObject;
		//if (arrow == null) return;
		//SetArrow(node, arrow);
	}

	public void SetChildNodeRight(GameObject node, int childKey)
	{
		rightNode = node;
		InitChildNode(rightNode, childKey);

		//GameObject arrow = gameObject.transform.Find("ArrowRight").gameObject;
		//if (arrow == null) return;
		//SetArrow(node, arrow);
	}

	private void InitChildNode(GameObject childNode, int childKey)
	{
		// we init child node. this is its parent.
		// set parentnode of node
		// set level
		// set key
		// set position
		childNode.GetComponent<NodeScript>().ParentNode = gameObject;
		childNode.GetComponent<NodeScript>().SetKey(childKey);
		childNode.GetComponent<NodeScript>().Level = level + 1;
		//childNode.GetComponent<NodeScript>().SetPosition();
	}

	void SetArrow(GameObject node, GameObject arrow)
	{
		if (node == null || arrow == null) return;
		ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
		if (arrowScript == null) return;

		arrowScript.ToNode = node.transform;
	}

	void SetArrows()
	{
		if(leftNode != null)
		{
			GameObject arrow = gameObject.transform.Find("ArrowLeft").gameObject;
			if (arrow == null) return;
			SetArrow(leftNode, arrow);
		}

		if(rightNode != null)
		{
			GameObject arrow = gameObject.transform.Find("ArrowRight").gameObject;
			if (arrow == null) return;
			SetArrow(rightNode, arrow);
		}
	}

	public void SetPosition()
	{
		if(level == 0)
		{
			gameObject.transform.localPosition = NodeManager.ROOT_POSITION;
			return;
		}

		bool isLeftNode = (parentNode.GetComponent<NodeScript>().leftNode == gameObject) ? true : false;

		float x = (isLeftNode) ? parentNode.transform.localPosition.x - NodeManager.X_DIFF / level : parentNode.transform.localPosition.x + NodeManager.X_DIFF / level;
		Vector3 position = new Vector3(x, parentNode.transform.localPosition.y - NodeManager.Y_DIFF, 0.0f);
		gameObject.transform.localPosition = position;
	}

	public void SetKey(int val)
	{
		key = val;
		SetKeyText();
	}

	void SetKeyText()
	{
		foreach(Text t in gameObject.GetComponentsInChildren<Text>())
		{
			if(t.name == "NodeNumber")
			{
				t.text = key.ToString();
			}
		}
	}

	public int GetNumChildren()
	{
		int numChildren = 0;
		if (leftNode != null) numChildren++;
		if (rightNode != null) numChildren++;

		return numChildren;
	}

	public List<GameObject> GetChildren()
	{
		List<GameObject> list = new List<GameObject>();
		if (leftNode != null) list.Add(leftNode);
		if (rightNode != null) list.Add(rightNode);

		return list;
	}

	// gets called when one node gets deleted and new parent&children need to be set
	public void RefreshNode(GameObject newParentNode)
	{
		// TODO: change level!!!

		// if newParent is null then this node becomes the new root node
		if(newParentNode == null)
		{
			return;
		}

		parentNode = newParentNode;
		// becomes right node
		if (key > parentNode.GetComponent<NodeScript>().Key)
			parentNode.GetComponent<NodeScript>().rightNode = gameObject;
		else
			parentNode.GetComponent<NodeScript>().leftNode = gameObject;
	}
}
