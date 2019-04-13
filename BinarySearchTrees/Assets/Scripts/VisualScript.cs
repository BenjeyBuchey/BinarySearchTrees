using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualScript : MonoBehaviour {

	private BSTVisual _bstVisual;
	private float waitTime = 1.0f;
	public GameObject logs, scrollView;
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
	}

	void HandleVisualizationItem(BSTVisualItem item, bool isDefaultColor)
	{
		GameObject node = item.Node;
		if (node == null) return;

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
				HandleSpawnNode(node, isDefaultColor);
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

	void HandleSpawnNode(GameObject node, bool isSpawned)
	{
		if (node == null || isSpawned) return;
		node.GetComponent<NodeScript>().Activate(true);
	}
}
