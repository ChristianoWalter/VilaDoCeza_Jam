using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    // Variável externa referente ao slider da barra de vida
    [SerializeField] Slider slider;

    // Método destinado à função de configurar os valores do slider
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    // Método destinado a atualizar os valores da vida pelo slider
    public void SetHealth(float health)
    {
        slider.value = health;
    }
}
