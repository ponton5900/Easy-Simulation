using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum UnitType { Player,Enemy}

public class Unit : MonoBehaviour
{
    //自軍or敵軍
    public Tile currentTile;//どのマスにいるか

    public UnitType unitType;
    public UnitState unitState = UnitState.Wait;

    private Tilemap groundTilemap;

    public Vector3 v3currentPos;
    public Vector2Int v2currentPos;//セル座標
    public Vector2Int v2newPos;//ターンを終えるまでの移動中のセル座標
    public float attack = 50f;
    public int moveRange;
    public int curOtherUnitTypeDistance;
    

    

    //移動関数
    public void PlaceOnTile(Tile tile)
    {
        currentTile = tile;
        transform.position = tile.transform.position+new Vector3(0,0,-2);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        groundTilemap = TileManager.instance.groundTilemap;
        

  
    }

    // Update is called once per frame
    void Update()
    {
        if (UnitActionUI.Instance.isSelecting || TurnManager.turnInstance.currentTurn == TurnManager.Turn.Transition) return;

        if (unitState == UnitState.Active)
        {
            if (TurnManager.turnInstance.currentTurn == TurnManager.Turn.Player && unitType == UnitType.Player)
            {
                HandlePlayerInput();

            }
            if (TurnManager.turnInstance.currentTurn == TurnManager.Turn.Enemy && unitType == UnitType.Enemy)
            {
                List<Vector2Int> walkableTiles = TileManager.instance.GetTileBfs(v2currentPos, moveRange, this);
                Vector2Int v2playerPos = TileManager.instance.GetPlayerPosition();
                Vector2Int bestEnemyTile = TileManager.instance.FindBestTiles(walkableTiles, v2playerPos);//移動可能なマスとプレイヤーの場所を渡す

                foreach (var tiles in walkableTiles)
                {
                    Debug.Log(tiles);
                }
                Debug.Log(walkableTiles.Count);
                Debug.Log(v2playerPos);
                transform.position = groundTilemap.GetCellCenterWorld((Vector3Int)bestEnemyTile);
                v2currentPos = bestEnemyTile;
                v2newPos = bestEnemyTile;
                Debug.Log(bestEnemyTile);
                if (UnitManager.unitManagerInstance.CheckDistanceOfEnemy())//攻撃処理
                {
                    Unit attacker = UnitManager.unitManagerInstance.activeUnit;
                    Unit target = UnitManager.unitManagerInstance.passiveUnit;
                    target.GetComponent<UnitHP>().attackHP(attacker.attack);
                }

                //ここからターン遷移の準備
                this.unitState = UnitState.Finished;
                TurnManager.Turn currentTurn = TurnManager.turnInstance.currentTurn;
                Unit nextUnit = UnitManager.unitManagerInstance.NextActiveUnit(currentTurn);
                if (null != nextUnit){
                    TurnManager.turnInstance.ChangeUnit(nextUnit);
                }
                else
                {
                    UnitManager.unitManagerInstance.ChangetoWait();
                    TurnManager.turnInstance.NextTurn();
                }
                
            }
            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(UnitActionUI.Instance.waitSelect());
            }*/
        }

    }
    void HandlePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryMove(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TryMove(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryMove(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryMove(Vector2Int.right);
        }
    }

    void TryMove(Vector2Int dir)
    {

        if (TileManager.instance.CanMove(v2newPos+dir,v2currentPos,this))
        {
            v2newPos = v2newPos+dir;
            Vector3 v3newPos = groundTilemap.GetCellCenterWorld((Vector3Int)v2newPos);
            transform.position = v3newPos;
            //TileManager.instance.ShowRange(currentPos, 1,TurnManager.turnInstance.currentTurn);
            
        }
        
    }

    

    

     

    





}
