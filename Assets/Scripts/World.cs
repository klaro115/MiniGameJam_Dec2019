using UnityEngine;
using System.Collections;

public class World : MonoBehaviour
{
	public Vector2 worldSize = new Vector3(10, 5);

	private static World mainWorld = null;

	public static World Main => mainWorld;

	private AudioSource audioSrc = null;
	public AudioClip audioIntro = null;
	public AudioClip audioLoop = null;
	private float audioLoopStartTime = 0.0f;
	public float audioStartTimeDelay = -5.0f;

	void Awake()
	{
		mainWorld = this;

		if (audioSrc == null) audioSrc = GetComponent<AudioSource>();
		audioLoopStartTime = Time.time + (audioIntro != null ? audioIntro.length + audioStartTimeDelay : 0);
		audioSrc.loop = false;
		audioSrc.clip = audioIntro;
		audioSrc.Play();
	}

	void Start()
	{
		Camera cam = Camera.main;
		worldSize = cam.ViewportToWorldPoint(Vector3.one);
	}

	private void Update()
	{
		if (audioLoopStartTime > 0 && Time.time > audioLoopStartTime)
		{
			audioSrc.clip = audioLoop;
			audioSrc.loop = true;
			audioSrc.Play();
			audioLoopStartTime = -1;
		}
	}

	public bool IsOutsideBorders(Vector3 position)
	{
		return Mathf.Abs(position.x) > worldSize.x || Mathf.Abs(position.y) > worldSize.y;
	}
}
