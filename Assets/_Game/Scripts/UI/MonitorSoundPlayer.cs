using Assets.Source.Scripts.AudioLogic;
using UnityEngine;
using UnityEngine.UI;

public class MonitorSoundPlayer : MonoBehaviour
{
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(() => AudioPlayer.Instance.PlayMonitorEnable());
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}