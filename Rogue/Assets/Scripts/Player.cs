﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

	public AudioClip moveSound1, moveSound2, eatSound1, eatSound2, drinkSound1, drinkSound2, gameOverSound;

	public int wallDamage = 1;
	public int enemyDamage = 2;
	public int pointPerfood = 10;
	public int pointPerSoda = 20;
	public int pointPerAmmo = 2;
	public int pointPerCoin = 2;
	public int pointPerKillEnemy = 1;
	public float restartLEvelDelay = 1f;
	public Text healthText;
	public Text ammoText;
	public Text scoreText;
	public Text killsEnemiesText;
	bool disparo = false;

	private Animator animator;
	private int health;
	private int ammo;
	private int score;
	private int killsEnemies;

	public int cantidadPasos = 0;

	public GameObject bulletObject;


	protected override void Awake(){
		animator = GetComponent<Animator> ();
		base.Awake ();
	}

	protected override void Start(){
		
		health = GameManager.instance.playerHealthtPoints;
		ammo = GameManager.instance.playerammoPoints;
		score = GameManager.instance.playerScorePoints;
		killsEnemies = GameManager.instance.playerKillsPoints;
		healthText.text = "Health Point: " + health;
		ammoText.text = "Ammo: " + ammo;
		scoreText.text = "Score : " + score;
		killsEnemiesText.text = "Kills: " + killsEnemies;
		base.Start ();
	}

	/*OnDisable se ejecuta antes de ejecutar el OnDestroy, que se ejecutara
	 * cuando el objeto Player sea destruido para recargar la escena con una nueva
	 * Esto hara que se guarde en la clase GameManager la comida que se lleva

	private void OnDisable(){
		GameManager.instance.PlayerHealthtPoints = health;
		GameManager.instance.PlayerammoPoints = ammo;
	}
	*/

	void CheckIfGameOver(){
		if (health <= 0) {
			SoundManager.instance.musicSource.Stop ();
			SoundManager.instance.PlaySingle (gameOverSound);
			GameManager.instance.GameOver ();
		}
	}


	protected override bool AttempMove(int xDir, int yDir){
		bool canMove = base.AttempMove(xDir, yDir);
		CheckIfGameOver ();
		GameManager.instance.PlayerTurn = false;
		return canMove;

	}
	
	// Update is called once per frame
	void Update () {
		//Si no es turno del jugador
		if(!GameManager.instance.PlayerTurn || GameManager.instance.doingSetup || disparo){
			return;
		}
		if (GameManager.instance.enemigoMuerto) {
			Debug.Log ("Tentativamente funciono");
			killsEnemies += 1;
			killsEnemiesText.text = "kills: " + killsEnemies;
			GameManager.instance.enemigoMuerto = false;
		}

		int horizontal=0;
		int vertical=0;

		//////////////////////////   ZONA PRUEBA
		if(Input.GetKeyDown(KeyCode.M)){
			Debug.Log ("Entro en la letra M a mapiar");
			GoalLook ();

		}
		if(Input.GetKeyDown(KeyCode.R)){
			RedNeuronal rn = new RedNeuronal();
			double[,] matriz1 = new double [8, 3]{ {-1,-1,-1}, {-1,-1,1}, {-1,1,-1}, {-1,1,1}, {1,-1,-1}, {1,-1,1}, {1,1,-1}, {1,1,1} };
			double[,] matriz2 = new double [1, 8]{{0,1,1,0,1,0,0,1}};

			rn.invocar_algoritmo_entrenamiento (matriz1, matriz2);

		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			vertical = 1;
		}else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			vertical = -1;
		}else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			horizontal = -1;
		}else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			horizontal = 1;
		}

		if ((horizontal == 0 && vertical == 0) &&( Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.S) || 
			Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.D)) && (ammo > 0) ) {
			Debug.Log ("Entro a disparar");
			//accedemos al Script del objeto bala
			Bullet Scriptbullet = bulletObject.GetComponent<Bullet>();
			/*por ahora repito el cambiar de turno en cada if para evitar que los enemigos se muevan antes de que el jugador termine de moverse*/
			if (Input.GetKeyDown (KeyCode.W)) {//Ataque hacia arriba
				Debug.Log ("W");
				Scriptbullet.direccionArma = Direccion.Vertical;
				Scriptbullet.velocidad = 50;
				disparo = true;
				//GameManager.instance.PlayerTurn = false;
			}else if (Input.GetKeyDown (KeyCode.S)) {
				Debug.Log ("S");
				Scriptbullet.direccionArma = Direccion.Vertical;
				Scriptbullet.velocidad = -50;
				disparo = true;
				//GameManager.instance.PlayerTurn = false;
			}else if (Input.GetKeyDown (KeyCode.A)) {
				Debug.Log ("A");
				Scriptbullet.direccionArma = Direccion.Horizontal;
				Scriptbullet.velocidad = -50;
				disparo = true;
				//GameManager.instance.PlayerTurn = false;
			}else if (Input.GetKeyDown (KeyCode.D)) {
				Debug.Log ("D");
				Scriptbullet.direccionArma = Direccion.Horizontal;
				Scriptbullet.velocidad = 50;
				disparo = true;
				//GameManager.instance.PlayerTurn = false;
			}
			ammo--;
			ammoText.text = " Ammo: " + ammo;
			//Se crea la bala en la escena, en la posicion del jugador
			Instantiate (bulletObject, transform.position, Quaternion.identity);

			disparo = false;
		}
			
		if(horizontal !=0){
			vertical = 0;
		}
		if ((horizontal != 0 || vertical != 0) && !disparo) {
			bool canMove = AttempMove (horizontal, vertical);
			cantidadPasos += 1;
			if (canMove) {
				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
			}
			Debug.Log ("Cambio aqui");
			GameManager.instance.PlayerTurn = false;
		}
		//disparo = false;
	}

	/*Se encarga de ver si el objeto que no a dejado mover el jugador es un muro
	 * y si es un muro interno entonces le restara vida al muro si puede dañarlo
	 * entonces se ejecuta la animacion del jugador
	*/
	protected override void OnCantMove(GameObject go){
		Wall hitWall = go.GetComponent<Wall> ();
		if(hitWall != null){
			hitWall.DamageWall (wallDamage);
			animator.SetTrigger ("playerChop");
		}
		Enemy hitEnemy = go.GetComponent<Enemy> ();
		if (hitEnemy != null) {
			bool respuesta = hitEnemy.LoseHealth (enemyDamage);
			animator.SetTrigger ("playerChop");
			if (respuesta) {
				killsEnemies += pointPerKillEnemy;
				killsEnemiesText.text = "kills: " + killsEnemies;
			}
		}

	}

	void Restart (){
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public void LoseHealth(int loss){
		health -= loss;
		healthText.text = "-" + loss +" Health Point: " + health;
		animator.SetTrigger ("playerHit");
		CheckIfGameOver ();
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Exit")) {
			GameManager.instance.numeroPasosJugador.Add (cantidadPasos);
			GameManager.instance.playerHealthtPoints = health;
			GameManager.instance.playerammoPoints = ammo;
			GameManager.instance.playerScorePoints = score;
			GameManager.instance.playerKillsPoints = killsEnemies;
			cantidadPasos = 0;
			GameManager.instance.guardar (0);
			Invoke ("Restart", restartLEvelDelay);
			enabled = false; // para que no se pueda seguir moviendo el jugador
			
		} else if (other.CompareTag ("Food")) {

			health += pointPerfood;
			SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
			healthText.text = " Health Points: " + health;
			other.gameObject.SetActive (false);
			
		} else if (other.CompareTag ("Soda")) {

			health += pointPerSoda;
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
			healthText.text = " Health Points: " + health;
			other.gameObject.SetActive (false);
		} else if (other.CompareTag ("Ammo")) {

			ammo += pointPerAmmo;
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
			ammoText.text = " Ammo: " + ammo;
			other.gameObject.SetActive (false);

		} else if (other.CompareTag ("Coin")) {
			Debug.Log ("Coin");
			score += pointPerCoin;
			scoreText.text = "Score: " + score;
			other.gameObject.SetActive (false);
			
		}else if (other.CompareTag ("Node")) {
			Debug.Log ("Funciono =) (/&%$#");
			GameManager.instance.setCoordeNode ((Vector2)other.gameObject.transform.position);
		}
	}

}
