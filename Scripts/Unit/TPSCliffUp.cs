using develop_common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCliffUp : MonoBehaviour
{
    [Header("LineDatas")]
    [SerializeField] private LineData _wallLineData;
    [SerializeField] private LineData _cliffLineData;

    // Wall Cliff Check
    public bool IsWall { private set; get; }
    public bool IsCliff { private set; get; }

    public bool CheckCliffUp()
    {
        if (_cliffLineData != null)
        {
            IsWall = UtilityFunction.CheckLineData(_wallLineData, transform);
            IsCliff = UtilityFunction.CheckLineData(_cliffLineData, transform);
        }

        return IsWall && !IsCliff;
    }
}
