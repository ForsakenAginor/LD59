using System;
using System.Collections;
using Assets.Source.Scripts.DI.Services.Global;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class JokerSelectionCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private Image _icon;
    
    private IJokerConfiguration _jokerConfiguration;
    private string _jokerId;
    private bool _isSelected;

    public event Action<string> JokerSelected; 
    
    public bool IsSelected => _isSelected;
    
    [Inject]
    public void Construct(IJokerConfiguration jokerConfiguration)
    {
        _jokerConfiguration = jokerConfiguration;
        _button.onClick.AddListener(ChooseJoker);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(ChooseJoker);
    }

    public void Init(string id)
    {
        _jokerId = id;
        var data = _jokerConfiguration.GetJokerData(_jokerId);
        
        _name.text = data.Name;
        _description.text = data.Description;
        _icon.sprite = data.Icon;
        _isSelected = false;
    }

    public void SetInteractable(bool isInteractable)
    {
        _button.interactable = isInteractable;
    }

    private void ChooseJoker()
    {
        _isSelected = true;
        _button.interactable = false;
        JokerSelected?.Invoke(_jokerId);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button.interactable)
            transform.localScale = Vector3.one * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_button.interactable)
            transform.localScale = Vector3.one;
    }
}