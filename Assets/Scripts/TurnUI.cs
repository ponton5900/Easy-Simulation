using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TurnUI : MonoBehaviour
{
    public static TurnUI Instance;
    public TextMeshProUGUI turnText;
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ShowTurn(string text,TurnManager.Turn current)
    {
        turnText.text = text;
        StartCoroutine(FadeRoutine(current));
    }

    private IEnumerator FadeRoutine(TurnManager.Turn current)
    {
        //フェードイン
        for(float t = 0; t < 1; t += Time.deltaTime)
        {
            canvasGroup.alpha = t;
            yield return null;
        }
        canvasGroup.alpha = 1;

        UnitManager.unitManagerInstance.NextActiveUnit(current);
        Unit unit = UnitManager.unitManagerInstance.activeUnit;
        
        //★分けなくても行けるが分かりやすさのため
        if (current == TurnManager.Turn.Player)
        {
            Vector2Int v2playerPos = unit.v2currentPos;
            int moveRange = unit.moveRange;
            List<Vector2Int> walkableTiles = TileManager.instance.GetTileBfs(v2playerPos, moveRange, unit);
            TileManager.instance.ShowRange(walkableTiles,v2playerPos,moveRange,current);
        }
        else if (current == TurnManager.Turn.Enemy)
        {
            Vector2Int v2enemyPos = unit.v2currentPos;
            int moveRange = unit.moveRange;
            List<Vector2Int> walkableTiles = TileManager.instance.GetTileBfs(v2enemyPos, moveRange, unit);
            TileManager.instance.ShowRange(walkableTiles,v2enemyPos,moveRange,current);
        }

        yield return new WaitForSeconds(1f);//表示時間

        //フェードアウト
        for(float t = 1; t > 0; t -= Time.deltaTime)
        {
            canvasGroup.alpha = t;
            yield return null;
        }
        canvasGroup.alpha = 0;
        TurnManager.turnInstance.currentTurn = current;
        
    }

    public void SameTurnShowTurn(string text, TurnManager.Turn current,Unit unit)
    {
        turnText.text = text;
        StartCoroutine(SameTurnFadeRoutine(current,unit));
    }

    private IEnumerator SameTurnFadeRoutine(TurnManager.Turn current,Unit unit)
    {
        //フェードイン
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            canvasGroup.alpha = t;
            yield return null;
        }
        canvasGroup.alpha = 1;

        //★分けなくても行けるが分かりやすさのため
        if (current == TurnManager.Turn.Player)
        {
            Vector2Int v2playerPos = unit.v2currentPos;
            int moveRange = unit.moveRange;
            List<Vector2Int> walkableTiles = TileManager.instance.GetTileBfs(v2playerPos, moveRange, unit);
            TileManager.instance.ShowRange(walkableTiles, v2playerPos, moveRange, current);
        }
        else if (current == TurnManager.Turn.Enemy)
        {
            Vector2Int v2enemyPos = unit.v2currentPos;
            int moveRange = unit.moveRange;
            List<Vector2Int> walkableTiles = TileManager.instance.GetTileBfs(v2enemyPos, moveRange, unit);
            TileManager.instance.ShowRange(walkableTiles, v2enemyPos, moveRange, current);
        }

        yield return new WaitForSeconds(1f);//表示時間

        //フェードアウト
        for (float t = 1; t > 0; t -= Time.deltaTime)
        {
            canvasGroup.alpha = t;
            yield return null;
        }
        canvasGroup.alpha = 0;
        TurnManager.turnInstance.currentTurn = current;

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
