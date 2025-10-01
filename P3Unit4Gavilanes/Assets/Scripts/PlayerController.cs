using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Rigidbody playerRb;
    public float speed = 2.0f;
    public bool hasPowerup;
    private float powerupStrength = 15.0f;

    private GameObject focalPoint;

    public GameObject powerUpIndicator;

    public PowerUpType currentPowerUp = PowerUpType.None;

    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountDown;

    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;

    bool smashing = false;
    float floorY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("FocalPoint");
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);

        powerUpIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);

        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets();
        }

        if(currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerup = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            powerUpIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);

            if(powerupCountDown != null)
            {
                StopCoroutine(powerupCountDown);
            }
            powerupCountDown = StartCoroutine(PowerUpCountdownRoutine());
        }
    }

    IEnumerator PowerUpCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        currentPowerUp = PowerUpType.None;
        powerUpIndicator.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.Pushback)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);
            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);

            Debug.Log("Player collided with: " + collision.gameObject.name + " with powerup set to " + currentPowerUp.ToString());
        }
    }

    void LaunchRockets()
    {
        foreach(var enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehavior>().Fire(enemy.transform);
        }
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        //Store the y position before taking off
        floorY = transform.position.y;

        //Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;
        
        while(Time.time < jumpTime)
        {
            //move the player up while still keeping their x velocity
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, smashSpeed);
            yield return null;
        }

        //Noiw move the player down
        while(transform.position.y > floorY)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, -smashSpeed * 2);
            yield return null;
        }

        //Cycle through all enemies
        for (int i = 0; i < enemies.Length; i++)
        {
            //Apply an explosion force that originates from our position
            if (enemies[i] != null)
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
            }
        }

        //We are no longer smashing, so set the boolean to false
        smashing = false;
    }
}
