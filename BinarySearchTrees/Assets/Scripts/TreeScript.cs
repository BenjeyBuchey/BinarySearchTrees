using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeScript : MonoBehaviour {

	public InputField inputFieldAddNode, inputFieldSearchNode, inputFieldDeleteNode;
	public GameObject nodePrefab;
	private GameObject root = null;
	// Use this for initialization
	void Start () {
		AddNode(40);
		AddNode(33);
		AddNode(44);
		AddNode(55);
		AddNode(32);
		AddNode(16);
		AddNode(50);
		AddNode(35);
		Inorder(root);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Inorder(GameObject node)
	{
		if(node != null)
		{
			Inorder(node.GetComponent<NodeScript>().LeftNode);
			Debug.Log(node.GetComponent<NodeScript>().Key);
			Inorder(node.GetComponent<NodeScript>().RightNode);
		}
	}

	private GameObject AddNode(int key)
	{
		return Insert(root, false, key, null);
	}

	public void ButtonAddNode()
	{
		Debug.Log("ADDING: " +inputFieldAddNode.text);
		int key = -1;
		if (!int.TryParse(inputFieldAddNode.text, out key)) return;
		GameObject go = AddNode(key);
	}

	public void ButtonSearchNode()
	{
		Debug.Log("SEARCHING: " + inputFieldSearchNode.text);
		int key = -1;
		if (!int.TryParse(inputFieldSearchNode.text, out key)) return;
		GameObject go = Search(root, key);
		if (go != null)
			Debug.Log("KEY FOUND!");
		else
			Debug.Log("KEY NOT FOUND!");
	}

	public void ButtonDeleteNode()
	{
		Debug.Log("DELETING: " + inputFieldDeleteNode.text);
		int key = -1;
		if (!int.TryParse(inputFieldDeleteNode.text, out key)) return;

		GameObject go = Search(root, key);
		if (go != null)
		{
			// if node is leaf
			Destroy(go);

			// if node has only one child

			// if node has two children
		}
			
	}

	private GameObject Insert(GameObject node, bool isLeftNode, int key, GameObject parentNode)
	{
		if (node == null) return SpawnNode(node, isLeftNode, key, parentNode);

		if (key < node.GetComponent<NodeScript>().Key)
			Insert(node.GetComponent<NodeScript>().LeftNode, true, key, node);
		else if (key > node.GetComponent<NodeScript>().Key)
			Insert(node.GetComponent<NodeScript>().RightNode, false, key, node);

		return node;
	}

	private GameObject SpawnNode(GameObject node, bool isLeftNode, int key, GameObject parentNode)
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

	private GameObject Search(GameObject node, int key)
	{
		// root null or root has key
		if (node == null || node.GetComponent<NodeScript>().Key == key)
			return node;

		// key > node key
		if (node.GetComponent<NodeScript>().Key < key)
			return Search(node.GetComponent<NodeScript>().RightNode, key);

		// key <= node key
		return Search(node.GetComponent<NodeScript>().LeftNode, key);
	}
}
