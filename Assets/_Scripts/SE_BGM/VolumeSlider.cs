using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider bgmVolumeSlider;
    public Slider seVolumeSlider;

    public SoundVolume soundVolume;

    void Start()
    {
        bgmVolumeSlider.value = soundVolume.keepVolume.bgmVolume * 2;
        seVolumeSlider.value = soundVolume.keepVolume.seVolume * 2;
    }

    public void BGMVolume(){
        soundVolume.keepVolume.bgmVolume = (bgmVolumeSlider.value / 2);
        soundVolume.BGM_SoundVolume();
        //Debug.Log(soundVolume.keepVolume.bgmVolume);
    }
    public void SEVolume(){
        soundVolume.keepVolume.seVolume = (seVolumeSlider.value / 2);
        soundVolume.SE_SoundVolume();
        //Debug.Log(soundVolume.keepVolume.seVolume);
    }
}
