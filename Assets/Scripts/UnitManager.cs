using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

/*
// TODO:
//[] ユニットを増やして生成するために関数に生成の手順をまとめる
//[] ユニットリストを辞書型にするなど
//[] ユニットの名前を渡す感じにしてhpバーと含めてリストにする
//[] TurnUIのimgは他のスクリプトの変数に代入するのではなくTurnUI自身が持っておく
//[] ユニットへのポジションの代入処理はunitスクリプトでやる
*/

public enum UnitState
{
    Wait,Active,Finished
}

public class UnitManager : MonoBehaviour
{
    public static UnitManager unitManagerInstance;
    public Transform canvasTransform;
    [SerializeField] private Transform hpBarList;
    [SerializeField] private GameObject enemyHPSliderPrefab;
    [SerializeField] private GameObject playerHPSliderPrefab;
    public List<Unit> allUnits = new List<Unit>();
    public Unit activeUnit;
    public Unit passiveUnit;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    private Vector2Int v2playerPos = new Vector2Int(4, 0);
    private Vector2Int v21playerPos = new Vector2Int(2, 0);
    private Vector2Int v2enemyPos = new Vector2Int(2, 8);
    private Vector2Int v21enemyPos=new Vector2Int(6, 8);

    private void Awake()
    {
        if (unitManagerInstance == null) unitManagerInstance = this;
        else Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InstantiateUnits(playerPrefab, playerHPSliderPrefab, UnitType.Player,v2playerPos);
        InstantiateUnits(playerPrefab, playerHPSliderPrefab, UnitType.Player,v21playerPos);
        InstantiateUnits(enemyPrefab, enemyHPSliderPrefab, UnitType.Enemy, v2enemyPos);
        InstantiateUnits(enemyPrefab, enemyHPSliderPrefab, UnitType.Enemy, v21enemyPos);



        TurnManager.turnInstance.img = TurnUI.Instance.GetComponent<Image>();
        TurnManager.turnInstance.NextTurn();//ターンが切り替わる時はしっかりactiveUnit設定されてるから大丈夫
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public List<Unit> GetPlayerUnits()
    {
        List<Unit> players = new List<Unit>();
        foreach(var unit in allUnits)
        {
            if (unit.unitType == UnitType.Player)
                players.Add(unit);
        }
        return players;
    }

    //敵ユニットを返す
    public List<Unit> GetEnemyUnits()
    {
        List<Unit> enemies = new List<Unit>();
        foreach(var unit in allUnits)
        {
            if (unit.unitType == UnitType.Enemy)
                enemies.Add(unit);
        }
        return enemies;
    }*/
    public void InstantiateUnits(GameObject UnitPrefab,GameObject HPSliderPrefab,UnitType unitType,Vector2Int v2unitPos)
    {
        //自軍を左下に配置
        Vector3 v3unitPos = TileManager.instance.groundTilemap.GetCellCenterWorld((Vector3Int)v2unitPos);
        GameObject playerObj = Instantiate(UnitPrefab, v3unitPos, Quaternion.identity);
        Unit playerUnit = playerObj.GetComponent<Unit>();
        
        allUnits.Add(playerUnit);
        
        playerUnit.unitType = unitType;
        
        playerUnit.v3currentPos = v3unitPos;
        
        playerUnit.v2currentPos = v2unitPos;
        
        playerUnit.v2newPos = v2unitPos;
        

        GameObject playerhpPanel = Instantiate(HPSliderPrefab, hpBarList);
        UnitStatusUI playerStatusUI = playerhpPanel.GetComponent<UnitStatusUI>();
        playerUnit.GetComponent<UnitHP>().hpSlider = playerStatusUI.hpSlider;



        //敵を右上に配置



    } 
    public Unit NextActiveUnit(TurnManager.Turn currentTurn)
    {
        if (currentTurn == TurnManager.Turn.Player)
        {
            foreach (var unit in allUnits)
            {
                if (unit.unitType == UnitType.Player && unit.unitState == UnitState.Wait)
                {
                    unit.unitState = UnitState.Active;
                    activeUnit = unit;
                    return unit;
                }
            }
            return null;
        }
        else if (currentTurn == TurnManager.Turn.Enemy)
        {
            foreach (var unit in allUnits)
            {
                if (unit.unitType == UnitType.Enemy && unit.unitState == UnitState.Wait)
                {
                    unit.unitState = UnitState.Active;
                    activeUnit = unit;
                    return unit;
                }
            }
            return null;
        }
        else
        {
            Debug.Log("想定外");
            return null;
        }
    }

    public void ChangetoWait()
    {
        foreach(var unit in allUnits)
        {
            unit.unitState = UnitState.Wait;
        }
    }

    public void CalculateDistanceOfOtherTypeUnit(TurnManager.Turn currentTurn)//違うタイプのユニットとの距離を記録する
    {
        if (currentTurn == TurnManager.Turn.Player)
        {
            foreach(var unit in allUnits)
            {
                if(unit.unitType == UnitType.Enemy)
                {
                    unit.curOtherUnitTypeDistance = ManhattanDistance(activeUnit.v2newPos, unit.v2newPos);
                }
            }
        }
        else if(currentTurn == TurnManager.Turn.Enemy)
        {
            foreach(var unit in allUnits)
            {
                if (unit.unitType == UnitType.Player)
                {
                    unit.curOtherUnitTypeDistance = ManhattanDistance(activeUnit.v2newPos, unit.v2newPos);
                }
            }
        }
        
    }

    public bool CheckDistanceOfEnemy()//ユニット同士の距離が1かどうか(攻撃できるか）に変更
    {
        CalculateDistanceOfOtherTypeUnit(TurnManager.turnInstance.currentTurn);

        if(TurnManager.turnInstance.currentTurn == TurnManager.Turn.Player)
        {
            foreach (var unit in allUnits)
            {
                if (unit.unitType==UnitType.Enemy && unit.curOtherUnitTypeDistance == 1)
                {
                    passiveUnit = unit;
                    return true;
                }
            }
            return false;
        }
        else if(TurnManager.turnInstance.currentTurn == TurnManager.Turn.Enemy)
        {
            foreach(var unit in allUnits)
            {
                if(unit.unitType == UnitType.Player && unit.curOtherUnitTypeDistance == 1)
                {
                    passiveUnit = unit;
                    return true;
                }
            }
            return false;
        }
        else
        {
            Debug.Log("想定外");
            return false;
        }

        
    }

    public int ManhattanDistance(Vector2Int v2Pos, Vector2Int otherv2Pos)
    {
        int d = Mathf.Abs(v2Pos.x - otherv2Pos.x)
                + Mathf.Abs(v2Pos.y - otherv2Pos.y);

        return d;
    }

    

    
}
