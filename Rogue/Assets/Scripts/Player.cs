using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

	public AudioClip moveSound1, moveSound2, eatSound1, eatSound2, drinkSound1, drinkSound2, gameOverSound;

	public int wallDamage = 1;
	public int pointPerfood = 10;
	public int pointPerSoda = 20;
	public int pointPerAmmo = 10;
	public float restartLEvelDelay = 1f;
	public Text foodText;

	private Animator animator;
	private int health;


	protected override void Awake(){
		animator = GetComponent<Animator> ();
		base.Awake ();
	}

	protected override void Start(){
		
		health = GameManager.instance.PlayerHealthtPoints;
		foodText.text = "Health Point: " + health;
		base.Start ();
	}

	/*OnDisable se ejecuta antes de ejecutar el OnDestroy, que se ejecutara
	 * cuando el objeto Player sea destruido para recargar la escena con una nueva
	 * Esto hara que se guarde en la clase GameManager la comida que se lleva
	*/
	private void OnDisable(){
		GameManager.instance.PlayerHealthtPoints = health;
	}

	void CheckIfGameOver(){
		if (health < 0) {
			SoundManager.instance.musicSource.Stop ();
			SoundManager.instance.PlaySingle (gameOverSound);
			GameManager.instance.GameOver ();
		}
	}

	/**
	 * Metodo sobreescrito del script movingObject
	*/
	protected override bool AttempMove(int xDir, int yDir){
		health--;
		foodText.text = "Health Points: " + health;
		bool canMove = base.AttempMove(xDir, yDir);
		CheckIfGameOver ();
		GameManager.instance.PlayerTurn = false;
		return canMove;

	}
	
	// Update is called once per frame
	void Update () {
		//Si no es turno del jugador
		if(!GameManager.instance.PlayerTurn || GameManager.instance.doingSetup){
			return;
		}

		int horizontal;
		int vertical;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");
		if(horizontal !=0){
			vertical = 0;
		}
		if(horizontal !=0 || vertical !=0){
			bool canMove = AttempMove (horizontal, vertical);
			if (canMove) {
				SoundManager.instance.RandomizeSfx (moveSound1,moveSound2);
			}
		}
	}

	/*Se encarga de ver si el objeto que no a dejado mover el jugador es un muro
	 * y si es un muro interno entonces le restara vida al muro y si puede dañarlo
	 * entonces se ejecuta la animacion del jugador
	*/
	protected override void OnCantMove(GameObject go){
		Wall hitWall = go.GetComponent<Wall> ();
		if(hitWall != null){
			hitWall.DamageWall (wallDamage);
			animator.SetTrigger ("playerChop");
		}

	}

	void Restart (){
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public void LoseFood(int loss){
		health -= loss;
		foodText.text = "-" + loss +" Health Point: " + health;
		animator.SetTrigger ("playerHit");
		CheckIfGameOver ();
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Exit")) {
			Invoke ("Restart", restartLEvelDelay);
			Rigidbody2D rb2D =  GetComponent<Rigidbody2D> ();
			enabled = false; // para que no se pueda seguir moviendo el jugador
			
		} else if (other.CompareTag ("Food")) {

			health += pointPerfood;
			SoundManager.instance.RandomizeSfx (eatSound1,eatSound2);
			foodText.text = "+" + pointPerfood +" Health Points: " + health;
			other.gameObject.SetActive (false);
			
		} else if (other.CompareTag ("Soda")) {

			health += pointPerSoda;
			SoundManager.instance.RandomizeSfx (drinkSound1,drinkSound2);
			foodText.text = "+" + pointPerSoda +" Health Points: " + health;
			other.gameObject.SetActive (false);
		}
	}

}
