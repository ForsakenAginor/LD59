using UnityEngine;

namespace Assets.Source.Scripts.Utility
{
    public class SwitchableElement : MonoBehaviour, ISwitchableElement
    {
        public virtual void Enable()
        {
            gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}