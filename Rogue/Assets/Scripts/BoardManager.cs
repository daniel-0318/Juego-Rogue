using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public int columns = 8;
	public int rows = 8;
	private Transform boardHolder;

	public GameObject [] floorTiles, outerWallTiles; // Arrays donde estan los gameObjects de los pisos y las paredes externas.

	public void SetupScene(){
		
		Debug.Log ("Se ejecuto!!");
		BoardSetup ();
	}

	//Encargado de pintar el tablero
	void BoardSetup(){

		boardHolder = new GameObject ("board").transform;
		for(int i = -1; i < columns +1 ;i++){
			Debug.Log ("Entro");

			for(int j = -1; j < rows +1 ;j++){
				Debug.Log ("Entro2");

				GameObject toInstantiate = GetRandomInArray (floorTiles);

				if (i == -1 || j == -1 || i == columns || j == rows) {
					toInstantiate = GetRandomInArray (outerWallTiles);
				}
			
				GameObject instance = Instantiate (toInstantiate,new Vector2(i,j), Quaternion.identity);
				instance.transform.SetParent (boardHolder);
			}

		}

	}

	GameObject GetRandomInArray(GameObject [] array){
		Debug.Log ("pidio");
		return array [Random.Range (0, array.Length)];
	}
}
