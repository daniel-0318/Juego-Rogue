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

	//corrutina donde se hace el movimiento
	protected IEnumerator SmoothMovement(Vector2 end){
		float remainingDistance = Vector2.Distance (rb2D.position, end);
		while (remainingDistance > float.Epsilon) {
			Vector2 newPosition = Vector2.MoveTowards (rb2D.position, end, movementSpeed * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			remainingDistance = Vector2.Distance (rb2D.position, end);
			yield return null; // que se espere hasta el proximo fotograma
		}
	}


	/*devuelve true si el objecto se a movido y de lo contrario falso*/
	protected bool Move(int XDir, int yDir, out RaycastHit2D hit){
		Vector2 start = transform.position; // posicion donde esta el objeto
		Vector2 end = start + new Vector2 (XDir, yDir); // posicion final.

		/*se desactiva el boxcollider del objeto para evitar que se detecte a ella misma como si fuera una "colision"
		**y luego de los calculos se vuelve a activar*/
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blokingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {

			StartCoroutine (SmoothMovement(end));

			return true;
		}
		return false;


	}

	protected abstract void OnCantMove (GameObject go);


	protected virtual void AttempMove(int xDir, int yDir){
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);
		if (canMove) {
			return;
		}
		OnCantMove (hit.transform.gameObject);
	}
}
