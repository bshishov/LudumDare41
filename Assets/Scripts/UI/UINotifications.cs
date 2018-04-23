using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UINotifications : Singleton<UINotifications>
    {
        public UINotification NotificationPrefab;

        public void Show(Transform obj, string text, Color color, float time = 1f)
        {
            if(NotificationPrefab == null)   
                return;

            var go = GameObject.Instantiate(NotificationPrefab, gameObject.transform, true);
            var notification = go.GetComponent<UINotification>();
            notification.FadeTime = time;
            notification.Text.text = text;
            notification.Target = obj;
            notification.Text.color = color;
        }
    }
}
