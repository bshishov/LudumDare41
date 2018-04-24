#if DEBUG
using UnityEditor;
#endif

using UnityEngine;

namespace Assets.Scripts.Utils
{
    //Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
    //Altered by Brecht Lecluyse http://www.brechtos.com

#if DEBUG
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
#endif
    public class TagSelectorAttribute : PropertyAttribute
    {
        public bool UseDefaultTagFieldDrawer = false;
    }
}