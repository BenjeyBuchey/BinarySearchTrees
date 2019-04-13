﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeScript : MonoBehaviour {

	public InputField inputFieldAddNode, inputFieldSearchNode, inputFieldDeleteNode;
	public GameObject nodePrefab;
	private GameObject root = null;
	private BSTVisual bstVisual = new BSTVisual();
	private bool isInitializing = false;
	private bool isRunning = false;

	public bool IsRunning
	{
		get
		{
			return isRunning;
		}

		set
		{
			isRunning = value;
		}
	}

	// Use this for initialization
	void Start () {
		isInitializing = true;
		AddNode(50);
		AddNode(30);
		AddNode(20);
		AddNode(40);
		AddNode(70);
		AddNode(60);
		AddNode(80);
		Inorder(root);
		isInitializing = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetRoot(GameObject node)
	{
		root = node;
	}

	public void PrintNodes()
	{
		Inorder(root);
	}

	private void StartVisualization()
	{
		// get visual script & start
		gameObject.GetComponent<VisualScript>().Visualize(bstVisual);
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
		int key = -1;
		if (!int.TryParse(inputFieldAddNode.text, out key) || isRunning) return;

		isRunning = true;
		bstVisual.ClearItems();
		bstVisual.Key = key;
		GameObject go = AddNode(key);

		StartVisualization();
	}

	public void ButtonSearchNode()
	{
		int key = -1;
		if (!int.TryParse(inputFieldSearchNode.text, out key)) return;

		isRunning = true;
		bstVisual.ClearItems();
		bstVisual.Key = key;
		GameObject go = Search(root, key);
		if (go != null)
			Debug.Log("KEY FOUND!");
		else
			Debug.Log("KEY NOT FOUND!");
		StartVisualization();
	}

	public void ButtonDeleteNode()
	{
		Debug.Log("DELETING: " + inputFieldDeleteNode.text);
		int key = -1;
		if (!int.TryParse(inputFieldDeleteNode.text, out key)) return;

		//GameObject go = Search(root, key);
		bstVisual.ClearItems();
		bstVisual.Key = key;
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
		if (node == null) return SpawnNode(isLeftNode, key, parentNode);
		bstVisual.Items.Add(new BSTVisualItem(node, (int)VisualType.Node));

		if (key < node.GetComponent<NodeScript>().Key)
		{
			bstVisual.Items.Add(new BSTVisualItem(node, (int)VisualType.LeftArrow));
			Insert(node.GetComponent<NodeScript>().LeftNode, true, key, node);
		}
		else if (key > node.GetComponent<NodeScript>().Key)
		{
			bstVisual.Items.Add(new BSTVisualItem(node, (int)VisualType.RightArrow));
			Insert(node.GetComponent<NodeScript>().RightNode, false, key, node);
		}

		return node;
	}

	private GameObject SpawnNode(bool isLeftNode, int key, GameObject parentNode)
	{
		// spawn new node
		GameObject node = Instantiate(nodePrefab, gameObject.transform);

		if (root == null)
		{
			root = node;
			root.GetComponent<NodeScript>().SetKey(key);
			root.GetComponent<NodeScript>().Activate(true);
			return root;
		}

		bstVisual.Items.Add(new BSTVisualItem(node, (int)VisualType.SpawnNode));

		if (isLeftNode)
			parentNode.GetComponent<NodeScript>().SetChildNodeLeft(node, key);
		else
			parentNode.GetComponent<NodeScript>().SetChildNodeRight(node, key);

		if (isInitializing)
			node.GetComponent<NodeScript>().Activate(true);
		else
			node.GetComponent<NodeScript>().Activate(false);

		return node;
	}

	private GameObject Search(GameObject node, int key)
	{
		bstVisual.Items.Add(new BSTVisualItem(node, (int)VisualType.Node));
		// root null or root has key
		if (node == null || node.GetComponent<NodeScript>().Key == key)
		{
			bstVisual.Items.Add(new BSTVisualItem(node, (int)VisualType.FoundNode));
			return node;
		}

		// key > node key
		if (node.GetComponent<NodeScript>().Key < key)
		{
			bstVisual.Items.Add(new BSTVisualItem(node, (int)VisualType.RightArrow));
			return Search(node.GetComponent<NodeScript>().RightNode, key);
		}

		// key <= node key
		bstVisual.Items.Add(new BSTVisualItem(node, (int)VisualType.LeftArrow));
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
