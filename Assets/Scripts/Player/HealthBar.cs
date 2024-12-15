using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float health = 100f;
    public Image healthBar;
    public Text healthText;
    public GameObject deadPause;
    private bool isDead=false;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text=health.ToString();
        if(health <= 0 && !isDead)
        {
            isDead=true;
            deadPause.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / 100;
    }
}
