using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    //Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
    //Altered by Brecht Lecluyse http://www.brechtos.com

    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorAttribute : PropertyAttribute
    {
        public bool UseDefaultTagFieldDrawer = false;
    }
}