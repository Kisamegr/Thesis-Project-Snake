using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Options : MonoBehaviour {

	public Slider musicSlider;
	public Slider soundsSlider;
	public Slider aiSlider;


	// Use this for initialization
	void Start () {
		musicSlider.value = Save.instance.GetMusicVolume();
		soundsSlider.value = Save.instance.GetSoundsVolume();
		aiSlider.value = Save.instance.GetAiDepth();
	}

	public void ChangeMusicVolume() {
		Save.instance.SetMusicVolume(musicSlider.value);




		Debug.Log("MUSIC " + musicSlider.value);



	}

	public void ChangeSoundsVolume() {
		Save.instance.SetSoundsVolume(soundsSlider.value);
		Debug.Log("SOUND " +soundsSlider.value);
	}

	public void ChangeAiDepth() {
		Save.instance.SetAiDEpth((int)aiSlider.value);
		Debug.Log("AI " +aiSlider.value);
	}

	public void Back() {
		Application.LoadLevel(0);
	}



}
