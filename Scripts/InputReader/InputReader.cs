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
    [CreateAssetMenu(fileName = "New Input TPS Reader", menuName = "Input/Input Reader (TPS)")]
    public class InputReader : ScriptableObject, InputReader_TPS.IPlayerActions
    {
        // Event
        public event Action<Vector2> MoveEvent; // 移動値
        public event Action<Vector2> LookEvent;
        public event Action<bool> PrimaryFireEvent; // 射撃用
        public event Action<bool> PrimaryJumpEventEvent; // ジャンプ
        public event Action<bool> PrimarySubFireEvent; // Aim
        public event Action StartedJumpEvent;
        public event Action<bool> PrimaryDashEvent;
        public event Action StartedRFireEvent;
        public event Action StartedOpenItemEvent;
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
        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryFireEvent?.Invoke(true);
            else if (context.canceled)
                PrimaryFireEvent?.Invoke(false);
        }
        //public void OnAim(InputAction.CallbackContext context)
        //{
        //    AimPosition = context.ReadValue<Vector2>(); // マウスの座標を取得
        //}
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                StartedJumpEvent?.Invoke();
                PrimaryJumpEventEvent?.Invoke(true);
            }
            else if (context.canceled)
                PrimaryJumpEventEvent?.Invoke(false);
        }
        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
                PrimaryDashEvent?.Invoke(true);
            else if (context.canceled)
                PrimaryDashEvent?.Invoke(false);
        }
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
                case "Fire":
                    return _control.Player.Fire;
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
    }
}
