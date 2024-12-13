using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private float health = 100f;
    public Image healthBar;
    public Text healthText;
    public GameObject pause;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text=health.ToString();
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / 100;
    }
}
