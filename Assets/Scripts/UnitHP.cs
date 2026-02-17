using UnityEngine;
using UnityEngine.UI;

public class UnitHP : MonoBehaviour
{
    public Slider hpSlider;
    public float HP = 100f;

    public void SetMaxHP(float maxHP)
    {
        hpSlider.maxValue = maxHP;
        hpSlider.value = maxHP;
    }

    public void SetHP(float currentHP)
    {
        hpSlider.value = currentHP;
    }

    public void attackHP(float attack)
    {
        HP -= attack;
        hpSlider.value = HP;

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetMaxHP(HP);
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
