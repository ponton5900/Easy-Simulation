using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    //グリッド上での座標
    public Vector2Int gridPos;

    //例えばユニットがいるかどうかを管理
    public GameObject unitOnTile;

    //ハイライト用
    private SpriteRenderer spriteRenderer;
    private Color defaultColor;

    private TextMeshPro text;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;

        //子オブジェクトにTextMeshProを作る
        text=GetComponentInChildren<TextMeshPro>();
        
    }

    private void Start()
    {
        if (text != null)
        {
            text.text = $"({gridPos.x},{gridPos.y})";
        }
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        defaultColor = color;//リセット用にも反映
    }
    public void Highlight(Color color)
    {
        spriteRenderer.color = color;
    }

    public void ResetHighlight()
    {
        spriteRenderer.color = defaultColor;
    }
}
