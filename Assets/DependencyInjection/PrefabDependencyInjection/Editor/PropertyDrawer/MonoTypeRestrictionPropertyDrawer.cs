using UnityEditor;
using UnityEngine;

namespace Platinio.DependencyInjection
{
    [CustomPropertyDrawer(typeof(MonoTypeRestriction))]
    public class MonoTypeRestrictionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("ObjectValue");
            
            if (valueProperty == null) return;
            
            EditorGUI.PropertyField(position, valueProperty, new GUIContent(ConvertToInspectorName(property.name)));

            var attrib = attribute as MonoTypeRestriction;

            if (attrib != null)
            {
                var component = valueProperty.objectReferenceValue as Component;

                if (component != null)
                {
                    valueProperty.objectReferenceValue = component.gameObject.GetComponent(attrib.RestrictedType);
                    return;
                }

                var gameObject = valueProperty.objectReferenceValue as GameObject;
                if (gameObject != null)
                {
                    valueProperty.objectReferenceValue  = gameObject.GetComponent(attrib.RestrictedType);
                    return;
                }

                valueProperty.objectReferenceValue  = null;
            }
        }
        
        private string ConvertToInspectorName(string varName)
        {
            string inspectorName = varName.Replace("m_", string.Empty).Replace("_", string.Empty);
            inspectorName = $"{char.ToUpper(inspectorName[0])}{inspectorName.Substring(1)}";
          
            for (int n = inspectorName.Length - 1; n > 0; n--)
            {
                if (char.IsUpper(inspectorName[n]))
                {
                    inspectorName = $"{inspectorName.Substring(0, n)} {inspectorName.Substring(n, inspectorName.Length - n )}";
                   
                }
            }         
            
            return inspectorName;
        }
    }
}