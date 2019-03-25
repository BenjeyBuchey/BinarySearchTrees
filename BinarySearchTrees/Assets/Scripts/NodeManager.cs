using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NodeManager {

	public static Vector3 ROOT_POSITION = new Vector3(-60.0f, 180.0f, 0.0f);
	public static float X_DIFF = 160.0f, Y_DIFF = 60.0f;

	// returns random number for a node
	public static int GetNumber()
	{
		return Random.Range(1, 100);
	}
}
