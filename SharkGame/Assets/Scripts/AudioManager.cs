using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Ocean")]
    public AudioSource oceanSound;
    public float normalOceanVolume;
    public float stormOceanVolume;
    public float oceanSoundFadeTime;
    [Header("Tornado")]
    public AudioSource tornadoSound;

    public float tornadoSoundVolume;
    public float tornadoSoundFadeTime;
    [Header("Electricity")]
    public AudioSource electricitySound;
    public float electricityVolume;
    public float electricityFadeTime;

    Coroutine oceanFade;
    Coroutine tornadoFade;
    Coroutine electricityFade;


    // Start is called before the first frame update
    void Start()
    {
        oceanSound.volume = normalOceanVolume;
        tornadoSoundVolume = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartStorm() {
        if (oceanFade != null) { StopCoroutine(oceanFade); }
        if (tornadoFade != null) { StopCoroutine(tornadoFade); }
        oceanFade = StartCoroutine(LinearFade(oceanSound, oceanSoundFadeTime, oceanSound.volume, stormOceanVolume));
        tornadoFade = StartCoroutine(LinearFade(tornadoSound, tornadoSoundFadeTime, tornadoSound.volume, 1.0f));
    }

    public void EndStorm() {
        if (oceanFade != null) { StopCoroutine(oceanFade); }
        if (tornadoFade != null) { StopCoroutine(tornadoFade); }
        oceanFade = StartCoroutine(LinearFade(oceanSound, oceanSoundFadeTime, oceanSound.volume, normalOceanVolume));
        tornadoFade = StartCoroutine(LinearFade(tornadoSound, tornadoSoundFadeTime, tornadoSound.volume, 0.0f));
    }

    public void StartElectricity() {
        if (electricityFade != null) { StopCoroutine(electricityFade); }
        electricityFade = StartCoroutine(LinearFade(oceanSound, oceanSoundFadeTime, electricitySound.volume, electricityVolume));
    }

    public void EndElectricity() {
        if (electricityFade != null) { StopCoroutine(electricityFade); }
        electricityFade = StartCoroutine(LinearFade(electricitySound, electricityFadeTime, electricitySound.volume, 0.0f));
    }


    IEnumerator LinearFade(AudioSource audio, float fadeTime, float startVolume, float targetVolume) {
        audio.volume = startVolume;
        if (audio == electricitySound) { Debug.Log("Pikachu start:" + startVolume + ", target: " + targetVolume);}
        if (!audio.isPlaying) { audio.Play(); }
        float deltaVolume = (targetVolume - startVolume) / fadeTime;
        while (!NearEqual(audio.volume, targetVolume, 0.01f)) {
            audio.volume = audio.volume + deltaVolume * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (NearEqual(audio.volume, 0.0f, 0.01f)) { audio.Stop(); }
    }

    public bool NearEqual(float a, float b, float epsilon) {
        return (Mathf.Abs(a - b) <= epsilon);
    }
}
