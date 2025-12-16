using System.Collections.Generic;
using UnityEngine;

public enum LineType
{
    Left,
    Right,
    Center
}

public enum NodeType
{
    Empty,
    Battle,
    Shop,
    Chest,
    MiniBoss,
    Boss
}

[System.Serializable]
public class LineNodes
{
    public LineType line;
    public List<NodeType> nodes;
}

[CreateAssetMenu(fileName = "MapTemplate", menuName = "Neoti/MapTemplate")]
public class MapTemplate : ScriptableObject
{
    [System.Serializable]
    public class FloorData
    {
        public int floorIndex;
        public int nodeCount = 10;

        // inspector Ç≈ÇÕÇ±ÇÍÇæÇØï“èWÇ∑ÇÈ
        public List<LineNodes> lineNodes;

        // é¿çséû: dictionary Ç…ïœä∑Ç∑ÇÈ
        [System.NonSerialized]
        public Dictionary<LineType, List<NodeType>> dict;

        public void buildDictionary()
        {
            dict = new Dictionary<LineType, List<NodeType>>();

            foreach (var ln in lineNodes)
            {
                dict[ln.line] = ln.nodes;
            }
        }
    }

    public List<FloorData> floors;

    public void buildAllDictionaries()
    {
        foreach (var f in floors)
        {
            f.buildDictionary();
        }
    }
}
