using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NodeManager {

	public static Vector3 ROOT_POSITION = new Vector3(0.0f, 150.0f, 0.0f); // X -60.0f 
	public static float X_DIFF = 150.0f, Y_DIFF = 60.0f;

	// returns random number for a node
	public static int GetNumber()
	{
		return Random.Range(1, 100);
	}
}
