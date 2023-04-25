using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FSMC
{
    [System.Serializable]
    public enum FSMParameterType
    {
        Integer,
        Float,
        Bool,
        Trigger
    }

    [System.Serializable]
    public abstract class FSMParameter : ScriptableObject
    {

        [SerializeField] private FSMParameterType _type;
        public FSMParameterType Type { get => _type; private set => _type = value; }


        public static FSMParameter CreateParameter(string name, FSMParameterType type, object value, FSMController controller)
        {
            FSMParameter param = null;

            switch (type)
            {
                case FSMParameterType.Integer:
                    if (value is not int) return null;
                    var paramInt = ScriptableObject.CreateInstance<FSMIntegerParameter>();
                    Undo.RegisterCreatedObjectUndo(paramInt, "Add parameter");
                    param = paramInt;
                    (param as FSMIntegerParameter).Value = (int)value;
                    break;
                case FSMParameterType.Float:
                    if (value is not float) return null;
                    var paramFloat = ScriptableObject.CreateInstance<FSMFloatParameter>();
                    Undo.RegisterCreatedObjectUndo(paramFloat, "Add parameter");
                    param = paramFloat;
                    (param as FSMFloatParameter).Value = (float)value;
                    break;
                case FSMParameterType.Bool:
                    if (value is not bool) return null;
                    var paramBool = ScriptableObject.CreateInstance<FSMBoolParameter>();
                    Undo.RegisterCreatedObjectUndo(paramBool, "Add parameter");
                    param = paramBool;
                    (param as FSMBoolParameter).Value = (bool)value;
                    break;
                case FSMParameterType.Trigger:
                    if (value is not bool) return null;
                    var paramTrigger = ScriptableObject.CreateInstance<FSMBoolParameter>();
                    Undo.RegisterCreatedObjectUndo(paramTrigger, "Add parameter");
                    param = paramTrigger;
                    (param as FSMBoolParameter).Value = (bool)value;
                    break;

            }
            if (param)
            {
                param.name = name;
                param.Type = type;
                AssetDatabase.AddObjectToAsset(param, controller);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return param;
            }
            return null;
        }
    }
}
