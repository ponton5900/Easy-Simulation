using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class TileManager : MonoBehaviour
{
    
    public static TileManager instance;
    public int width = 9;
    public int height = 9;

    public Tilemap groundTilemap;
    public Tilemap rangeTilemap;
    public TileBase redTile;
    public TileBase blueTile;

    private bool[,] visited;



    readonly private Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    private struct SearchNode
    {
        public Vector2Int v2currentPos;
        public int moveStep;//総移動量
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        visited = new bool[width,height];

    }

    public Vector3 cellToWorld(Vector2Int cellPos,Tilemap tilemap)
    {
        Vector3Int v3Int = new Vector3Int(cellPos.x, cellPos.y, 0);
        Vector3 worldPos = tilemap.GetCellCenterWorld(v3Int);
        return worldPos;
    }

    public bool CanMove(Vector2Int newPos, Vector2Int currentPos,Unit currentUnit)//違うユニットが要る場合通れないことも考慮
    {
        Vector2Int movement = newPos - currentPos;

        if (newPos.x >= 0 && newPos.x < width
            && newPos.y >= 0 && newPos.y < height
            && Mathf.Abs(movement.x) + Mathf.Abs(movement.y) <= currentUnit.moveRange)
        {
            if (!IsOccupied(newPos,currentUnit)) return true;
            else return false;
        }
        else
        {
            return false;
        }
    }

    public bool IsOccupied(Vector2Int newPos,Unit currentUnit)//既にそのマスに誰かいるか
    {
        foreach (var unit in UnitManager.unitManagerInstance.allUnits)
        {
            if (unit == currentUnit) continue;
            if (newPos == unit.v2currentPos)
            {
                return true;
            }
        }
        return false;
    }

    public Vector2Int GetPlayerPosition()//敵ターン時にプレイヤーのポジションを特定するため②の途中
    {
        UnitManager.unitManagerInstance.CalculateDistanceOfOtherTypeUnit(TurnManager.turnInstance.currentTurn);
        int min = (width - 1) + (height - 1);
        Unit bestNearPlayerUnit = null;
        foreach (var unit in UnitManager.unitManagerInstance.allUnits)
        {
            if (unit.unitType == UnitType.Player && unit.curOtherUnitTypeDistance < min)
            {
                min = unit.curOtherUnitTypeDistance;
                bestNearPlayerUnit = unit;
            }
        }
        return bestNearPlayerUnit.v2currentPos;//見つからない場合
    }

    //移動可能な範囲のマスをすべてリストに格納
    public List<Vector2Int> GetTileBfs(Vector2Int v2currentPos, int moveRange,Unit currentUnit)//移動力①
    {
        System.Array.Clear(visited, 0, visited.Length);//Lengthは要素数の合計
        Queue<SearchNode> queue = new Queue<SearchNode>();
        List<Vector2Int> walkableTiles = new List<Vector2Int>();

        SearchNode newNode;
        newNode.v2currentPos = v2currentPos;
        newNode.moveStep = 0;
        queue.Enqueue(newNode);
        walkableTiles.Add(newNode.v2currentPos);//今の移動前のマスも入れるのでリストには何か必ず入る
        visited[v2currentPos.x, v2currentPos.y] = true;//trueが訪問済みを指す

        while (queue.Count > 0)
        {
            SearchNode currentNode = queue.Dequeue();
            foreach (var dir in directions)
            {
                Vector2Int v2newPos = currentNode.v2currentPos + dir;
                if (CanMove(v2newPos, currentNode.v2currentPos,currentUnit) && currentNode.moveStep < moveRange && !visited[v2newPos.x, v2newPos.y])
                {
                    newNode.v2currentPos = v2newPos;
                    newNode.moveStep = currentNode.moveStep+1;
                    queue.Enqueue(newNode);
                    walkableTiles.Add(v2newPos);
                }
            }
        }

        return walkableTiles;
    }

    public Vector2Int FindBestTiles(List<Vector2Int> walkableTiles, Vector2Int v2playerPos)//② 
    {
        int min = (width - 1) + (height - 1);//今のマップで最大の移動距離を入れておく
        Vector2Int v2bestEnemyPos = Vector2Int.zero;
        foreach (var v2enemyPos in walkableTiles)
        {
            int d = Mathf.Abs(v2enemyPos.x - v2playerPos.x) + Mathf.Abs(v2enemyPos.y - v2playerPos.y);
            if (min > d)
            {
                min = d;
                v2bestEnemyPos = v2enemyPos;
            }
        }
        return v2bestEnemyPos;
    }

    //移動可能範囲表示
    public void ShowRange(List<Vector2Int> walkableTiles,Vector2Int center,int range,TurnManager.Turn turn)//
    {
        rangeTilemap.ClearAllTiles();
        List<Vector3Int> v3walkableTiles = new List<Vector3Int>();
        foreach(var tile in walkableTiles)
        {
            v3walkableTiles.Add((Vector3Int)tile);
        }

        if (turn == TurnManager.Turn.Player)
        {
            foreach(var v3tile in v3walkableTiles)
            {
                rangeTilemap.SetTile(v3tile, blueTile);
            }
        }
        else if(turn== TurnManager.Turn.Enemy)
        {
            foreach (var v3tile in v3walkableTiles)
            {
                rangeTilemap.SetTile(v3tile, redTile);
            }
        }  


    }
    public void ClearRange()
    {
        rangeTilemap.ClearAllTiles();
    }

    //マスの外のところは移動可能範囲表示しない
    public bool inMap(Vector3Int pos)
    {
        if(pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
