using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputReader_TPS;
using static UnityEngine.InputSystem.DefaultInputActions;
using System.Linq;
using UnityEngine.InputSystem.Utilities;

namespace develop_tps
{
    public enum EInputReader
    {
        None,
        R1,
        R2,
        L1,
        L2,
        Cross,
        Square,
        Triangle,
        Circle,
        Start,
        Select,
        TouchPad,
        Home,
        DPadDown,
        DPadLeft,
        DPadUp,
        DPadRight,
        LeftThumb,
        RightThumb,
        MoveForward,
        MoveBackward,
        MoveLeft,
        MoveRight,
    }


    [CreateAssetMenu(fileName = "New Input TPS Reader", menuName = "Input/Input Reader (TPS)")]
    public class InputReader : ScriptableObject, InputReader_TPS.IPlayerActions
    {
        // Event
        public event Action<Vector2> MoveEvent; // 移動値
        public event Action<Vector2> LookEvent;

        public event Action<bool, EInputReader> PrimaryR1Event; // 射撃用
        public event Action<bool, EInputReader> PrimaryR2Event; // Aim
        public event Action<bool, EInputReader> PrimaryL1Event; 
        public event Action<bool, EInputReader> PrimaryL2Event;
        public event Action<bool, EInputReader> PrimaryActionCrossEvent; 
        public event Action<bool, EInputReader> PrimaryActionSquareEvent;
        public event Action<bool, EInputReader> PrimaryActionTriangleEvent;
        public event Action<bool, EInputReader> PrimaryActionCircleEvent;
        public event Action<bool, EInputReader> PrimaryActionStartEvent;
        public event Action<bool, EInputReader> PrimaryActionSelectEvent;
        public event Action<bool, EInputReader> PrimaryActionTouchPadEvent;
        public event Action<bool, EInputReader> PrimaryActionHomeEvent;
        public event Action<bool, EInputReader> PrimaryActionDPadDownEvent;
        public event Action<bool, EInputReader> PrimaryActionDPadLeftEvent;
        public event Action<bool, EInputReader> PrimaryActionDPadUpEvent;
        public event Action<bool, EInputReader> PrimaryActionDPadRightEvent;
        public event Action<bool, EInputReader> PrimaryActionLeftThumbEvent;
        public event Action<bool, EInputReader> PrimaryActionRightThumbEvent;

        // 参照用
        public float ScrollY { get; private set; }

        private InputReader_TPS _control;

        public event Action<string> OnChangeDevice;
        private string _lastInputDevice;

        private void OnEnable()
        {
            if (_control == null)
            {
                _control = new InputReader_TPS();
                _control.Player.SetCallbacks(this);
            }

            _control.Player.Enable();
            InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
        }
        private void OnAnyButtonPress(InputControl control)
        {
            if (control.device is Gamepad)
            {
                _lastInputDevice = "Gamepad";
            }
            else if (control.device is Keyboard)
            {
                _lastInputDevice = "Keyboard";
            }
            else if (control.device is Mouse)
            {
                _lastInputDevice = "Mouse";
            }
            else
            {
                _lastInputDevice = control.device.name;
            }
            OnChangeDevice?.Invoke(_lastInputDevice);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookEvent?.Invoke(context.ReadValue<Vector2>());
        }
        //public void OnFire(InputAction.CallbackContext context)
        //{
        //    if (context.performed)
        //        PrimaryFireEvent?.Invoke(true);
        //    else if (context.canceled)
        //        PrimaryFireEvent?.Invoke(false);
        //}
        ////public void OnAim(InputAction.CallbackContext context)
        ////{
        ////    AimPosition = context.ReadValue<Vector2>(); // マウスの座標を取得
        ////}
        //public void OnJump(InputAction.CallbackContext context)
        //{
        //    if (context.started)
        //    {
        //        StartedJumpEvent?.Invoke();
        //        PrimaryJumpEventEvent?.Invoke(true);
        //    }
        //    else if (context.canceled)
        //        PrimaryJumpEventEvent?.Invoke(false);
        //}
        //public void OnDash(InputAction.CallbackContext context)
        //{
        //    if (context.performed)
        //        PrimaryDashEvent?.Invoke(true);
        //    else if (context.canceled)
        //        PrimaryDashEvent?.Invoke(false);
        //}
        //public void OnWheel(InputAction.CallbackContext context)
        //{
        //    ScrollY = context.ReadValue<Vector2>().y;
        //}

        //public void OnSubFire(InputAction.CallbackContext context)
        //{
        //    if (context.performed)
        //        PrimarySubFireEvent?.Invoke(true);
        //    else if (context.canceled)
        //        PrimarySubFireEvent?.Invoke(false);
        //}

        //public void OnRFire(InputAction.CallbackContext context)
        //{
        //    if (context.started)
        //        StartedRFireEvent?.Invoke();
        //}

