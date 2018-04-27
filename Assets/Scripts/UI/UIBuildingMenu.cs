using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(UICanvasGroupFader))]
    public class UIBuildingMenu : Singleton<UIBuildingMenu>
    {
        public bool IsActive { get { return _canvasGroupFader.State == UICanvasGroupFader.FaderState.FadedIn; } }

        public GameObject TurretItem;
        public ScrollRect ScrollRect;

        public TurretInfo ActiveSelection { get; set; }
        public TurretsList Turrets;
        public RectTransform Content;
        public Vector2 Spacing;
        public Vector2 ElementSize;

        [Header("Description")]
        public Text SelectedName;
        public Text SelectedDescription;

        private UICanvasGroupFader _canvasGroupFader;
        private readonly List<UITurretItem> _turretItems = new List<UITurretItem>();

        void Start ()
        {
            _canvasGroupFader = GetComponent<UICanvasGroupFader>();
            foreach (var turret in Turrets.Turrets)
            {
                var pos = new Vector2(
                    turret.PositionInBuildingMenu.x * (ElementSize.x + Spacing.x), 
                    -turret.PositionInBuildingMenu.y * (ElementSize.y + Spacing.y));
                var tObj = GameObject.Instantiate(TurretItem);
                var rt = tObj.GetComponent<RectTransform>();
                rt.SetParent(Content, false);
                rt.anchoredPosition += pos;
                rt.sizeDelta = ElementSize;

                var t = tObj.GetComponent<UITurretItem>();
                if (t == null)
                    continue;

                t.Setup(turret);
                _turretItems.Add(t);
            }
            

            _canvasGroupFader.StateChanged += () =>
            {
                if (_canvasGroupFader.State == UICanvasGroupFader.FaderState.FadedIn)
                    HIghlightFirst();
            };
        }
        
        void Update()
        {
            if (!IsActive)
                return;

            UpdateSelectionInfo();
        }

        public void UpdateSelectionInfo()
        {
            if (ActiveSelection != null)
            {
                if (SelectedName != null)
                    SelectedName.text = ActiveSelection.Name;

                if (SelectedDescription != null)
                    SelectedDescription.text = ActiveSelection.Description;
            }
            else
            {
                if (SelectedName != null)
                    SelectedName.text = "";

                if (SelectedDescription != null)
                    SelectedDescription.text = "";
            }
        }

        private void MoveSelectionUp()
        {
        }

        private void MoveSelectionDown()
        {
        }

        public void Show()
        {
            if(IsActive)
                return;
            
            _canvasGroupFader.FadeIn();
        }

        public void Hide()
        {
            if (!IsActive)
                return;
            
            _canvasGroupFader.FadeOut();
        }

        public void Toggle()
        {
            if(IsActive)
                Hide();
            else
                Show();
        }

        private void HIghlightFirst()
        {
            if (EventSystem.current.currentSelectedGameObject != null)
                return;

            if (_turretItems.Count > 0)
            {
                var selectable = _turretItems[0].GetComponent<Selectable>();
                if(selectable != null)
                    selectable.Select();
            }
        }
    }
}
