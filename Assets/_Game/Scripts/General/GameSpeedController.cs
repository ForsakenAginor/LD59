using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedController : MonoBehaviour
{
    [SerializeField] private Slider _speedSlider;
    [SerializeField] private TMP_Text _speedText;

    private void Awake()
    {
        _speedSlider.onValueChanged.AddListener(OnSpeedChanged);
    }

    private void OnDestroy()
    {
        _speedSlider.onValueChanged.RemoveListener(OnSpeedChanged);
    }

    private void OnSpeedChanged(float amount)
    {
        Time.timeScale = amount;
        _speedText.text = amount.ToString("0.0");
    }
}