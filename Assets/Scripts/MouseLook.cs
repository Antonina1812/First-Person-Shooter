using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Чувствительность мыши
    public Transform playerBody; // Ссылка на объект игрока
    private float xRotation = 0f; // Угол вращения по оси X

    void Start()
    {
        // Скрыть курсор и зафиксировать его в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Получаем движение мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Вращаем камеру по оси Y (горизонтальное вращение)
        playerBody.Rotate(Vector3.up * mouseX);

        // Ограничиваем вращение по оси X (вертикальное вращение)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Ограничиваем угол вращения
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}