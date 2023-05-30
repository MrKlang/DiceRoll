using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody), typeof(MeshCollider), typeof(SphereCollider))] // we need rigidbody and mesh collider for physics and sphere collider for drag'n drop
public class DiceController : MonoBehaviour, IThrowable
{
    #region Serialized Fields

    [Header("Dice properties")]
    [SerializeField] private DiceType _diceType;

    [Header("Object References")]
    [SerializeField] private DiceFace _diceFacePrefab;
    [SerializeField] private List<Transform> _faceSlots;

    #endregion

    #region Private Fields

    private Transform _table;

    private AutoRollStats _autoRollStats;
    private DiceSettings _diceSettings;
    private List<DiceFace> _diceFaces;

    private MeshCollider _diceCollider;
    private SphereCollider _diceGrabTrigger;
    private Collider _autoRollSpawnAreaCollider;

    private Rigidbody _diceRigidbody;

    #endregion

    #region Public Properties

    public AutoRollStats AutoRollStats
    {
        get
        {
            return _autoRollStats;
        }
        set
        {
            _autoRollStats = value;
        }
    }

    public DiceSettings DiceSettings
    {
        get
        {
            return _diceSettings;
        }
        set
        {
            _diceSettings = value;
        }
    }

    public Collider AutoRollSpawnAreaCollider
    {
        get
        {
            return _autoRollSpawnAreaCollider;
        }
        set
        {
            _autoRollSpawnAreaCollider = value;
        }
    }

    public Transform TableTransform
    {
        get
        {
            return _table;
        }
        set
        {
            _table = value;
        }
    }

    #endregion

    #region Public Actions

    public UnityAction<Vector3> OnPickup;
    public UnityAction OnDrag;
    public UnityAction OnDrop;
    public UnityAction InvokeAutoRoll;

    #endregion

    #region Private Methods

    private void Awake()
    {
        _diceRigidbody = GetComponent<Rigidbody>();
        _diceCollider = GetComponent<MeshCollider>();
        _diceGrabTrigger = GetComponent<SphereCollider>();

        _diceCollider.convex = true;
        _diceGrabTrigger.isTrigger = true;

        InvokeAutoRoll += DoAutoRoll;
        _diceFaces = new List<DiceFace>();
    }

    private Vector3 GetRandomSpawnPointInBounds(Bounds bounds) // Randomly select new spawn point for automatic (on button click) roll
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private Vector3 GetAutoRollTorque() // Randomly decide how much torque will be added to the dice
    {
        float dirX = Random.Range(_autoRollStats.MinRollTorque, _autoRollStats.MaxRollTorque);
        float dirY = Random.Range(_autoRollStats.MinRollTorque, _autoRollStats.MaxRollTorque);
        float dirZ = Random.Range(_autoRollStats.MinRollTorque, _autoRollStats.MaxRollTorque);

        return new Vector3(dirX, dirY, dirZ);
    }

    private void DoAutoRoll() //As name suggests
    {
        if (MainController.CanRoll)
        {
            transform.position = GetRandomSpawnPointInBounds(_autoRollSpawnAreaCollider.bounds);
            Vector3 rollDirection = (_table.position - transform.position).normalized;
            _diceRigidbody.AddForce(rollDirection * Random.Range(_autoRollStats.MinRollForce, _autoRollStats.MaxRollForce));
            _diceRigidbody.AddTorque(GetAutoRollTorque());
        }
    }

    #endregion

    #region Public Methods

    public void Instantiate()
    {
        DiceData diceData = _diceSettings.GetDice(_diceType);
        int facesCount = diceData.faces.Count;
        int currentFaceDataIndex = 0;

        for (int i = 0; i < _faceSlots.Count; i++) // Let's make some faces
        {
            if (_faceSlots[i].Equals(null)) //in case of someone forgetting about list resizing in inspector
            {
                break;
            }

            if (currentFaceDataIndex.Equals(facesCount)) // this is done mainly for D4 as it's result concept tends to slip my mind
            {
                currentFaceDataIndex = 0;
            }

            DiceFace newFace = Instantiate(_diceFacePrefab, _faceSlots[i].position, Quaternion.identity, _faceSlots[i]);
            newFace.transform.LookAt(transform);
            newFace.transform.Rotate(_diceFacePrefab.transform.localEulerAngles);
            newFace.FaceImageSprite = diceData.faces[currentFaceDataIndex].faceImageSprite;
            newFace.FaceScoreValue = diceData.faces[currentFaceDataIndex].faceScoreValue;
            newFace.FaceText = diceData.faces[currentFaceDataIndex].faceText;
            newFace.UseImageAsSymbol = diceData.faces[currentFaceDataIndex].useImageAsSymbol;
            _diceFaces.Add(newFace);
            currentFaceDataIndex++;
        }
    }

    public int GetResult()
    {
        float highestPositionedFaceYValue = _diceFaces.Max(obj => obj.transform.position.y);
        DiceFace result = _diceFaces.First(obj => obj.transform.position.y == highestPositionedFaceYValue);
        return result.FaceScoreValue;
    }

    public bool IsNotMoving()
    {
        return _diceRigidbody.IsSleeping();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Rigidbody GetRigidbody()
    {
        return _diceRigidbody;
    }
    public void SetGravity(bool useGravity)
    {
        _diceRigidbody.useGravity = useGravity;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        _diceRigidbody.velocity = newVelocity;
    }

    public void OnMouseDown()
    {
        OnPickup?.Invoke(transform.position); //let's clamp dices together
    }

    public void OnMouseDrag()
    {
        OnDrag?.Invoke();
    }

    public void OnMouseUp()
    {
        OnDrop?.Invoke();
    }

    #endregion
}
