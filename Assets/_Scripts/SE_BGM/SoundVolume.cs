using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolume : MonoBehaviour
{
    public List<AudioSource> bgmList;
    public List<AudioSource> seList;

    public float bgmVolume = 0.1f;
    public float seVolume = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        BGM_SoundVolume();
        SE_SoundVolume();
    }

    
    public void BGM_SoundVolume()
    {
        for(int i = 0; i < bgmList.Count; i++)
        {
            bgmList[i].volume = bgmVolume;
        }
    }

    public void SE_SoundVolume()
    {
        for (int i = 0; i < seList.Count; i++)
        {
            seList[i].volume = seVolume;
        }
    }
}
