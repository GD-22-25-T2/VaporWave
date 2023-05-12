using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    internal delegate void InputEventFloat(float value);

    internal delegate void InputEventBool(bool state);
}
