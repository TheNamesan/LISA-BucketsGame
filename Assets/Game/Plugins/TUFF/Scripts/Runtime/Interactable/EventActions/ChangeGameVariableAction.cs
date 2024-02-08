using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ChangeGameVariableAction : EventAction
    {
        public int variableIndex = 0;
        public GameVariableValueType valueType = GameVariableValueType.BoolValue;
        public GameVariableAssignType assignType = GameVariableAssignType.Constant;
        public bool boolValue = false;
        public float numberValue = 0;
        public string stringValue = "";
        public Vector2 vectorValue = new Vector2();
        // Random
        public bool onlyIntegers = false;
        public float numberValueMin = 0;
        public List<string> randomStrings = new();
        public Vector2 vectorValueMin = new Vector2();
        

        public ChangeGameVariableAction()
        {
            eventName = "Change Game Variable";
            eventColor = EventGUIColors.gameProgression;
        }
        public override void Invoke()
        {
            object value = GetValue(this);
            if (assignType == GameVariableAssignType.Random) value = GetRandomValue(this);
            PlayerData.instance.AssignGameVariableValue(variableIndex, value);
            isFinished = true;
        }
        public static object GetValue(ChangeGameVariableAction action)
        {
            switch (action.valueType)
            {
                case GameVariableValueType.BoolValue:
                    return action.boolValue;
                case GameVariableValueType.NumberValue:
                    return action.numberValue;
                case GameVariableValueType.StringValue:
                    return action.stringValue;
                case GameVariableValueType.VectorValue:
                    return action.vectorValue;
            }
            return null;
        }
        public static object GetRandomValue(ChangeGameVariableAction action)
        {
            switch (action.valueType)
            {
                case GameVariableValueType.BoolValue:
                    return Random.Range(0, 2) > 0;
                case GameVariableValueType.NumberValue:
                    var number = action.numberValue;
                    if (action.onlyIntegers) number = Random.Range((int)action.numberValueMin, (int) number + 1);
                    else number = Random.Range(action.numberValueMin, number);
                    return number;
                case GameVariableValueType.StringValue:
                    var text = "";
                    if (action.randomStrings == null || action.randomStrings.Count <= 0) return text;
                    text = action.randomStrings[Random.Range(0, action.randomStrings.Count)];
                    return text;
                case GameVariableValueType.VectorValue:
                    var vector = action.vectorValue;
                    if (action.onlyIntegers) 
                        vector = new Vector2(Random.Range((int)action.vectorValueMin.x, (int)vector.x + 1), 
                            Random.Range((int)action.vectorValueMin.y, (int)vector.y + 1));
                    else vector = new Vector2(Random.Range(action.vectorValueMin.x, vector.x),
                            Random.Range(action.vectorValueMin.y, vector.y));
                    return vector;
            }
            return null;
        }
    }
}

