using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    // Vari�vel externa referente ao slider da barra de vida
    [SerializeField] Slider slider;

    // M�todo destinado � fun��o de configurar os valores do slider
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    // M�todo destinado a atualizar os valores da vida pelo slider
    public void SetHealth(float health)
    {
        slider.value = health;
    }
}
