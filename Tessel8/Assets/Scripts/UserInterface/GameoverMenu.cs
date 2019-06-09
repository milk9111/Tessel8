using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class GameoverMenu : MonoBehaviour {

	public GameController gameController;

	public void OnRestart()
	{
		gameController.StartGame();
	}

	public void OnExit()
	{
		Application.Quit();
	}
}
