using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace develop_tps
{
    [Serializable]
    public class WaveInfo
    {
        public float DelayTime = 5f;
        public List<Transform> CreatePositions = new List<Transform>();
        public List<GameObject> CreatePrefabs = new List<GameObject>();
    }
}