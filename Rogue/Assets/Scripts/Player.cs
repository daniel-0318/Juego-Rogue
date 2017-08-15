using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {


	public int wallDamage = 1;
	public int pointPerfood = 10;
	public int pointPerSoda = 20;
	public float restartLEvelDelay = 1f;
	public Text foodText;

	private Animator animator;
	private int food;

	protected override void Awake(){
		animator = GetComponent<Animator> ();
		base.Awake ();
	}

	protected override void Start(){
		
		food = GameManager.instance.PlayerFoodPoints;
		foodText.text = "Food: " + food;
		base.Start ();
	}

	/*OnDisable se ejecuta antes de ejecutar el OnDestroy, que se ejecutara
	 * cuando el objeto Player sea destruido para recargar la escena con una nueva
	 * Esto hara que se guarde en la clase GameManager la comida que se lleva
	*/
	private void OnDisable(){
		GameManager.instance.PlayerFoodPoints = food;
	}

	void CheckIfGameOver(){
		if (food < 0)
			GameManager.instance.GameOver ();
	}

	protected override void AttempMove(int xDir, int yDir){
		food--;
		foodText.text = "Food: " + food;
		base.AttempMove(xDir, yDir);
		CheckIfGameOver ();
		GameManager.instance.PlayerTurn = false;

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
			AttempMove (horizontal, vertical);
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
		food -= loss;
		foodText.text = "-" + loss +" Food: " + food;
		animator.SetTrigger ("playerHit");
		CheckIfGameOver ();
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Exit")) {

			Invoke ("Restart", restartLEvelDelay);
			enabled = false; // para que no se pueda seguir moviendo el jugador
			
		} else if (other.CompareTag ("Food")) {

			food += pointPerfood;
			foodText.text = "+" + pointPerfood +" Food: " + food;
			other.gameObject.SetActive (false);
			
		} else if (other.CompareTag ("Soda")) {

			food += pointPerSoda;
			foodText.text = "+" + pointPerSoda +" Food: " + food;
			other.gameObject.SetActive (false);
		}
	}

}
