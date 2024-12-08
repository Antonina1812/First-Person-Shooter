using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Источник звука
    [SerializeField] private AudioClip shootSound;    // Звук стрельбы
    [SerializeField] private AudioClip reloadSound;   // Звук перезарядки

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Воспроизведение звука стрельбы
    public void PlayShootSound()
    {
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    // Воспроизведение звука перезарядки
    public void PlayReloadSound()
    {
        if (reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }
}
