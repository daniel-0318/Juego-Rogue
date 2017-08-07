using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {
	
	public Sprite dmgSprite;
	public int hp = 4;
	private SpriteRenderer sprinteRenderer;


	// Use this for initialization
	private void Awake () {
		
		sprinteRenderer = GetComponent<SpriteRenderer>();

	}

	public void DamageWall(int loss){

		sprinteRenderer.sprite = dmgSprite;
		hp -= loss;
		if(hp<0){
			/*No es aconsejable destruir un objeto durante la partida
			para que no se ejecute el recolector de basura y disminuya
			el rendimiento del juego. Es mejor deshabilitarlo*/
			//Destroy (gameObject);
			gameObject.SetActive(false);
		}
	}
}