using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // Public Variables: Player Health Settings
    [Header("Player Health Settings")]
    public int maxHealth = 100; // Maximum health of the player
    public float HealthDecreaseSpeed; // Speed at which health decreases when away from light

    // Public Variables: UI Elements
    [Header("UI Elements")]
    public GameObject GameOverPanel; // Panel displayed on game over
    public Slider healthbar; // Health bar slider UI

    // Private Variables
    private float currentHealth; // Current health of the player
    private Animator anim; // Animator component for animations
    private bool IsCloseToLight = true; // Checks if the player is close to light
    private bool isDead = false; // Tracks if the player is dead

    void Start()
    {
        // Initialize variables
        anim = GetComponent<Animator>();
        currentHealth = maxHealth; // Set current health to max health
        GameOverPanel.SetActive(false); // Hide game over panel at start
        healthbar.maxValue = maxHealth; // Set health bar max value
        healthbar.value = maxHealth; // Set health bar initial value
    }

    private void Update()
    {
        // Decrease health when away from light
        if (!IsCloseToLight && currentHealth > 0)
        {
            currentHealth -= 1 * Time.deltaTime * HealthDecreaseSpeed;
        }
        // Gradually recover health when close to light
        else if (IsCloseToLight && currentHealth < maxHealth)
        {
            currentHealth += 1 * Time.deltaTime;
        }

        // Handle player death
        if (currentHealth <= 0 && !isDead)
        {
            StartCoroutine(Die());
        }

        // Update health bar value
        healthbar.value = currentHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce health by damage amount
        anim.SetTrigger("Damage"); // Play damage animation
    }

    IEnumerator Die()
    {
        isDead = true;
        anim.SetTrigger("Death"); // Play death animation
        yield return new WaitForSeconds(2f); // Wait before disabling the player
        this.gameObject.SetActive(false); // Disable player object
        Time.timeScale = 0; // Pause the game
        GameOverPanel.SetActive(true); // Show game over panel
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Detect proximity to light
        if (collision.gameObject.CompareTag("Light"))
        {
            IsCloseToLight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Detect leaving light area
        if (collision.gameObject.CompareTag("Light"))
        {
            IsCloseToLight = false;
        }
    }
}
