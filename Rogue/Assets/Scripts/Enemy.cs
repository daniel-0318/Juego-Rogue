using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

	public AudioClip enemyAttack1, enemyAttack2;

	public int playerDamage;

	private Animator animator;
	private Transform target; //el jugador que es el objetivo del enemigo
	//private bool skipmove;
	private int healthPoints=10;
	public Vector2 MovEsquive = new Vector2 (0,0);
	private int a=0;
	private int b=0;
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
//		if (skipmove) {
//			skipmove = false;
//			return false;
//		}
		bool canMove = base.AttempMove (xDir, yDir);
//		skipmove = true;
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
		Debug.Log ("Entro a MoveEnemy, movEsquive y la compacion es: " +MovEsquive +  " "+ !(MovEsquive.Equals(new Vector2(xDir,yDir))) );
		if (!(MovEsquive.Equals (new Vector2 (xDir, yDir)))) {//si no acaba de esquivar y para donde quiere moverse es donde estaba atorado
			Debug.Log ("Entro a moverse sin problema por ahora");
			bool resp = AttempMove (xDir, yDir);
			if (resp) {
				MovEsquive.Set (0,0);
			}
		} else {
			Debug.Log ("Va a esquivar porque ya estuvo ahi : " + xDir + " " + yDir);
			esquivar (new Vector2(0,0));
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
				esquivar (new Vector2 (pared.transform.position.x, pared.transform.position.y));
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


	/**Version mejorada de PreparseParaEsquivar*/
	public void esquivar(Vector2 obstaclePos){
		Debug.Log ("Esquivando");
		float posX = transform.position.x, posY = transform.position.y;
		float posObsX = obstaclePos.x, posObsY = obstaclePos.y;
		float targetX = target.position.x, targetY = target.position.y;
		bool obsR = AroundObstacle (new Vector2 (1, 0));
		bool obsU = AroundObstacle (new Vector2 (0, 1));
		bool obsL = AroundObstacle (new Vector2 (-1, 0));
		bool obsD = AroundObstacle (new Vector2 (0, -1));
		if (posX <= targetX || posY == targetY) {
			Debug.Log ("1");
			if (obsR && !MovEsquive.Equals(new Vector2 (1, 0))) {
				Debug.Log ("1.1");
				AttempMove (1, 0);
				MovEsquive.Set (-1, 0);
			} else if (obsU && !MovEsquive.Equals(new Vector2 (0, 1))) {
				Debug.Log ("1.2");
				AttempMove (0, 1);
				MovEsquive.Set (0, -1);
			} else if (obsL && !MovEsquive.Equals(new Vector2 (-1, 0))) {
				Debug.Log ("1.3");
				AttempMove (-1, 0);
				MovEsquive.Set (1, 0);
			} else if (obsD && !MovEsquive.Equals(new Vector2 (0, -1))) {
				Debug.Log ("1.4");
				AttempMove (0, -1);
				MovEsquive.Set (0, 1);
			} else {
				Debug.Log ("Turno perdido del enemigo en: " + posX + " " + posY);
			}
				
		} else if (posX > targetX) {
			Debug.Log ("2");
			if (obsL && !MovEsquive.Equals(new Vector2 (-1, 0))) {
				Debug.Log ("2.1");
				AttempMove (-1, 0);
				MovEsquive.Set (1, 0);
			} else if (obsU && !MovEsquive.Equals(new Vector2 (0, 1))) {
				Debug.Log ("2.2");
				AttempMove (0, 1);
				MovEsquive.Set (0, -1);
			} else if (obsR && !MovEsquive.Equals(new Vector2 (1, 0))) {
				Debug.Log ("2.3");
				AttempMove (1, 0);
				MovEsquive.Set (-1, 0);
			} else if (obsD && !MovEsquive.Equals(new Vector2 (0, -1))) {
				Debug.Log ("2.4");
				AttempMove (0, -1);
				MovEsquive.Set (0, 1);
			} else {
				Debug.Log ("Turno perdido del enemigo en: " + posX + " " + posY);
			}
		}
		
	}
		
		
}
