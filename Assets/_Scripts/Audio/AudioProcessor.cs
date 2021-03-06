﻿#define NEW
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
[RequireComponent (typeof(SpectrumAnalyzer))]

public class AudioProcessor : MonoBehaviour
{
	public enum FreqRange {
		SubBass,
		Bass,
		LowMidrange,
		Midrange,
		UpperMidrange,
		Presence,
		Brilliance,
		Full
	};

	public struct SpectrumRange {
		public string name;
		public int min;
		public int max;
	};

	public FreqRange rangeSelector;
	private SpectrumRange[] spectrumRange;
	private SpectrumRange rangeToProcess;
	private float[] spectrumData;
	[HideInInspector]
	public int numSamples = 1024;
	private int frequency;
	private int frequencyNyquist;
	private float freqPerBin;
	private AudioSource audioSource;
	#if NEW
	private SpectrumAnalyzer spectrumAnalyzer;
	#else
	private SpectralFluxAnalyzer spectrumAnalyzer;
	#endif
	private BandGenerator bandGenerator;


    void Start()
    {
		audioSource = GetComponent<AudioSource>();
		audioSource.time = 17f;
		audioSource.Play();
		#if NEW
		spectrumAnalyzer =  GetComponent<SpectrumAnalyzer>();
		#else
		spectrumAnalyzer =  GetComponent<SpectralFluxAnalyzer>();
		#endif
		spectrumData = new float[numSamples];
		GetFreqDataFromClip();
		SetSpectrumRange(rangeSelector);
    }

    void Update()
    {
		audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
		#if NEW
		spectrumAnalyzer.AnalyzeSpectrum(spectrumData,rangeToProcess);
		#else
		spectrumAnalyzer.analyzeSpectrum(spectrumData,audioSource.time,rangeToProcess);
		#endif
    }

	public float CurrentSongTime() {return audioSource.time;}

	void GetFreqDataFromClip()
	{
		frequency = audioSource.clip.frequency;
		frequencyNyquist = frequency / 2;
		freqPerBin = (float)frequencyNyquist / (float)numSamples;
	}


	void SetSpectrumRange(FreqRange freqRange)
	{
		spectrumRange = new SpectrumRange[8];
		spectrumRange[0].name = "SubBass";
		spectrumRange[0].min = (int) (20f/freqPerBin);
		spectrumRange[0].max = (int) (60f/freqPerBin);

		spectrumRange[1].name = "Bass";
		spectrumRange[1].min = spectrumRange[0].max;
		spectrumRange[1].max = (int) (250f/freqPerBin);

		spectrumRange[2].name = "LowMidrange";
		spectrumRange[2].min = spectrumRange[1].max;
		spectrumRange[2].max = (int) (500f/freqPerBin);

		spectrumRange[3].name = "Midrange";
		spectrumRange[3].min = spectrumRange[2].max;
		spectrumRange[3].max = (int) (2000f/freqPerBin);

		spectrumRange[4].name = "UpperMidrange";
		spectrumRange[4].min = spectrumRange[3].max;
		spectrumRange[4].max = (int) (4000f/freqPerBin);

		spectrumRange[5].name = "Presence";
		spectrumRange[5].min = spectrumRange[4].max;
		spectrumRange[5].max = (int) (6000f/freqPerBin);

		spectrumRange[6].name = "Brilliance";
		spectrumRange[6].min = spectrumRange[5].max;
		spectrumRange[6].max = (int) (20000f/freqPerBin);

		spectrumRange[7].name = "Full";
		spectrumRange[7].min = spectrumRange[0].min;
		spectrumRange[7].max = spectrumRange[6].max;
		
		rangeToProcess = spectrumRange[(int)freqRange];
	}
}