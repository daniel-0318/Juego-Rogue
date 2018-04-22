using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

	//tiempo que va a tardar el objecto en moverse de una casilla a otra.
	public float moveTime = 0.1f;
	//solos los que tengan como layer "blokingLayer" no dejaran que pasen por encima de ellos.
	public LayerMask blokingLayer;

	private  float movementSpeed;
	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;

	protected virtual void Awake(){
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
	}
	protected virtual void Start () {
		movementSpeed = 1f / moveTime;
	}

	//corrutina donde se hace el movimiento ######### tentativamente esta funcion no se usa
	protected IEnumerator SmoothMovement(Vector2 end){
		float remainingDistance = Vector2.Distance (rb2D.position, end);
		Debug.Log ("init: "+ rb2D.position + " finish: " + remainingDistance);
		while (remainingDistance > float.Epsilon ) {
			Debug.Log (remainingDistance);
			Vector2 newPosition = Vector2.MoveTowards (rb2D.position, end, movementSpeed * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			remainingDistance = Vector2.Distance (rb2D.position, end);
			yield return null; // que se espere hasta el proximo fotograma
		}
	}


	/**
	 * devuelve true si el objecto se a movido y de lo contrario falso
	 * este metodo devuelve dos valores, un bool y otro por el out
	*/
	protected bool Move(int XDir, int yDir, out RaycastHit2D hit){
		Vector2 start = transform.position; // posicion donde esta el objeto
		Vector2 end = start + new Vector2 (XDir, yDir); // posicion final.

		/*se desactiva el boxcollider del objeto para evitar que se detecte a ella misma como si fuera una "colision"
		**y luego de los calculos se vuelve a activar*/
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blokingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {

			//StartCoroutine (SmoothMovement(end));
			rb2D.MovePosition (end);
			return true;
		}
		return false;


	}

	/**
	 * Metodo abstracto para que dependiendo de quien sea "go" (ej el jugador o los muros)
	 * s
	*/
	protected abstract void OnCantMove (GameObject go);


	/**
	 * Este metodo llama a move para intentar moverse, si se hizo el movimiento Raycasthit2d ya tiene algo
	 * sino se "notifica que no pudo" y ya se llama oncantmove que es abstracto para que el enemigo o el jugador
	 * haga lo que tenga que hacer.
	*/
	protected virtual bool AttempMove(int xDir, int yDir){
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);
		if (!canMove) {
			OnCantMove (hit.transform.gameObject);
		}
		return canMove;

	}
}
