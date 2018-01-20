using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioClip menuClip;
	public AudioClip gameClip;

	public AudioClip lowSnakeClip;
	public AudioClip highSnakeClip;

	public AudioClip startClip;
	public AudioClip finishClip;

	[SerializeField]
	private AudioSource music_source;
	[SerializeField]
	private AudioSource snake_source;


	private bool goHigh=false;

	public static SoundManager instance;

	void Awake() {
		if(instance !=null) {
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(transform.gameObject);
		instance = this;
	}
	// Use this for initialization
	void Start () {
		music_source = gameObject.GetComponents<AudioSource>()[0];
		snake_source = gameObject.GetComponents<AudioSource>()[1];


		//if(!music_source.isPlaying || (music_source.isPlaying && music_source.clip != menuClip)) {
		//	music_source.clip = menuClip;
		//	music_source.Play();
		//}

	}
	
	void OnLevelWasLoaded(int level) {

		if(level < 3) {
			if(!music_source.isPlaying || (music_source.isPlaying && music_source.clip != menuClip)) {
				music_source.clip = menuClip;
				music_source.Play();
			}
		}
		
		if(level == 3) {
			if(!music_source.isPlaying || (music_source.isPlaying && music_source.clip != gameClip)) {
				music_source.clip = gameClip;
				music_source.Play();
			}
		}


	}

	public void PlaySnakeMoveSound() {
		if(!goHigh) 
			snake_source.clip = lowSnakeClip;
		else 
			snake_source.clip = highSnakeClip;

		snake_source.Play();
		goHigh = !goHigh;


	}

	public void PlayStartSound() {
		AudioSource.PlayClipAtPoint(startClip,transform.position);
	}

	public void PlayFinishSound() {
		AudioSource.PlayClipAtPoint(finishClip,transform.position);
	}

	public void SetMusicVolume(float volume) {
		music_source.volume = volume;
	}
	public void SetSoundsVolume(float volume) {
		snake_source.volume = volume;
	}

}
