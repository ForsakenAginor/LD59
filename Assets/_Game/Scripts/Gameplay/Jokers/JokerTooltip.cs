using Assets.Source.Scripts.Utility;
using TMPro;
using UnityEngine;

public class JokerTooltip : SwitchableElement
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private RectTransform _transform;
    
    public void Enable(string name, string description)
    {
        Enable();
        _description.text = description;
        _name.text = name;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}