        //public void OnOpenItem(InputAction.CallbackContext context)
        //{
        //    if (context.started)
        //        StartedOpenItemEvent?.Invoke();
        //}

        // キーバインド用
        // アクション名に基づいてアクションを取得するメソッド
        public InputAction GetActionByName(string actionName)
        {
            switch (actionName)
            {
                case "Move":
                    return _control.Player.Move;
                case "Look":
                    return _control.Player.Look;
       
                default:
                    throw new ArgumentException($"Action {actionName} not found");
            }
        }
        public void DisableInput()
        {
            _control.Player.Disable();
        }

        public void EnableInput()
        {
            _control.Player.Enable();
        }
        // すべてのアクションを取得するメソッド
        public IEnumerable<InputAction> GetAllActions()
        {
            return _control.asset.FindActionMap("Player").actions;
        }
        // 入力デバイスを判定するメソッド
        public string GetCurrentInputDevice()
        {
            return _lastInputDevice ?? "Unknown";
        }

        public void OnR1(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryR1Event?.Invoke(true, EInputReader.R1);
            else if (context.canceled)
                PrimaryR1Event?.Invoke(false, EInputReader.R1);
        }

        public void OnR2(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryR2Event?.Invoke(true, EInputReader.R2);
            else if (context.canceled)
                PrimaryR2Event?.Invoke(false, EInputReader.R2);
        }

        public void OnL1(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryL1Event?.Invoke(true, EInputReader.L1);
            else if (context.canceled)
                PrimaryL1Event?.Invoke(false, EInputReader.L1);
        }

        public void OnL2(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryL2Event?.Invoke(true, EInputReader.L2);
            else if (context.canceled)
                PrimaryL2Event?.Invoke(false, EInputReader.L2);
        }

        public void OnActonCross(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionCrossEvent?.Invoke(true,EInputReader.Cross);
            else if (context.canceled)
                PrimaryActionCrossEvent?.Invoke(false, EInputReader.Cross);
        }

        public void OnActionSquare(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionSquareEvent?.Invoke(true,EInputReader.Square);
            else if (context.canceled)
                PrimaryActionSquareEvent?.Invoke(false, EInputReader.Square);
        }

        public void OnActionTriangle(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionTriangleEvent?.Invoke(true, EInputReader.Triangle);
            else if (context.canceled)
                PrimaryActionTriangleEvent?.Invoke(false, EInputReader.Triangle);
        }

        public void OnActionCircle(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionCircleEvent?.Invoke(true, EInputReader.Circle);
            else if (context.canceled)
                PrimaryActionCircleEvent?.Invoke(false, EInputReader.Circle);
        }

        public void OnStart(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionStartEvent?.Invoke(true, EInputReader.Start);
            else if (context.canceled)
                PrimaryActionStartEvent?.Invoke(false, EInputReader.Start);
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionSelectEvent?.Invoke(true, EInputReader.Select);
            else if (context.canceled)
                PrimaryActionSelectEvent?.Invoke(false, EInputReader.Select);
        }

        public void OnTouchPad(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionTouchPadEvent?.Invoke(true, EInputReader.TouchPad);
            else if (context.canceled)
                PrimaryActionTouchPadEvent?.Invoke(false, EInputReader.TouchPad);
        }

        public void OnHome(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionHomeEvent?.Invoke(true, EInputReader.Home);
            else if (context.canceled)
                PrimaryActionHomeEvent?.Invoke(false, EInputReader.Home);
        }

        public void OnDPadDown(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionDPadDownEvent?.Invoke(true, EInputReader.DPadDown);
            else if (context.canceled)
                PrimaryActionDPadDownEvent?.Invoke(false, EInputReader.DPadDown);
        }

        public void OnDPadLeft(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionDPadLeftEvent?.Invoke(true, EInputReader.DPadLeft);
            else if (context.canceled)
                PrimaryActionDPadLeftEvent?.Invoke(false, EInputReader.DPadLeft);
        }

        public void OnDPadUp(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionDPadUpEvent?.Invoke(true, EInputReader.DPadUp);
            else if (context.canceled)
                PrimaryActionDPadUpEvent?.Invoke(false, EInputReader.DPadUp);
        }

        public void OnDPadRight(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionDPadRightEvent?.Invoke(true, EInputReader.DPadRight);
            else if (context.canceled)
                PrimaryActionDPadRightEvent?.Invoke(false, EInputReader.DPadRight);
        }

        public void OnLeftThumb(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionLeftThumbEvent?.Invoke(true, EInputReader.LeftThumb);
            else if (context.canceled)
                PrimaryActionLeftThumbEvent?.Invoke(false, EInputReader.LeftThumb);
        }

        public void OnRightThumb(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryActionRightThumbEvent?.Invoke(true, EInputReader.RightThumb);
            else if (context.canceled)
                PrimaryActionRightThumbEvent?.Invoke(false, EInputReader.RightThumb);
        }
    }
}
