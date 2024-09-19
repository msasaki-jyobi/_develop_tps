using develop_tps;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace develop_tps
{


    public class ConfigScreen : MonoBehaviour
    {
        public RebindManager rebindManager;
        public TextMeshProUGUI keyboardJumpText;
        public TextMeshProUGUI gamepadJumpText;

        private void Start()
        {
            UpdateBindings();
        }

        public void UpdateBindings()
        {
            string keyboardBinding = rebindManager.GetBindingString("Fire", "Keyboard");
            string gamepadBinding = rebindManager.GetBindingString("Fire", "Gamepad");

            keyboardJumpText.text = keyboardBinding != null ? $"Fire: {keyboardBinding}" : "Fire: Unknown";
            gamepadJumpText.text = gamepadBinding != null ? $"Fire: {gamepadBinding}" : "Fire: Unknown";
        }
    }
}