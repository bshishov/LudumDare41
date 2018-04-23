using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public TurretInfo[] Turrets;
        
        private UICanvasGroupFader _canvasGroupFader;
        private readonly List<UITurretItem> _turretItems = new List<UITurretItem>();

        void Start ()
        {
            _canvasGroupFader = GetComponent<UICanvasGroupFader>();

            for (var i = 0; i < 10; i++)
            {
                foreach (var turret in Turrets)
                {
                    var tObj = GameObject.Instantiate(TurretItem, ScrollRect.content);
                    var t = tObj.GetComponent<UITurretItem>();
                    if (t == null)
                        continue;

                    t.Setup(turret);
                    _turretItems.Add(t);
                }
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
