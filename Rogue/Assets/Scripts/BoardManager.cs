﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public int columnas = 16;
	public int filas = 16;
	//contenedor para los objetos del piso y muros externos del juego para que no quede todo regado
	private Transform boardHolder;

	//Contenedor de los objetos internos como muros internos, comida, enemigos, y exit
	private Transform itemsHolder;

	// Arrays donde estan los gameObjects que se generan en el juego como las paredes, el piso, la comida, enemigos y la salida.
	public GameObject [] floorTiles, outerWallTiles, wallTiles, foodTiles, enemyTiles, ammos, nodePoints, coins;
	public GameObject exit;

	private List<Vector2> gridPositions = new List<Vector2>();//lista de casillas libres.


	public void SetupScene(int level){
		//genera el tablero.
		BoardSetup ();
		InitializeList (); // inicializa la lista de posiciones
		itemsHolder = new GameObject ("Items").transform;
		LayoutObjectAtRandom (wallTiles,5,9);
		LayoutObjectAtRandom (foodTiles,1,5);
		RandomCoins ();
		RandomAmmon ();
		RadomNodes (); //nodos para waypoint
		int enemyCount = (int)Mathf.Log (level+5, 2);// La cantidad de enemigos creceria logaritmicamente.
		LayoutObjectAtRandom (enemyTiles,enemyCount,enemyCount);
		GameObject objectInstan = Instantiate (exit, new Vector2(columnas-1, filas-1), Quaternion.identity ); //quaternion es la rotacion.
		objectInstan.transform.SetParent (itemsHolder);
	}

	//llena la lista
	public void InitializeList(){
		gridPositions.Clear();
		for (int x = 1; x<columnas-1 ; x++) {
			for(int y=1;y<filas-1;y++){
				gridPositions.Add (new Vector2(x,y));
			}
		}
	}

	/*Devuelve una position del tablero (x,y) para generar algun objeto en ella y tener un control para que luego en la
	 * misma posicion no se trate de colocar otro objeto*/
	Vector2 RandomPosition(){
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector2 randomPosition = gridPositions [randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	//Encargado de instanciar los objetos en el tablero (enemigos, comida o muros internos)
	public void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max){
		int objectCount = Random.Range (min,max+1); // numero de objetos que se instanciaran.
		for(int i=0; i < objectCount ; i++){
			Vector2 randomPosition = RandomPosition(); //posicion a instanciar
			GameObject tileChoice = GetRandomInArray(tileArray); // objeto que se va a instanciar (la imagen)
			GameObject objectInstan = Instantiate(tileChoice,randomPosition,Quaternion.identity);
			objectInstan.transform.SetParent (itemsHolder); // aca estamos metiendo el suelo en el objeto padre

			
		}
		
	}

	//Encargado de pintar el tablero que son el piso y los bordes del tableros
	public void BoardSetup(){
		Debug.Log ("Tamaño de column y row:  " + columnas + " " + filas);
		boardHolder = new GameObject ("board").transform; //Es el objeto contenedor para el suelo
		for(int i = -1; i < columnas +1 ;i++){
			for(int j = -1; j < filas +1 ;j++){

				GameObject toInstantiate = GetRandomInArray (floorTiles);

				if (i == -1 || j == -1 || i == columnas || j == filas) {
					toInstantiate = GetRandomInArray (outerWallTiles);
				}
			
				GameObject instance = Instantiate (toInstantiate,new Vector2(i,j), Quaternion.identity);
				instance.transform.SetParent (boardHolder);
			}

		}

	}

	GameObject GetRandomInArray(GameObject [] array){
		return array [Random.Range (0, array.Length)];
	}

	//Funcion que sirve para generar municion en el mapa con una probabilidad del 30%
	public void RandomAmmon(){
		int probabilidad = Random.Range (1, 10);
		if (probabilidad >= 7)
			LayoutObjectAtRandom (ammos, 1, 3);

	}

	public void RadomNodes(){
		LayoutObjectAtRandom (nodePoints, 3, 6);
	}

	public void RandomCoins(){
		LayoutObjectAtRandom (coins, 1, 5);
	}
}
