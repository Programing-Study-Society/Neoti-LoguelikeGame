using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemplateSaveData
{
    //セーブしたいデータをpublicで入れる場所
    //例：public ObjectData objectData = new ObjectData();   
    public float bgmVolume = 0.1f;
    public float seVolume = 0.25f;
}