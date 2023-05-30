using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AutoRollStatistics", menuName = "ScriptableObjects/AutoRollStatistics")]
public class AutoRollStats : ScriptableObject
{
    #region Serialized Fields

    [SerializeField] private float _minRollForce;
    [SerializeField] private float _maxRollForce;
    [SerializeField] private float _minRollTorque;
    [SerializeField] private float _maxRollTorque;

    #endregion

    #region Public Properties

    public float MinRollForce => _minRollForce;
    public float MaxRollForce => _maxRollForce;
    public float MinRollTorque => _minRollTorque;
    public float MaxRollTorque => _maxRollTorque;

    #endregion
}
