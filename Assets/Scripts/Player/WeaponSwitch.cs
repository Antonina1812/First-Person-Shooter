using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject grenade;
    [SerializeField] Transform grenadeSpawnPoint; // Позиция появления гранаты

    private GameObject activeGrenade;

    private void Update()
    {
        // Переключение между оружием и гранатой
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (weapon.activeInHierarchy)
            {
                weapon.SetActive(false);
                grenade.SetActive(true);
            }
            else if (grenade.activeInHierarchy)
            {
                grenade.SetActive(false);
                weapon.SetActive(true);
            }
        }

        // Бросок гранаты
        if (grenade.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            ThrowGrenade();
        }
    }

    private void ThrowGrenade()
    {
        if (grenadeSpawnPoint == null)
        {
            Debug.LogError("Grenade Spawn Point is not assigned in the inspector.");
            return;
        }

        if (activeGrenade != null) return;

        // Создание гранаты перед игроком
        activeGrenade = Instantiate(grenade, grenadeSpawnPoint.position, grenadeSpawnPoint.rotation);

        // Добавление компонента Rigidbody только при броске
        Rigidbody rb = activeGrenade.AddComponent<Rigidbody>();
        rb.mass = 1f; // Масса гранаты

        // Получаем направление броска
        Vector3 throwDirection = CalculateThrowDirection();

        // Применяем силу в это направление
        rb.AddForce(throwDirection * 10f, ForceMode.Impulse);

        // Отключаем триггер для включения физики (если Collider уже был на гранате)
        Collider grenadeCollider = activeGrenade.GetComponent<Collider>();
        if (grenadeCollider != null)
        {
            grenadeCollider.isTrigger = false; // Отключаем триггер
        }

        // Передаем состояние гранаты "брошена"
        Grenade grenadeComponent = activeGrenade.GetComponent<Grenade>();
        grenadeComponent.hasBeenThrown = true;

        // Переключаем гранату в оружие
        grenade.SetActive(false);
        weapon.SetActive(true);
    }


    // Метод для вычисления направления броска
    private Vector3 CalculateThrowDirection()
    {
        // Луч из центра экрана
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // Используем только направление из луча
        return ray.direction;
    }




}
