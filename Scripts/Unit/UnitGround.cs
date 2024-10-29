using develop_common;
using System.Collections;
using UnityEngine;

namespace develop_tps
{
    public class UnitGround : MonoBehaviour
    {
        [Header("LineDatas")]
        [SerializeField] private LineData _groundLineData;

        // Ground Check
        public bool CanJump { private set; get; }

        private void Update()
        {
            CheckGround();
        }

        private void CheckGround()
        {
            CanJump = UtilityFunction.CheckLineData(_groundLineData, transform);
        }

    }
}