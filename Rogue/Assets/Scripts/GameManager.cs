using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour {

	public static GameManager instance;//para el singleton
	public float turnDelay = 0.01f;
	public float levelStartDelay = 2f; // total de segundo que la ventana de inicio sera visible
	public bool doingSetup; //sirve para saber si aun se esta preparando la escena, pasa a true luego del tiempo levelStartDelay

	public BoardManager boardScript;

	public int PlayerHealthtPoints = 100;
	public int PlayerammoPoints = 10;
 	public bool PlayerTurn = true; //Por defecto comienza moviendo el player
	public List<int> numeroPasosJugador = new List<int>();

	private string rutaGuardarCargar;
	private int numero_archivo_txt;

	private List<Enemy> enemies = new List<Enemy>(); //lista de enemigos para controlar los moviendo de ellos
	private bool enemiesMoving; //Por defecto se inicializa en falso


	SaveLoad datos = new SaveLoad();

	GameObject[] sodas,foods,ammos; //Toca tener sus instancias ya que una vez desactivadas no aparecen con findObjectswhithtag

	private int level = 0;
	private GameObject levelImage;
	private Text levelText;

	private bool moveToNode = false;
	private Vector2 coordeNode;


	private void Awake(){
		rutaGuardarCargar =  Application.persistentDataPath + "/datos.dat";
		numero_archivo_txt =  numero_de_guardado_libre ();
		//Directorio donde se guarda el archivo serializado
		Debug.Log (Application.persistentDataPath);
		//esto es para volverlo singleton
		if (GameManager.instance == null) {
			
			GameManager.instance = this;

		}else if (GameManager.instance != this){
			
			Destroy (gameObject);
			
		}

		DontDestroyOnLoad (gameObject);

		boardScript = GetComponent<BoardManager> ();

	}

	void InitGame(){
		doingSetup = true;
		moveToNode = false;
		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text>();
		levelText.text = "Day " + level;
		levelImage.SetActive (true);
		enemies.Clear ();
		boardScript.SetupScene (level); //crear nivel
		setListaItems();
		Invoke ("HideLevelImage",levelStartDelay); //invoke sirve para ejecutar un metodo luego de cierto tiempo (levelstardelay)
	
	}

	private void HideLevelImage(){
		levelImage.SetActive(false);
		doingSetup = false;
	}

	public void GameOver(){
		guardar (1);
		levelText.text = "After " + level + " days, you starved.";
		levelImage.SetActive(true);
		enabled = false;
	}

	IEnumerator MoveEnemies(){
		enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);
		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}

		for(int i=0;i<enemies.Count;i++){
			if (!enemies [i].gameObject.activeInHierarchy) {
			}
			if (enemies [i].gameObject.activeInHierarchy) {

				if (enemies [i].tipoMovimiento == 3) {
					enemies [i].SetPosicionNodo ((int)coordeNode.x, (int)coordeNode.y);
					enemies [i].RealizarMovimiento ();
				} else {
					enemies [i].RealizarMovimiento ();
				}
			}

			yield return new WaitForSeconds (enemies[i].moveTime);
			
		}

		PlayerTurn = true;
		enemiesMoving = false;

	}

	private void Update(){
		if (PlayerTurn || enemiesMoving || doingSetup)
			return;

		StartCoroutine (MoveEnemies());
	}

	public void AddEnemyToList(Enemy enemy){
		enemies.Add (enemy);
	}

	/**Proposito: Cuando el componente GameManager se activa se notifica a SceneLoaded
	 *  y que llame a OnLevelFinishedLoading cuando se detecte que se a cargado la escena
	 */
	private void OnEnable(){
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}


	/**Proposito: Cuando el componente GameManager se activa se notifica a SceneLoaded
	 *  y que quite a OnLevelFinishedLoading;
	 */
	private void OnDisable(){
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}


	/**Se llama cada vez que se carga alguna escena*/
	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode){
		Debug.Log ("Se inicio");
		level++;
		InitGame();
	}

	public void setCoordeNode(Vector2 coorde){
		coordeNode = coorde;
		resetNodeEnemies ();
		Debug.Log ("vector nodo: "  + coorde);
	}

	public float getCoordeNodeX(){
		return coordeNode.x;
	}

	public float getCoordeNodeY(){
		return coordeNode.y;
	}

	public void resetNodeEnemies(){
		Debug.Log ("Enemigo, reseteando nodo");
		for (int i = 0; i < enemies.Count; i++) {
			enemies [i].setGoalOk (false);
		}
	}

	public void setListaItems(){
		sodas = GameObject.FindGameObjectsWithTag ("Soda");
		foods = GameObject.FindGameObjectsWithTag ("Food");
		ammos = GameObject.FindGameObjectsWithTag ("Ammo");
	}

	/*Función que sirve para obtener la lista de items de cada nivel (posx, posy, adquiriojugador)*/
	public List<List<int>> getListaItems(){
		
		List<List<int>> listaItems = new List<List<int>> ();

		for (int i = 0; i < sodas.Length; i++) {
			List <int> item = new List<int> (); //		SE PODRIA GUARDAR EL VECTOR POSICION PERO ASI SE PUEDE AÑADIR NUEVA INFORMACION A GUARDAR
			item.Add ((int)sodas [i].transform.position.x);
			item.Add ((int)sodas [i].transform.position.y);
			if (sodas [i].activeInHierarchy) {
				item.Add (1);
			} else {
				item.Add (0);
			}
			listaItems.Add (item);
		}
		for (int i = 0; i < foods.Length; i++) {
			List <int> item = new List<int> ();
			item.Add ((int)foods [i].transform.position.x);
			item.Add ((int)foods [i].transform.position.y);
			if (foods [i].activeInHierarchy) {
				item.Add (1);
			} else {
				item.Add (0);
			}
			listaItems.Add (item);
		}
		for (int i = 0; i < ammos.Length; i++) {
			List <int> item = new List<int> ();
			item.Add ((int)ammos [i].transform.position.x);
			item.Add ((int)ammos [i].transform.position.y);
			if (ammos [i].activeInHierarchy) {
				item.Add (1);
			} else {
				item.Add (0);
			}
			listaItems.Add (item);
		}

		return listaItems;
	}

	/*Función que sirve para obtener la distancia minima a un item desde la posicion del jugador */
	public int itemMasCercanoAlJugador(int posJugadorX, int posJugadorY){
		List<List<int>> listaItems = getListaItems ();
		int distanciaMinima= 99; //Distancia minima al item mas cercano

		for (int i = 0; i < listaItems.Count; i++) {

			int posX = listaItems [i][0];
			int posY = listaItems [i][1];

			int x = Math.Abs (posJugadorX - posX);
			int y = Math.Abs (posJugadorY - posY);

			if ((x + y) < distanciaMinima) {
				distanciaMinima = x + y;
			}

		}
		Debug.Log ("######### Distancia minima encontrada: " + distanciaMinima);
		return distanciaMinima;

	}

	public void guardar(int playerDead){
		Debug.Log ("Guardando");
		List<int> enemigosNivelActual = new List<int> (); //Guada de cada enemigo cantidadGolpes, IA, posFinalEnemigo(x,y) cada 4 posiones es un enemigo.
		List<List<int>> listaGolpesEnemigosNivelActual = new List<List<int>>();
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (rutaGuardarCargar);
		for (int i = 0;i<enemies.Count;i++) {
			enemigosNivelActual.Add (enemies [i].vecesGolepandoJugador);
			enemigosNivelActual.Add (enemies [i].tipoMovimiento);
			enemigosNivelActual.Add ((int)enemies [i].transform.position.x);
			enemigosNivelActual.Add ((int)enemies [i].transform.position.y);
			listaGolpesEnemigosNivelActual.Add (enemies [i].LugarDelGolpe);
		}

		//datos a guardar

		datos.numeroPasosJugador = numeroPasosJugador;
		datos.vidajugador.Add (PlayerHealthtPoints);
		if (playerDead == 1) {
			Debug.Log ("Se murio------------------------------------------------");
			List<int> posicion = enemies [0].getTargetPosition ();
			datos.jugadorMuerto = 1;
			datos.posicionMuerteJugador[0] = posicion [0];
			datos.posicionMuerteJugador[1] = posicion [1];
		} else {
			datos.posicionMuerteJugador.Clear ();
			datos.posicionMuerteJugador.Add (-1);
			datos.posicionMuerteJugador.Add (-1);
		}
		
		datos.listadoEnemigos.Add (enemigosNivelActual);
		datos.ListadoGolpesEnemigosNiveles.Add (listaGolpesEnemigosNivelActual);
		datos.ListadoitemsNiveles.Add (getListaItems ());

		datos.GuardarParaExportar (numero_archivo_txt);

		bf.Serialize (file, datos);
		file.Close ();

	}

	public void cargar(){
		if (datos == null) {
			Debug.Log ("Si es el error que tenia");
		}
		if (File.Exists(rutaGuardarCargar)) {
			Debug.Log ("cargando");
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (rutaGuardarCargar, FileMode.Open);
			SaveLoad datos1 = (SaveLoad)bf.Deserialize (file);
			file.Close ();

			datos.leerArchivosTxt (numero_archivo_txt);

		}
	}


	/*Funcion que sirve para elegir el nombre del archivo que se va a guardar que este libre (ej: datos0, datos1, datos2*/
	public int numero_de_guardado_libre(){
		int numero = 0;
		for (int i = 0; i < 100; i++) {
			if (!File.Exists ("Assets/datos"+i+".txt")) {
				Debug.Log ("No existe el txt numero:" + i);
				numero = i;
				break;
			}
		}

		return numero;

	}
		
}