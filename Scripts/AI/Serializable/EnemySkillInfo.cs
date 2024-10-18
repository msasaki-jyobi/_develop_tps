using System;
using System.Collections;
using UnityEngine;

namespace develop_tps
{
    [Serializable]
    public class EnemySkillInfo
    {
        public float Distance; // 発動可能距離
        public GameObject SkillAction; // 発動技
    }
}