using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletMarkPrefab; // Префаб следа от пули
    [SerializeField] private float bulletMarkLifetime = 10f; // Время жизни следа

    private int bulletDamage = 25; //Урон пули

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) //Если попали в игрока
        {
            Debug.Log("Попали в игрока");
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy")) //Если попали во врага
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
        else //Если не попали во врага или в игрока
        {
            if (bulletMarkPrefab == null)
            {
                Debug.LogError("Префаб следа от пули не назначен");
                return;
            }

            // Создаем след на месте столкновения
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.LookRotation(-contact.normal); // Поворачиваем след в направлении нормали
            GameObject bulletMark = Instantiate(bulletMarkPrefab, contact.point, rotation);

            // Смещаем след немного наружу по нормали
            bulletMark.transform.position += contact.normal * 0.01f;

            // Устанавливаем масштаб
            bulletMark.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

            // Привязываем след к объекту
            bulletMark.transform.SetParent(collision.transform);

            // Уничтожаем след через заданное время
            Destroy(bulletMark, bulletMarkLifetime);

            // Уничтожаем пулю
            Destroy(gameObject);
        }
    }
}
