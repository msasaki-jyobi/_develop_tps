using develop_tps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace develop_common
{
    public class ActionSample : MonoBehaviour
    {
        [SerializeField] private develop_tps.InputReader _inputReader;
        [SerializeField] private UnitActionLoader _unitActionLoader;

        [SerializeField] private List<GameObject> _actionDataR1;
        [SerializeField] private List<GameObject> _actionDataR2;
        [SerializeField] private List<GameObject> _actionDataL1;
        [SerializeField] private List<GameObject> _actionDataL2;
        [SerializeField] private List<GameObject> _actionDataCross;
        [SerializeField] private List<GameObject> _actionDataSquare;
        [SerializeField] private List<GameObject> _actionDataTriangle;
        [SerializeField] private List<GameObject> _actionDataCircle;



        public int DownValue = 0;

        private void Start()
        {
            _unitActionLoader.StartAdditiveParameterEvent += OnStartAdditiveParameterHandle;
            _unitActionLoader.FinishAdditiveParameterEvent += OnFinishAdditiveParameterHandle;

            _inputReader.MoveEvent += OnMoveHandle;
            _inputReader.PrimaryR1Event += OnR1Handle;
            _inputReader.PrimaryR2Event += OnR2Handle;
            _inputReader.PrimaryL1Event += OnL1Handle;
            _inputReader.PrimaryL2Event += OnL2Handle;
            _inputReader.PrimaryActionCrossEvent += OnCrossHandle;
            _inputReader.PrimaryActionSquareEvent += OnSquareHandle;
            _inputReader.PrimaryActionTriangleEvent += OnTriangleHandle;
            _inputReader.PrimaryActionCircleEvent += OnCircleHandle;
        }

        private void OnCircleHandle(bool arg1, EInputReader reader)
        {
            if(arg1)
            {
                foreach (var ac in _actionDataCircle)
                    _unitActionLoader.LoadAction(ac, reader);
            }
        }

        private void OnTriangleHandle(bool arg1, EInputReader reader)
        {
            if (arg1)
            {
                foreach (var ac in _actionDataTriangle)
                    _unitActionLoader.LoadAction(ac, reader);
            }
        }

        private void OnSquareHandle(bool arg1, EInputReader reader)
        {
            if (arg1)
            {
                foreach (var ac in _actionDataSquare)
                    _unitActionLoader.LoadAction(ac, reader);
            }
        }

        private void OnCrossHandle(bool arg1, EInputReader reader)
        {
            if (arg1)
            {
                foreach (var ac in _actionDataCross)
                    _unitActionLoader.LoadAction(ac, reader);
            }
        }

        private void OnL2Handle(bool arg1, EInputReader reader)
        {
            if (arg1)
            {
                foreach (var ac in _actionDataL2)
                    _unitActionLoader.LoadAction(ac, reader);
            }
        }

        private void OnL1Handle(bool arg1, EInputReader reader)
        {
            if (arg1)
            {
                foreach (var ac in _actionDataL1)
                    _unitActionLoader.LoadAction(ac, reader);
            }
        }

        private void OnR2Handle(bool arg1, EInputReader reader)
        {
            if (arg1)
            {
                foreach (var ac in _actionDataR2)
                    _unitActionLoader.LoadAction(ac, reader);
            }
        }

        private void OnR1Handle(bool arg1, EInputReader reader)
        {
            if (arg1)
            {
                foreach (var ac in _actionDataR1)
                    _unitActionLoader.LoadAction(ac, reader);
            }
        }

        private void Update()
        {

            //if (Input.GetKeyDown(KeyCode.C))
            //{
            //    _unitActionLoader.LoadAction(_actionDataC, EInputReader.R1);
            //}

            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    _unitActionLoader.LoadAction(_actionDataX, EInputReader.R2);
            //}
            //if (Input.GetKeyDown(KeyCode.Z))
            //{
            //    foreach (var ac in _actionDatasZ)
            //        _unitActionLoader.LoadAction(ac, EInputReader.Square);
            //}
        }

        private void OnMoveHandle(Vector2 movement)
        {
            // 移動できるバグの原因
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
                    Debug.Log($"{parameterName}, {value} Hello Start！");
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
                    Debug.Log($"{parameterName}, {value} Get Finish！");
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

        // Animatorでよくあるモーション用関数
        private void FootR()
        {

        }
        // Animatorでよくあるモーション用関数
        private void FootL()
        {

        }
        private void Hit()
        {

        }
    }
}


