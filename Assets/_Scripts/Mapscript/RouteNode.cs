using System;
using System.Collections.Generic;
using UnityEngine;

public enum StageType
{
    battle,
    treasure,
    shop,
    MidBoss,
    FinalBoss
}

[Serializable]
public class RouteNode
{
    public int id;
    public Vector2 position;
    public List<int> nextNodeIds = new List<int>();
    public StageType stageType = StageType.Normal;

    // 後でステージ内容を差し替えるためのID（シーン名やScriptableObject名など）
    public string stageId;
}