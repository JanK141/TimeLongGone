using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts
{
    public class StatusDebug : MonoBehaviour
    {
        public static StatusDebug Instance;

        private void Awake() => Instance = this;

        public void UpdateText(string txt) => GetComponent<Text>().text = txt;
    }
}
