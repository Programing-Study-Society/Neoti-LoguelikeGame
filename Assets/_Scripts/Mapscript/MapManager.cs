using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapTemplate[] templates;
    public MapTemplate current;

    public int currentFloor = 0;

    void Start()
    {
        current = templates[Random.Range(0, templates.Length)];
        current.buildAll();

        dump();
    }

    public NodeData getNode(LineType line, int index)
    {
        return current.floors[currentFloor].dict[line][index];
    }

    // Šm”F—p
    void dump()
    {
        var floor = current.floors[currentFloor];
        foreach (var kv in floor.dict)
        {
            foreach (var n in kv.Value)
                Debug.Log($"{kv.Key} : {n.type}");
        }
    }
}
