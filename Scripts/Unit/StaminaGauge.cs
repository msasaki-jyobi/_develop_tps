using develop_tps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaGauge : MonoBehaviour
{
    public TPSUnitController TPSUnitController;
    public Image StaminaImage;

    private void Start()
    {
        TPSUnitController.StaminaUpdateEvent += OnUpdateStaminaHandle;
    }

    private void Update()
    {
    }

    private void OnUpdateStaminaHandle(float value, float max)
    {
        StaminaImage.fillAmount = value / max;

    }
}
