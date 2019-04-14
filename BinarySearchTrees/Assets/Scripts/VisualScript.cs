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
		StartCoroutine(DoVisualize());
	}

	IEnumerator DoVisualize()
	{
		// add log msg, change color of node and arrow
		foreach(BSTVisualItem item in _bstVisual.Items)
		{
			waitTime = visualizationSpeedSlider.value * -1;
			logs.GetComponent<Text>().text += item.GetItemMessage() + System.Environment.NewLine;
			scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
			HandleVisualizationItem(item, false);

			yield return new WaitForSeconds(waitTime);

			HandleVisualizationItem(item, true);
		}
		Reset();
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
		//if (node == null) return;

		switch(item.Type)
		{
			case (int)VisualType.Node:
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
		}
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
