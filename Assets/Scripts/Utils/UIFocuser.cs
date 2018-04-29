using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Utils
{
    class UIFocuser : MonoBehaviour
    {
        private GameObject _selectedObj;

        void Start()
        {
            _selectedObj = EventSystem.current.currentSelectedGameObject;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(_selectedObj);

            _selectedObj = EventSystem.current.currentSelectedGameObject;
        }
    }
}
