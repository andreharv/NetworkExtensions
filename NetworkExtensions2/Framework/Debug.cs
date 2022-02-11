using System;

namespace Transit.Framework
{
#if DEBUG
    public static class Debug
    {
        public static void Out(ColossalFramework.Plugins.PluginManager.MessageType messageType, bool useComma, params System.Object[] o)
        {
            try
            {
                string s = "";
                for (int i = 0; i < o.Length; i++)
                {
                    s += o[i].ToString();
                    if (i < o.Length - 1 && useComma)
                        s += "  ,  ";
                }
                DebugOutputPanel.AddMessage(messageType, s);
            }
            catch (Exception)
            {
            }
        }

        public static void Log(params System.Object[] o)
        {
            Message(o);
        }

        public static void Message(params System.Object[] o)
        {
            Message(true, o);
        }

        public static void Message(bool useComma, params System.Object[] o)
        {
            Out(ColossalFramework.Plugins.PluginManager.MessageType.Message, useComma, o);
        }

        public static void Warning(params System.Object[] o)
        {
            Warning(true, o);
        }

        public static void Warning(bool useComma, params System.Object[] o)
        {
            Out(ColossalFramework.Plugins.PluginManager.MessageType.Warning, useComma, o);
        }

        public static void Error(params System.Object[] o)
        {
            Error(true, o);
        }

        public static void Error(bool useComma, params System.Object[] o)
        {
            Out(ColossalFramework.Plugins.PluginManager.MessageType.Error, useComma, o);
        }
    }
#endif
}
