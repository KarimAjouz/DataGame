#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.DreamOS
{
    [CustomEditor(typeof(DateAndTime))]
    public class DateAndTimeEditor : Editor
    {
        private DateAndTime dtTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            dtTarget = (DateAndTime)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = DreamOSEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = DreamOSEditorHandler.GetLightEditor(customSkin); }
        }

        override public void OnInspectorGUI()
        {
            var enableAmPmLabel = serializedObject.FindProperty("enableAmPmLabel");
            var addSeconds = serializedObject.FindProperty("addSeconds");
            var objectType = serializedObject.FindProperty("objectType");
            var dateFormat = serializedObject.FindProperty("dateFormat");
            
            /// #BeginKazChange [KA] (31.12.2024): Update Date and Time manager with a parameter to stop time stepping in real time.
            var updateInRealTime = serializedObject.FindProperty("UpdateInRealTime");
            /// #EndKazChange [KA] (31.12.2024): Update Date and Time manager with a parameter to stop time stepping in real time.

            var clockHourHand = serializedObject.FindProperty("clockHourHand");
            var clockMinuteHand = serializedObject.FindProperty("clockMinuteHand");
            var clockSecondHand = serializedObject.FindProperty("clockSecondHand");
            var textObj = serializedObject.FindProperty("textObj");

            DreamOSEditorHandler.DrawHeader(customSkin, "Header_Settings", 6);
            if (dtTarget.objectType == DateAndTime.ObjectType.AnalogClock) { GUI.enabled = false; }
            enableAmPmLabel.boolValue = DreamOSEditorHandler.DrawToggle(enableAmPmLabel.boolValue, customSkin, "Enable AM/PM Label");
            
            /// #BeginKazChange [KA] (31.12.2024): Update Date and Time manager with a parameter to stop time stepping in real time.
            updateInRealTime.boolValue = DreamOSEditorHandler.DrawToggle(updateInRealTime.boolValue, customSkin, "Update in Real Time");
            /// #EndKazChange [KA] (31.12.2024): Update Date and Time manager with a parameter to stop time stepping in real time.

            GUI.enabled = true;
            addSeconds.boolValue = DreamOSEditorHandler.DrawToggle(addSeconds.boolValue, customSkin, "Add Seconds");
            DreamOSEditorHandler.DrawProperty(objectType, customSkin, "Object Type");
            DreamOSEditorHandler.DrawProperty(dateFormat, customSkin, "Date Format");

            if (dtTarget.objectType == DateAndTime.ObjectType.DigitalDate || dtTarget.objectType == DateAndTime.ObjectType.DigitalClock)
            {
                DreamOSEditorHandler.DrawProperty(textObj, customSkin, "Text Object");
            }

            else if (dtTarget.objectType == DateAndTime.ObjectType.AnalogClock)
            {
                DreamOSEditorHandler.DrawProperty(clockHourHand, customSkin, "Hour Hand");
                DreamOSEditorHandler.DrawProperty(clockMinuteHand, customSkin, "Minute Hand");
                DreamOSEditorHandler.DrawProperty(clockSecondHand, customSkin, "Second Hand");
            }

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif