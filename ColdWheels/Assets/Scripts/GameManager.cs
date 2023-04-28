using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool paused;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    public float timePassed;
    public CarSingle car;
    public Bot[] bots;
    public GameObject pauseMenu;
    public bool raceStarted;
    public float countdown;

    public Text countdownText;
    public Text lapsText;
    public Slider nitroSlider;
    public GameObject[] powerUps;
    private bool powerUpsActive = false;

    void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0) StartCoroutine(StartRace());
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) Pause(!paused);
    }

    private void FixedUpdate()
    {
        if (raceStarted)
        {
            timePassed += Time.fixedDeltaTime;
            float minutes = Mathf.FloorToInt(timePassed / 60);
            float seconds = Mathf.FloorToInt(timePassed % 60);
            countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void ActivatePowerUps()
    {
        if (powerUpsActive) return;
        powerUpsActive = true;
        foreach (GameObject powerUp in powerUps) powerUp.SetActive(true);
    }

    public void Pause(bool pause)
    {
        if (CanvasManager.multiplayer == 1 || SceneManager.GetActiveScene().buildIndex == 0) return;

        paused = pause;
        pauseMenu.SetActive(pause);
        if (pause)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public IEnumerator StartRace()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        countdown = 5;

        car.laps = 0;
        car.canMove = false;
        foreach (WheelCollider wheel in car.wheels)
            wheel.brakeTorque = car.brakeForce;

        foreach (Bot bot in bots)
        {
            bot.car.laps = 0;
            bot.car.canMove = false;
            foreach (WheelCollider wheel in bot.car.wheels)
                wheel.brakeTorque = bot.car.brakeForce;
        }

        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            countdownText.text = Mathf.Ceil(countdown).ToString();

            yield return null;
        }

        car.canMove = true;
        foreach (WheelCollider wheel in car.wheels)
            wheel.brakeTorque = 0;

        foreach (Bot bot in bots)
        {
            bot.car.canMove = true;
            foreach (WheelCollider wheel in bot.car.wheels)
                wheel.brakeTorque = 0;
        }

        countdownText.text = "";
        raceStarted = true;
    }

    public IEnumerator FinishRace()
    {
        car.canMove = false;
        foreach (WheelCollider wheel in car.wheels)
        {
            wheel.brakeTorque = car.brakeForce;
            wheel.motorTorque = 0;
        }

        countdown = 3;

        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            countdownText.text = Mathf.Ceil(countdown).ToString();

            yield return null;
        }

        SceneManager.LoadScene(0);
        // Load scene received from server
    }

    public void LoadLevel(int index)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(index);
    }
}
