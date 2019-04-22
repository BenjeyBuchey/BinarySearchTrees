using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualScript : MonoBehaviour {

	private BSTVisual _bstVisual;
	private float waitTime = 1.0f;
	public GameObject logs, scrollView;
	public Slider visualizationSpeedSlider;
	private bool isBusy = false;
	private int _visualizationCounter = 0;
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
		StartCoroutine(DoVisualize());
	}

	public void StepForward()
	{
		if (isBusy) return;

		if (_visualizationCounter <= _bstVisual.Items.Count)
			StartCoroutine(DoStepForward());
	}

	public void StepBackwards()
	{
		if (isBusy) return;

		if (_visualizationCounter <= _bstVisual.Items.Count && _visualizationCounter > 0) // because with forward visual we are already one item ahead
			StartCoroutine(DoStepBackwards());
	}

    public void StepBegin()
    {
        if (isBusy) return;
        if (_visualizationCounter <= _bstVisual.Items.Count && _visualizationCounter > 0)
            StartCoroutine(DoStepBegin());
    }

	public void StepEnd()
	{
		if (isBusy) return;

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
			while (IsPaused() || isBusy)
				yield return null;

			if (_visualizationCounter > 0)
				HandleVisualizationItem(_bstVisual.Items[_visualizationCounter - 1], true);

			if (_visualizationCounter >= _bstVisual.Items.Count)
			{
				Reset();
				yield break;
			}

			UpdateVisualizationSpeed();
			AddLogEntry(_bstVisual.Items[_visualizationCounter].GetItemMessage());
			HandleVisualizationItem(_bstVisual.Items[_visualizationCounter], false);

			yield return new WaitForSeconds(waitTime);

			//HandleVisualizationItem(item, true);
		}
		if (_visualizationCounter <= _bstVisual.Items.Count)
			HandleVisualizationItem(_bstVisual.Items[_visualizationCounter - 1], true);

		Reset();
	}

	IEnumerator DoStepForward()
	{
		isBusy = true;
		UpdateVisualizationSpeed();

		if (_visualizationCounter > 0)
			HandleVisualizationItem(_bstVisual.Items[_visualizationCounter - 1], true);

        if (_visualizationCounter >= _bstVisual.Items.Count)
        {
            isBusy = false;
            yield break;
        }

		AddLogEntry(_bstVisual.Items[_visualizationCounter].GetItemMessage());
		HandleVisualizationItem(_bstVisual.Items[_visualizationCounter], false);
		yield return new WaitForSeconds(waitTime);

		_visualizationCounter++;
		isBusy = false;
	}

	IEnumerator DoStepBackwards()
	{
		isBusy = true;
        _visualizationCounter--; // now we are at last processed item
        Debug.Log("VISUAL COUNTER: " + _visualizationCounter + " - ITEM COUNT: " + _bstVisual.Items.Count);
        if (_visualizationCounter < _bstVisual.Items.Count)
			HandleVisualizationItemBackwards(_bstVisual.Items[_visualizationCounter], true); // execute item here

		UpdateVisualizationSpeed();
		RemoveLastLogEntry();
        if (_visualizationCounter > 0)
        {
            HandleVisualizationItemBackwards(_bstVisual.Items[_visualizationCounter - 1], false); // only set color here
        }
        else
        {
            isBusy = false;
            yield break;
        }

        yield return new WaitForSeconds(waitTime);
        //_visualizationCounter--;
        isBusy = false;
	}

    IEnumerator DoStepBegin()
    {
        isBusy = true;
        waitTime = 0.1f;

        for (; _visualizationCounter >= 0; --_visualizationCounter)
        {
            if (_visualizationCounter < _bstVisual.Items.Count)
                HandleVisualizationItemBackwards(_bstVisual.Items[_visualizationCounter], true);

            RemoveLastLogEntry();
            if (_visualizationCounter > 0)
            {
                HandleVisualizationItemBackwards(_bstVisual.Items[_visualizationCounter - 1], false);
            }
            else
            {
                isBusy = false;
                yield break;
            }
            yield return new WaitForSeconds(waitTime);
        }

            isBusy = true;
    }

	IEnumerator DoStepEnd()
	{
		isBusy = true;
		waitTime = 0.1f;

		for (; _visualizationCounter < _bstVisual.Items.Count; _visualizationCounter++)
		{
			if (_visualizationCounter > 0)
				HandleVisualizationItem(_bstVisual.Items[_visualizationCounter - 1], true);

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
			HandleVisualizationItem(_bstVisual.Items[_visualizationCounter - 1], true);

		Reset();
		isBusy = false;
	}

	private void AddLogEntry(string msg)
	{
		if (logs.GetComponent<Text>().text != String.Empty)
			logs.GetComponent<Text>().text += System.Environment.NewLine;

		logs.GetComponent<Text>().text += msg;

        Canvas.ForceUpdateCanvases();
        scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
	}

	private void RemoveLastLogEntry()
	{
		string text = logs.GetComponent<Text>().text;
        int index = text.LastIndexOf(System.Environment.NewLine);

        if (index < 0)
            logs.GetComponent<Text>().text = String.Empty;
        else
            logs.GetComponent<Text>().text = text.Substring(0, index);

        Canvas.ForceUpdateCanvases();
        scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
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
		GameObject node = item.Node;

		switch(item.Type)
		{
			case (int)VisualType.Node:
            case (int)VisualType.InorderSuccessor:
            case (int)VisualType.InorderSuccessorFound:
                HandleNode(node, isDefaultColor);
				break;
			case (int)VisualType.LeftArrow:
				HandleArrow(node, isDefaultColor, true);
				break;
			case (int)VisualType.RightArrow:
				HandleArrow(node, isDefaultColor, false);
				break;
			case (int)VisualType.SpawnNode:
				HandleSpawnNode(isDefaultColor, item.IsLeftNode, item.EnteredKey, item.ParentNode);
				break;
			case (int)VisualType.DestroyNode:
				HandleDestroyNode(node, isDefaultColor);
				break;
			case (int)VisualType.RefreshNode:
				HandleRefreshNode(node, isDefaultColor, item.ParentNode);
				break;
			case (int)VisualType.SetNodeKey:
				HandleSetNodeKey(node, isDefaultColor, item.EnteredKey);
				break;
            case (int)VisualType.InorderSuccessorMove:
                HandleInorderSuccessorMove(item, isDefaultColor);
                break;
            case (int)VisualType.InorderSuccessorDestroy:
                HandleInorderSuccessorDestroy(item, isDefaultColor);
                break;
        }
	}

    void HandleVisualizationItemBackwards(BSTVisualItem item, bool isDefaultColor)
	{
		GameObject node = item.Node;

		switch (item.Type)
		{
			case (int)VisualType.Node:
            case (int)VisualType.InorderSuccessor:
            case (int)VisualType.InorderSuccessorFound:
                HandleNode(node, isDefaultColor);
				break;
			case (int)VisualType.LeftArrow:
				HandleArrow(node, isDefaultColor, true);
				break;
			case (int)VisualType.RightArrow:
				HandleArrow(node, isDefaultColor, false);
				break;
			case (int)VisualType.SpawnNode:
				HandleSpawnNodeBackwards(isDefaultColor, item.IsLeftNode, item.EnteredKey, item.ParentNode);
				break;
            case (int)VisualType.InorderSuccessorMove:
                HandleInorderSuccessorMoveBackwards(item, isDefaultColor);
                break;
            case (int)VisualType.InorderSuccessorDestroy:
                HandleInorderSuccessorDestroyBackwards(item, isDefaultColor);
                break;
                //case (int)VisualType.DestroyNode:
                //	HandleDestroyNode(node, isDefaultColor);
                //	break;
                //case (int)VisualType.RefreshNode:
                //	HandleRefreshNode(node, isDefaultColor, item.ParentNode);
                //	break;
                //case (int)VisualType.SetNodeKey:
                //	HandleSetNodeKey(node, isDefaultColor, item.EnteredKey);
                //	break;
        }
	}

    private void HandleSpawnNodeBackwards(bool isSpawned, bool isLeftNode, int key, GameObject parentNode)
    {
        //if (isSpawned) return;

        // just destroy this node (it's always leaf)
        if (isLeftNode)
            Destroy(parentNode.GetComponent<NodeScript>().LeftNode);
        else
            Destroy(parentNode.GetComponent<NodeScript>().RightNode);
    }

    // remove references and move inorderSuccessor to original node position
    private void HandleInorderSuccessorMove(BSTVisualItem item, bool isDefaultColor)
    {
        if (item.Node == null) return;

        item.Node.GetComponent<NodeScript>().SetNodeColor(isDefaultColor);
        // remove references from this node
        if (!isDefaultColor)
        {
            item.Node.GetComponent<NodeScript>().RemoveReferences();
            LeanTween.moveLocal(item.Node, item.Dest, waitTime);
        }
    }

    //delete inorderSuccessor. set original node key to inorderSuccessor key
    private void HandleInorderSuccessorDestroy(BSTVisualItem item, bool isDefaultColor)
    {
        if (item.TempNode == null) return;

        item.TempNode.GetComponent<NodeScript>().SetNodeColor(isDefaultColor);
        if (!isDefaultColor)
        {
            GameObject inorderSuccessor = item.Node;
            GameObject originalNode = item.TempNode;
            originalNode.GetComponent<NodeScript>().SetKey(inorderSuccessor.GetComponent<NodeScript>().Key);
            //Destroy(inorderSuccessor);
        }
    }

    private void HandleInorderSuccessorMoveBackwards(BSTVisualItem item, bool isDefaultColor)
    {
        if (item.Node == null) return;

        item.Node.GetComponent<NodeScript>().SetNodeColor(isDefaultColor);
        // set references & move
        if (!isDefaultColor) return;

        item.ParentNode.GetComponent<NodeScript>().SetChildNodeLeft(item.Node, item.Node.GetComponent<NodeScript>().Key);
        LeanTween.moveLocal(item.Node, item.InorderPosition, waitTime);

        // set node of previous visualItem (inorderSuccessor)
        int prevIndex = _visualizationCounter - 1;
        if (prevIndex >= 0 || prevIndex < _bstVisual.Items.Count)
            _bstVisual.Items[prevIndex].Node = item.Node;
    }

    private void HandleInorderSuccessorDestroyBackwards(BSTVisualItem item, bool isDefaultColor)
    {
        if (item.TempNode == null) return;

        item.TempNode.GetComponent<NodeScript>().SetNodeColor(isDefaultColor);
        if (!isDefaultColor) return;

        GameObject inorderSuccessor = gameObject.GetComponent<TreeScript>().SpawnNode();
        inorderSuccessor.transform.localPosition = item.InorderPosition;
        inorderSuccessor.GetComponent<NodeScript>().SetKey(item.EnteredKey);
        item.Node = inorderSuccessor;

        GameObject originalNode = item.TempNode;
        originalNode.GetComponent<NodeScript>().SetKey(_bstVisual.Key);

        // set node of previous visualItem
        int prevIndex = _visualizationCounter - 1;
        if (prevIndex >= 0 || prevIndex < _bstVisual.Items.Count)
            _bstVisual.Items[prevIndex].Node = inorderSuccessor;
    }

    void HandleNode(GameObject node, bool isDefaultColor)
	{
		if (node == null) return;
		node.GetComponent<NodeScript>().SetNodeColor(isDefaultColor);
	}

	void HandleArrow(GameObject node, bool isDefaultColor, bool isLeftArrow)
	{
		if (node == null) return;
		node.GetComponent<NodeScript>().SetArrowColor(isDefaultColor, isLeftArrow);
	}

	void HandleSpawnNode(bool isSpawned, bool isLeftNode, int key, GameObject parentNode)
	{
		if (isSpawned) return;
		gameObject.GetComponent<TreeScript>().SpawnNode(isLeftNode, key, parentNode);
	}

	void HandleDestroyNode(GameObject node, bool isDeleted)
	{
		if (isDeleted) return;
		if(node.GetComponent<NodeScript>().ParentNode != null)
			node.GetComponent<NodeScript>().ParentNode.GetComponent<NodeScript>().IsLocked = false;
		Destroy(node);
	}

	void HandleRefreshNode(GameObject node, bool isRefreshed, GameObject parentNode)
	{
		if (isRefreshed) return;

		//if (parentNode != null)
			//parentNode.GetComponent<NodeScript>().IsLocked = false;
		node.GetComponent<NodeScript>().RefreshNode(parentNode);
		Reposition(node, parentNode.transform.localPosition);
	}

	private void Reposition(GameObject node, Vector3 parentPos)
	{
		if (node == null) return;

		Vector3 newPos = node.GetComponent<NodeScript>().GetNewPositionByParentPosition(parentPos);
		LeanTween.moveLocal(node, newPos, waitTime);
		Debug.Log("MOVING " + node.GetComponent<NodeScript>().Key + " - Current Pos: " + node.transform.localPosition + " - New Pos: " + newPos);

		if (node.GetComponent<NodeScript>().LeftNode != null)
			Reposition(node.GetComponent<NodeScript>().LeftNode, newPos);

		if (node.GetComponent<NodeScript>().RightNode != null)
			Reposition(node.GetComponent<NodeScript>().RightNode, newPos);
	}

	void HandleSetNodeKey(GameObject node, bool isSet, int key)
	{
		if (node == null || isSet) return;

		node.GetComponent<NodeScript>().SetKey(key);
	}
}
