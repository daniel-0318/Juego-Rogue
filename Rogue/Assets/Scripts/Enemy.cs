using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

	public AudioClip enemyAttack1, enemyAttack2;

	public int playerDamage;
	private Animator animator;
	private Transform target; //el jugador que es el objetivo del enemigo
	private bool skipmove;
	private int healthPoints=10;
	public Vector2 MovEsquive = new Vector2 (0,0);
	private bool aunEsquivando;
	private bool goalOk = false; //ya llego a la posicion establecida
	private Vector2 posicionNodo = new Vector2 (-1, -1);

	public int tipoMovimiento = 0; // para saber que movimiento realiza (aleatorio=0,normal=1, esqui_obstaculos=2, nodo=3 )
	private int tipoMovimientoAntes = 0;
	public List<int> LugarDelGolpe = new List<int>(); //las dos primeras posiciones son del enemigo y las otras dos del jugador
	public int vecesGolepandoJugador = 0;

	private int identifiedPlayer = -1; //sirve para comenzar a contar el tipo que mantiene cerca del objeto
	private int maxTimeCamper = 0;
	private int timeElapsedCamper = 0; 
	private int timeResetCamper = 0;
	private int areaCamper= -1;
	private int seguir = -1; //Sirve para saber a que enemigo debe seguir

	protected override void Awake(){
		animator = GetComponent<Animator> ();
		base.Awake ();  
	}

	protected override void Start(){
		float tipoIA = Random.Range (0.0f, 3.9f);

		if (tipoIA <= 0.9) {
			tipoMovimiento = 0;
			tipoMovimientoAntes = 0;
		}else if(tipoIA > 0.9 && tipoIA <= 1.9){
			tipoMovimiento = 1;
			tipoMovimientoAntes = 1;
		}else if(tipoIA > 1.9 && tipoIA <= 2.9){
			tipoMovimiento = 2;
			tipoMovimientoAntes = 2;
		}else if(tipoIA > 2.9 && tipoIA <= 3.9){
			tipoMovimiento = 3;
			tipoMovimientoAntes = 3;
		}
			
		Debug.Log ("Este enemigo sera: " + tipoMovimiento);

		GameManager.instance.AddEnemyToList (this);//Cada enemigo se añade a la lista por su cuenta
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		base.Start ();
	}

	/**
	 *Proposito hacer que los enemigos se muevan una vez y otra vez no cuando les toque mover llama a la base
	 *para realizar el movimiento
	*/
	protected override bool AttempMove(int xDir, int yDir){
		bool canMove = base.AttempMove (xDir, yDir);
		return canMove;
	}

	public void RealizarMovimiento(){
		if (!skipmove) {
			if (tipoMovimiento == 0) { //movimiento normal del juego
				movimientoNormal ();
			} else if (tipoMovimiento == 1) {//Movimiento aleatorio
				MoveEnemyRandom ();
			} else if (tipoMovimiento == 2) {//movimiento esquivar obstaculos
				MoveEnemy();
			} else if (tipoMovimiento == 3) {//movimiento ir a nodo
				if (goalOk) {
					MoveEnemyRandom ();
				} else {
					MoveEnemyToNode ();
				}
			}
			skipmove = true;
		} else {
			skipmove = false;
		}
	}

	public void movimientoNormal(){
		int xDir=0, yDir=0;
		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon) {
			yDir = target.position.y > transform.position.y ? 1 : -1;
		} else {
			xDir = target.position.x > transform.position.x ? 1 : -1;
		}
		AttempMove (xDir, yDir);

	}

	public Vector2 PlayerNear(){
		int xDir=0, yDir=0;
		if (Mathf.Abs (target.position.x - transform.position.x) <= 1 && Mathf.Abs (target.position.y - transform.position.y) <= 1) {
			if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon) {
				yDir = target.position.y > transform.position.y ? 1 : -1;
			} else {
				xDir = target.position.x > transform.position.x ? 1 : -1;
			}
		}

			return new Vector2 (xDir, yDir);
	}

	/**
	 * Proposito: Mover al enemigo.
	 * Procedimiento: Primero revisa si el jugador esta en la misma columna, si lo esta entonces procede a ver si
	 * se mueve para arriba o para abajo, de lo contrario revisa en que columna esta para ir acercandose.
	*/
	public void MoveEnemy(){
		Vector2 playerIsNear = PlayerNear ();
		if (playerIsNear.x != 0f || playerIsNear.y != 0f) {
			//Debug.Log ("Enemigo, jugador cerca");
			AttempMove ((int)playerIsNear.x, (int)playerIsNear.y);
		} else {
			esquivar ((Vector2) target.position, 0);
		}
	}

	/*Proposito: Mover al enemigo rumbo al nodo que fue activado por el jugador con ayuda de attempMove*/
	public void MoveEnemyToNode(){
		//Debug.Log ("enemigo, movimiento hacia nodo");
		Vector2 playerIsNear = PlayerNear ();
		if (playerIsNear.x != 0f || playerIsNear.y != 0f) {
		//	Debug.Log ("Enemigo, jugador cerca");
			AttempMove ((int)playerIsNear.x, (int)playerIsNear.y);
		}else {//Acercarse al nodo que fue activado
//			Debug.Log("°°°°°°° en el else de node " + (posicionNodo == (Vector2)transform.position) );
//			Debug.Log ("Miremos que dara "+ near_object() + " tipo jugador" + identifiedPlayer);
			//Este primer if es si el enemigo es capaz de estar en la misma posicion del objetivo (nodo o item)
			if (posicionNodo == (Vector2)transform.position  && (identifiedPlayer == -1 || identifiedPlayer == 0)) {
//				Debug.Log ("Enemigo, ya llego a la meta");
				goalOk = true;
				MoveEnemyRandom ();
			// El enemigo esta en una posicion aledaña a su posicion objetivo
			}else if(near_object()  && (identifiedPlayer == 2 || identifiedPlayer == 1)){
//				Debug.Log ("Enemigo, ya llego cerca de otro");
				goalOk = true;
				MoveEnemyRandom ();
			} else {
//				Debug.Log ("Enemigo, esquivar");
				esquivar (posicionNodo, 1);
			}
		}
	}


	/*Proposito: hacer que el enemigo se mueva aleatoriamente siempre y cuando el jugador no este cerca de él.*/
	public void MoveEnemyRandom(){
		//Debug.Log ("enemigo, movimiento aleatorio");
		bool moveOk = false;
		Vector2 mov = generarMovimiento();
		moveOk = AroundObstacle(mov);
		Vector2 playerIsNear = PlayerNear ();
		if (playerIsNear.x != 0 || playerIsNear.y != 0) {
			AttempMove ((int)playerIsNear.x, (int)playerIsNear.y);
		} else {
			while (!moveOk) {
				mov = generarMovimiento ();
				moveOk = AroundObstacle (mov);
			}
			AttempMove ((int)mov.x, (int)mov.y);
		}
	}

	/*Metodo auxiliar para moveEnemyRandom para decirdir un movieminto */
	public Vector2 generarMovimiento(){
		int xDir=0, yDir=0, num=0;
		float desicionXOrY, desicionLR;
		desicionXOrY = Random.Range (0.0f, 1.0f);
		desicionLR = Random.Range (0.0f, 1.0f);

		if (desicionLR > 0.5) {
			num = 1;
		} else {
			num = -1;
		}
		if (desicionXOrY > 0.5) {
			yDir = num;
		} else {
			xDir = num;
		}
		return new Vector2 (xDir, yDir);
	}


	/**Metodo usado para esquivar cuando se usa esquivar obstaculos o si es el jugador atacarlo*/
	protected override void OnCantMove(GameObject go){
		Player hitPlayer = go.GetComponent<Player> ();
		if (hitPlayer != null) {
			//Debug.Log ("Encontro al jugador");
			vecesGolepandoJugador += 1; //se aumenta 1 la cantida de daño que le hizo al jugador (para las metricas)
//			LugarDelGolpe.Add ( (int) transform.position.x);
//			LugarDelGolpe.Add ((int)transform.position.y);
//			LugarDelGolpe.Add ((int)target.position.x);
//			LugarDelGolpe.Add ((int)target.position.y);
//			int disMinItem = GameManager.instance.itemMasCercanoAlJugador ((int) target.position.x,(int) target.position.y);
//			LugarDelGolpe.Add (disMinItem);
			hitPlayer.LoseHealth (playerDamage);
			animator.SetTrigger ("enemyAttack");
			SoundManager.instance.RandomizeSfx (enemyAttack1, enemyAttack2);
		} else {
			//Debug.Log ("No pudo mover.");
			Wall pared = go.GetComponent<Wall> ();
			if (pared != null) {
				//Debug.Log ("Pared encontrada en: " + new Vector2 (pared.transform.position.x, pared.transform.position.y));
				esquivar((Vector2) target.position, 0);
			}
		}
	}

	/**Metodo que sirve para disminuir los puntos de vida de los enemigos y de llegar esta a cero, destruirlo	*/
	public bool LoseHealth(int damage){
		healthPoints -= damage;
		//Debug.Log (healthPoints <= 0);
		if (healthPoints <= 0) {
			gameObject.SetActive(false);
			return true;
		}
		return false;
	}


	/**Version mejorada de PreparseParaEsquivar
	 *Ultima modif: mayo 28 2018 
	 *Parametros: Vector2 -> posicion del objetivo, isNode -> si su objetivo es ir a un nodo entonces 1, si es el jugador entonces 0
	*/
	public void esquivar(Vector2 objetivo, int isnode){
		//Debug.Log ("Esquivando");
		float posX = transform.position.x, posY = transform.position.y;
		float targetX = objetivo.x, targetY = objetivo.y;
		//Debug.Log ("posiciones, enemigo: " + posX +" " + posY +" Tarjet: " + targetX + " " + targetY );
		bool obsR = AroundObstacle (new Vector2 (1, 0));
		bool obsU = AroundObstacle (new Vector2 (0, 1));
		bool obsL = AroundObstacle (new Vector2 (-1, 0));
		bool obsD = AroundObstacle (new Vector2 (0, -1));


		if (posX <= targetX) {
			//Debug.Log ("1");
			if (obsR && !MovEsquive.Equals(new Vector2 (1, 0))) {
				//Debug.Log ("1.1");
				AttempMove (1, 0);
				MovEsquive.Set (-1, 0);
			} else if (obsU && !MovEsquive.Equals(new Vector2 (0, 1))) {
				//Debug.Log ("1.2");
				if (AroundObstacle (new Vector2 (0, -1)) && targetY < posY) {
				//	Debug.Log ("1.2.1");
					AttempMove (0, -1);
					MovEsquive.Set (0, 1);
				} else {
					//Debug.Log ("1.2.2");
					AttempMove (0, 1);
					MovEsquive.Set (0, -1);
				}
			} else if (obsL && !MovEsquive.Equals(new Vector2 (-1, 0))) {
				//Debug.Log ("1.3");
				AttempMove (-1, 0);
				MovEsquive.Set (1, 0);
			} else if (obsD && !MovEsquive.Equals(new Vector2 (0, -1))) {
				//Debug.Log ("1.4");
				AttempMove (0, -1);
				MovEsquive.Set (0, 1);
			} else {
				//Debug.Log ("Turno perdido del enemigo en: " + posX + " " + posY);
				if(obsD || obsL || obsR || obsU){
					if (obsD) {
						AttempMove (0, -1);
						MovEsquive.Set (0, 1);
					} else if (obsL) {
						AttempMove (-1, 0);
						MovEsquive.Set (1, 0);
					} else if (obsU) {
						AttempMove (0, 1);
						MovEsquive.Set (0, -1);
					} else if (obsR) {
						AttempMove (1, 0);
						MovEsquive.Set (-1, 0);
					}
				}
			}
				
		} else if (posX > targetX) {
			//Debug.Log ("2");
			if (obsL && !MovEsquive.Equals(new Vector2 (-1, 0))) {
				//Debug.Log ("2.1");
				AttempMove (-1, 0);
				MovEsquive.Set (1, 0);
			} else if (obsU && !MovEsquive.Equals(new Vector2 (0, 1))) {
			//	Debug.Log ("2.2");
				if (AroundObstacle (new Vector2 (0, -1)) && targetY < posY) {
					//Debug.Log ("2.2.1");
					AttempMove (0, -1);
					MovEsquive.Set (0, 1);
				} else {
					//Debug.Log ("2.2.2");
					AttempMove (0, 1);
					MovEsquive.Set (0, -1);
				}
			} else if (obsR && !MovEsquive.Equals(new Vector2 (1, 0))) {
				//Debug.Log ("2.3");
				AttempMove (1, 0);
				MovEsquive.Set (-1, 0);
			} else if (obsD && !MovEsquive.Equals(new Vector2 (0, -1))) {
				//Debug.Log ("2.4");
				AttempMove (0, -1);
				MovEsquive.Set (0, 1);
			} else {
				//Debug.Log ("Turno perdido del enemigo en: " + posX + " " + posY);
				if(obsD || obsL || obsR || obsU){
					if (obsD) {
						AttempMove (0, -1);
						MovEsquive.Set (0, 1);
					} else if (obsL) {
						AttempMove (-1, 0);
						MovEsquive.Set (1, 0);
					} else if (obsU) {
						AttempMove (0, 1);
						MovEsquive.Set (0, -1);
					} else if (obsR) {
						AttempMove (1, 0);
						MovEsquive.Set (-1, 0);
					}
				}
			}
		}
		
	}

	public bool getGoalOk(){
		return goalOk;
	}

	public void setGoalOk(bool valor){
		goalOk = valor;
	}

	public void SetPosicionNodo(int posX, int posY){
		posicionNodo = new Vector2 (posX, posY);
	}

	public List<int> getTargetPosition(){
		List<int> posicion = new List<int> ();
		posicion.Add ((int)target.position.x);
		posicion.Add ((int)target.position.y);
		return posicion;
	}

	/*
	 * Funcion: calcular si el enemigo esta en una posicion aledaña a la posicion objetivo (nodo)
	 * Salida: Devuelve "true" si la posicion del enemigo es aledaña a la posicion objeto, caso contrario devuelve "false"
	*/
	public bool near_object(){
		bool respuesta = false;

		int x = (int) Mathf.Abs (transform.position.x - posicionNodo.x);
		int y = (int) Mathf.Abs (transform.position.y - posicionNodo.y);

		if (x <= 1 && y <= 1) {
			respuesta = true;
		}

		return respuesta;
	}

	public void setTypeOfMoviment(int type){
		tipoMovimiento = type;
	}

	public void set_identifiedPlayer(int type){
		identifiedPlayer = type;
	}

	public void set_timeElapsedCamper(int quantity){
		timeElapsedCamper += quantity;
	}

	public int get_timeElapsedCamper(){
		return timeElapsedCamper;
	}

	public void restoreMove(){
		if (tipoMovimientoAntes == 0) {
			tipoMovimiento = 0;
		} else if (tipoMovimientoAntes == 1) {
			tipoMovimiento = 1;
		} else if (tipoMovimientoAntes == 2) {
			tipoMovimiento = 2;
		} else {
			tipoMovimiento = 3;
		}
		identifiedPlayer = -1;
			
	}

	public void set_maxTimeCamper(){
		maxTimeCamper = (int)Random.Range (1.0f, 7.0f);
		timeResetCamper = maxTimeCamper * 2;
	}

	public int get_maxTimeCamper(){
		return maxTimeCamper;
	}

	public bool get_goalOK(){
		return goalOk;
	}

	public void set_goalOk(bool valor){
		goalOk = valor;
	}

	public bool get_skipMove(){
		return skipmove;
	}

	public int get_timeResetCamper(){
		return timeResetCamper;
	}

	public void reset_timesCampers(){
		timeElapsedCamper = 0;
		maxTimeCamper = 0;
		timeResetCamper = 0;
		areaCamper = -1;
		seguir = -1;
	}

	public void set_areaCamper(int value){
		areaCamper = value;
	}

	public int get_areaCamper(){
		return areaCamper;
	}

	public int get_seguir(){
		return seguir;
	}

	public void set_seguir(int valor){
		seguir = valor;
	}
}
