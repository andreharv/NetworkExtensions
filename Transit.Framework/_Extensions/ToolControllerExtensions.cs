using System.Reflection;
using UnityEngine;

namespace Transit.Framework
{
    public static class ToolControllerExtensions
    {
        public static T AddTool<T>(this ToolController toolController) where T : ToolBase
        {
            var toolInstance = toolController.GetComponent<T>();
            if (toolInstance != null)
                return toolInstance;

            toolInstance = toolController.gameObject.AddComponent<T>();

            UpdateTools(toolController);

            return toolInstance;
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
