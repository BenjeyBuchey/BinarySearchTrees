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
		AddNode(50);
		AddNode(30);
		AddNode(20);
		AddNode(40);
		AddNode(70);
		AddNode(60);
		AddNode(80);
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

		//GameObject go = Search(root, key);
		Delete(root, key);
	}

	private GameObject Delete(GameObject node, int key)
	{
		if (node == null) return node;

		// key to be deleted smaller than root key --> it's in the left subtree
		if (key < node.GetComponent<NodeScript>().Key)
		{
			node.GetComponent<NodeScript>().LeftNode = Delete(node.GetComponent<NodeScript>().LeftNode, key);
		}
		// key greater --> right subtree
		else if (key > node.GetComponent<NodeScript>().Key)
		{
			node.GetComponent<NodeScript>().RightNode = Delete(node.GetComponent<NodeScript>().RightNode, key);
		}
		// key equal --> this node gets deleted
		else
		{
			// one child or no child
			if (node.GetComponent<NodeScript>().LeftNode == null)
			{
				GameObject temp = node.GetComponent<NodeScript>().RightNode;
				if (temp != null)
					temp.GetComponent<NodeScript>().RefreshNode(node.GetComponent<NodeScript>().ParentNode);

				Destroy(node);
				return temp;
			}
			else if (node.GetComponent<NodeScript>().RightNode == null)
			{
				GameObject temp = node.GetComponent<NodeScript>().LeftNode;
				if (temp != null)
					temp.GetComponent<NodeScript>().RefreshNode(node.GetComponent<NodeScript>().ParentNode);

				Destroy(node);
				return temp;
			}

			// node has two children --> find inorder successor, set node to inorder successor value, delete inorder successor
			GameObject inorderSuccessor = InorderSuccessor(node.GetComponent<NodeScript>().RightNode);
			if(inorderSuccessor != null)
			{
				node.GetComponent<NodeScript>().SetKey(inorderSuccessor.GetComponent<NodeScript>().Key);
				node.GetComponent<NodeScript>().RightNode = Delete(node.GetComponent<NodeScript>().RightNode, inorderSuccessor.GetComponent<NodeScript>().Key);
			}
		}
		return node;
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
			//root.GetComponent<NodeScript>().SetPosition();
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

	// gets the inorder successor (smallest element in the right subtree) of a node
	private GameObject InorderSuccessor(GameObject node)
	{
		GameObject current = node;
		if (current == null) return null;

		while (current.GetComponent<NodeScript>().LeftNode != null)
			current = current.GetComponent<NodeScript>().LeftNode;

		return current;
	}
}
