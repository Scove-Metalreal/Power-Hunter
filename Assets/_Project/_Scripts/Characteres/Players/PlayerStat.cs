using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    private float targetHealth;
    public float HeathPlayer;
    public float StaminaPlayer;
    private float targetStamina;
    [Header("UI")]
    public Slider HeathSilder;
    public TextMeshProUGUI HeathText;
    public Slider StaminaSlider;

    
    void Start()
    {
        StaminaPlayer = 100;
        HeathPlayer = 100;
        targetStamina = StaminaPlayer;
        targetHealth = HeathPlayer;
        StaminaSlider.maxValue = StaminaPlayer;
        HeathSilder.maxValue = HeathPlayer;
        HeathSilder.value = HeathPlayer;
        StaminaSlider.value = StaminaPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        HeathSilder.value = Mathf.MoveTowards(HeathSilder.value, targetHealth, 25f * Time.unscaledDeltaTime);
        HeathText.text = $"{Math.Round(HeathSilder.value, 2)}%";

        if (StaminaPlayer < StaminaSlider.maxValue)
        {
            StaminaPlayer += 10f * Time.unscaledDeltaTime; 
            if (StaminaPlayer > StaminaSlider.maxValue)
                StaminaPlayer = StaminaSlider.maxValue;

            targetStamina = StaminaPlayer;
        }
        if (targetStamina < StaminaSlider.value)
        {
            StaminaSlider.value = Mathf.MoveTowards(StaminaSlider.value, targetStamina, 25f * Time.unscaledDeltaTime);
        }
        else
        {
            StaminaSlider.value = Mathf.MoveTowards(StaminaSlider.value, targetStamina, 10f * Time.unscaledDeltaTime);
        }
    }

    public void UseStamina(float enegy)
    {
        StaminaPlayer -= enegy;
        if(StaminaPlayer <0) {  StaminaPlayer = 0; }

        targetStamina = StaminaPlayer;
    }
    public void TakeDamage(float damage)
    {
        HeathPlayer -= damage;
        if (HeathPlayer < 0) HeathPlayer = 0;

        
        targetHealth = HeathPlayer;
    }
}
