using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RouteNodeView : MonoBehaviour
{
    public int NodeId { get; set; }
    public RouteMapGenerator Generator { get; set; }

    private void OnMouseUpAsButton()
    {
        if (Generator != null)
        {
            Generator.OnNodeClicked(NodeId);
        }
    }
}