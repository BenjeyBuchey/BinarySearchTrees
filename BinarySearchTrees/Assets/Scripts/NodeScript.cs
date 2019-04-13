using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeScript : MonoBehaviour {

	private int level = 0, key = 0;
	private GameObject parentNode = null, leftNode = null, rightNode = null;
	private static Color DEFAULT_COLOR = Color.black;
	private static Color VISUALIZATION_COLOR = Color.red;
	private bool isInitialized = false;
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

	public bool IsInitialized
	{
		get
		{
			return isInitialized;
		}

		set
		{
			isInitialized = value;
		}
	}


	// Use this for initialization
	void Start () {
		//gameObject.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {
		if (isInitialized)
		{
			SetPosition();
			SetArrows();
			//gameObject.SetActive(isInitialized);
		}
	}

	public void Activate(bool isActive)
	{
		gameObject.SetActive(isActive);
		isInitialized = isActive;
		if (parentNode != null)
			parentNode.GetComponent<NodeScript>().ActivateArrows(isActive, gameObject);
	}

	public void ActivateArrows(bool isActive, GameObject childNode)
	{
		// need to set arrows to childNode active/inactive
		string arrStr = string.Empty;
		if (childNode == leftNode)
			arrStr = "ArrowLeft";
		else if (childNode == rightNode)
			arrStr = "ArrowRight";

		GameObject arrow = gameObject.transform.Find(arrStr).gameObject;
		ArrowScript ascript = arrow.GetComponent<ArrowScript>();
		ascript.IsInitialized = isActive;
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
		// if newParent is null then this node becomes the new root node
		Debug.Log("KEY: " +key + " - CURRENT LEVEL: " + level);

		parentNode = newParentNode;
		RefreshLevels();
		Debug.Log("KEY: " + key + " - NEW LEVEL: " + level);
		if (parentNode == null) return;

		// becomes right node
		if (key > parentNode.GetComponent<NodeScript>().Key)
			parentNode.GetComponent<NodeScript>().rightNode = gameObject;
		else
			parentNode.GetComponent<NodeScript>().leftNode = gameObject;
	}

	public void RefreshLevels()
	{
		if (parentNode == null)
			SetRoot();
		else
			level = parentNode.GetComponent<NodeScript>().Level + 1;

		if (leftNode != null)
			leftNode.GetComponent<NodeScript>().RefreshLevels();
		if(rightNode != null)
			rightNode.GetComponent<NodeScript>().RefreshLevels();
		//Debug.Log("KEY: " + key + " - PARENT KEY: " + parentNode.GetComponent<NodeScript>().Key);
		
	}

	private void SetRoot()
	{
		level = 0;
		TreeScript ts = gameObject.transform.parent.gameObject.GetComponent<TreeScript>();
		if (ts == null) return;

		ts.SetRoot(gameObject);
	}

	public void SetNodeColor(bool isDefault)
	{
		foreach (Text t in gameObject.GetComponentsInChildren<Text>())
		{
			if (t.name == "NodeNumber")
			{
				if (isDefault)
					t.color = DEFAULT_COLOR;
				else
					t.color = VISUALIZATION_COLOR;
			}
		}
	}

	public void SetArrowColor(bool isDefault, bool isLeftArrow)
	{
		string arrowString = isLeftArrow ? "ArrowLeft" : "ArrowRight";
		GameObject arrow = gameObject.transform.Find(arrowString).gameObject;
		if (arrow == null) return;

		ArrowScript script = arrow.GetComponent<ArrowScript>();
		if (script == null) return;

		if (isDefault)
			script.SetDefaultColor();
		else
			script.SetVisualizationColor();
	}
}
