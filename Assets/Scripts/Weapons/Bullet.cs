using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletMarkPrefab; // ������ ����� �� ����
    [SerializeField] private float bulletMarkLifetime = 10f; // ����� ����� �����

    private void OnCollisionEnter(Collision collision)
    {
        if (bulletMarkPrefab == null)
        {
            Debug.LogError("������ ����� �� ���� �� ��������");
            return;
        }

        // ������� ���� �� ����� ������������
        ContactPoint contact = collision.contacts[0];
        Quaternion rotation = Quaternion.LookRotation(-contact.normal); // ������������ ���� � ����������� �������
        GameObject bulletMark = Instantiate(bulletMarkPrefab, contact.point, rotation);

        // ������� ���� ������� ������ �� �������
        bulletMark.transform.position += contact.normal * 0.01f;

        // ������������� �������
        bulletMark.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        // ����������� ���� � �������
        bulletMark.transform.SetParent(collision.transform);

        // ���������� ���� ����� �������� �����
        Destroy(bulletMark, bulletMarkLifetime);

        // ���������� ����
        Destroy(gameObject);
    }
}
