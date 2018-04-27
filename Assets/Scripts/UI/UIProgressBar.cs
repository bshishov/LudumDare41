using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIProgressBar : MonoBehaviour
    {
        public RectTransform FillTransform;

        public float Value
        {
            get { return _value; }
            set
            {
                if (FillTransform != null)
                {
                    _value = Mathf.Clamp01(value);
                    FillTransform.sizeDelta = new Vector2(_initialSize.x * _value, _initialSize.y);
                }
            }
        }
        public float Initial;

        private float _value;
        private Vector2 _initialSize;

        void Awake()
        {
            _initialSize = FillTransform.sizeDelta;
            Value = Initial;
        }
    }
}
