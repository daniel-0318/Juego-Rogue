using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public BoardManager boardScript;

	private void Awake(){
		boardScript = GetComponent<BoardManager> ();

	}

	private void Start(){
		InitGame ();
	}

	void InitGame(){
		boardScript.SetupScene ();
	
	}

}
