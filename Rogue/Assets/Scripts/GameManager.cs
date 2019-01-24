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

	GameObject[] sodas,foods,ammos; //Toca tener sus instancias ya que una vez desactivadas no aparecen con findObjectswhithtag

	private int level = 0;
	private GameObject levelImage;
	private Text levelText;
	private Text dayText;

	private bool moveToNode = false;
	private Vector2 coordeNode;

	public bool yaEjecuto = false;
	private int nivel_a_ejecutar_red_reuronal = 2;
	private int nivel_a_detectar_tipo_jugador = 7;


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

				}else if(ChangeEnemyMovement && tipo_jugador_rn == 1){
					Debug.Log ("##### ¡¡¡ Jugador tipo explorador !!!!!!!!");
					Vector2 respuesta = itemMasCercanoAlJugador ((int)enemies [i].transform.position.x, (int)enemies [i].transform.position.y);
					Debug.Log ("Posicion del objeto" + respuesta);

					if (enemies[i].get_timeElapsedCamper() < 4) {
						Debug.Log ("++++++++ cambio de movimiento a 3");
						enemies [i].setTypeOfMoviment (3);
					}
					enemies [i].set_timeElapsedCamper (1);
					enemies [i].SetPosicionNodo ((int)respuesta.x, (int)respuesta.y);
					enemies [i].RealizarMovimiento ();

					if (enemies [i].get_timeElapsedCamper() == 6) {
						Debug.Log ("----- restauracion de movimiento");
						enemies [i].restoreMove();
					}
					if (enemies [i].get_timeElapsedCamper() >= 12) {
						Debug.Log (" ******* cambio de movimiento a 3");
						enemies [i].setTypeOfMoviment (3);
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
		Debug.Log ("######### Distancia minima encontrada: " + distanciaMinima);
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




}