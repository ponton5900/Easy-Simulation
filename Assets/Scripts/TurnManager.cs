using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager turnInstance;//ターン変数参照のため 

    public enum Turn { Player,Enemy,Transition}
    public Turn currentTurn = Turn.Enemy;
    private Unit unit;
    [HideInInspector] public Image img;

    private void Awake()
    {
        if (turnInstance == null) turnInstance = this;
        else Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void ChangeUnit(Unit unit)//味方ターンの別のユニットまたは敵ターンの別の敵ユニットに切り替えの時
    {
        if (currentTurn == Turn.Player)
        {
            SoundManager.Instance.PlayTurnSound(SoundManager.Instance.playerTurnClip);
            currentTurn = Turn.Transition;
            img.color = new Color32(22, 129, 225, 100);//2Dの場合元々青色っぽい背景ある
            TurnUI.Instance.SameTurnShowTurn("Player Turn", Turn.Player,unit);

        }
        else if (currentTurn == Turn.Enemy)
        {
            SoundManager.Instance.PlayTurnSound(SoundManager.Instance.enemyTurnClip);
            currentTurn = Turn.Transition;
            img.color = new Color32(241, 22, 77, 100);//2Dの場合元々青色っぽい背景ある
            TurnUI.Instance.SameTurnShowTurn("Enemy Turn", Turn.Enemy,unit);

        }
    }
    public void NextTurn()
    {
        if (currentTurn == Turn.Player)
        {
            SoundManager.Instance.PlayTurnSound(SoundManager.Instance.enemyTurnClip);
            currentTurn = Turn.Transition;
            img.color = new Color32(241, 22, 77, 100);//2Dの場合元々青色っぽい背景ある
            TurnUI.Instance.ShowTurn("Enemy Turn",Turn.Enemy);
            
        }
        else if(currentTurn == Turn.Enemy)
        {
            SoundManager.Instance.PlayTurnSound(SoundManager.Instance.playerTurnClip);
            currentTurn = Turn.Transition;
            img.color = new Color32(22, 129, 225, 100);//2Dの場合元々青色っぽい背景ある
            TurnUI.Instance.ShowTurn("Player Turn",Turn.Player);
            
        }

        Debug.Log("Current Turn: " + currentTurn);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (currentTurn == Turn.Transition) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(UnitActionUI.Instance.waitSelect());
        }
    }
}
