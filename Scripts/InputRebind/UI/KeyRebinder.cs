using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace develop_tps
{

    public class KeyRebinder : MonoBehaviour
    {
        public RebindManager RebindManager;
        public string ActionName;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(StartRebinding);
        }

        private void StartRebinding()
        {
            RebindManager.StartRebind(ActionName);
        }
    }
}