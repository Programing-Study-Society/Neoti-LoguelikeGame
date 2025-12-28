using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider bgmVolumeSlider;
    public Slider seVolumeSlider;

    public SoundVolume soundVolume;

    // Start is called before the first frame update
    void Start()
    {
        bgmVolumeSlider.value = soundVolume.bgmVolume * 5;
        seVolumeSlider.value = soundVolume.seVolume * 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BGMVolume(){
        soundVolume.bgmVolume = (bgmVolumeSlider.value / 5);
        soundVolume.BGM_SoundVolume();
        Debug.Log(soundVolume.bgmVolume);
    }
    public void SEVolume(){
        soundVolume.seVolume = (seVolumeSlider.value / 2);
        soundVolume.SE_SoundVolume();
        Debug.Log(soundVolume.seVolume);
    }
}
