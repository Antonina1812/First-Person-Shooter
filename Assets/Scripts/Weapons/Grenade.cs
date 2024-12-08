using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float delay = 2f;
    [SerializeField] float damageRadius = 20f;
    [SerializeField] float explosionForce = 1200f;
    float countdown;
    bool hasExploded = false;
    public bool hasBeenThrown = false;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if (hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    private void Explode()
    {
        CreateExplosionEffect();
        ApplyDamageAndForce();
        Destroy(gameObject);
    }

    private void CreateExplosionEffect()
    {
        // ���������� ������ ������
        GameObject explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
    }

    private void ApplyDamageAndForce()
    {
        // ���������� ���� ������ � ��������
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }

            // ������ ��������� �����
            // ����� �������� ���� ������, ���� � ��� ���� ������ Health
        }
    }
}
