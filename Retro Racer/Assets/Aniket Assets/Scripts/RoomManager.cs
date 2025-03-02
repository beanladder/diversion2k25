using Fusion;
using UnityEngine;
using TMPro;

public class RoomManager : MonoBehaviour
{
    public NetworkRunner runner; // Reference to the NetworkRunner component
    public TMP_InputField roomCodeInput; // Input field for the room code
    public TMP_InputField userName;
    public GameObject UIScreen;

    // Method to create a new room with the entered code
    public async void CreateRoom()
    {
        string roomCode = roomCodeInput.text;

        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("Room code cannot be empty.");
            return;
        }

        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomCode, // Use the entered code as the session name
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        var result = await runner.StartGame(startGameArgs);

        if (result.Ok)
        {
            Debug.Log("Room created successfully.");
            // Load the gameplay scene or perform other actions as needed
        }
        else
        {
            Debug.LogError($"Failed to create room: {result.ShutdownReason}");
        }
        UIScreen.SetActive(false);
    }
    public async void JoinRoom()
    {
        string roomCode = roomCodeInput.text;

        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("Room code cannot be empty.");
            return;
        }

        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomCode, // Use the entered code as the session name
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        var result = await runner.StartGame(startGameArgs);

        if (result.Ok)
        {
            Debug.Log("Joined room successfully.");
            // Load the gameplay scene or perform other actions as needed
        }
        else
        {
            Debug.LogError($"Failed to join room: {result.ShutdownReason}");
            // Optionally, inform the user that the room does not exist
        }
        UIScreen.SetActive(false);
    }

}
