using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour {

	private int level = 0, number = 0;
	private GameObject parentNode = null, leftNode = null, rightNode = null;
	// TODO: static class for node x,y position differences
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

	public int Number
	{
		get
		{
			return number;
		}

		set
		{
			number = value;
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


	// Use this for initialization
	void Start () {
		if (gameObject.transform.parent.gameObject.tag == "Node")
		{
			// set parent node
			parentNode = gameObject.transform.parent.gameObject;

			// set level
			NodeScript ns = parentNode.GetComponent<NodeScript>();
			level = ns.Level + 1;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetChildNodeLeft(GameObject node)
	{
		leftNode = node;
		foreach (GameObject arrow in gameObject.GetComponentsInChildren<GameObject>())
		{
			if (arrow.name == "ArrowLeft")
			{
				SetChildNode(node, arrow);
				break;
			}
		}
	}

	void SetChildNodeRight(GameObject node)
	{
		rightNode = node;
		foreach (GameObject arrow in gameObject.GetComponentsInChildren<GameObject>())
		{
			if (arrow.name == "ArrowRight")
			{
				SetChildNode(node, arrow);
				break;
			}
		}
	}

	void SetChildNode(GameObject node, GameObject arrow)
	{
		if (node == null || arrow == null) return;
		ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
		if (arrowScript == null) return;

		arrowScript.ToNode = node.transform;
	}
}
