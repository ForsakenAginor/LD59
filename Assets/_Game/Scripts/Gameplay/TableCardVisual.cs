using UnityEngine;

public class TableCardVisual : CardVisual
{
    [SerializeField] private FlyingScore _flyingScore;
    
    public FlyingScore FlyingScore => _flyingScore;
}