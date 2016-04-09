using UnityEngine;

namespace Transit.Framework.UI
{
    public class InputEvent
    {
        public KeyCode? KeyCode { get; set; }
        public MouseKeyCode? MouseKeyCode { get; set; }

        public static InputEvent None = new InputEvent();
    }
}