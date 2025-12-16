using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapTemplate[] templates;
    public MapTemplate current;

    public int currentFloor = 0;
    public int currentIndex = 0;
    public LineType currentLine = LineType.Left;

    void Start()
    {
        pickRandomTemplate();
        debugDumpCurrentFloor();
    }

    void pickRandomTemplate()
    {
        current = templates[Random.Range(0, templates.Length)];
        current.buildAllDictionaries();
    }

    public void debugDumpCurrentFloor()
    {
        var floor = current.floors[currentFloor];

        Debug.Log($"floor {floor.floorIndex} dump start");

        foreach (var kv in floor.dict)
        {
            var line = kv.Key;
            var nodes = kv.Value;

            string s = $"{line}: ";

            for (int i = 0; i < nodes.Count; i++)
            {
                s += nodes[i] + (i < nodes.Count - 1 ? ", " : "");
            }

            Debug.Log(s);
        }

        Debug.Log("=== dump end ===");
    }

    public NodeType getNode(LineType line, int index)
    {
        return current.floors[currentFloor].dict[line][index];
    }
}
