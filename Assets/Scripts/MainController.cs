using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Main script, without it, things won't work
public class MainController : MonoBehaviour
{
    #region Serialized Fields

    [Header("Controllers")]
    [SerializeField] private List<DiceController> _diceControllers;
    [SerializeField] private DragAndDropController _dragAndDropController;
    [SerializeField] private DiceRollUIController _diceRollUIController;

    [Header("Scriptable Objects")]
    [SerializeField] private AutoRollStats _autoRollStats;
    [SerializeField] private DiceSettings _diceSettings;

    [Header("Object References")]
    [SerializeField] private Collider _autoRollSpawnAreaCollider;
    [SerializeField] private Transform _tableTransform;

    [Header("Dice layer and tag")]
    [SerializeField] private LayerMask _diceLayer;
    [SerializeField] private string _diceTag;

    #endregion

    #region Private Fields

    private int _totalThrowValue;

    #endregion

    #region Static Fields

    public static bool CanRoll = true; // One bool to bind them all

    #endregion

    #region Private Methods
    
    private void Awake()
    {
        for(int i = 0; i < _diceControllers.Count; i++)
        {
            _dragAndDropController.Throwables.Add(_diceControllers[i]);
        }

        for (int i = 0; i < _diceControllers.Count; i++)
        {
            // pass SO references
            _diceControllers[i].AutoRollStats = _autoRollStats;
            _diceControllers[i].DiceSettings = _diceSettings;
            
            //pass obj refs
            _diceControllers[i].TableTransform = _tableTransform;
            _diceControllers[i].AutoRollSpawnAreaCollider = _autoRollSpawnAreaCollider;

            //pass tag and set layer
            _diceControllers[i].tag = _diceTag;
            _diceControllers[i].gameObject.layer = (int)Mathf.Log(_diceLayer.value, 2);
            _diceControllers[i].Instantiate();
        }

        _dragAndDropController.OnRollBegin += _diceRollUIController.OnBeginRoll;
        _dragAndDropController.OnRollBegin += AwaitResults;
    }

    private void Start()
    {
        for (int i = 0; i < _diceControllers.Count; i++)
        {
            _diceControllers[i].OnPickup += _dragAndDropController.OnAnyObjectPickedUp;
            _diceControllers[i].OnDrop += _dragAndDropController.OnAnyObjectDropped;
            _diceControllers[i].OnDrag += _dragAndDropController.OnAnyObjectDragged;

            _diceRollUIController.BindActionToButton(_diceControllers[i].InvokeAutoRoll);
        }

        _diceRollUIController.BindActionToButton(() => AwaitResults());
    }

    private void AwaitResults()
    {
        if (!CanRoll)
        {
            return;
        }

        StartCoroutine(WaitForResults());
    }

    private IEnumerator WaitForResults()
    {
        CanRoll = false;
        _totalThrowValue = 0;
        _diceRollUIController.OnBeginRoll();

        yield return new WaitUntil(() => _diceControllers.All(e => e.IsNotMoving()));
        
        for(int i = 0; i < _diceControllers.Count; i++)
        {
            _totalThrowValue += _diceControllers[i].GetResult();
        }

        _diceRollUIController.UpdateScore(_totalThrowValue);
        CanRoll = true;
    }

    #endregion
}
