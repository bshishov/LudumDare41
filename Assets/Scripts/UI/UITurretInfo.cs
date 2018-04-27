using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(UIFollowSceneObject))]
    [RequireComponent(typeof(UICanvasGroupFader))]
    public class UITurretInfo : Singleton<UITurretInfo>
    {
        public UIProgressBar CoolDownProgressBar;
        public Image Arrow;
        public Text ActivateText;

        public GameObject Target
        {
            get
            {
                if(_followSceneObject != null)
                    return _followSceneObject.Target;
                return null;
            }
            set
            {
                if (_followSceneObject != null)
                    _followSceneObject.Target = value;
            }
        }

        private UICanvasGroupFader _fader;
        private UIFollowSceneObject _followSceneObject;

        public float CoolDownPercentage
        {
            get
            {
                if (CoolDownProgressBar != null)
                    return CoolDownProgressBar.Value;

                return 0f;
            }

            set
            {
                if (CoolDownProgressBar != null)
                    CoolDownProgressBar.Value = value;
            }
        }

        public Direction ArrowDirection
        {
            set
            {
                if (Arrow != null)
                {
                    var s = Arrow.transform.localScale;
                    if (value == Direction.Left)
                    {
                        Arrow.transform.localScale = new Vector3(1, s.y, s.z);
                        Arrow.color = Color.white;
                    }

                    if (value == Direction.Right)
                    {
                        Arrow.transform.localScale = new Vector3(-1, s.y, s.z);
                        Arrow.color = Color.white;
                    }

                    if (value == Direction.None)
                        Arrow.color = new Color(1, 1, 1, 0);
                }
            }
        }

        public bool ShowActivateText
        {
            set
            {
                if (ActivateText != null)
                {
                    ActivateText.color = value ? Color.white : new Color(1, 1, 1, 0);
                }
            }
        }

        public void Show()
        {
            _fader.FadeIn();
        }

        public void Hide()
        {
            _fader.FadeOut();
        }

        void Start()
        {
            _fader = GetComponent<UICanvasGroupFader>();
            _followSceneObject = GetComponent<UIFollowSceneObject>();
        }
    }
}
