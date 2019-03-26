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
		GameObject go = InsertNew(root, false, key, null);
		//if (root == null)
		//{
		//	root = go;
		//	go.GetComponent<NodeScript>().SetKey(key);
		//	go.GetComponent<NodeScript>().SetPosition();
		//}
	}

	private GameObject Insert(GameObject node, int key)
	{
		if (node == null) return SpawnNode(node);

		if (key < node.GetComponent<NodeScript>().Key)
			node.GetComponent<NodeScript>().SetChildNodeLeft(Insert(node.GetComponent<NodeScript>().LeftNode, key),key);
		else if (key > node.GetComponent<NodeScript>().Key)
			node.GetComponent<NodeScript>().SetChildNodeRight(Insert(node.GetComponent<NodeScript>().RightNode, key),key);

		return node;
	}

	private GameObject SpawnNode(GameObject node)
	{
		// spawn new node
		node = Instantiate(nodePrefab, gameObject.transform);
		return node;
	}

	private GameObject InsertNew(GameObject node, bool isLeftNode, int key, GameObject parentNode)
	{
		if (node == null) return SpawnNodeNew(node, isLeftNode, key, parentNode);

		if (key < node.GetComponent<NodeScript>().Key)
			InsertNew(node.GetComponent<NodeScript>().LeftNode, true, key, node);
		else if (key > node.GetComponent<NodeScript>().Key)
			InsertNew(node.GetComponent<NodeScript>().RightNode, false, key, node);

		return node;
	}

	private GameObject SpawnNodeNew(GameObject node, bool isLeftNode, int key, GameObject parentNode)
	{
		// spawn new node
		node = Instantiate(nodePrefab, gameObject.transform);

		if (root == null)
		{
			root = node;
			root.GetComponent<NodeScript>().SetKey(key);
			root.GetComponent<NodeScript>().SetPosition();
			return root;
		}

		if (isLeftNode)
			parentNode.GetComponent<NodeScript>().SetChildNodeLeft(node, key);
		else
			parentNode.GetComponent<NodeScript>().SetChildNodeRight(node, key);

		return node;
	}
}
