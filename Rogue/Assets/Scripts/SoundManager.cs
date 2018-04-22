using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioSource musicSource;
	public AudioSource sfxSource;


	public float lowPitchRange = 0.95f;
	public float highPitchRange = 1.05f;

	public static SoundManager instance; //para el singlenton


	private void Awake(){
		if (SoundManager.instance == null) {
			SoundManager.instance = this;
		} else if(SoundManager.instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
	
	}
		

	public void PlaySingle(AudioClip clip){

		sfxSource.pitch = 1f;
		sfxSource.clip = clip;
		sfxSource.Play ();
	
	}

	public void RandomizeSfx(params AudioClip[] clips){

		int randomIndex = Random.Range (0, clips.Length);
		float randomPitch = Random.Range (lowPitchRange, highPitchRange);
		sfxSource.pitch = randomPitch;
		sfxSource.clip = clips [randomIndex];
		sfxSource.Play ();

	}

}
