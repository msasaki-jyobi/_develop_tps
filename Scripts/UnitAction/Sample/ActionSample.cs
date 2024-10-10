using develop_common;
using develop_tps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace develop_common
{ 
    public class ActionSample : MonoBehaviour
    {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private UnitActionLoader _unitActionLoader;

        [SerializeField] private GameObject _actionDataC;
        [SerializeField] private GameObject _actionDataX;

        public int DownValue = 0;

        private void Start()
        {
            _unitActionLoader.StartAdditiveParameterEvent += OnStartAdditiveParameterHandle;
            _unitActionLoader.FinishAdditiveParameterEvent += OnFinishAdditiveParameterHandle;

            _inputReader.MoveEvent += OnMoveHandle;
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.C))
            {
                _unitActionLoader.LoadAction(_actionDataC);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                _unitActionLoader.LoadAction(_actionDataX);
            }
        }

        private void OnMoveHandle(Vector2 movement)
        {
            // �ړ��ł���o�O�̌���
            return;
            //if (Mathf.Abs(movement.x + movement.y) == 1)
            //{
            //    DownValue--;
            //    if (DownValue <= 0)
            //        if (TryGetComponent<TPSUnitController>(out var controller))
            //            controller.ChangeDisableInputControl(false);
            //}
        }

        private void OnStartAdditiveParameterHandle(string parameterName, int value)
        {
            switch (parameterName)
            {
                case "Kousoku":
                    Debug.Log($"{parameterName}, {value} Hello Start�I");
                    if (TryGetComponent<AnimatorStateController>(out var controller))
                    {
                        //controller.StatePlay("State1", EStatePlayType.SinglePlay, false);
                    }
                    if (TryGetComponent<TPSUnitController>(out var controller2))
                    {
                        DownValue = value;
                        //controller2.ChangeDisableInputControl(true);
                    }
                    break;
            }
        }
        private void OnFinishAdditiveParameterHandle(string parameterName, int value)
        {
            switch (parameterName)
            {
                case "Kousoku":
                    Debug.Log($"{parameterName}, {value} Get Finish�I");
                    if (TryGetComponent<AnimatorStateController>(out var controller0))
                    {
                        //controller0.StatePlay("State1", EStatePlayType.SinglePlay, false);
                    }
                    if (TryGetComponent<TPSUnitController>(out var controller))
                    {
                        DownValue = value;
                        //controller.ChangeDisableInputControl(true);
                    }
                    break;
            }
        }
    }
}


