using System;
using System.Collections.Generic;
using UnityEngine;

public enum StageType
{
    Start,
    Battle,
    Treasure,
    Shop,
    MidBoss,
    FinalBoss
}

[Serializable]
public class RouteNode
{
    public int id;
    public Vector2 position;
    public List<int> nextNodeIds = new List<int>();
    public StageType stageType = StageType.Battle;

    // 後でステージ内容を差し替えるためのID（シーン名やScriptableObject名など）
    public string stageId;
    // public ScriptableObject stageData; // 必要になったら有効化
}