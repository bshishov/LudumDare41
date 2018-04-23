using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UINotification : MonoBehaviour
    {
        public Text Text;
        public float FadeTime = 1f;
        public Vector3 Offset;
        public Vector3 OffsetSpeed = Vector3.up;
        public Transform Target;

        void Start ()
        {
            Destroy(gameObject, FadeTime);
            Text.CrossFadeAlpha(0f, FadeTime, true);

            OffsetSpeed += Random.insideUnitSphere;
        }
	
        void Update ()
        {
            if (Target == null || Text == null)
                return;

            var scr = Camera.main.WorldToScreenPoint(Target.transform.position + Offset);
            transform.position = new Vector3(scr.x, scr.y, transform.position.z);

            Offset += OffsetSpeed * Time.deltaTime;
        }
    }
}
