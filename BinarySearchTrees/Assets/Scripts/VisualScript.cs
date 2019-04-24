﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualScript : MonoBehaviour {

	private BSTVisual _bstVisual;
	private float waitTime = 1.0f;
	public GameObject logs, scrollView;
	public Slider visualizationSpeedSlider;
	private bool _isBusy = false;
	private int _visualizationCounter = 0;
    private const string TEMP_INORDER_SUCCESSOR_NODE = "TempInorderSuccessorNode";
    private List<BSTVisualItem> _nodeReferences;

    public bool IsBusy { get => _isBusy; set => _isBusy = value; }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Visualize(BSTVisual bstVisual)
	{
		if (bstVisual == null || bstVisual.Items.Count == 0) return;

		_bstVisual = bstVisual;
		logs.GetComponent<Text>().text = string.Empty;
		_visualizationCounter = 0;
        _nodeReferences = new List<BSTVisualItem>();
		StartCoroutine(DoVisualize());
	}

	public void StepForward()
	{
		if (_isBusy) return;

		if (_visualizationCounter <= _bstVisual.Items.Count)
			StartCoroutine(DoStepForward());
	}

	public void StepBackwards()
	{
		if (_isBusy) return;

		if (_visualizationCounter <= _bstVisual.Items.Count && _visualizationCounter > 0) // because with forward visual we are already one item ahead
			StartCoroutine(DoStepBackwards());
	}

    public void StepBegin()
    {
        if (_isBusy) return;
        if (_visualizationCounter <= _bstVisual.Items.Count && _visualizationCounter > 0)
            StartCoroutine(DoStepBegin());
    }

	public void StepEnd()
	{
		if (_isBusy) return;

		if (_visualizationCounter <= _bstVisual.Items.Count)
			StartCoroutine(DoStepEnd());
	}

	public void ResumeVisualization()
	{
		if (_bstVisual == null || _bstVisual.Items.Count == 0) return;

		if (!gameObject.GetComponent<TreeScript>().IsRunning)
			StartCoroutine(DoVisualize());
	}

	IEnumerator DoVisualize()
	{
		gameObject.GetComponent<TreeScript>().IsRunning = true;

		for (; _visualizationCounter < _bstVisual.Items.Count; _visualizationCounter++)
		{
			while (IsPaused() || _isBusy)
				yield return null;

			if (_visualizationCounter > 0)
				HandleColors(_bstVisual.Items[_visualizationCounter - 1], true);

			if (_visualizationCounter >= _bstVisual.Items.Count)
			{
				Reset();
				yield break;
			}

			UpdateVisualizationSpeed();
			AddLogEntry(_bstVisual.Items[_visualizationCounter].GetItemMessage());
			HandleVisualizationItem(_bstVisual.Items[_visualizationCounter], false);

			yield return new WaitForSeconds(waitTime);
		}
		if (_visualizationCounter <= _bstVisual.Items.Count)
			HandleColors(_bstVisual.Items[_visualizationCounter - 1], true);
	
		Reset();
	}

	IEnumerator DoStepForward()
	{
		_isBusy = true;
		UpdateVisualizationSpeed();

		if (_visualizationCounter > 0)
			HandleColors(_bstVisual.Items[_visualizationCounter - 1], true);

		if (_visualizationCounter >= _bstVisual.Items.Count)
        {
            _isBusy = false;
            yield break;
        }

		AddLogEntry(_bstVisual.Items[_visualizationCounter].GetItemMessage());
		HandleVisualizationItem(_bstVisual.Items[_visualizationCounter], false);
		yield return new WaitForSeconds(waitTime);

		_visualizationCounter++;
		_isBusy = false;
	}

	IEnumerator DoStepBackwards()
	{
		_isBusy = true;
        _visualizationCounter--;
		if ((_visualizationCounter+1) < _bstVisual.Items.Count)
			HandleColors(_bstVisual.Items[_visualizationCounter+1], true);

		UpdateVisualizationSpeed();
		RemoveLastLogEntry();
        if (_visualizationCounter > 0)
        {
            HandleVisualizationItemBackwards(_bstVisual.Items[_visualizationCounter], false);
        }
        else
        {
            _isBusy = false;
            yield break;
        }

        yield return new WaitForSeconds(waitTime);
        _isBusy = false;
	}

    IEnumerator DoStepBegin()
    {
        _isBusy = true;
        waitTime = 0.1f;
        _visualizationCounter--;
        Debug.Log("STEP BEGIN VISUAL COUNTER: " + _visualizationCounter);

        for (; _visualizationCounter >= 0; _visualizationCounter--)
        {
            if ((_visualizationCounter + 1) < _bstVisual.Items.Count)
				HandleColors(_bstVisual.Items[_visualizationCounter+1], true);

			RemoveLastLogEntry();
            if (_visualizationCounter > 0)
            {
                HandleVisualizationItemBackwards(_bstVisual.Items[_visualizationCounter], false);
            }
            else
            {
                _isBusy = false;
                yield break;
            }
            yield return new WaitForSeconds(waitTime);
        }
        _isBusy = true;
    }

	IEnumerator DoStepEnd()
	{
		_isBusy = true;
		waitTime = 0.1f;

		for (; _visualizationCounter < _bstVisual.Items.Count; _visualizationCounter++)
		{
			if (_visualizationCounter > 0)
				HandleColors(_bstVisual.Items[_visualizationCounter-1], true);

			if (_visualizationCounter >= _bstVisual.Items.Count)
			{
				Reset();
				yield break;
			}

			AddLogEntry(_bstVisual.Items[_visualizationCounter].GetItemMessage());
			HandleVisualizationItem(_bstVisual.Items[_visualizationCounter], false);

			yield return new WaitForSeconds(waitTime);
		}

		if (_visualizationCounter <= _bstVisual.Items.Count)
			HandleColors(_bstVisual.Items[_visualizationCounter-1], true);

		Reset();
		_isBusy = false;
	}

	private void AddLogEntry(string msg)
	{
        int logNumber = GetLogNumber();
        Debug.Log("VISUAL COUNTER: " + _visualizationCounter + " - NUMBER OF LOG MSGS: " + logNumber);
        // don't remove log msg on same step
        if (_visualizationCounter == logNumber-1) return;

        if (logs.GetComponent<Text>().text != String.Empty)
			logs.GetComponent<Text>().text += System.Environment.NewLine;

		logs.GetComponent<Text>().text += msg;

        Canvas.ForceUpdateCanvases();
        scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
	}

	private void RemoveLastLogEntry()
	{
        int logNumber = GetLogNumber();
        Debug.Log("VISUAL COUNTER: " + _visualizationCounter + " - NUMBER OF LOG MSGS: " + logNumber);
        // don't remove log msg on same step
        if (_visualizationCounter == logNumber-1) return;

        string text = logs.GetComponent<Text>().text;
        int index = text.LastIndexOf(System.Environment.NewLine);

        if (index < 0)
            logs.GetComponent<Text>().text = String.Empty;
        else
            logs.GetComponent<Text>().text = text.Substring(0, index);

        Canvas.ForceUpdateCanvases();
        scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
	}

    private int GetLogNumber()
    {
        if (logs.GetComponent<Text>().text == String.Empty) return 0;

        return logs.GetComponent<Text>().text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length;
    }

	private void UpdateVisualizationSpeed()
	{
		waitTime = visualizationSpeedSlider.value * -1;
	}

	private bool IsPaused()
	{
		SwapManagerScript sms = GameObject.Find("Controls").GetComponent<SwapManagerScript>();
		if (sms == null) return false;

		return sms.isPaused;
	}

	private void Reset()
	{
		// set isRunning to false
		gameObject.GetComponent<TreeScript>().IsRunning = false;
		gameObject.GetComponent<TreeScript>().Unlock();
	}

	void HandleVisualizationItem(BSTVisualItem item, bool isDefaultColor)
	{
		switch(item.Type)
		{
			case (int)VisualType.Node:
            case (int)VisualType.InorderSuccessor:
                HandleNode(item, false);
				break;
			case (int)VisualType.InorderSuccessorFound:
				HandleInorderSuccessorFound(item);
				break;
			case (int)VisualType.LeftArrow:
				HandleArrow(item, false);
				break;
			case (int)VisualType.RightArrow:
				HandleArrow(item, false);
				break;
			case (int)VisualType.SpawnNode:
				HandleSpawnNode(isDefaultColor, item.IsLeftNode, item.EnteredKey, item.ParentNode);
				break;
			case (int)VisualType.DestroyNode:
				HandleDestroyNode(item, isDefaultColor);
				break;
			case (int)VisualType.SetNodeKey:
				HandleSetNodeKey(item, isDefaultColor);
				break;
            case (int)VisualType.InorderSuccessorMove:
                HandleInorderSuccessorMove(item, isDefaultColor);
                break;
        }
	}

	void HandleVisualizationItemBackwards(BSTVisualItem item, bool isDefaultColor)
	{
		switch (item.Type)
		{
			case (int)VisualType.Node:
            case (int)VisualType.InorderSuccessor:
                HandleNode(item, false);
				break;
			case (int)VisualType.InorderSuccessorFound:
				HandleInorderSuccessorFoundBackwards(item);
				break;
			case (int)VisualType.LeftArrow:
				HandleArrow(item, false);
				break;
			case (int)VisualType.RightArrow:
				HandleArrow(item, false);
				break;
			case (int)VisualType.SpawnNode:
				HandleSpawnNodeBackwards(item.IsLeftNode, item.ParentNode);
				break;
            case (int)VisualType.InorderSuccessorMove:
                HandleInorderSuccessorMoveBackwards(item, isDefaultColor);
                break;
            case (int)VisualType.DestroyNode:
                HandleDestroyNodeBackwards(item, isDefaultColor);
                break;
            case (int)VisualType.SetNodeKey:
                HandleSetNodeKeyBackwards(item, isDefaultColor);
                break;
        }
	}

	private void HandleInorderSuccessorFound(BSTVisualItem item)
	{
		HandleColors(item, false);
		DeleteTempInorderSuccessorNode();
	}

	private void HandleInorderSuccessorFoundBackwards(BSTVisualItem item)
	{
        HandleColors(item, false);
        DeleteTempInorderSuccessorNode();
	}

	private void HandleSpawnNodeBackwards(bool isLeftNode, GameObject parentNode)
    {
        // just destroy this node (it's always leaf)
        if (isLeftNode)
            Destroy(parentNode.GetComponent<NodeScript>().LeftNode);
        else
            Destroy(parentNode.GetComponent<NodeScript>().RightNode);
    }

    // remove references and move inorderSuccessor to original node position
    private void HandleInorderSuccessorMove(BSTVisualItem item, bool isDefaultColor)
    {
		DeleteTempInorderSuccessorNode();
		if (item.Node == null) return;

        // spawn new Node with inorderSucc key+pos. move node to original node & destroy it.
        GameObject tempNode;
        tempNode = gameObject.GetComponent<TreeScript>().SpawnNode();
        tempNode.name = TEMP_INORDER_SUCCESSOR_NODE;
        tempNode.GetComponent<NodeScript>().SetKey(item.Node.GetComponent<NodeScript>().Key);
        tempNode.GetComponent<NodeScript>().SetNodeColor(false);
        tempNode.transform.position = item.Node.transform.position;

        LeanTween.moveLocal(tempNode, item.Dest, waitTime);
    }

    private void HandleInorderSuccessorMoveBackwards(BSTVisualItem item, bool isDefaultColor)
    {
		DeleteTempInorderSuccessorNode();
        // find gameobject with inorderSucc key
        if (item.Node == null)
            item.Node = GetNodeByKey(item.InorderKey);

		// also need to set previous BSTVisualItem node (type Node)
		if ((_visualizationCounter - 1) >= 0)
			_bstVisual.Items[_visualizationCounter - 1].Node = item.Node;

        // spawn new Node with inorderSucc key+dest. move node to inorderSucc & destroy it.
        GameObject tempNode;
        tempNode = gameObject.GetComponent<TreeScript>().SpawnNode();
        tempNode.name = TEMP_INORDER_SUCCESSOR_NODE;
        tempNode.GetComponent<NodeScript>().SetKey(item.Node.GetComponent<NodeScript>().Key);
        tempNode.GetComponent<NodeScript>().SetNodeColor(false);
        tempNode.transform.localPosition = item.Dest;

		LeanTween.moveLocal(tempNode, item.Node.transform.localPosition, waitTime);
    }

    void HandleNode(BSTVisualItem item, bool isDefaultColor)
	{
		if (item.Node == null) return;
		HandleColors(item, isDefaultColor);
	}

	void HandleArrow(BSTVisualItem item, bool isDefaultColor)
	{
		if (item.Node == null) return;
		HandleColors(item, isDefaultColor);
	}

	void HandleSpawnNode(bool isSpawned, bool isLeftNode, int key, GameObject parentNode)
	{
		gameObject.GetComponent<TreeScript>().SpawnNode(isLeftNode, key, parentNode);
	}

	void HandleDestroyNode(BSTVisualItem item, bool isDeleted)
	{
		if(item.Node.GetComponent<NodeScript>().ParentNode != null)
			item.Node.GetComponent<NodeScript>().ParentNode.GetComponent<NodeScript>().IsLocked = false;
		Destroy(item.Node);

        // if item.tempNode is set we need to refresh & reposition
        if (item.TempNode == null) return;

        // need to take node below here
        item.TempNode.GetComponent<NodeScript>().RefreshNode(item.ParentNode);
        Reposition(item.TempNode, item.ParentNode.transform.localPosition);
    }

    void HandleDestroyNodeBackwards(BSTVisualItem item, bool isDeleted)
    {
        if (item.ParentNode != null)
            item.ParentNode.GetComponent<NodeScript>().IsLocked = false;

        // item.Node is null at this point. spawn new node and set it to item.Node
        bool isLeftNode = false;
        if (item.ParentNode != null && item.ParentNode.GetComponent<NodeScript>().Key > item.EnteredKey)
            isLeftNode = true;

        GameObject node = gameObject.GetComponent<TreeScript>().SpawnNode(isLeftNode, item.EnteredKey, item.ParentNode);
        item.Node = node;

        // we also need to set previous BSTVisualItem item.Node
        if ((_visualizationCounter - 1) >= 0)
            _bstVisual.Items[_visualizationCounter - 1].Node = item.Node;

        // if item.tempNode is set we need to refresh & reposition
        if (item.TempNode == null) return;

        // need to take node below here
        item.TempNode.GetComponent<NodeScript>().RefreshNode(node);
        Reposition(item.TempNode, node.transform.localPosition);
    }

	private void Reposition(GameObject node, Vector3 parentPos)
	{
		if (node == null) return;

		Vector3 newPos = node.GetComponent<NodeScript>().GetNewPositionByParentPosition(parentPos);
		LeanTween.moveLocal(node, newPos, waitTime);

		if (node.GetComponent<NodeScript>().LeftNode != null)
			Reposition(node.GetComponent<NodeScript>().LeftNode, newPos);

		if (node.GetComponent<NodeScript>().RightNode != null)
			Reposition(node.GetComponent<NodeScript>().RightNode, newPos);
	}

	void HandleSetNodeKey(BSTVisualItem item, bool isSet)
	{
		if (item.Node == null) return;

		GameObject tempNode = GameObject.Find(TEMP_INORDER_SUCCESSOR_NODE);
		if (tempNode != null)
			Destroy(tempNode);

		item.Node.GetComponent<NodeScript>().SetNodeColor(isSet);
        item.Node.GetComponent<NodeScript>().SetKey(item.InorderKey);
	}

    void HandleSetNodeKeyBackwards(BSTVisualItem item, bool isSet)
    {
        if (item.Node == null) return;

        item.Node.GetComponent<NodeScript>().SetNodeColor(isSet);
        item.Node.GetComponent<NodeScript>().SetKey(item.EnteredKey);
    }

    private GameObject GetNodeByKey(int key)
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        foreach(GameObject node in nodes)
        {
            if (node.GetComponent<NodeScript>().Key == key)
                return node;
        }
        return null;
    }
	
	private void HandleColors(BSTVisualItem item, bool isDefaultColor)
	{
		switch (item.Type)
		{
			case (int)VisualType.Node:
			case (int)VisualType.InorderSuccessor:
			case (int)VisualType.InorderSuccessorFound:
			case (int)VisualType.SetNodeKey:
				if(item.Node != null)
					item.Node.GetComponent<NodeScript>().SetNodeColor(isDefaultColor);
				break;
			case (int)VisualType.LeftArrow:
				if (item.Node != null)
					item.Node.GetComponent<NodeScript>().SetArrowColor(isDefaultColor, true);
				break;
			case (int)VisualType.RightArrow:
				if (item.Node != null)
					item.Node.GetComponent<NodeScript>().SetArrowColor(isDefaultColor, false);
				break;
			case (int)VisualType.SpawnNode:
				break;
			case (int)VisualType.DestroyNode:
				break;
			case (int)VisualType.InorderSuccessorMove:
				break;
		}
	}

	private void DeleteTempInorderSuccessorNode()
	{
		GameObject tempNode = GameObject.Find(TEMP_INORDER_SUCCESSOR_NODE);
		if (tempNode != null)
			Destroy(tempNode);
	}
}
