using TMPro;
using UnityEngine;
using Zenject;

public class TablePreview : MonoBehaviour
{
    [SerializeField] private TMP_Text _comboName;
    [SerializeField] private TMP_Text _base;
    [SerializeField] private TMP_Text _multiplier;
    [SerializeField] private HandVisual _hand;

    private ICombinationsConfiguration _combinationsConfiguration;

    [Inject]
    public void Construct(ICombinationsConfiguration configuration)
    {
        _combinationsConfiguration = configuration;
    }
    
    private void Start()
    {
        ClearPreview();
        _hand.SelectedCardsChanged += OnSelectedCardsChanged;
    }

    private void OnDestroy()
    {
        _hand.SelectedCardsChanged -= OnSelectedCardsChanged;
    }

    private void ClearPreview()
    {
        _comboName.text = "";
        _base.text = "0";
        _multiplier.text = "0";
    }

    private void OnSelectedCardsChanged(PreviewData data)
    {
        if (data != null)
        {
            _comboName.text = _combinationsConfiguration.GetValue(data.Type).Name;
            _base.text = data.BaseValue.ToString();
            _multiplier.text = data.Multiplier.ToString("0.0");
        }
        else
        {
            ClearPreview();
        }
    }
}