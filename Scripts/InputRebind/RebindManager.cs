using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;
using develop_tps;

namespace develop_tps
{
    public class RebindManager : MonoBehaviour
    {
        public InputReader inputReader;
        public ConfigScreen _configScreen;
        private string _currentActionName;

        public void StartRebind(string actionName)
        {
            _currentActionName = actionName;
            inputReader.DisableInput();
            // UI上に「キーを押してください」というメッセージを表示するなど
        }

        private void Update()
        {
            _configScreen.UpdateBindings();
            if (!string.IsNullOrEmpty(_currentActionName))
            {
                // キーボードの入力を確認
                if (Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    foreach (KeyControl key in Keyboard.current.allKeys)
                    {
                        if (key.wasPressedThisFrame)
                        {
                            Debug.Log($"Rebinding to key: {key.name}");
                            RebindAction($"<Keyboard>/{key.name}");
                            return;
                        }
                    }
                }

                // ゲームパッドの入力を確認
                if (Gamepad.current != null)
                {
                    foreach (InputControl control in Gamepad.current.allControls)
                    {
                        if (control is ButtonControl button && button.wasPressedThisFrame)
                        {
                            Debug.Log($"Rebinding to gamepad button: {button.name}");
                            RebindAction($"<Gamepad>/{button.name}");
                            return;
                        }
                    }
                }
            }
        }

        private void RebindAction(string bindingPath)
        {
            var action = inputReader.GetActionByName(_currentActionName);
            if (action != null)
            {
                Debug.Log($"Rebinding action: {_currentActionName} to {bindingPath}");
                action.ApplyBindingOverride(new InputBinding { overridePath = bindingPath });
                SaveBinding(action);
            }
            _currentActionName = null;
            inputReader.EnableInput();
        }

        private void SaveBinding(InputAction action)
        {
            var rebinds = new List<SerializableBinding>();

            foreach (var binding in action.bindings)
            {
                rebinds.Add(new SerializableBinding { path = binding.overridePath ?? binding.path, interactions = binding.interactions, processors = binding.processors });
            }

            var rebindsJson = JsonUtility.ToJson(new SerializableBindings { bindings = rebinds });
            PlayerPrefs.SetString(action.actionMap.name + "_" + action.name, rebindsJson);
            PlayerPrefs.Save();
            Debug.Log($"Saved binding: {action.name} -> {rebindsJson}");
        }

        public void LoadBindings()
        {
            var actions = inputReader.GetAllActions();
            foreach (var action in actions)
            {
                string rebindsJson = PlayerPrefs.GetString(action.actionMap.name + "_" + action.name, string.Empty);
                if (!string.IsNullOrEmpty(rebindsJson))
                {
                    var rebinds = JsonUtility.FromJson<SerializableBindings>(rebindsJson).bindings;
                    action.RemoveAllBindingOverrides();
                    for (int i = 0; i < rebinds.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(rebinds[i].path))
                        {
                            action.ApplyBindingOverride(i, rebinds[i].path);
                        }
                    }
                    Debug.Log($"Loaded binding: {action.name} -> {rebindsJson}");
                }
            }
        }

        private void Start()
        {
            LoadBindings();
        }

        public string GetBindingString(string actionName, string device)
        {
            var action = inputReader.GetActionByName(actionName);
            if (action == null)
            {
                return null;
            }

            foreach (var binding in action.bindings)
            {
                if (binding.effectivePath.Contains(device))
                {
                    return binding.effectivePath;
                }
            }

            return null;
        }

        [Serializable]
        public class SerializableBindings
        {
            public List<SerializableBinding> bindings;
        }

        [Serializable]
        public class SerializableBinding
        {
            public string path;
            public string interactions;
            public string processors;
        }
    }
}