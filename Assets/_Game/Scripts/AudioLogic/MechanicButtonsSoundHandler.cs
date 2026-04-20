using Assets.Source.Scripts.AudioLogic;
using UnityEngine;
using UnityEngine.UI;

public class MechanicButtonsSoundHandler : MonoBehaviour
{
    [SerializeField] private Button[] _buttons;

    private void Awake()
    {
        foreach (Button button in _buttons)
        {
            button.onClick.AddListener(PlayMechanicButton);
        }
    }

    private void OnDestroy()
    {
        foreach (Button button in _buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void PlayMechanicButton()
    {
        AudioPlayer.Instance.PlayMechanicButton();
    }
}