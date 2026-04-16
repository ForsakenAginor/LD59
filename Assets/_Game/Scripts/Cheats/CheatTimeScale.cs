using UnityEngine;

namespace Assets.Source.Scripts.Cheats
{
    public class CheatTimeScale : MonoBehaviour
    {
        [Header("Time Scale Settings")]
        [SerializeField] private float _normalTimeScale = 1f;
        [SerializeField] private float _slowTimeScale = 0.1f;
        [SerializeField] private float _fastTimeScale = 5f;

        private State _state = State.Normal;

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                SetState(State.Normal);
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                SetState(State.Slow);
            }
            else if (Input.GetKeyDown(KeyCode.F8))
            {
                SetState(State.Fast);
            }
        }

        private void SetState(State newState)
        {
            if (_state == newState)
                return;

            _state = newState;
            ApplyTimeScale();
            Debug.Log($"TimeScale changed to: {_state}");
        }

        private void ApplyTimeScale()
        {
            switch (_state)
            {
                case State.Normal:
                    Time.timeScale = _normalTimeScale;
                    break;
                case State.Slow:
                    Time.timeScale = _slowTimeScale;
                    break;
                case State.Fast:
                    Time.timeScale = _fastTimeScale;
                    break;
            }
        }

        private enum State
        {
            Normal,
            Slow,
            Fast,
        }
    }
}