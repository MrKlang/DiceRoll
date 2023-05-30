using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DiceRollUIController : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private TextMeshProUGUI _throwResultsText;
    [SerializeField] private TextMeshProUGUI _totalResultsText;
    [SerializeField] private Button _autoRollButton;

    #endregion

    #region Private Fields

    private long _totalScore; //not sure how long people will roll this sooooo long

    #endregion

    #region Private Methods

    private void Start()
    {
        _throwResultsText.text = $"{UITexts.ThrowResultsText} 0";
        _totalResultsText.text = $"{UITexts.TotalResultsText} 0";
    }

    private void OnDestroy()
    {
        _autoRollButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region Public Methods

    public void OnBeginRoll()
    {
        _throwResultsText.text = $"{UITexts.ThrowResultsText} ?";
    }

    public void UpdateScore(int currentRollScore)
    {
        _totalScore += currentRollScore;
        _throwResultsText.text = $"{UITexts.ThrowResultsText} {currentRollScore}";
        _totalResultsText.text = $"{UITexts.TotalResultsText} {_totalScore}";
    }

    public void BindActionToButton(UnityAction action)
    {
        _autoRollButton.onClick.AddListener(action);
    }

    #endregion
}
