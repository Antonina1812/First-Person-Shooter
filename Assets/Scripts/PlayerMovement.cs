using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    private float rotationX = 0f;

    void Update()
    {
        // Получаем ввод с клавиатуры
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Двигаем игрока
        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        transform.position += movement * moveSpeed * Time.deltaTime;

        // Получаем ввод мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Вращаем камеру
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}