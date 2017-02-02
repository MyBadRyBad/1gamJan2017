using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnvironmentMusicManager : MonoBehaviour {

	AudioSource m_audioSource;
	private int m_LastSample;

	void Start() {
		m_audioSource = GetComponent<AudioSource>();
		m_LastSample = 0;
	}

	void Update() {
		AudioSource audio = GetComponent<AudioSource>();

		float freq = (float)audio.clip.frequency;
		int sample = audio.timeSamples;
		int sampleDelta = sample - m_LastSample;
		if (sampleDelta < 0)
			sampleDelta += audio.clip.samples;

		float delta = (float)sampleDelta / freq;

		Debug.Log (string.Format("Freq={0} Curr={1} Last={2} Delta={3} Time.deltaTime={4}", freq, sample, m_LastSample, delta, Time.deltaTime));

		m_LastSample = sample;
	}
}
