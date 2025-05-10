using UnityEngine;
using System;

// Unity edit�r�nde sadece okunabilir alanlar i�in �zel bir nitelik
[AttributeUsage(AttributeTargets.Field)]
public class ReadOnlyAttribute : PropertyAttribute
{
    // Bu �zel bir PropertyAttribute olarak �al���r
    // �lgili Property Drawer EditMode klas�r�nde tan�mlanmal�d�r
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // GUI'yi devre d��� b�rakarak salt okunur hale getir
            bool previousGUIState = GUI.enabled;
            GUI.enabled = false;

            // Varsay�lan property field'� g�ster ama devre d��� olarak
            EditorGUI.PropertyField(position, property, label);

            // GUI durumunu eski haline getir
            GUI.enabled = previousGUIState;
        }
    }
}
#endif