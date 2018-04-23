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

	protected override void Awake(){
		animator = GetComponent<Animator> ();
		base.Awake ();
	}

	protected override void Start(){
		GameManager.instance.AddEnemyToList (this);//Cada enemigo se añade a la lista por su cuenta
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		base.Start ();
	}

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
			xDir = target.position.x > transform.position.y ? 1 : -1;
		}
		AttempMove (xDir, yDir);
	}

	protected override void OnCantMove(GameObject go){
		Player hitPlayer = go.GetComponent<Player> ();
		if(hitPlayer != null){
			hitPlayer.LoseHealth (playerDamage);
			animator.SetTrigger ("enemyAttack");
			SoundManager.instance.RandomizeSfx (enemyAttack1,enemyAttack2);
		}


	}

	public void LoseHealth(int damage){
		healthPoints -= damage;
		if (healthPoints <= 0)
			Destroy (gameObject);
	}
		
}
