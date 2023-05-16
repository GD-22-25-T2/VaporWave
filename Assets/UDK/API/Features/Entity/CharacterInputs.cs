using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputs : MonoBehaviour
{
    internal event EventManager.InputEventFloat WalkEvent;
    internal event EventManager.InputEventBool JumpEvent;

    void Update()
    {
        KeyboardInput();
    }

    void KeyboardInput()
    {
        float m_speed = Input.GetAxis("Horizontal");

        if(m_speed != 0) WalkEvent?.Invoke(Input.GetAxis("Horizontal"));
        

        JumpEvent?.Invoke(Input.GetKey(KeyCode.Space));

    }

}
