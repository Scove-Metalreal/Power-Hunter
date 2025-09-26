using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    private float targetHealth;
    public float HeathPlayer;
    [Header("UI")]
    public Slider HeathSilder;
    public TextMeshProUGUI HeathText;
    

    
    void Start()
    {
        HeathPlayer = 100;
        targetHealth = HeathPlayer;
        HeathSilder.maxValue = HeathPlayer;
        HeathSilder.value = HeathPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        HeathSilder.value = Mathf.MoveTowards(HeathSilder.value, targetHealth, 25f * Time.unscaledDeltaTime);
        HeathText.text = $"{System.Math.Round(HeathSilder.value, 2)}%";
    }
    public void TakeDamage(float damage)
    {
        HeathPlayer -= damage;
        if (HeathPlayer < 0) HeathPlayer = 0;

        
        targetHealth = HeathPlayer;
    }
}
