using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Public Variables: UI Panels
    [Header("UI Panels")]
    public GameObject pauseMenuPanel; // Pause menu panel
    public GameObject quitMenuPanel; // Quit confirmation panel
    public GameObject pausePanel; // Pause overlay panel
    public GameObject ControlPanel; // Control instructions panel

    // Public Variables: Level Settings
    [Header("Level Settings")]
    public int currentLevelNum; // Current level number

    // Public Variables: Mirror Settings
    [Header("Mirror Settings")]
    public GameObject mirrorPrefab; // Prefab for the mirror
    public GameObject player; // Reference to the player
    public int mirrorCount; // Number of mirrors available
    public TextMeshProUGUI num; // UI text to display mirror count
    public GameObject poolParent;//assign single parent to all the created mirror for pool

    // Public Variables: Audio
    [Header("Audio Settings")]
    public AudioSource bgm; // Background music audio source
    public AudioClip bg; // Alternate background music clip
    public bool isChangebgm = false; // Whether to change the BGM after narration

    // Hidden/Public Variables
    [HideInInspector]
    public bool isMirrorInHand = false; // Whether a mirror is currently in hand
    [HideInInspector]
    public GameObject MirrorInHand; // Reference to the mirror in hand

    // Private Variables
    private bool isPlayingNarration = true; // Whether narration audio is playing
    [HideInInspector]
    public bool gamePaused = false; //to check if game is paused or not
    // Singleton Instance
    public static GameManager instance;
    private Queue<GameObject> mirrorPool =new Queue<GameObject>();
    void Start()
    {
        // Initialize game state
        Time.timeScale = 1;
        if(instance==null)
        {
            instance = this;
        }
       // Cursor.visible = false;
        // Set initial UI states
        pauseMenuPanel.SetActive(false);
        quitMenuPanel.SetActive(false);
        pausePanel.SetActive(false);
        InitializeMirrorPool();
        // Start music coroutine
        StartCoroutine(PlayMusic());
    }

    void Update()
    {
        // Handle pause menu toggle
        if (Input.GetKey(KeyCode.Escape))
        {
            PauseGame();
        }

        // Handle mirror destruction
        if (isMirrorInHand && Input.GetButton("Fire2"))
        {
            ReturnMirrorToPool(MirrorInHand);
        }

        // Update mirror count display
        num.text = mirrorCount.ToString();
    }

    // Pause Menu Functions
    public void OnResumeButtonClicked()
    {
        ResumeGame();
    }

    public void OnCloseButtonClicked()
    {
        ControlPanel.SetActive(false);
    }

    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(currentLevelNum);
        Time.timeScale = 1f;
    }

    public void OnQuitButtonClicked()
    {
        quitMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
    }

    public void OnYesButtonClicked()
    {
        SceneManager.LoadScene(0); // Go to main menu or home screen
    }

    public void OnNoButtonClicked()
    {
        quitMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    // Spawn Mirror
    private void InitializeMirrorPool()
    {
        for (int i = 0; i < mirrorCount; i++)
        {
            GameObject mirror = Instantiate(mirrorPrefab,poolParent.transform);
            mirror.SetActive(false);
            mirrorPool.Enqueue(mirror);
        }
    }

    // Get a mirror from the pool
    private GameObject GetMirrorFromPool()
    {
        if (mirrorPool.Count > 0)
        {
            GameObject mirror = mirrorPool.Dequeue();
            mirror.SetActive(true);
            return mirror;
        }
        else
        {
            // If the pool is empty, create a new mirror (optional)
            GameObject mirror = Instantiate(mirrorPrefab,poolParent.transform);
            mirror.SetActive(true);
            return mirror;
        }
    }

    // Return a mirror to the pool
    private void ReturnMirrorToPool(GameObject mirror)
    {
        mirror.SetActive(false);
        mirrorPool.Enqueue(mirror);
        isMirrorInHand = false;
    }

    // Spawn Mirror
    public void SpawnMirror()
    {
        if (mirrorCount > 0 && !isMirrorInHand)
        {
            isMirrorInHand = true;
            MirrorInHand = GetMirrorFromPool();
            MirrorInHand.transform.position = new Vector2(player.transform.position.x + 3f, player.transform.position.y);
            MirrorInHand.transform.rotation = quaternion.identity;
            MirrorInHand.transform.parent = player.transform;
            MirrorInHand.transform.localScale = mirrorPrefab.transform.localScale;
        }
    }


    // Home and Retry Functions
    public void OnHomeButtonClicked()
    {
        SceneManager.LoadScene(0); // Go to main menu or home screen
    }

    public void OnRetryButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(currentLevelNum); // Reload current level
    }

    // Background Music Control
    IEnumerator PlayMusic()
    {
        if (isChangebgm)
        {
            yield return new WaitForSeconds(bgm.clip.length); // Wait for narration to finish
            bgm.clip = bg;
            bgm.loop = true;
            bgm.Play();
            isPlayingNarration = false;
        }
    }

    // Helper Functions
    private void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        pausePanel.SetActive(true);
        gamePaused = true;
       // Cursor.visible = true;
        Time.timeScale = 0f;

        if (isPlayingNarration)
        {
            bgm.Pause();
        }
    }

    private void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        pausePanel.SetActive(false);
        gamePaused = false;
       // Cursor.visible = false;
        Time.timeScale = 1f;
        bgm.UnPause();
    }
}
