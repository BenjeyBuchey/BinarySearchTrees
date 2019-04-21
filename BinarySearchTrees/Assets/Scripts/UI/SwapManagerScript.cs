using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwapManagerScript : MonoBehaviour {

	public Button pauseResume;
	public Sprite pauseSprite, resumeSprite;
	public bool isPaused = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPauseResumeClick()
	{
		SetPaused(!isPaused);
	}

	private void SetPaused(bool isPaused_)
	{
		isPaused = isPaused_;
		if (isPaused)
		{
			pauseResume.image.sprite = resumeSprite;
		}
		else
		{
			pauseResume.image.sprite = pauseSprite;
			// resume movescripts
			Resume();
		}
	}

	private void Resume()
	{
		VisualScript visualScript = FindObjectOfType<VisualScript>();
		if (visualScript == null) return;

		visualScript.ResumeVisualization();

		//MoveScript[] moveScripts = FindObjectsOfType<MoveScript>() as MoveScript[];
		//if (moveScripts == null || moveScripts.Length == 0) return;

		//foreach (MoveScript ms in moveScripts)
		//{
		//	ms.ResumeVisualization();
		//}
	}

	public void Begin()
	{
		if (!isPaused) return;

		
	}

	public void End()
	{
		if (!isPaused) return;

		VisualScript visualScript = FindObjectOfType<VisualScript>();
		if (visualScript == null) return;

		visualScript.StepEnd();
	}

	// execute next step for all sorting boxes that are in use (we can just find all movescripts)
	public void Next()
	{
		if (!isPaused) return;

		VisualScript visualScript = FindObjectOfType<VisualScript>();
		if (visualScript == null) return;

		visualScript.StepForward();
	}

	public void Previous()
	{
		if (!isPaused) return;

		//MoveScript[] moveScripts = FindObjectsOfType<MoveScript>() as MoveScript[];
		//if (moveScripts == null || moveScripts.Length == 0) return;

		//foreach (MoveScript ms in moveScripts)
		//{
		//	ms.StepBackwards();
		//}
	}
}
