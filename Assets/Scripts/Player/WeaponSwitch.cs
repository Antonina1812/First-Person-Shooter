using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject grenade;
    [SerializeField] Transform grenadeSpawnPoint; // ������� ��������� �������

    private GameObject activeGrenade;

    private void Update()
    {
        // ������������ ����� ������� � ��������
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

        // ������ �������
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

        // �������� ������� ����� �������
        activeGrenade = Instantiate(grenade, grenadeSpawnPoint.position, grenadeSpawnPoint.rotation);

        // ���������� ���������� Rigidbody ������ ��� ������
        Rigidbody rb = activeGrenade.AddComponent<Rigidbody>();
        rb.mass = 1f; // ����� �������

        // �������� ����������� ������
        Vector3 throwDirection = CalculateThrowDirection();

        // ��������� ���� � ��� �����������
        rb.AddForce(throwDirection * 10f, ForceMode.Impulse);

        // ��������� ������� ��� ��������� ������ (���� Collider ��� ��� �� �������)
        Collider grenadeCollider = activeGrenade.GetComponent<Collider>();
        if (grenadeCollider != null)
        {
            grenadeCollider.isTrigger = false; // ��������� �������
        }

        // �������� ��������� ������� "�������"
        Grenade grenadeComponent = activeGrenade.GetComponent<Grenade>();
        grenadeComponent.hasBeenThrown = true;

        // ����������� ������� � ������
        grenade.SetActive(false);
        weapon.SetActive(true);
    }


    // ����� ��� ���������� ����������� ������
    private Vector3 CalculateThrowDirection()
    {
        // ��� �� ������ ������
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // ���������� ������ ����������� �� ����
        return ray.direction;
    }




}
