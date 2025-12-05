using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2 gridScale = new Vector2(2, 2);
    [SerializeField] private Vector2Int gridSize = new Vector2Int(7, 10);
    [SerializeField] private Present presentPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TestGridFillWithPresents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TestGridFillWithPresents()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 spawnPosition = ConvertPosition(x, y);
                Instantiate(presentPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    public Vector3 ConvertPosition(int x, int y)
    {
        return new Vector3((y * gridScale.y) + (-gridSize.y + 1), 1, (x * gridScale.x) + (-gridSize.x + 1));
    }
}
