using Assets.Source.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class JokerCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private FlyingScore _flyingScore;
    [SerializeField] private FlyingText _flyingText;
    [SerializeField] private RectTransform _transform;
    
    private JokerTooltip _jokerTooltip;
    private string _id;
    private IJokerConfiguration _configuration;

    public string Id => _id;
    
    public FlyingScore FlyingScore => _flyingScore;
    
    public FlyingText FlyingText => _flyingText;

    [Inject]
    public void Construct(IJokerConfiguration configuration)
    {
        _configuration = configuration;    
    }
    
    public void Init(string id, JokerTooltip tooltip)
    {
        _id = id;
        JokerData jokerData = _configuration.GetJokerData(id);
        _icon.sprite = jokerData.Icon;
        _jokerTooltip = tooltip;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        JokerData jokerData = _configuration.GetJokerData(_id);
        _jokerTooltip.Enable(jokerData.Name, jokerData.Description);
        _jokerTooltip.SetPosition(transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _jokerTooltip.Disable();
    }
}