﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direccion{Horizontal, Vertical}

public class Bullet : MonoBehaviour {

	public Direccion direccionArma = Direccion.Horizontal;
	public int velocidad = 50;
	public int enemyDamage= 4;

	private Rigidbody2D rb2D;

	// Use this for initialization
	void Start () {
		rb2D = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Establecemos el arma en horizontal
		if(direccionArma == Direccion.Horizontal){
			rb2D.transform.Translate (new Vector3(velocidad,0,0)*Time.deltaTime);
		}else if(direccionArma == Direccion.Vertical){
			rb2D.transform.Translate (new Vector3(0,velocidad,0)*Time.deltaTime);
		}
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Enemy")) {
			Enemy hit = other.gameObject.GetComponent<Enemy> ();
			if (hit != null) {
				Debug.Log ("--------------------------------Toco al enemigo");
				bool respuesta = hit.LoseHealth (enemyDamage);
				Destroy (gameObject);
				if (respuesta) {
					Debug.Log ("La bala balio :v");
					GameManager.instance.enemigoMuerto = true;
				}
				GameManager.instance.PlayerTurn = false;
			}
		} else if (other.CompareTag ("Wall")) {
			Wall hit = other.gameObject.GetComponent<Wall> ();
			if (hit != null) {
				hit.DamageWall (enemyDamage);
				Destroy (gameObject);
				GameManager.instance.PlayerTurn = false;
			}
		} else if (other.CompareTag ("OuterWall")) {
			Destroy (gameObject);
			GameManager.instance.PlayerTurn = false;
		}

	}

	public void SetDamage(int damage){
		enemyDamage = damage;
	}
}
