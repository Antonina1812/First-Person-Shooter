// WeaponSwitch.cs
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject grenade;

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
    }

    public void SwitchToWeapon()
    {
        grenade.SetActive(false);
        weapon.SetActive(true);
    }
}
