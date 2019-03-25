using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeScript : MonoBehaviour {

	public InputField inputFieldAddNode;
	public GameObject nodePrefab;
	private GameObject root = null;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddNode()
	{
		Debug.Log("ADDING: " +inputFieldAddNode.text);
		int key = -1;
		if (!int.TryParse(inputFieldAddNode.text, out key)) return;
		GameObject go = Insert(root, key, false);
		if (root == null)
		{
			root = go;
			go.GetComponent<NodeScript>().SetKey(key);
			go.GetComponent<NodeScript>().SetPosition();
		}	
	}

	private GameObject Insert(GameObject node, int key, bool isLeftNode)
	{
		if (node == null) return SpawnNode(key, node, isLeftNode);

		if (key < node.GetComponent<NodeScript>().Key)
			node.GetComponent<NodeScript>().SetChildNodeLeft(Insert(node.GetComponent<NodeScript>().LeftNode, key, true),key);
		else if (key > node.GetComponent<NodeScript>().Key)
			node.GetComponent<NodeScript>().SetChildNodeRight(Insert(node.GetComponent<NodeScript>().RightNode, key, false),key);

		return node;
	}

	private GameObject SpawnNode(int key, GameObject parentNode, bool isLeftNode)
	{
		// spawn new node
		// set parentnode of node
		// set level
		// set key
		// set position here OR set position in SetChildNodeLeft/Right ??

		GameObject go = Instantiate(nodePrefab, gameObject.transform);
		go.GetComponent<NodeScript>().ParentNode = parentNode;
		go.GetComponent<NodeScript>().SetKey(key);
		//go.GetComponent<NodeScript>().SetPosition(isLeftNode);

		return go;
	}
}
