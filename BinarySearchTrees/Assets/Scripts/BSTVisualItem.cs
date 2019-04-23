using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum VisualType { Node = 0, LeftArrow = 1, RightArrow = 2, SpawnNode = 3, FoundNode = 4, DestroyNode = 5, RefreshNode = 6, SetNodeKey = 7, InorderSuccessor = 8, InorderSuccessorMove = 9,
InorderSuccessorDestroy = 10, InorderSuccessorFound = 11 }

public class BSTVisualItem {

    private GameObject _node, _parentNode, _tempNode;
	private readonly int _type;
	private readonly int _enteredKey, _inorderKey;
	private readonly bool _isLeftNode;
	private const string cmd = " >> ";
    private Vector3 _inorderPosition, _dest;

	public GameObject Node
	{
		get
		{
			return _node;
		}

        set
        {
            _node = value;
        }
	}

	public int Type
	{
		get
		{
			return _type;
		}
	}

	public int EnteredKey
	{
		get
		{
			return _enteredKey;
		}
	}

	public bool IsLeftNode
	{
		get
		{
			return _isLeftNode;
		}
	}

	public GameObject ParentNode
	{
		get
		{
			return _parentNode;
		}

        set
        {
            _parentNode = value;
        }
    }

	public GameObject TempNode
	{
		get
		{
			return _tempNode;
		}

        set
        {
            _tempNode = value;
        }
    }

    public Vector3 InorderPosition { get => _inorderPosition; set => _inorderPosition = value; }
    public Vector3 Dest { get => _dest; set => _dest = value; }

    public int EnteredKey1 => _enteredKey;

    public int InorderKey => _inorderKey;

    public BSTVisualItem(GameObject node, int type, int enteredKey = 0, bool isLeftNode = false, GameObject parentNode = null, GameObject tempNode = null, Vector3 inorderPosition = new Vector3(),
        Vector3 dest = new Vector3(), int inorderKey = 0)
	{
		_node = node;
		_type = type;
		_enteredKey = enteredKey;
		_isLeftNode = isLeftNode;
		_parentNode = parentNode;
		_tempNode = tempNode;
        _inorderPosition = inorderPosition;
        _dest = dest;
        _inorderKey = inorderKey;
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
			case (int)VisualType.DestroyNode:
				return DestroyNodeMsg();
			case (int)VisualType.RefreshNode:
				return RefreshNodeMsg();
			case (int)VisualType.SetNodeKey:
				return SetNodeKeyMsg();
            case (int)VisualType.InorderSuccessor:
                return SetInorderSuccessorMsg();
            case (int)VisualType.InorderSuccessorMove:
                return SetInorderSuccessorMoveMsg();
            case (int)VisualType.InorderSuccessorDestroy:
                return SetInorderSuccessorDestroyMsg();
            case (int)VisualType.InorderSuccessorFound:
                return SetInorderSuccessorFoundMsg();
        }
		return string.Empty;
	}

    private string SetInorderSuccessorFoundMsg()
    {
        if (_node == null) return string.Empty;
        return cmd + "Inorder Successor found: " + _node.GetComponent<NodeScript>().Key;
    }

    private string SetInorderSuccessorDestroyMsg()
    {
        if (_node == null) return string.Empty;
        return cmd + "Removing Inorder Successor " + _node.GetComponent<NodeScript>().Key;
    }

    private string SetInorderSuccessorMoveMsg()
    {
        if (_node == null) return string.Empty;
        return cmd + "Setting Node " + _enteredKey + " to Inorder Successor " + _node.GetComponent<NodeScript>().Key;
    }

    private string SetInorderSuccessorMsg()
    {
        if (_node == null) return string.Empty;
        return cmd + "Node " + _node.GetComponent<NodeScript>().Key +" has 2 children --> Searching Inorder Successor!";
    }

    private string DestroyNodeMsg()
	{
		if (_node == null) return string.Empty;
		return cmd + "Deleted Node: " + _node.GetComponent<NodeScript>().Key;
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
		if (_parentNode == null) return string.Empty;
		return cmd + "Spawned Node " + _enteredKey + "!";
	}

	private string FoundNodeMsg()
	{
		if (_node == null)
			return cmd +"Node NOT found!";
		else
			return cmd + "Node " + _node.GetComponent<NodeScript>().Key + " found!";
	}

	private string RefreshNodeMsg()
	{
		if (_node == null) return string.Empty;

		if (_parentNode == null)
			return cmd + "Set Node " + _node.GetComponent<NodeScript>().Key + " to Root Node!";
		else
			return cmd + "Set Node " + _node.GetComponent<NodeScript>().Key + " Parent to " +_parentNode.GetComponent<NodeScript>().Key;
	}

	private string SetNodeKeyMsg()
	{
		if (_node == null) return string.Empty;
		return cmd + "Set Node " + +_node.GetComponent<NodeScript>().Key + " to "+ _enteredKey + "!";
	}
}
