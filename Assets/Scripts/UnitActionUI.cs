using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Tilemaps;

public class UnitActionUI : MonoBehaviour
{
    public static UnitActionUI Instance { get; private set;}
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button waitButton;
    public Tilemap groundTilemap;
    public bool isSelecting = false;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Hide();
        attackButton.onClick.AddListener(OnAttack);
        waitButton.onClick.AddListener(OnWait);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Hide()
    {
        actionPanel.SetActive(false);
    }


    void OnAttack()
    {
        if (TurnManager.turnInstance.currentTurn == TurnManager.Turn.Player)
        {
            Unit attacker = UnitManager.unitManagerInstance.activeUnit;
            Unit target = UnitManager.unitManagerInstance.passiveUnit;
            target.GetComponent<UnitHP>().attackHP(attacker.attack);
        }
        else if(TurnManager.turnInstance.currentTurn == TurnManager.Turn.Enemy)
        {
            Unit attacker = UnitManager.unitManagerInstance.activeUnit;
            Unit target = UnitManager.unitManagerInstance.passiveUnit;
            target.GetComponent<UnitHP>().attackHP(attacker.attack);
        }
        isSelecting = false;
        actionPanel.SetActive(false);
    }

    void OnWait()
    {
        isSelecting = false;
        actionPanel.SetActive(false);
    }

    public IEnumerator waitSelect()//多重実行の可能性あり
    {
        if (isSelecting) yield break;
        isSelecting = true;
        TileManager.instance.ClearRange();
        actionPanel.SetActive(true);
        if (UnitManager.unitManagerInstance.CheckDistanceOfEnemy())
        {
            attackButton.gameObject.SetActive(true); 
        }
        else
        {
            attackButton.gameObject.SetActive(false);
        }
        
        yield return new WaitUntil(() => !isSelecting);
        Debug.Log("ターン遷移");
        if (TurnManager.turnInstance.currentTurn == TurnManager.Turn.Player)
        {
            Vector3Int tmp = groundTilemap.WorldToCell(UnitManager.unitManagerInstance.activeUnit.transform.position);
            UnitManager.unitManagerInstance.activeUnit.v3currentPos = tmp;
            UnitManager.unitManagerInstance.activeUnit.v2currentPos = new Vector2Int(tmp.x, tmp.y);
            UnitManager.unitManagerInstance.activeUnit.v2newPos = new Vector2Int(tmp.x, tmp.y);
        }
        else
        {
            Vector3Int tmp = groundTilemap.WorldToCell(UnitManager.unitManagerInstance.activeUnit.transform.position);
            UnitManager.unitManagerInstance.activeUnit.v3currentPos = tmp;
            UnitManager.unitManagerInstance.activeUnit.v2currentPos = new Vector2Int(tmp.x, tmp.y);
            UnitManager.unitManagerInstance.activeUnit.v2newPos = new Vector2Int(tmp.x,tmp.y);
        }

        UnitManager.unitManagerInstance.activeUnit.unitState = UnitState.Finished;
        TurnManager.Turn currentTurn = TurnManager.turnInstance.currentTurn;
        Unit nextUnit = UnitManager.unitManagerInstance.NextActiveUnit(currentTurn);
        if (null != nextUnit)
        {
            TurnManager.turnInstance.ChangeUnit(nextUnit);
        }
        else
        {
            UnitManager.unitManagerInstance.ChangetoWait();
            TurnManager.turnInstance.NextTurn();
        }
    }


}
