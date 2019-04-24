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
				//HandleVisualizationItem(_bstVisual.Items[_visualizationCounter - 1], true);

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
			HandleColors(_bstVisual.Items[_visualizationCounter - 1], true);
			//HandleVisualizationItem(_bstVisual.Items[_visualizationCounter - 1], true);
	
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
        Debug.Log("VISUAL COUNTER: " + _visualizationCounter + " - ITEM COUNT: " + _bstVisual.Items.Count);
		if ((_visualizationCounter+1) < _bstVisual.Items.Count)	//if (_visualizationCounter < _bstVisual.Items.Count)
			HandleColors(_bstVisual.Items[_visualizationCounter+1], true);	//HandleVisualizationItemBackwards(_bstVisual.Items[_visualizationCounter], true);

		UpdateVisualizationSpeed();
		RemoveLastLogEntry();
        if (_visualizationCounter > 0)
        {
            HandleVisualizationItemBackwards(_bstVisual.Items[_visualizationCounter], false); // -1
        }
        else
        {
            _isBusy = false;
            yield break;
        }

        yield return new WaitForSeconds(waitTime);
        //_visualizationCounter--;
        _isBusy = false;
	}

    IEnumerator DoStepBegin()
    {
        _isBusy = true;
        waitTime = 0.1f;

        for (; _visualizationCounter >= 0; --_visualizationCounter)
        {
            if (_visualizationCounter < _bstVisual.Items.Count)
				HandleColors(_bstVisual.Items[_visualizationCounter], true);	//HandleVisualizationItemBackwards(_bstVisual.Items[_visualizationCounter], true);

			RemoveLastLogEntry();
            if (_visualizationCounter > 0)
            {
                HandleVisualizationItemBackwards(_bstVisual.Items[_visualizationCounter - 1], false);
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
				HandleColors(_bstVisual.Items[_visualizationCounter-1], true);//HandleVisualizationItem(_bstVisual.Items[_visualizationCounter - 1], true);

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
			HandleColors(_bstVisual.Items[_visualizationCounter-1], true);	//HandleVisualizationItem(_bstVisual.Items[_visualizationCounter - 1], true);

		Reset();
		_isBusy = false;
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
		Debug.Log("DOING FORWARD STEP TYPE " + item.Type);

		switch(item.Type)
		{
			case (int)VisualType.Node:
            case (int)VisualType.InorderSuccessor:
            //case (int)VisualType.InorderSuccessorFound:
                HandleNode(item, isDefaultColor);
				break;
			case (int)VisualType.InorderSuccessorFound:
				HandleInorderSuccessorFound(item);
				break;
			case (int)VisualType.LeftArrow:
				HandleArrow(item, isDefaultColor, true);
				break;
			case (int)VisualType.RightArrow:
				HandleArrow(item, isDefaultColor, false);
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
            case (int)VisualType.InorderSuccessorDestroy:
                HandleInorderSuccessorDestroy(item, isDefaultColor);
                break;
        }
	}

	void HandleVisualizationItemBackwards(BSTVisualItem item, bool isDefaultColor)
	{
		Debug.Log("DOING BACKWARDS STEP TYPE " + item.Type);

		switch (item.Type)
		{
			case (int)VisualType.Node:
            case (int)VisualType.InorderSuccessor:
            //case (int)VisualType.InorderSuccessorFound:
                HandleNode(item, isDefaultColor);
				break;
			case (int)VisualType.InorderSuccessorFound:
				HandleInorderSuccessorFoundBackwards(item);
				break;
			case (int)VisualType.LeftArrow:
				HandleArrow(item, isDefaultColor, true);
				break;
			case (int)VisualType.RightArrow:
				HandleArrow(item, isDefaultColor, false);
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
            case (int)VisualType.DestroyNode:
                HandleDestroyNodeBackwards(item, isDefaultColor);
                break;
            //case (int)VisualType.RefreshNode:
            //	HandleRefreshNode(node, isDefaultColor, item.ParentNode);
            //	break;
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
		DeleteTempInorderSuccessorNode();
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
		DeleteTempInorderSuccessorNode();
		if (item.Node == null) return;

        GameObject tempNode;
        // spawn new Node with inorderSucc key+pos. move node to original node & destroy it.
        //if (!isDefaultColor)
        //{
            tempNode = gameObject.GetComponent<TreeScript>().SpawnNode();
            tempNode.name = TEMP_INORDER_SUCCESSOR_NODE;
            tempNode.GetComponent<NodeScript>().SetKey(item.Node.GetComponent<NodeScript>().Key);
            tempNode.GetComponent<NodeScript>().SetNodeColor(false);
            tempNode.transform.position = item.Node.transform.position;

            LeanTween.moveLocal(tempNode, item.Dest, waitTime);
        //}
        //else
        //{
        //    tempNode = GameObject.Find(TEMP_INORDER_SUCCESSOR_NODE);
        //    if (tempNode != null)
        //        Destroy(tempNode);
        //}
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

		GameObject tempNode;
        // spawn new Node with inorderSucc key+dest. move node to inorderSucc & destroy it.
        //if (!isDefaultColor)
        //{
            tempNode = gameObject.GetComponent<TreeScript>().SpawnNode();
            tempNode.name = TEMP_INORDER_SUCCESSOR_NODE;
            tempNode.GetComponent<NodeScript>().SetKey(item.Node.GetComponent<NodeScript>().Key);
            tempNode.GetComponent<NodeScript>().SetNodeColor(false);
            tempNode.transform.localPosition = item.Dest;
			Debug.Log("TEMPNODE SPAWN: " + tempNode.transform.position);
			Debug.Log("TEMPNODE DEST: " + item.Node.transform.position);

			LeanTween.moveLocal(tempNode, item.Node.transform.localPosition, waitTime);
        //}
        //else
        //{
        //    tempNode = GameObject.Find(TEMP_INORDER_SUCCESSOR_NODE);
        //    if (tempNode != null)
        //        Destroy(tempNode);
        //}
    }

    //delete inorderSuccessor. set original node key to inorderSuccessor key
    private void HandleInorderSuccessorDestroy(BSTVisualItem item, bool isDefaultColor)
    {
        //if (item.TempNode == null) return;

        //item.TempNode.GetComponent<NodeScript>().SetNodeColor(isDefaultColor);
        //if (!isDefaultColor)
        //{
        //    GameObject inorderSuccessor = item.Node;
        //    GameObject originalNode = item.TempNode;
        //    originalNode.GetComponent<NodeScript>().SetKey(inorderSuccessor.GetComponent<NodeScript>().Key);
        //    Destroy(inorderSuccessor);
        //}
    }

    private void HandleInorderSuccessorDestroyBackwards(BSTVisualItem item, bool isDefaultColor)
    {
        //if (item.TempNode == null) return;

        //item.TempNode.GetComponent<NodeScript>().SetNodeColor(isDefaultColor);
        //if (!isDefaultColor) return;

        //GameObject inorderSuccessor = gameObject.GetComponent<TreeScript>().SpawnNode();
        //inorderSuccessor.transform.localPosition = item.InorderPosition;
        //inorderSuccessor.GetComponent<NodeScript>().SetKey(item.EnteredKey);
        //item.Node = inorderSuccessor;

        //GameObject originalNode = item.TempNode;
        //originalNode.GetComponent<NodeScript>().SetKey(_bstVisual.Key);

        //// set node of previous visualItem
        //int prevIndex = _visualizationCounter - 1;
        //if (prevIndex >= 0 || prevIndex < _bstVisual.Items.Count)
        //    _bstVisual.Items[prevIndex].Node = inorderSuccessor;
    }

    void HandleNode(BSTVisualItem item, bool isDefaultColor)
	{
		if (item.Node == null) return;
		//item.Node.GetComponent<NodeScript>().SetNodeColor(isDefaultColor);

		HandleColors(item, false);
	}

	void HandleArrow(BSTVisualItem item, bool isDefaultColor, bool isLeftArrow)
	{
		if (item.Node == null) return;
		//item.Node.GetComponent<NodeScript>().SetArrowColor(isDefaultColor, isLeftArrow);

		HandleColors(item, false);
	}

	void HandleSpawnNode(bool isSpawned, bool isLeftNode, int key, GameObject parentNode)
	{
		//if (isSpawned) return;
		gameObject.GetComponent<TreeScript>().SpawnNode(isLeftNode, key, parentNode);
	}

	void HandleDestroyNode(BSTVisualItem item, bool isDeleted)
	{
		//if (isDeleted) return;
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
        //if (!isDeleted) return;
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
