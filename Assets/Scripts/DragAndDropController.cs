using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DragAndDropController : MonoBehaviour
{
    #region Serialized Fields
    
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float _minVelocityMagnitude;
    [SerializeField] private float _maxVelocityMagnitude;
    [SerializeField] private float _yObjectOffset;

    #endregion

    #region Private Fields
    
    private Vector3 _mousePosition;
    private Vector3 _previousMousePosition;
    private Vector3 _objectScreenPoint;
    private Vector3 _newVelocity;

    private float _groundingYPos;

    private bool _objectsDropped;
    private bool _isDragging;

    private List<IThrowable> _throwables = new List<IThrowable>();

    #endregion

    #region Public Properties

    public List<IThrowable> Throwables
    {
        get
        {
            return _throwables;
        }
        set
        {
            _throwables = value;
        }
    }

    #endregion

    #region Public Actions

    public UnityAction OnRollBegin;
    public UnityAction<Vector3> OnAnyObjectPickedUp;
    public UnityAction OnAnyObjectDropped;
    public UnityAction OnAnyObjectDragged;

    #endregion

    private void Awake()
    {
        OnAnyObjectPickedUp += DragStarted;
        OnAnyObjectDropped += OnDrop;
        OnAnyObjectDragged += OnDragging;
    }

    #region Private Methods

    private void DragStarted(Vector3 position)
    {
        if (MainController.CanRoll && _throwables.All(e => e.IsNotMoving()))
        {
            _objectScreenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            _mousePosition = new Vector3(Input.mousePosition.x, _objectScreenPoint.z, Input.mousePosition.y); //due to perspective mouse Y becomes world Z
            _isDragging = true;
            _objectsDropped = false;

            foreach(IThrowable throwable in _throwables)
            {
                throwable.GetTransform().position = position;
                throwable.SetGravity(false);
            }
        }
    }

    private void OnDragging()
    {
        _previousMousePosition = _mousePosition;
        _mousePosition = new Vector3(Input.mousePosition.x, _objectScreenPoint.z + _yObjectOffset, Input.mousePosition.y);  //due to perspective mouse Y becomes world Z
        _newVelocity = _mousePosition - _previousMousePosition; //Changes the force to be applied

        for(int i = 0; i < _throwables.Count; i++)
        {
            if (Physics.Raycast(_throwables[i].GetTransform().position, Vector3.down, 1000f, groundMask)) //Move only when above table/ground
            {
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(_mousePosition);
                _throwables[i].GetTransform().position = curPosition;
            }
        }
    }

    private void OnDrop()
    {
        if (_objectsDropped) // don't drop what was dropped
        {
            return;
        }

        for (int i = 0; i < _throwables.Count; i++)
        {
            var curentRigidbody = _throwables[i].GetRigidbody();
            if (curentRigidbody.velocity.magnitude < _minVelocityMagnitude) //set velocity to 0 if magnitude is below minimal value
            {
                AbortRoll();
                return;
            }

            if (curentRigidbody.velocity.magnitude > _maxVelocityMagnitude) //Make sure that velocity magnitude isn't off the charts
            {
                _newVelocity = curentRigidbody.velocity.normalized * _maxVelocityMagnitude;
            }

            _throwables[i].SetGravity(true);
        }

        _isDragging = false;
        _objectsDropped = true;
        OnRollBegin?.Invoke();
    }

    private void FixedUpdate()
    {
        if (_isDragging)
        {
            foreach(IThrowable throwable in _throwables)
            {
                throwable.SetVelocity(_newVelocity);
            }
        }
    }

    private void GetGroundY(IThrowable throwable)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, 1000f, groundMask);
        _groundingYPos = hit.point.y + throwable.GetTransform().GetComponent<Collider>().bounds.extents.y;
    }

    private void AbortRoll()
    {
        foreach (IThrowable throwable in _throwables)
        {
            GetGroundY(throwable);
            Vector3 currentPosition = throwable.GetTransform().position;
            throwable.GetTransform().position = new Vector3(currentPosition.x, _groundingYPos, currentPosition.z); // Return from whence thou cam'st (ground it)
            _newVelocity = Vector3.zero;

            throwable.SetGravity(true);
        }

        _objectsDropped = true;
        _isDragging = false;
        MainController.CanRoll = true;
    }

    #endregion
}
