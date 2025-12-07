using UnityEngine;

public class IndicatorCell : MonoBehaviour
{
    [SerializeField] private GameObject meshObject;

    public Vector2Int Position { get; private set; }

    public bool isFirstRow = false;

    public void SetPosition(int x, int y)
    {
        Position = new Vector2Int(x, y);
    }

    public void ToggleIndicator(bool state)
    {
        meshObject.SetActive(state);
    }

    public void SetMaterial(Material material)
    {
        meshObject.GetComponent<Renderer>().material = material;
    }
}
