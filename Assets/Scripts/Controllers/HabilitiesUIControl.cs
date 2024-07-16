using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HabilitiesUIControl : MonoBehaviour
{
    public Image habilityFill;

    public void UpdateFill(float _fillAmount)
    {
        habilityFill.fillAmount = _fillAmount;
    }
}
