using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FSMC.Windows
{
    public class CreateStatePopup : PopupWindowContent
    {
        private TextField textField;
        private Action<string> action;
        public CreateStatePopup(Action<string> action)
        {
            this.action = action;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 40);
        }

        public override void OnGUI(Rect rect)
        {

        }

        public override void OnOpen()
        {
            textField = new TextField();
            textField.value = "State";
            var root = editorWindow.rootVisualElement;
            root.Add(textField);
            root.style.marginBottom = 5;
            root.style.marginTop = 5;
            root.style.marginLeft = 5;
            root.style.marginRight = 5;
            root.style.justifyContent = Justify.Center;

            textField.RegisterCallback<KeyDownEvent>(evt => {
                if (evt.keyCode == KeyCode.Return)
                {
                    action.Invoke(textField.value);
                    evt.StopPropagation();
                    evt.PreventDefault();
                    editorWindow.Close();
                }
            });
        }
    }

}