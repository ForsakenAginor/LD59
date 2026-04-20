using Assets.Source.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSwitcher : MonoBehaviour
{
    [SerializeField] private SwitchableElement[] _panels;
    [SerializeField] private Button[] _buttons;

    private void Start()
    {
        // Назначаем обработчики на кнопки
        for (int i = 0; i < _buttons.Length && i < _panels.Length; i++)
        {
            int index = i; // Локальная копия для замыкания
            _buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    private void OnButtonClick(int panelIndex)
    {
        // Отключаем все панели
        foreach (var panel in _panels)
        {
            panel.Disable();
        }

        // Включаем выбранную панель
        if (panelIndex >= 0 && panelIndex < _panels.Length)
        {
            _panels[panelIndex].Enable();
        }
    }
}