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

	private List<Enemy> enemies = new List<Enemy>(); //lista de enemigos para controlar los moviendo de ellos
	private bool enemiesMoving; //Por defecto se inicializa en falso
	private int enemiesAllInNode = 0;
	public List<List<int>> listadoEnemigos = new List<List<int>>();
	public List<int> enemigosNivelActual = new List<int> ();

	private int level = 0;
	private GameObject levelImage;
	private Text levelText;

	private bool moveToNode = false;
	private Vector2 coordeNode;


	private void Awake(){
		rutaGuardarCargar =  Application.persistentDataPath + "/datos.dat";
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
		Invoke ("HideLevelImage",levelStartDelay); //invoke sirve para ejecutar un metodo luego de cierto tiempo (levelstardelay)
	
	}

	private void HideLevelImage(){
		levelImage.SetActive(false);
		doingSetup = false;
	}

	public void GameOver(){
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
		Debug.Log ("Cantidad de enemigos " + enemies.Count); 
		for(int i=0;i<enemies.Count;i++){
			Debug.Log ("Enemigo, Valor de los nodos esta en: " + moveToNode);
			//Si el jugador toca un nodo los enemigos iran al nodo mientras no esten cerca del jugador
			Debug.Log("Estado del enemigo: " + enemies[i].gameObject.activeSelf);
			if (enemies [i].gameObject.activeSelf == true) {
				if (moveToNode) {
					Debug.Log ("Se ira al nodo " + coordeNode);
					if (enemies [i].getGoalOk ()) { //Si ya llego al nodo siga sus movimientos aleatorios
						enemiesAllInNode++;
						enemies [i].moveEnemyRandom ();
					} else {
						enemies [i].moveEnemyToNode (coordeNode);//De lo contrario siga acercandose al nodo
					}
				} else {
					enemies [i].moveEnemyRandom ();
				}
				if (enemiesAllInNode >= enemies.Count) {
					Debug.Log ("Enemigo, todos los enemigos llegaron, se resetear sus nodos");
					moveToNode = false;
					resetNodeEnemies ();
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

	/**Proposito: cuando un enemigo sea eliminado desde este metodo se buscara su posicion para eliminarlo*/
	public void DeleteEnemyToList(Transform enemyFound){
		for (int i = 0; i < enemies.Count; i++) {
			if(enemies[i].transform.position.Equals(enemyFound.position)){
				enemigosNivelActual.Add (enemies [i].vecesGolepandoJugador);
				enemigosNivelActual.Add (enemies [i].tipoMovimiento);
				enemigosNivelActual.Add ((int)enemies [i].transform.position.x);
				enemigosNivelActual.Add ((int)enemies [i].transform.position.y);
				enemies.RemoveAt (i);
			}
		}
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
		
	public void ActiveNode(bool valor){
		moveToNode = valor;
		Debug.Log ("Cambio de valor a nodo " + moveToNode);
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
		enemiesAllInNode = 0;
	}

	public void guardar(){
		Debug.Log ("Guardando");
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (rutaGuardarCargar);
		for (int i = 0;i<enemies.Count;i++) {
			enemigosNivelActual.Add (enemies [i].vecesGolepandoJugador);
			enemigosNivelActual.Add (enemies [i].tipoMovimiento);
			enemigosNivelActual.Add ((int)enemies [i].transform.position.x);
			enemigosNivelActual.Add ((int)enemies [i].transform.position.y);
		}
		listadoEnemigos.Add (enemigosNivelActual);
		enemigosNivelActual.Clear ();//se limpia para que el sgte nivel no guarde más
		//datos a guardar
		DatosSaveLoad datos = new DatosSaveLoad();
		datos.numeroPasosJugador = numeroPasosJugador;
		datos.listadoEnemigos = listadoEnemigos; //cada posicion corresponde a un nivel.
		bf.Serialize (file, datos);

		file.Close ();
		
	}

	public void cargar(){
		if (File.Exists(rutaGuardarCargar)) {
			Debug.Log ("cargando");
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (rutaGuardarCargar, FileMode.Open);

			DatosSaveLoad datos = (DatosSaveLoad)bf.Deserialize (file);
			Debug.Log (datos.listadoEnemigos.Count);
			Debug.Log ("pasos jugador, tamaño lista: " + datos.numeroPasosJugador.Count); 

			file.Close ();
		}
	}

		
}

[Serializable]
public class DatosSaveLoad{
	//variables
	public List<int> numeroPasosJugador= new List<int>();
	public List<int> vidajugador = new List<int>();
	public bool jugadorMuerto = false;
	public List<List<Vector2>> posJugadorMuerto = new List<List<Vector2>> ();
	public List<List<int>> listadoEnemigos = new List<List<int>>(); //cada 3 numeros son tiposIA, PosX y posY de cada enemigo en cada nivel (de la lista interna)

		

}
