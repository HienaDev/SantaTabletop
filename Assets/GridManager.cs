using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2 gridScale = new Vector2(2, 2);
    [SerializeField] private Vector2Int gridSize = new Vector2Int(7, 10);
    [SerializeField] private Present presentPrefab;
    [SerializeField] private float presentHeight = 0.1f;

    [SerializeField] private IndicatorCell cellPrefab;

    private IndicatorCell[,] cellArray;
    private GameObject[,] gridArray;

    [SerializeField] private Bullet testFilipe;

    [SerializeField] private GameObject wilson;
    private int wilsonPositionX;

    [SerializeField] private LayerMask interectableMask;

    private Coroutine cellIndicatorCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        if (gridArray == null)
            gridArray = new GameObject[gridSize.x, gridSize.y];

        if (cellArray == null)
            cellArray = new IndicatorCell[gridSize.x, gridSize.y];

        SpawnIndicatorCells();

        GenerateRandomPresentFirstLine();

        yield return new WaitForSeconds(0.35f);

        MoveAllObjectsDown();

        yield return new WaitForSeconds(0.35f);

        GenerateRandomPresentFirstLine();

        yield return new WaitForSeconds(0.35f);

        MoveAllObjectsDown();

        yield return new WaitForSeconds(0.35f);

        GenerateRandomPresentFirstLine();

        yield return new WaitForSeconds(0.35f);

        MoveAllObjectsDown();

        yield return new WaitForSeconds(0.35f);

        GenerateRandomPresentFirstLine();

        yield return new WaitForSeconds(0.35f);

        MoveAllObjectsDown();

        yield return new WaitForSeconds(0.35f);

        GenerateRandomPresentFirstLine();

        yield return new WaitForSeconds(0.35f);

        //MoveAllObjectsDown();

        //yield return new WaitForSeconds(0.35f);

        //GenerateRandomPresentFirstLine();

        //SpawnBullet(testFilipe, 3, 9);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnIndicatorCells()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 spawnPosition = ConvertPosition(x, y);
                IndicatorCell cellTemp = Instantiate(cellPrefab, spawnPosition, Quaternion.identity);
                cellTemp.transform.position = new Vector3(cellTemp.transform.position.x, 0.51f, cellTemp.transform.position.z);

                cellTemp.transform.parent = gameObject.transform;

                if(y == gridSize.y - 1)
                {
                    int maskValue = interectableMask.value;

                    int layerIndex = (int)Mathf.Log(maskValue, 2);

                    cellTemp.gameObject.layer = layerIndex;
                }

                cellTemp.SetPosition(x, y);

                cellArray[x, y] = cellTemp;
            }
        }

        TurnOffAllIndicatorCells();
    }

    public void TurnOnCellIndicator(int x, int y)
    {
        cellArray[x, y].ToggleIndicator(true);
    }

    public void TurnOffAllIndicatorCells()
    {
        if(cellIndicatorCoroutine != null)
        {
            StopCoroutine(cellIndicatorCoroutine);
        }

        foreach (IndicatorCell cell in cellArray)
        {
            cell.ToggleIndicator(false);
        }
    }

    public void TurnOnPathIndicators(List<Vector2Int> positions)
    {
        TurnOffAllIndicatorCells();

        cellIndicatorCoroutine = StartCoroutine(TurnOnIndicatorsCR(positions));
    }

    public IEnumerator TurnOnIndicatorsCR(List<Vector2Int> positions, float delay = 0.05f)
    {
        foreach (Vector2Int pos in positions)
        {
            cellArray[pos.x, pos.y].ToggleIndicator(true);
            yield return new WaitForSeconds(delay);
        }
    }

    private void GenerateRandomPresentFirstLine()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            if(Random.value > 0.5f)
                continue;

            Vector3 spawnPosition = ConvertPosition(x, 0, presentHeight);
            Present presentTemp = Instantiate(presentPrefab, spawnPosition, Quaternion.identity);

            AddObjectToGrid(x, 0, presentTemp.gameObject);
        }

        MoveWilson(0);// Random.Range(0, gridSize.x));
    }

    private void MoveWilson(int x)
    {
        wilson.transform.DOMove(ConvertPosition(x, -1), 0.3f).SetEase(Ease.InOutSine);
        wilsonPositionX = x;
    }

    private void MoveAllObjectsDown()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = gridSize.y - 1; y > 0; y--)
            {
                gridArray[x, y] = gridArray[x, y - 1];
                if (gridArray[x, y] != null)
                {
                    Debug.Log("Trying to move object at: " + gridArray[x, y]);
                    gridArray[x, y].transform.DOMove(ConvertPosition(x, y, presentHeight), 0.2f).SetEase(Ease.InOutSine);
                }
            }
            gridArray[x, 0] = null;
        }
    }

    public GameObject GetObjectAtPosition(int x, int y)
    {
        return gridArray[x, y];
    }

    public void SpawnBullet(Bullet bulletPrefab, int x, int y)
    {
        Vector3 spawnPosition = ConvertPosition(x, y);
        Bullet bulletTemp = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        if (bulletTemp.TryGetComponent<Bullet>(out Bullet bullet))
        {
            bullet.Initialize(x, y);
        }
    }

    public bool CheckIfPositionOutsideGrid(int x, int y)
    {
        return (x < 0 || x >= gridSize.x || y < 0 || y >= gridSize.y);
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

    public void AddObjectToGrid(int x, int y, GameObject objectAux)
    {
        gridArray[x, y] = objectAux;
    }

    public Vector3 ConvertPosition(int x, int y, float height = 1f)
    {
        return new Vector3((y * gridScale.y) + (-gridSize.y + 1), height, (x * gridScale.x) + (-gridSize.x + 1));
    }

    public void CheckIfHitWilson(int x)
    {
        if(x == wilsonPositionX)
        {
            Debug.Log("Hit Wilson!");
            wilson.transform.DOPunchPosition(new Vector3(0, 0, -0.2f), 0.5f, 10, 1);
        }
    }
}
