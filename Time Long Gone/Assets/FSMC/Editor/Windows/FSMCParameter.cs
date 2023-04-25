using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FSMC.Windows
{
    public class FSMCParameter : VisualElement
    {
        public FSMParameterType type = FSMParameterType.Integer;
        private VisualElement valueContainer;
        public FSMCParameter()
        {
            TextField paramName = new TextField() { name = "paramName" };
            Add(paramName);

            valueContainer = new VisualElement() { name = "paramValue"};
            
            valueContainer.Add(new IntegerField() { name="IntegerValue"});
            valueContainer.Add(new FloatField() { name="FloatValue"});
            valueContainer.Add(new Toggle() { name="BoolValue"});
            valueContainer.Add(new Toggle() { name="TriggerValue"});


            Add(valueContainer);
        }
        public void RedrawValue()
        {
            valueContainer.Q<IntegerField>(name: "IntegerValue").style.display = DisplayStyle.None;
            valueContainer.Q<FloatField>(name: "FloatValue").style.display = DisplayStyle.None;
            valueContainer.Q<Toggle>(name: "BoolValue").style.display = DisplayStyle.None;
            valueContainer.Q<Toggle>(name: "TriggerValue").style.display = DisplayStyle.None;

            switch (type)
            {
                case FSMParameterType.Integer:
                    valueContainer.Q<IntegerField>(name: "IntegerValue").style.display = DisplayStyle.Flex;
                    break;
                case FSMParameterType.Float:
                    valueContainer.Q<FloatField>(name: "FloatValue").style.display = DisplayStyle.Flex;
                    break;
                case FSMParameterType.Bool:
                    valueContainer.Q<Toggle>(name: "BoolValue").style.display = DisplayStyle.Flex;
                    break;
                case FSMParameterType.Trigger:
                    valueContainer.Q<Toggle>(name: "TriggerValue").style.display = DisplayStyle.Flex;
                    break;

            }
        }

        
    }
}