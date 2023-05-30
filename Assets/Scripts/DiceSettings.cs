using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum DiceType
{
    D2 = 0,
    D4 = 1,
    D6 = 2,
    D8 = 3,
    D10 = 4,
    D12 = 5,
    D20 = 6,
    D100 = 7,
}

[Serializable]
public class DiceData
{
    public DiceType diceType;
    public List<DiceFaceData> faces = new List<DiceFaceData>();
}

[Serializable]
public class DiceFaceData
{
    public int faceScoreValue;
    public string faceText;
    public Sprite faceImageSprite;
    public bool useImageAsSymbol;
}

[Serializable]
[CreateAssetMenu(fileName = "DiceSettings", menuName = "ScriptableObjects/DiceSettings")]
public class DiceSettings : ScriptableObject
{
    public List<DiceData> dices;

    public DiceData GetDice(DiceType diceType)
    {
        return dices.First(e => e.diceType.Equals(diceType));
    }
}