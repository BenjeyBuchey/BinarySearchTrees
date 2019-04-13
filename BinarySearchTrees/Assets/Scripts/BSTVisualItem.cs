using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum VisualType { Node = 0, LeftArrow = 1, RightArrow = 2, SpawnNode = 3, FoundNode = 4 }

public class BSTVisualItem {

	private readonly GameObject _node;
	private readonly int _type;
	private const string cmd = ">>";

	public GameObject Node
	{
		get
		{
			return _node;
		}
	}

	public int Type
	{
		get
		{
			return _type;
		}
	}

	public BSTVisualItem(GameObject node, int type)
	{
		_node = node;
		_type = type;
	}

	public string GetItemMessage()
	{
		switch(_type)
		{
			case (int)VisualType.Node:
				return NodeMsg();
			case (int)VisualType.LeftArrow:
				return LeftArrowMsg();
			case (int)VisualType.RightArrow:
				return RightArrowMsg();
			case (int)VisualType.SpawnNode:
				return SpawnNodeMsg();
			case (int)VisualType.FoundNode:
				return FoundNodeMsg();
		}
		return string.Empty;
	}

	private string NodeMsg()
	{
		if (_node == null) return string.Empty;
		return cmd + "Next Node: " + _node.GetComponent<NodeScript>().Key;
	}

	private string LeftArrowMsg()
	{
		if (_node == null) return string.Empty;
		return cmd + _node.GetComponent<NodeScript>().Key + " is bigger --> Go left child";
	}

	private string RightArrowMsg()
	{
		if (_node == null) return string.Empty;
		return cmd + _node.GetComponent<NodeScript>().Key + " is smaller --> Go right child";
	}

	private string SpawnNodeMsg()
	{
		if (_node == null) return string.Empty;
		return cmd + "Spawned Node " + _node.GetComponent<NodeScript>().Key + "!";
	}

	private string FoundNodeMsg()
	{
		if (_node == null)
			return cmd +"Node NOT found!";
		else
			return cmd + "Node " + _node.GetComponent<NodeScript>().Key + " found!";
	}
}
