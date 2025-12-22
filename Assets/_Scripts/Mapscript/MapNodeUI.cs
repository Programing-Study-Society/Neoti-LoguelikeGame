using UnityEngine;
using UnityEngine.UI;

public class MapNodeUI : MonoBehaviour
{
    public Button button;
    public Image iconImage;

    LineType line;
    int index;

    public void setup(LineType l, int i, NodeData data)
    {
        line = l;
        index = i;

        // アイコン即表示
        iconImage.sprite = data.icon;
        iconImage.enabled = true;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
    }

    void onClick()
    {
        var mgr = FindObjectOfType<MapManager>();
        var node = mgr.getNode(line, index);

        Debug.Log($"clicked {line}:{index} type={node.type}");
    }
}
