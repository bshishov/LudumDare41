using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIScrollRectPosition : MonoBehaviour
    {

        RectTransform _scrollRectTransform;
        RectTransform _contentPanel;
        RectTransform _selectedRectTransform;
        GameObject _lastSelected;

        private Vector2 _target;
        private Vector2 _vel;

        void Start()
        {
            _scrollRectTransform = GetComponent<RectTransform>();
            _contentPanel = GetComponent<ScrollRect>().content;
        }

        void Update()
        {
            // Get the currently selected UI element from the event system.
            var selected = EventSystem.current.currentSelectedGameObject;
            _contentPanel.anchoredPosition = Vector2.SmoothDamp(_contentPanel.anchoredPosition, _target, ref _vel, 0.2f, float.MaxValue, Time.deltaTime);

            // Return if there are none.
            if (selected == null)
                return;
            
            // Return if the selected game object is not inside the scroll rect.
            if (selected.transform.parent != _contentPanel.transform)
            {
                return;
            }
            // Return if the selected game object is the same as it was last frame,
            // meaning we haven't moved.
            if (selected == _lastSelected)
            {
                //return;
            }

            // Get the rect tranform for the selected game object.
            _selectedRectTransform = selected.GetComponent<RectTransform>();
            // The position of the selected UI element is the absolute anchor position,
            // ie. the local position within the scroll rect + its height if we're
            // scrolling down. If we're scrolling up it's just the absolute anchor position.
            float selectedPositionY = Mathf.Abs(_selectedRectTransform.anchoredPosition.y) + _selectedRectTransform.rect.height;

            // The upper bound of the scroll view is the anchor position of the content we're scrolling.
            float scrollViewMinY = _contentPanel.anchoredPosition.y;
            // The lower bound is the anchor position + the height of the scroll rect.
            float scrollViewMaxY = _contentPanel.anchoredPosition.y + _scrollRectTransform.rect.height;

            var scrolling = false;
            // If the selected position is below the current lower bound of the scroll view we scroll down.
            if (selectedPositionY > scrollViewMaxY)
            {
                float newY = selectedPositionY - _scrollRectTransform.rect.height;
                _target = new Vector2(_contentPanel.anchoredPosition.x, newY);
                scrolling = true;
            }
            // If the selected position is above the current upper bound of the scroll view we scroll up.
            else if (Mathf.Abs(_selectedRectTransform.anchoredPosition.y) < scrollViewMinY)
            {
                _target = new Vector2(_contentPanel.anchoredPosition.x, Mathf.Abs(_selectedRectTransform.anchoredPosition.y) - 100f);
                scrolling = true;
            }

            if (scrolling)
            {
                
            }

            _lastSelected = selected;
        }
    }
}
