using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Player : MovingObject {

	public AudioClip moveSound1, moveSound2, eatSound1, eatSound2, drinkSound1, drinkSound2, gameOverSound;
	public GameObject[] foodTiles;

	public int wallDamage = 1;
	public int enemyDamage = 2;
	public int pointPerfood = 10;
	public int pointPerSoda = 20;
	public int pointPerAmmo = 2;
	public int pointPerCoin = 1;
	public int pointPerKillEnemy = 1;
	public float restartLEvelDelay = 1f;
	public Text healthText;
	public Text ammoText;
	public Text scoreText;
	public Text killsEnemiesText;
	public int secretosEncontrados = 0; // por ahora si es mas de 2 veces que encuentra es porque si esta buscando en los muros
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
			killsEnemies += 1;
			GameManager.instance.enemigoMuerto = false;
		}

		int horizontal=0;
		int vertical=0;


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
			}else if (Input.GetKeyDown (KeyCode.S)) {
				Debug.Log ("S");
				Scriptbullet.direccionArma = Direccion.Vertical;
				Scriptbullet.velocidad = -50;
				disparo = true;
			}else if (Input.GetKeyDown (KeyCode.A)) {
				Debug.Log ("A");
				Scriptbullet.direccionArma = Direccion.Horizontal;
				Scriptbullet.velocidad = -50;
				disparo = true;
			}else if (Input.GetKeyDown (KeyCode.D)) {
				Debug.Log ("D");
				Scriptbullet.direccionArma = Direccion.Horizontal;
				Scriptbullet.velocidad = 50;
				disparo = true;
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
			//solo si el muro es golpeado con las manos puede dar alguna bonificacion.
			bool respuesta  = hitWall.DamageWall (wallDamage);
			cantidadPasos -= 1;
			if (respuesta) {
				probar_suerte_premio (hitWall);
			}
			animator.SetTrigger ("playerChop");
		}
		Enemy hitEnemy = go.GetComponent<Enemy> ();
		if (hitEnemy != null) {
			bool respuesta = hitEnemy.LoseHealth (enemyDamage);
			cantidadPasos -= 1;
			animator.SetTrigger ("playerChop");
			if (respuesta) {
				killsEnemies += pointPerKillEnemy;
				killsEnemiesText.text = "kills: " + killsEnemies;
			}
		}

	}

	public void probar_suerte_premio (Wall hitWall){
		float probabilidad_de_dar_premio = Random.Range (0.0f, 1f);
		Debug.Log ("La suerte es de: " + probabilidad_de_dar_premio);
		if (probabilidad_de_dar_premio >= 0.5f) {
			Debug.Log("Tuvo suerte!!!!!!");
			float premio = Random.Range (0.0f, 4.0f);
			if (premio < 1) {
				Instantiate(foodTiles[3],hitWall.transform.position,Quaternion.identity);
			} else if (premio >= 1 && premio < 2) {
				Instantiate(foodTiles[2],hitWall.transform.position,Quaternion.identity);
			} else if (premio >= 2 && premio < 3) {
				Instantiate(foodTiles[0],hitWall.transform.position,Quaternion.identity);
			} else if (premio >= 3 && premio <= 4) {
				Instantiate(foodTiles[1],hitWall.transform.position,Quaternion.identity);
			}
			secretosEncontrados += 1;
				
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
			if (cantidadPasos > 256) {
				cantidadPasos = 256;
			}

			if (GameManager.instance.get_nivel_a_reset_pasos() == GameManager.instance.getLevel()) {
				GameManager.instance.reset_lista ();
				GameManager.instance.set_nivel_a_reset_pasos (2);
			}
			GameManager.instance.numeroPasosJugador.Add (cantidadPasos);
			GameManager.instance.playerHealthtPoints = health;
			GameManager.instance.playerammoPoints = ammo;
			GameManager.instance.playerScorePoints = score;
			GameManager.instance.playerKillsPoints = killsEnemies;
			GameManager.instance.secretosEncontrados = secretosEncontrados;
			cantidadPasos = 0;
			GameManager.instance.guardar (0);
			secretosEncontrados = 0;
			Invoke ("Restart", restartLEvelDelay);
			enabled = false; // para que no se pueda seguir moviendo el jugador
			
		} else if (other.CompareTag ("Food")) {
			GameManager.instance.comida_adquirida += 1;
			health += pointPerfood;
			SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
			healthText.text = " Health Points: " + health;
			other.gameObject.SetActive (false);
			
		} else if (other.CompareTag ("Soda")) {
			GameManager.instance.comida_adquirida += 1;
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
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
			scoreText.text = "Score: " + score;
			other.gameObject.SetActive (false);
			
		}else if (other.CompareTag ("Node")) {
			GameManager.instance.setCoordeNode ((Vector2)other.gameObject.transform.position);
		}
	}

}
