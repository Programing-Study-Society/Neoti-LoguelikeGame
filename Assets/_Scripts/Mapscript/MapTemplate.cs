using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum LineType
{
    Left,
    Right,
    Center
}

public enum NodeType
{
    Battle,
    Shop,
    Chest,
    MiniBoss,
    Boss
}

[System.Serializable]
public class NodeData
{
    public NodeType type;
    public Sprite icon; // マップ上で表示するアイコン
}

[System.Serializable]
public class LineNodes
{
    public LineType line;
    public List<NodeData> nodes;
}

[CreateAssetMenu(fileName = "MapTemplate", menuName = "Neoti/MapTemplate")]
public class MapTemplate : ScriptableObject
{
    [System.Serializable]
    public class FloorData
    {
        public List<LineNodes> lineNodes;

        [System.NonSerialized]
        public Dictionary<LineType, List<NodeData>> dict;

        public void buildDictionary()
        {
            dict = new Dictionary<LineType, List<NodeData>>();
            foreach (var ln in lineNodes)
                dict[ln.line] = ln.nodes;
        }
    }

    public List<FloorData> floors;

    public void buildAll()
    {
        foreach (var f in floors)
            f.buildDictionary();
    }
}
