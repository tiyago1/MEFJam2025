using UnityEngine;
using System;

// Unity editöründe sadece okunabilir alanlar için özel bir nitelik
[AttributeUsage(AttributeTargets.Field)]
public class ReadOnlyAttribute : PropertyAttribute
{
    // Bu özel bir PropertyAttribute olarak çalýþýr
    // Ýlgili Property Drawer EditMode klasöründe tanýmlanmalýdýr
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // GUI'yi devre dýþý býrakarak salt okunur hale getir
            bool previousGUIState = GUI.enabled;
            GUI.enabled = false;

            // Varsayýlan property field'ý göster ama devre dýþý olarak
            EditorGUI.PropertyField(position, property, label);

            // GUI durumunu eski haline getir
            GUI.enabled = previousGUIState;
        }
    }
}
#endif