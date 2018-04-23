using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UICanvasGroupFader : MonoBehaviour
    {
        public enum FaderState
        {
            FadedIn,
            FadingIn,
            FadedOut,
            FadingOut
        }

        public float FadeTime = 1f;
        public FaderState Initial;
        public FaderState State { get; private set; }

        public Action StateChanged;
        private float _transition = 0f;
        private CanvasGroup _canvasGroup;

        void Start ()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            State = Initial;

            if (State == FaderState.FadedOut)
            {
                _canvasGroup.interactable = false;
                _canvasGroup.alpha = 0f;
                if (StateChanged != null)
                    StateChanged.Invoke();
            }

            if (State == FaderState.FadedIn)
            {
                _canvasGroup.interactable = true;
                _canvasGroup.alpha = 1f;
                if (StateChanged != null)
                    StateChanged.Invoke();
            }
        }
	
        void Update ()
        {
            if (State == FaderState.FadingIn)
            {
                _transition += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(_transition / FadeTime);

                if (_transition > FadeTime)
                {
                    State = FaderState.FadedIn;
                    _canvasGroup.interactable = true;
                    _canvasGroup.alpha = 1f;
                    if (StateChanged != null)
                        StateChanged.Invoke();
                }
            }

            if (State == FaderState.FadingOut)
            {
                _transition += Time.deltaTime;
                _canvasGroup.alpha = 1 - Mathf.Clamp01(_transition / FadeTime);

                if (_transition > FadeTime)
                {
                    State = FaderState.FadedOut;
                    _canvasGroup.interactable = false;
                    _canvasGroup.alpha = 0f;
                    if (StateChanged != null)
                        StateChanged.Invoke();
                }
            }
        }

        public void FadeIn()
        {
            if (State == FaderState.FadedOut)
            {
                _transition = 0;
                State = FaderState.FadingIn;
                if (StateChanged != null)
                    StateChanged.Invoke();
            }
        }

        public void FadeOut()
        {
            if (State == FaderState.FadedIn)
            {
                _transition = 0;
                State = FaderState.FadingOut;
                if (StateChanged != null)
                    StateChanged.Invoke();
            }
        }
    }
}
