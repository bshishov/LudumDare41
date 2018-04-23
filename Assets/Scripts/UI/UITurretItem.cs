using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UITurretItem : MonoBehaviour, ISelectHandler
    {
        public Text Name;
        public Text Description;
        public Text Cooldown;
        public Text Cost;
        public Image Icon;

        public TurretInfo TurretInfo { get; private set; }

        public void Update()
        {
            var selected = EventSystem.current.currentSelectedGameObject;
        }

        public void Setup(TurretInfo turret)
        {
            if(turret == null)
                return;

            TurretInfo = turret;

            if(Name != null)
                Name.text = turret.Name;

            if(Description != null)
                Description.text = turret.Description;

            if(Icon != null)
                Icon.sprite = turret.Icon;

            if(Cost != null)
                Cost.text = turret.Cost.ToString();

            if(Cooldown != null)
                Cooldown.text = turret.Cooldown.ToString();
        }

        public void OnSelect(BaseEventData eventData)
        {
            UIBuildingMenu.Instance.ActiveSelection = TurretInfo;
        }
    }
}
