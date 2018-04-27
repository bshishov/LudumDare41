using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UINotifications : Singleton<UINotifications>
    {
        public float Proximity = 20f;
        public UINotification NotificationPrefab;

        private Transform _cameraTransform;
        void Start()
        {
            _cameraTransform = Camera.main.transform;
        }

        public void Show(Transform obj, string text, Color color, float time = 1f, float yOffset = 1f)
        {
            if(Vector3.Distance(obj.position, _cameraTransform.position) > Proximity)
                return;

            if(NotificationPrefab == null)   
                return;

            //var go = GameObject.Instantiate(NotificationPrefab, gameObject.transform, true);
            var go = GameObject.Instantiate(NotificationPrefab);
            go.transform.SetParent(gameObject.transform, false);
            var notification = go.GetComponent<UINotification>();
            notification.FadeTime = time;
            notification.Text.text = text;
            notification.Target = obj;
            notification.Text.color = color;
            notification.Offset += new Vector3(0, yOffset, 0);
        }
    }
}
