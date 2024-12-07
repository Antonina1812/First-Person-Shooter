using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 12f;
    private float gravity = -9.81f * 2;
    private CharacterController charController;
    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal");
        float deltaZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement *= Time.deltaTime * moveSpeed;
        movement = transform.TransformDirection(movement);

        movement.y = gravity * Time.deltaTime;

        charController.Move(movement);
    }
}
