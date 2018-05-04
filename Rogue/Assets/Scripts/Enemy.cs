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
	private Vector2 MovEsquive;
	private bool aunEsquivando;

	protected override void Awake(){
		animator = GetComponent<Animator> ();
		base.Awake ();
	}

	protected override void Start(){
		GameManager.instance.AddEnemyToList (this);//Cada enemigo se añade a la lista por su cuenta
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		base.Start ();
	}

	/**
	 *Proposito hacer que los enemigos se muevan una vez y otra vez no cuando les toque mover llama a la base
	 *para realizar el movimiento
	*/
	protected override bool AttempMove(int xDir, int yDir){
		if (skipmove) {
			skipmove = false;
			return false;
		}
		bool canMove = base.AttempMove (xDir, yDir);
		skipmove = true;
		return canMove;
	}

	/**
	 * Proposito: Mover al enemigo.
	 * Procedimiento: Primero revisa si el jugador esta en la misma columna, si lo esta entonces procede a ver si
	 * se mueve para arriba o para abajo, de lo contrario revisa en que columna esta para ir acercandose.
	*/
	public void MoveEnemy(){
		int xDir=0, yDir=0;
		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon) {
			yDir = target.position.y > transform.position.y ? 1 : -1;
		} else {
			xDir = target.position.x > transform.position.x ? 1 : -1;
		}
		if ( !(MovEsquive.Equals(new Vector2(xDir,yDir)))) {//si no acaba de esquivar y para donde quiere moverse es donde estaba atorado
			Debug.Log("Entro a moverse sin problema por ahora");
			AttempMove (xDir, yDir);
		} else {
			Debug.Log ("la posicion que queria era donde acaba de salir");
			PreparseParaEsquivar (new Vector2(xDir, yDir));
		}
	}

	protected override void OnCantMove(GameObject go){
		Player hitPlayer = go.GetComponent<Player> ();
		if (hitPlayer != null) {
			hitPlayer.LoseHealth (playerDamage);
			animator.SetTrigger ("enemyAttack");
			SoundManager.instance.RandomizeSfx (enemyAttack1, enemyAttack2);
		} else {
			Debug.Log ("No pudo mover.");
			Wall pared = go.GetComponent<Wall> ();
			if (pared != null) {
				Debug.Log ("Pared encontrada en: " + new Vector2 (pared.transform.position.x, pared.transform.position.y));
				PreparseParaEsquivar (new Vector2 (pared.transform.position.x, pared.transform.position.y));
			}
		}


	}

	/**Metodo que sirve para disminuir los puntos de vida de los enemigos y de llegar esta a cero, destruirlo	*/
	public void LoseHealth(int damage){
		healthPoints -= damage;
		Debug.Log (healthPoints <= 0);
		if (healthPoints <= 0) {
			GameManager.instance.DeleteEnemyToList (gameObject.transform);
			Destroy (gameObject);
		}
	}

	/**Metodo el cual hace que el enemigo esquive un obstaculo
	 * si el obstaculo esta en horizontal el buscara moverse verticialmente y viceversa
	*/
	public void PreparseParaEsquivar (Vector2 obstaclePos){
		
		float posX = transform.position.x, posY = transform.position.y;
		float posObsX = obstaclePos.x, posObsY = obstaclePos.y;
		Debug.Log ("Obstaculo en: " + obstaclePos);
		Debug.Log ("Posicion del enemigo: " + transform.position);
		Debug.Log ("posicion restringida? " + MovEsquive);

		if (posY != posObsY) {
			Debug.Log ("Obstaculo en Y");		
			bool right = AroundObstacle (new Vector2 (1, 0));
			Debug.Log ("right y 1 0 "+ right + " " + !MovEsquive.Equals (new Vector2 (posX + 1, posY)));
			Debug.Log ("left y -1 0: "+ AroundObstacle (new Vector2 (-1, 0)) + " " + !MovEsquive.Equals (new Vector2 (posX - 1, posY)));
			if (right && !MovEsquive.Equals (new Vector2 (posX + 1, posY))) { // la comparacion de vectores es por si acababa de salir de ahi antes
				Debug.Log ("Der libre");
				AttempMove (1, 0);
				MovEsquive = new Vector2 (-1, 0);
			} else if (AroundObstacle (new Vector2 (-1, 0)) && !MovEsquive.Equals (new Vector2 (posX - 1, posY))) {
				Debug.Log ("Izq libre");
				AttempMove (-1, 0);
				MovEsquive = new Vector2 (1, 0);
			} else {
				Debug.Log ("devolviendose por sanja");

				if (posY > posObsY) {
					AttempMove (0, 1);
					MovEsquive = new Vector2 (0, 1);
				} else {
					AttempMove (0, -1);
					MovEsquive = new Vector2 (0, -1);
				}
				Debug.Log ("Posicion que no deberia ir esta en: " + MovEsquive);
			}
		} else if (posX != posObsX) {
			Debug.Log ("Obstaculo en X");
			bool up = AroundObstacle (new Vector2 (0, 1));
			Debug.Log ("up y 0 1: "+ up + " " + !MovEsquive.Equals (new Vector2 (posX, posY + 1)));
			Debug.Log ("down y 0 -1: "+ AroundObstacle (new Vector2 (0, -1)) + " " + !MovEsquive.Equals (new Vector2 (posX, posY - 1)));

			if (up && !MovEsquive.Equals (new Vector2 (posX, posY + 1))) {
				Debug.Log ("Arriba libre");
				AttempMove (0, 1);
				MovEsquive = new Vector2 (0, -1);
			} else if (AroundObstacle (new Vector2 (0, -1)) && !MovEsquive.Equals (new Vector2 (posX, posY - 1))) {
				Debug.Log ("abajo libre");
				AttempMove (0, -1);
				MovEsquive = new Vector2 (0, 1);
			} else {
				Debug.Log ("devolviendose por sanja");

				if (posX > posObsX) {
					AttempMove (1, 0);
					MovEsquive = new Vector2 (1, 0);
				} else {
					AttempMove (-1, 0);
					MovEsquive = new Vector2 (-1, 0);
				}
				Debug.Log ("Posicion que no deberia ir esta en: " + MovEsquive);
			}
		}
		Debug.Log ("Aun esquivando esta en : " + aunEsquivando);
		aunEsquivando = aunEsquivando == false ? true : false; // para que esquive solo dos veces
		Debug.Log ("y paso a ser: " + aunEsquivando);
		if (!aunEsquivando)
			MovEsquive = new Vector2 (0, 0);
		Debug.Log ("MovEsquivado al final es : " + MovEsquive);

	}
		
}
