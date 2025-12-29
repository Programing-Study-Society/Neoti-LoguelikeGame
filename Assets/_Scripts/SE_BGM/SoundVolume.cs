using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolume : MonoBehaviour
{
    public List<AudioSource> bgmList;
    public List<AudioSource> seList;

    [HideInInspector]
    public KeepVolume keepVolume;

    void Start()//keepVolumeスクリプト取得
    {
        keepVolume = FindObjectOfType<KeepVolume>();

        if (keepVolume == null)
        {
            Debug.LogError("KeepVolume がシーン内に存在しません");
        }
        else
        {
            BGM_SoundVolume();
            SE_SoundVolume();
        }
    }
    
    public void BGM_SoundVolume()
    {
        for(int i = 0; i < bgmList.Count; i++)
        {
            bgmList[i].volume = keepVolume.bgmVolume;
        }
    }

    public void SE_SoundVolume()
    {
        for (int i = 0; i < seList.Count; i++)
        {
            seList[i].volume = keepVolume.seVolume;
        }
    }
}
