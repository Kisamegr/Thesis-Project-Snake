using UnityEngine;
using System.Collections;
using System;

public class Save : MonoBehaviour {

	[SerializeField]
	private float musicVolume;
	[SerializeField]
	private float soundsVolume;
	[SerializeField]
	private int aiDepth;

	public static Save instance;
	void Awake() {
		
		instance = this;
	}

	void Start() {

		if(!PlayerPrefs.HasKey("music")) {
			PlayerPrefs.SetFloat("music",0.9f);
			musicVolume = 0.9f;
		}
		else 
			musicVolume = PlayerPrefs.GetFloat("music");

		

		if(!PlayerPrefs.HasKey("sounds")) {
			PlayerPrefs.SetFloat("sounds",0.4f);
			soundsVolume = 0.4f;
		}
		else 
			soundsVolume = PlayerPrefs.GetFloat("sounds");
			
		

		if(!PlayerPrefs.HasKey("aiDepth")) {
			PlayerPrefs.SetInt("aiDepth",7);
			aiDepth = 7;
		}
		else {
			aiDepth = PlayerPrefs.GetInt("aiDepth");
		}

		SoundManager.instance.SetMusicVolume(musicVolume);
		SoundManager.instance.SetSoundsVolume(soundsVolume);


	}






	public void SetMusicVolume(float value) {
		 
		PlayerPrefs.SetFloat("music",value);
		SoundManager.instance.SetMusicVolume(musicVolume);
		musicVolume = value;

	}

	public void SetSoundsVolume(float value) {
		
		PlayerPrefs.SetFloat("sounds",value);
		SoundManager.instance.SetSoundsVolume(soundsVolume);
		soundsVolume = value;
	}

	public void SetAiDEpth(int value) {
		PlayerPrefs.SetInt("aiDepth",value);
		aiDepth = value;
	}

	public float GetMusicVolume() {
		return musicVolume;
	}

	public float GetSoundsVolume() {
		return soundsVolume;
	}

	public int GetAiDepth() {
		return aiDepth;
	}

	void OnLevelWasLoaded(int level) {

		if(level == 0) {
			Start ();

		}

	}

	 
}
