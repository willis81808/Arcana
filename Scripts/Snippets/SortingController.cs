using UnityEngine;

public class SortingController : MonoBehaviour
{
    public enum Layer
    {
        Default,
        Background,
        UI,
        Map,
        Card,
        Player1,
        Player2,
        Player3,
        Player4,
        Player5,
        Player6,
        Player7,
        Player8,
        Player9,
        Player10,
        Front,
        MapParticle,
        MostFront
    }

    [SerializeField]
    private Layer sortingLayer;

    [SerializeField]
    private int sortingOrder = 0;

    [SerializeField]
    private Renderer[] objectToSet;

    private void Start()
    {
        foreach (var obj in objectToSet)
        {
            if (obj == null) continue;
            obj.sortingLayerName = sortingLayer.ToString();
            obj.sortingOrder = sortingOrder;
        }
    }
}
