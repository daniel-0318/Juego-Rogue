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

	public int playerHealthtPoints = 100;
	public int playerammoPoints = 10;
	public int playerScorePoints = 0;
	public int playerKillsPoints = 0;

	public int comida_adquirida = 0;  // VARIABLE DE PRUEBA por si sale mas rentable contar cuantos items adquirio que almacenar la vida (numero grande)
	public int modenas_aquiridas = 0;

	public bool enemigoMuerto = false;
	public int secretosEncontrados = 0;
 	public bool PlayerTurn = true; //Por defecto comienza moviendo el player
	public List<int> numeroPasosJugador = new List<int>();

	private string rutaGuardarCargar;

	private List<Enemy> enemies = new List<Enemy>(); //lista de enemigos para controlar los moviendo de ellos
	private bool enemiesMoving; //Por defecto se inicializa en falso
	private bool ChangeEnemyMovement; // una vez detectado el tipo de jugador el tipo de movimiento de los enemigos cambia


	SaveLoad datos = new SaveLoad();
	int tipo_jugador_rn; //Tipo de jugador que la red neuronal detecto

	GameObject[] sodas,foods,ammos, coins; //Toca tener sus instancias ya que una vez desactivadas no aparecen con findObjectswhithtag

	private int level = 0;
	private GameObject levelImage;
	private Text levelText;
	private Text dayText;

	private bool moveToNode = false;
	private Vector2 coordeNode;

	public bool yaEjecuto = false;
	private int nivel_a_ejecutar_red_reuronal = 2;
	private int nivel_a_detectar_tipo_jugador = 7;

	private bool [] areas = new bool[5];


	private void Awake(){
		
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
		dayText = GameObject.Find ("DayText").GetComponent<Text>();
		dayText.text = "Day " + level;
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

				if (enemies [i].tipoMovimiento == 3 && !ChangeEnemyMovement) {
					enemies [i].SetPosicionNodo ((int)coordeNode.x, (int)coordeNode.y);
					enemies [i].RealizarMovimiento ();

				}else if(ChangeEnemyMovement && (tipo_jugador_rn == 1 || tipo_jugador_rn == 2 || tipo_jugador_rn == 0) ){//tipo jugador explorador o asesino encontrado

					Vector2 respuesta = new Vector2();
					if (enemies [i].get_skipMove () == true) {
						enemies [i].RealizarMovimiento ();
					} else if (!(tipo_jugador_rn == 2 && (i == 0)) ){
						//Debug.Log ("Posicion del objeto" + respuesta);

						if(enemies[i].get_maxTimeCamper() == 0){ //Dar un valor de cuando campeara el objeto
							Debug.Log("Cambio de tiempo de campeo");
							enemies[i].set_identifiedPlayer(tipo_jugador_rn);//sirve para que el goalOK del enemigo se maneje distinto dependiendo el tipo de jugador
							enemies [i].set_maxTimeCamper ();
							if (tipo_jugador_rn == 0) {
								Debug.Log ("##### ¡¡¡ Jugador tipo triunfador !!!!!!!!");
								respuesta = item_aleatorio ();
							} else if (tipo_jugador_rn == 2) {
								Debug.Log ("##### ¡¡¡ Jugador tipo asesino !!!!!!!!");
								respuesta = new Vector2(enemies [i - 1].transform.position.x, enemies [i - 1].transform.position.y);
								enemies [i].SetPosicionNodo ((int)respuesta.x, (int)respuesta.y);
							}else if(tipo_jugador_rn == 1){
								Debug.Log ("##### ¡¡¡ Jugador tipo explorador !!!!!!!!");
								Vector3 resp = selectedArea ();
								enemies [i].set_areaCamper ((int)resp.z);
								respuesta = new Vector2 (resp.x, resp.y);
							}
							enemies [i].SetPosicionNodo ((int)respuesta.x, (int)respuesta.y);
						}
						if (tipo_jugador_rn == 2) {
							Debug.Log ("##### ¡¡¡ Jugador tipo asesino !!!!!!!!");
							respuesta = new Vector2(enemies [i - 1].transform.position.x, enemies [i - 1].transform.position.y);
							enemies [i].SetPosicionNodo ((int)respuesta.x, (int)respuesta.y);
						}

						if (enemies[i].get_timeElapsedCamper() < enemies[i].get_maxTimeCamper()) {
							Debug.Log ("++++++++ cambio de movimiento a 3");
							enemies [i].setTypeOfMoviment (3);
						}

						//////////////////
						enemies [i].RealizarMovimiento ();
						/////////////////
						if (enemies [i].get_goalOK()) {/// si ya llego al objeto entonces comienza a sumar el tiempo de campeo
							Debug.Log("llegada al objetivo");
							enemies [i].set_timeElapsedCamper (1);
						}

						if (enemies [i].get_timeElapsedCamper() == enemies[i].get_maxTimeCamper()) {
							Debug.Log ("----- restauracion de movimiento");
							if (tipo_jugador_rn == 1) {
								areas [enemies [i].get_areaCamper ()] = false;
							}
							enemies [i].restoreMove();
						}

						Debug.Log ("~~~~~~ " + enemies [i].get_timeElapsedCamper () + " " +  enemies [i].get_timeResetCamper());
						if (enemies [i].get_timeElapsedCamper() > enemies [i].get_timeResetCamper() ) {
							Debug.Log (" ******* cambio de movimiento a 3");
							enemies [i].setTypeOfMoviment (3);
							enemies [i].reset_timesCampers ();
							enemies [i].set_goalOk(false);
						}
						
					}else if(tipo_jugador_rn == 2 && (i == 0)){
						Debug.Log ("##### ¡¡¡ Jugador tipo ASESINO primer zombi !!!!!!!!");
						enemies [i].RealizarMovimiento ();
					}

				}else {
					enemies [i].RealizarMovimiento ();
				}
			}

			yield return new WaitForSeconds (enemies[i].moveTime);
			
		}

		PlayerTurn = true;
		enemiesMoving = false;

	}

	private void Update(){
		if(level == nivel_a_ejecutar_red_reuronal && yaEjecuto == false){//Indica el nivel en el cual se ejecuta la red neuronal en python
			yaEjecuto = true;
			cargarRedNeuronal ();
		}

		if(level == (nivel_a_ejecutar_red_reuronal + nivel_a_detectar_tipo_jugador)){ //Indica en que nivel se buscara detectar al jugador
			revisar_tipo_jugador();
			Debug.Log ("========== " + tipo_jugador_rn);
			ChangeEnemyMovement = true;
			nivel_a_detectar_tipo_jugador += 4;
		}

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
		coins = GameObject.FindGameObjectsWithTag ("Coin");

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

		for (int i = 0; i < coins.Length; i++) {
			List <int> item = new List<int> ();
			item.Add ((int)coins [i].transform.position.x);
			item.Add ((int)coins [i].transform.position.y);
			if (coins [i].activeInHierarchy) {
				item.Add (1);
			} else {
				item.Add (0);
			}
			listaItems.Add (item);
		}

		return listaItems;
	}

	/*Función que sirve para obtener la distancia minima a un item desde una posicion dada */
	public Vector2 itemMasCercanoAlJugador(int posJugadorX, int posJugadorY){
		List<List<int>> listaItems = getListaItems ();
		int distanciaMinima= 99; //Distancia minima al item mas cercano
		Vector2 respuesta = new Vector2(0,0);

		for (int i = 0; i < listaItems.Count; i++) {

			int posX = listaItems [i][0];
			int posY = listaItems [i][1];

			int x = Math.Abs (posJugadorX - posX);
			int y = Math.Abs (posJugadorY - posY);

			if ((x + y) < distanciaMinima) {
				distanciaMinima = x + y;
				respuesta = new Vector2 (posX, posY);
			}

		}
		return respuesta;

	}

	public Vector2 item_aleatorio(){
		List<List<int>> listaItems = getListaItems ();
		int tamaño_lista_items = listaItems.Count;

		int pos_random = (int) UnityEngine.Random.Range (0, tamaño_lista_items);
		Vector2 respuesta = new Vector2 ( listaItems [pos_random][0], listaItems [pos_random][1]);

		return respuesta;
	}

	public void guardar(int playerDead){

		Debug.Log ("Guardando");

		datos.nivel = level;
		datos.numeroPasosJugador = numeroPasosJugador;
		datos.vidajugador.Add (playerHealthtPoints);

		datos.score.Add (playerScorePoints);
		datos.killsEnemies.Add (playerKillsPoints);

		if (secretosEncontrados >= 2) {
			datos.secretosEncontrados.Add (secretosEncontrados);
		} else {
			datos.secretosEncontrados.Add (0);
		}


		datos.GuardarParaExportar ();

	}

	public void cargarRedNeuronal(){

		RedNeuronal rn = new RedNeuronal();
		rn.ExecuteCommand ();

	}

	public void revisar_tipo_jugador(){
		SaveLoad tp = new SaveLoad();
		tipo_jugador_rn = tp.leerArchivosCsv ();
		ChangeEnemyMovement = true;
	}

	public Vector3 selectedArea(){
		Vector3 respuesta = new Vector3 (1, 1);
		int area = (int)UnityEngine.Random.Range (0.0f, 4.9f);
		Debug.Log ("---el area fue " + area);
		while (areas [area] == true) {
			Debug.Log ("en el while");
			area = (int)UnityEngine.Random.Range (0.0f, 3.9f);
		}
		areas [area] = true;
		int x1 = (int)UnityEngine.Random.Range (0.0f, 7f);;
		int y1 = (int)UnityEngine.Random.Range (0.0f, 7f);;
		int x2 = (int)UnityEngine.Random.Range (8.0f, 15f);
		int y2 = (int)UnityEngine.Random.Range (8.0f, 15f);
		int x3 = (int)UnityEngine.Random.Range (3.0f, 11f);
		int y3 = (int)UnityEngine.Random.Range (3.0f, 11f);

		if (area == 0) {
			respuesta = new Vector3 (x1, y1, 0);
			Debug.Log (" ------------- area 1 " + respuesta);
		} else if (area == 1) {
			respuesta = new Vector3 (x2, y1, 1);
			Debug.Log (" ------------- area 2 " + respuesta);
		} else if (area == 2) {
			respuesta = new Vector3 (x1, y2, 2);
			Debug.Log (" ------------- area 3 " + respuesta);
		} else if (area == 3) {
			respuesta = new Vector3 (x2, y2, 3);
			Debug.Log (" ------------- area 4 " + respuesta);
		}else if (area == 4) {
			respuesta = new Vector3 (x3, y3, 4);
			Debug.Log (" ------------- area 5 " + respuesta);
		}

		return respuesta;
	}

	///////////////// BORRAR LUEGO
	/// 
	public void borrameLuego(int type){
		ChangeEnemyMovement = true;
		tipo_jugador_rn = type;
	}
	/// 
	/// //////////////////////////


}