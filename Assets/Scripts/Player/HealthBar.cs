using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private float health = 100f;
    public Image healthBar;
    public Text healthText;
    private bool isMenuActive = false;
    public GameObject pause;
    void Start()
    {
        pause.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text=health.ToString();
        if(Input.GetKeyDown(KeyCode.T) && !isMenuActive)
        {
            Debug.Log("Yes");
            pause2.SetActive(true);
            isMenuActive = true;
        }
        if (Input.GetKeyDown(KeyCode.T) && isMenuActive)
        {
            pause.SetActive(false);
            isMenuActive = false;
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / 100;
        Debug.Log(health);
    }
}
