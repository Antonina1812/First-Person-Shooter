using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // �������� �����
    [SerializeField] private AudioClip explosionSound; // ���� ������ �������
    public AudioClip ExplosionSound => explosionSound; // �������� ��� ������� � �����

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // ��������������� ����� ������ �������
    public void PlayExplosionSound()
    {
        if (explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
        else
        {
            Debug.LogWarning("Explosion sound is not assigned!");
        }
    }
}
