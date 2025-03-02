using System;
using UnityEngine;
using TMPro;
using Fusion;
using Unity.VisualScripting;
public class CarController : NetworkBehaviour
{
    public Transform lastCheckpoint; // Stores the last crossed checkpoint
    public float currentTime;
    public float finalTime;
    public bool timeElapsed=false;
    public string lapTime;
    public TextMeshProUGUI timerText;
    public int lapCount = -1;
    public NetworkId NetworkCarId => Object.Id;
    public GameMode1 gameMode1;
    private void Start()
    {
        gameMode1 = FindAnyObjectByType<GameMode1>();
        lastCheckpoint = transform; // Default to the car's starting position
        Debug.Log($"{gameObject.name} initialized at {transform.position} and has NetworkID {NetworkCarId}");
    }
    private void OnTriggerEnter(Collider other)
    {
        // ✅ Checkpoint Logic
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpoint = other.transform;
            Debug.Log($"{gameObject.name} crossed checkpoint {other.gameObject.name} at {other.transform.position}");
        }

        // ☠ Death Zone Logic
        if (other.CompareTag("DeathZone"))
        {
            Debug.Log($"{gameObject.name} hit the death zone at {other.transform.position}");
            Respawn();
        }
        // Gamemode 1 logic
        if (other.CompareTag("Finish")){
            if(timeElapsed){
                lapCount++;
                if(lapCount==1){
                    timeElapsed=false;
                    finalTime=currentTime;
                    gameMode1.StoreTimes(NetworkCarId,finalTime);
                }
            }
            else{
                timeElapsed=true;
                lapCount++;
            }
        }
    }

    private void Update()
    {
        if(timeElapsed){
            currentTime+=Time.deltaTime;
            DisplayTime(currentTime);
        }
    }
    private void DisplayTime(float timeDisplay){
        float minutes = Mathf.FloorToInt(timeDisplay / 60);
        float seconds = Mathf.FloorToInt(timeDisplay % 60);

        lapTime = string.Format("{0:00}:{1:00}",minutes,seconds);
        if(timerText != null){
            timerText.text = lapTime;
        }
        Debug.Log(lapTime);
    }
    public void Respawn()
    {
        if (lastCheckpoint != null)
        {
            Debug.Log($"{gameObject.name} respawning to last checkpoint at {lastCheckpoint.position}");
            transform.position = lastCheckpoint.position;
            transform.rotation = lastCheckpoint.rotation;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero; // Stop momentum
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no checkpoint set! Respawn failed.");
        }
    }
}
