using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Vector2 gridScale = new Vector2(2, 2);
    [SerializeField] private Vector2Int gridSize = new Vector2Int(7, 10);
    public Vector2Int GridSize { get { return gridSize; } }
    [SerializeField] private Present presentPrefab;
    [SerializeField] private float presentHeight = 0.1f;

    [SerializeField] private float presentArcHeight = 10f;
    [SerializeField] private float presentArcSpeed = 5f;

    [SerializeField] private IndicatorCell cellPrefab;
    [SerializeField] private Material materialForFirstRowIndicator;

    [SerializeField] private int initialRowsToFill = 5;

    private IndicatorCell[,] cellArray;
    private GameObject[,] gridArray;
    public GameObject[,] GridArray { get { return gridArray; } }

    [SerializeField] private Bullet testFilipe;

    [SerializeField] private GameObject wilson;
    private int wilsonPositionX;

    [SerializeField] private LayerMask interectableMask;

    private Coroutine cellIndicatorCoroutine;

    [SerializeField] private int wilsonHealth = 3;
    private int wilsonCurrentHealth;
    [SerializeField] private Vector3[] presentPercentanges;
    public bool WilsonDoingStuff { get; private set; }

    private PlayerController playerController;

    private int currentTurn = 0;

    private WilsonLogic wilsonLogic;

    public bool fachadaDebuff = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wilsonLogic = FindAnyObjectByType<WilsonLogic>();
        playerController = FindAnyObjectByType<PlayerController>();

        wilsonCurrentHealth = wilsonHealth;

        if (gridArray == null)
            gridArray = new GameObject[gridSize.x, gridSize.y];

        if (cellArray == null)
            cellArray = new IndicatorCell[gridSize.x, gridSize.y];

        SpawnIndicatorCells();

        SetUp(true);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SetUp(bool firstRound = false)
    {
        StartCoroutine(SetUpCR(firstRound));
    }

    public IEnumerator SetUpCR(bool firstRound = false)
    {
        WilsonDoingStuff = true;
        ClearGrid();

        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < initialRowsToFill; i++)
        {

            if(firstRound && initialRowsToFill - i == 1)
            {
                break;
            }

            GenerateRandomPresentFirstLine();
            yield return new WaitForSeconds(0.8f);
            if(i < initialRowsToFill - 1)
                MoveAllObjectsDown();
            yield return new WaitForSeconds(0.25f);
        }

        if(firstRound)
        {
            NewRound();
        }
    }

    public void PushPresentCollumn(int column)
    {
        // 1. Destroy the top object if it exists (y = 0)
        GameObject topObj = gridArray[column, 0];
        if (topObj != null)
        {
            topObj.transform.DOMoveY(
                topObj.transform.position.y + 2f,
                0.5f
            )
            .SetEase(Ease.InOutSine)
            .OnComplete(() => Destroy(topObj));
        }

        // 2. Shift everything UP

        for (int y = 0; y < gridSize.y - 1; y++)
        {
            gridArray[column, y] = gridArray[column, y + 1];

            GameObject obj = gridArray[column, y];
            if (obj != null)
            {
                obj.transform.DOMove(
                    ConvertPosition(column, y, presentHeight),
                    0.2f
                ).SetEase(Ease.InOutSine);

                if (obj.TryGetComponent(out Present present))
                    present.PushPositionUp();
            }
        }

        // 3. Clear the bottom cell
        gridArray[column, gridSize.y - 1] = null;
    }

    public void SpawnNewRow()
    {
        StartCoroutine(SpawnNewRowCR());
    }

    public IEnumerator SpawnNewRowCR()
    {
        WilsonDoingStuff = true;
        GenerateRandomPresentFirstLine();
        yield return new WaitForSeconds(0.65f);
        MoveAllObjectsDown();
        yield return new WaitForSeconds(0.21f);
        WilsonDoingStuff = false;
        currentTurn++;
        playerController.StartTurn();
        
    }

    public void NewRound()
    {
        MoveWilson(Random.Range(0, gridSize.x));
        SpawnNewRow();
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

                if (y == gridSize.y - 1)
                {
                    int maskValue = interectableMask.value;

                    int layerIndex = (int)Mathf.Log(maskValue, 2);

                    cellTemp.SetMaterial(materialForFirstRowIndicator);

                    cellTemp.gameObject.layer = layerIndex;
                }

                cellTemp.SetPosition(x, y);

                cellArray[x, y] = cellTemp;
            }
        }

        TurnOffAllIndicatorCells();
        TurnOffFirstRowIndicator();
    }

    public void TurnOnCellIndicator(int x, int y)
    {
        cellArray[x, y].ToggleIndicator(true);
    }

    public void TurnOnFirstRowIndicator()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            cellArray[x, gridSize.y - 1].ToggleIndicator(true);

        }
    }

    public void TurnOffFirstRowIndicator()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            cellArray[x, gridSize.y - 1].ToggleIndicator(false);
        }
    }

    public void TurnOffAllIndicatorCells()
    {
        if (cellIndicatorCoroutine != null)
        {
            StopCoroutine(cellIndicatorCoroutine);
        }

        foreach (IndicatorCell cell in cellArray)
        {
            if(cell.Position.y == gridSize.y - 1)
            {
                continue;
            }
            cell.ToggleIndicator(false);
        }
    }

    public void DamageAllPresentsInCollumn(int collumn)
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            GameObject obj = gridArray[collumn, y];
            if (obj != null && obj.TryGetComponent(out Present present))
            {
                present.DamagePresent();
            }
        }

        if(wilsonPositionX == collumn)
        {
            wilsonLogic.StunWilson();
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
            Debug.Log(" - Turning on indicator at: " + pos);
            if (pos.y == gridSize.y - 1)
            {
                continue;
            }

            if (CheckIfPositionOutsideGrid(pos.x, pos.y))
            {
                continue;
            }

            cellArray[pos.x, pos.y].ToggleIndicator(true);
            yield return new WaitForSeconds(delay);
        }
    }

    private void GenerateRandomPresentFirstLine()
    {
        StartCoroutine(GenerateRandomPresentFirstLineCR());
    }

    private IEnumerator GenerateRandomPresentFirstLineCR()
    {
        float initialChance = 0.35f;
        for (int x = 0; x < gridSize.x; x++)
        {

            if (fachadaDebuff)
                continue;

            if (Random.value > initialChance)
            {
                Debug.Log("No present spawned at column " + x);
                initialChance += 0.1f;
                continue;
            }
                

            Vector3 spawnPosition = ConvertPosition(x, 0, presentHeight);
            Present presentTemp = Instantiate(presentPrefab, wilson.transform.position, Quaternion.identity);

            float randValue = Random.value;

            if (randValue < presentPercentanges[wilsonHealth - wilsonCurrentHealth].x)
            {
                Debug.Log("Rolled for 1 health present. Roll: " + randValue);
                presentTemp.Initialize(1, new Vector2Int(x, 0));
            }
            else if (randValue < presentPercentanges[wilsonHealth - wilsonCurrentHealth].y + presentPercentanges[wilsonHealth - wilsonCurrentHealth].x)
            {
                Debug.Log("Rolled for 2 health present. Roll: " + randValue);
                presentTemp.Initialize(2, new Vector2Int(x, 0));
            }
            else
            {
                Debug.Log("Rolled for 3 health present. Roll: " + randValue);
                presentTemp.Initialize(3, new Vector2Int(x, 0));
            }

            Debug.Log("Rolled random value: " + randValue);

            presentTemp.transform.DOJump(
            endValue: spawnPosition, // The target world position
            jumpPower: presentArcHeight,      // The height of the arc's peak
            numJumps: 1,        // The number of arcs (keep at 1 for mortar)
            duration: presentArcSpeed         // The total time of the jump
            ).SetEase(Ease.InOutSine);

            AddObjectToGrid(x, 0, presentTemp.gameObject);

            yield return new WaitForSeconds(0.05f);
        }

        fachadaDebuff = false;
    }

    private void MoveWilson(int x, bool pushed = false)
    {
        if (pushed)
            wilson.transform.DOMove(ConvertPosition(x, -1), 0.3f).SetEase(Ease.InOutElastic);
        else
            wilson.transform.DOMove(ConvertPosition(x, -1), 0.3f).SetEase(Ease.OutBack, overshoot: 4f);

        wilsonPositionX = x;
    }

    private void MoveAllObjectsDown()
    {
        bool presentsDestryoed = false;

        for (int x = 0; x < gridSize.x; x++)
        {
            // 1. HANDLE THE BOTTOM ROW BEFORE SHIFTING
            GameObject bottomObj = gridArray[x, gridSize.y - 1];
            if (bottomObj != null)
            {
                // Animate falling out of the grid
                bottomObj.transform.DOMoveY(
                    bottomObj.transform.position.y - 10f, 0.6f
                )
                .SetEase(Ease.InOutSine)
                .OnComplete(() => Destroy(bottomObj));

                presentsDestryoed = true;
            }

            // 2. SHIFT EVERYTHING DOWN
            for (int y = gridSize.y - 1; y > 0; y--)
            {
                gridArray[x, y] = gridArray[x, y - 1];

                if (gridArray[x, y] != null)
                {
                    gridArray[x, y].transform.DOMove(
                        ConvertPosition(x, y, presentHeight),
                        0.2f
                    ).SetEase(Ease.InOutSine);

                    if (gridArray[x, y].TryGetComponent(out Present present))
                        present.PushPositionDown();
                }
            }

            // 3. CLEAR THE TOP CELL
            gridArray[x, 0] = null;
        }

        if(presentsDestryoed)
        {
            wilsonLogic.Shoot(false);
        }
    }


    private void ClearGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = gridSize.y - 1; y > 0; y--)
            {
                if (gridArray[x, y] != null)
                {
                    gridArray[x, y].transform.DOMoveY(gridArray[x, y].transform.position.y - 2f, 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        Destroy(gridArray[x, y]);
                        gridArray[x, y] = null;
                    });
                }
            }
        }

        gridArray = new GameObject[gridSize.x, gridSize.y];
    }

    public GameObject GetObjectAtPosition(int x, int y)
    {
        if(CheckIfPositionOutsideGrid(x, y))
        {
            return null;
        }
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
        if (x == wilsonPositionX)
        {
            Debug.Log("Hit Wilson!");
            wilson.transform.DOPunchPosition(new Vector3(0, 0, -0.2f), 0.5f, 10, 1).OnComplete(() =>
            {
                // Additional logic on punch complete
                wilsonCurrentHealth--;
                if(wilsonCurrentHealth <= 0)
                {
                    Debug.Log("Wilson has been defeated!");
                    GameOver();
                }
                else
                {
                    Debug.Log("Wilson's current health: " + wilsonCurrentHealth);
                    SetUp(true);
                }
                
            });

        }

    }

    public void GameOver(bool win = true)
    {
        Debug.Log("Game Over!");
        if(win)
        {
            Debug.Log("You Win!");
        }
        else
        {
            Debug.Log("You Lose!");
        }
    }

    public void PushWilson(bool left)
    {
        //if(wilsonPositionX == 0 && left)
        //{
        //    return;
        //}

        //if(wilsonPositionX == gridSize.x - 1 && !left)
        //{
        //    return;
        //}

        int newX = wilsonPositionX + (left ? -1 : 1);
        newX = Mathf.Clamp(newX, 0, gridSize.x - 1);
        MoveWilson(newX, true);
    }
}
