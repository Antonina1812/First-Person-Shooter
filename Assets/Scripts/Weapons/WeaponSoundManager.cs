using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // �������� �����
    [SerializeField] private AudioClip shootSound;    // ���� ��������
    [SerializeField] private AudioClip reloadSound;   // ���� �����������

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // ��������������� ����� ��������
    public void PlayShootSound()
    {
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    // ��������������� ����� �����������
    public void PlayReloadSound()
    {
        if (reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }
}
