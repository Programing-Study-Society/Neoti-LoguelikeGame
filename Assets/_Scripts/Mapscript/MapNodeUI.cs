using UnityEngine;
using UnityEngine.UI;

public class MapNodeUI : MonoBehaviour
{
    public Button btn;
    public LineType line;
    public int index;

    public void setup(LineType l, int i)
    {
        line = l;
        index = i;
        
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            FindObjectOfType<MapManager>().onNodeClicked(line, index);
        });
        void onClick()
    {
        var mgr = FindObjectOfType<MapManager>();
        var type = mgr.getNode(line, index);

        Debug.Log($"clicked: line={line}, index={index}, type={type}");
    }
    }
}

