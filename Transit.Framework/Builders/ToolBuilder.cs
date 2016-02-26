using System;

namespace Transit.Framework.Builders
{
    public abstract class ToolBuilder<T> : Activable //Implementation of IToolBuilder is useless here
        where T: ToolBase
    {
        public bool IsInstalled { get; set; }
        public Type ToolType { get { return typeof (T); } }

        public void OnToolInstalled(ToolBase tool)
        {
            OnToolInstalled((T)tool);
        }

        protected virtual void OnToolInstalled(T tool) { }

        public void OnToolUninstalling(ToolBase tool)
        {
            OnToolUninstalling((T)tool);
        }

        protected virtual void OnToolUninstalling(T tool) { }
    }
}
