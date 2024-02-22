using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LayerMask groundLayer;
    private Rigidbody rb;

    public float moveSpeed = 3f;
    public bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement direction
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        // Move the player
        transform.Translate(movement * moveSpeed * Time.deltaTime);

        isGrounded = IsGrounded();
    }

    private bool IsGrounded()
    {
        // Perform a raycast downwards to check if the player is grounded on a tile
        Debug.DrawRay(transform.position, Vector3.down.normalized * 0.1f, Color.red);

        return Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);
    }
}
