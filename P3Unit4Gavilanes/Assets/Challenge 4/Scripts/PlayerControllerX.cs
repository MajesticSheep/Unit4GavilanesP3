using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private float speed = 500;
    private float speeder = 1000;
    private GameObject focalPoint;

    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;
    private Coroutine powerupCountDown;

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup

    public GameObject particleSmokePrefab;
    public ParticleSystem particleSmoke;

    public bool canUseSpace = true;
    

    
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");

        GameObject smokeInstance = Instantiate(particleSmokePrefab, transform.position + new Vector3 (0.6f, 0.6f, 0), Quaternion.identity);

        particleSmoke = smokeInstance.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime); 

        // Set powerup indicator position to beneath player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);

        particleSmoke.transform.position = transform.position + new Vector3(0, -0.6f, 0);

        if (Input.GetKeyDown(KeyCode.Space) && canUseSpace)
        {
            playerRb.AddForce(focalPoint.transform.forward * speeder * verticalInput);

            particleSmoke.Play();

            StartCoroutine(SpaceBarCooldown());
        }
        else
        {
            playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime);
        }

    }

    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);

            if (powerupCountDown != null)
            {
                StopCoroutine(powerupCountDown);
            }
            powerupCountDown = StartCoroutine(PowerupCooldown());
        }
    }

    // Coroutine to count down powerup duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.gameObject.SetActive(false);
    }

    IEnumerator SpaceBarCooldown()
    {
        canUseSpace = false;
        yield return new WaitForSeconds(1);
        canUseSpace = true;
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer =  collision.gameObject.transform.position - transform.position; 
           
            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }


        }
    }

}
