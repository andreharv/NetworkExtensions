using System.Reflection;
using UnityEngine;

namespace Transit.Framework.UI
{
    public static class ToolControllerExtensions
    {
        public static void AddTool<T>(this ToolController toolController) where T : ToolBase
        {
            if (toolController.GetComponent<T>() != null)
                return;

            toolController.gameObject.AddComponent<T>();

            UpdateTools(toolController);
        }

        public static void RemoveTool<T>(this ToolController toolController) where T : ToolBase
        {
            T tool = toolController.GetComponent<T>();
            if (tool == null)
                return;

            GameObject.Destroy(tool);

            UpdateTools(toolController);
        }

        // contributed by Japa
        private static void UpdateTools(ToolController toolController)
        {
            FieldInfo toolControllerField = typeof(ToolController).GetField("m_tools", BindingFlags.Instance | BindingFlags.NonPublic);
            if (toolControllerField != null)
                toolControllerField.SetValue(toolController, toolController.GetComponents<ToolBase>());

            FieldInfo toolModifierDictionary = typeof(ToolsModifierControl).GetField("m_Tools", BindingFlags.Static | BindingFlags.NonPublic);
            if (toolModifierDictionary != null)
                toolModifierDictionary.SetValue(null, null); // to force a refresh

            ToolsModifierControl.SetTool<DefaultTool>();
        }
    }
}
