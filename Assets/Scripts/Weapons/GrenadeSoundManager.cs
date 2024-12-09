using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Источник звука
    [SerializeField] private AudioClip explosionSound; // Звук взрыва гранаты
    public AudioClip ExplosionSound => explosionSound; // Свойство для доступа к звуку

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Воспроизведение звука взрыва гранаты
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
