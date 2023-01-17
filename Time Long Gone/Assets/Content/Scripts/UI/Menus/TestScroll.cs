using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestScroll : MonoBehaviour
{
    [SerializeField] float ScrollSpeed;
    [SerializeField] float FastScroll;

    private float activeSpeed;
    private float inputY = 0;
    void Update()
    {
        activeSpeed = Mathf.Lerp(ScrollSpeed, (inputY > 0 ? FastScroll : -FastScroll), Mathf.Abs(inputY));
        transform.position += new Vector3(0, activeSpeed*Time.deltaTime, 0);
    }

    public void ScrollWihtInput(InputAction.CallbackContext ctx)
    {
        inputY = ctx.ReadValue<Vector2>().y;
    }
}
