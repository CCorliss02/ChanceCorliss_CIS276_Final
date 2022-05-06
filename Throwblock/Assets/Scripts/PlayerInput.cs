using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private const string HORIZONTAL_INPUT = "Horizontal";
    private const string VERTICAL_INPUT = "Vertical";
    private const string CAM_HORIZONTAL_INPUT = "Mouse X";
    private const string CAM_VERTICAL_INPUT = "Mouse Y";
    private const string JUMP_INPUT = "Jump";
    private const string SPAWN_BOX_INPUT = "Fire3";
    private const string DASH_BOX_INPUT = "Fire1";

    public bool isInputEnabled;
    public Vector3 moveInput { get; private set; }
    public bool jumpInput { get; private set; }
    public Vector3 camMoveInput { get; private set; }
    public bool spawnBoxInput { get; private set; }
    public bool dashBoxInput { get; private set; }

    private void SetMoveInputs()
    {
        moveInput = new Vector3(Input.GetAxis(HORIZONTAL_INPUT), 0, Input.GetAxis(VERTICAL_INPUT));
        jumpInput = Input.GetButtonDown(JUMP_INPUT);
        camMoveInput = new Vector3(Input.GetAxis(CAM_HORIZONTAL_INPUT), Input.GetAxis(CAM_VERTICAL_INPUT), 0);
        spawnBoxInput = Input.GetButtonDown(SPAWN_BOX_INPUT);
        dashBoxInput = Input.GetButtonDown(DASH_BOX_INPUT);
    }

    private void ResetInputs()
    {
        moveInput = Vector3.zero;
        jumpInput = false;
        camMoveInput = Vector3.zero;
        spawnBoxInput = false;
        dashBoxInput = false;
    }

    private void Start()
    {
        isInputEnabled = true;
    }

    private void Update()
    {
        if (isInputEnabled)
        {
            SetMoveInputs();
        }

        else
        {
            ResetInputs();
        }
    }
}
