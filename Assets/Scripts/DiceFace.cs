using TMPro;
using UnityEngine;
using UnityEngine.UI;

// A class that represents dice face
public class DiceFace : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField] private TextMeshProUGUI _faceTextComponent;
    [SerializeField] private Image _faceImage;

    #endregion

    #region Private Fields

    private Sprite _faceImageSprite;

    private int _faceScoreValue;
    private bool _useImage;
    private string _faceText;

    #endregion

    #region Public Properties

    public int FaceScoreValue
    {
        get
        {
            return _faceScoreValue;
        }
        set
        {
            _faceScoreValue = value;
        }
    }

    public Sprite FaceImageSprite
    {
        get
        {
            return _faceImageSprite;
        }
        set
        {
            _faceImageSprite = value;
        }
    }

    public string FaceText
    {
        get
        {
            return _faceText;
        }
        set
        {
            _faceText = value;
        }
    }

    public bool UseImageAsSymbol
    {
        get
        {
            return _useImage;
        }
        set
        {
            _useImage = value;
            RefreshFace();
        }
    }

    #endregion

    #region Private Methods

    private void RefreshFace()
    {
        _faceImage.enabled = _useImage;
        _faceTextComponent.enabled = !_useImage;
        
        if (_useImage)
        {
            _faceImage.sprite = _faceImageSprite;
        }
        else
        {
            _faceTextComponent.text = _faceText;
        }
    }

    #endregion
}
