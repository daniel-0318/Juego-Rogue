using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	public AudioClip chopSound1, chopSound2;

	public Sprite dmgSprite;
	public int hp = 4;
	private SpriteRenderer sprinteRenderer;


	// Use this for initialization
	private void Awake () {
		
		sprinteRenderer = GetComponent<SpriteRenderer>();

	}

	public bool DamageWall(int loss){

		SoundManager.instance.RandomizeSfx (chopSound1,chopSound2);
		sprinteRenderer.sprite = dmgSprite;
		hp -= loss;
		if(hp<=0){
			gameObject.SetActive(false);
			return true;
		}
		return false;
	}
}