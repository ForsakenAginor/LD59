using System;

namespace Assets.Source.Scripts.Utility
{
    public interface IResource
    {
        public event Action ResourcesAmountChanged;

        public int Amount { get; }

        public int Maximum { get; }
    }
}