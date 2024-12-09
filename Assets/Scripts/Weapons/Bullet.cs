using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletMarkPrefab; // Ïðåôàá ñëåäà îò ïóëè
    [SerializeField] private float bulletMarkLifetime = 10f; // Âðåìÿ æèçíè ñëåäà

    private int bulletDamage = 25; //Óðîí ïóëè

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) //Åñëè ïîïàëè â èãðîêà
        {
            Debug.Log("Ïîïàëè â èãðîêà");
            Debug.Log("Player");
            collision.gameObject.GetComponentInParent<HealthBar>().TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy")) //Åñëè ïîïàëè âî âðàãà
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
        else //Åñëè íå ïîïàëè âî âðàãà èëè â èãðîêà
        {
            if (bulletMarkPrefab == null)
            {
                Debug.LogError("Ïðåôàá ñëåäà îò ïóëè íå íàçíà÷åí");
                return;
            }

            // Ñîçäàåì ñëåä íà ìåñòå ñòîëêíîâåíèÿ
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.LookRotation(-contact.normal); // Ïîâîðà÷èâàåì ñëåä â íàïðàâëåíèè íîðìàëè
            GameObject bulletMark = Instantiate(bulletMarkPrefab, contact.point, rotation);

            // Ñìåùàåì ñëåä íåìíîãî íàðóæó ïî íîðìàëè
            bulletMark.transform.position += contact.normal * 0.01f;

            // Óñòàíàâëèâàåì ìàñøòàá
            bulletMark.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

            // Ïðèâÿçûâàåì ñëåä ê îáúåêòó
            bulletMark.transform.SetParent(collision.transform);

            // Óíè÷òîæàåì ñëåä ÷åðåç çàäàííîå âðåìÿ
            Destroy(bulletMark, bulletMarkLifetime);

            // Óíè÷òîæàåì ïóëþ
            Destroy(gameObject);
        }
    }
}
