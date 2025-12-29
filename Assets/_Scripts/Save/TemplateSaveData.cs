using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TemplateSaveData: MonoBehaviour
{
    //セーブしたいデータをpublicで入れる場所
    //例：public ObjectData objectData = new ObjectData();   
    [HideInInspector]public float bgmVolume = 0.1f;
    [HideInInspector]public float seVolume = 0.25f;
}