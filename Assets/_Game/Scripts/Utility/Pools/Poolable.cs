using System;
using UnityEngine;

namespace Assets.Source.Scripts.Utility.Pools
{
    public abstract class Poolable : MonoBehaviour
    {
        public event Action<Poolable> ReadyToRelease;

        protected void CallReadyToReleaseEvent()
        {
            ReadyToRelease?.Invoke(this);
        }
    }
}