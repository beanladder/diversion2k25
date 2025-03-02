using Fusion;
using UnityEngine;
using TMPro;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class RoomManager1 : MonoBehaviour, INetworkRunnerCallbacks
{   
    public NetworkRunner runner;
    public TMP_InputField roomCodeInput;
    public GameObject UIScreen;
    public Button createButton;
    public Button joinButton;
    string roomCodeToCheck;
    bool isCreatingRoom;
    private float coolDownTimer = 0f;
    private const float CoolDownDuration = 4f;

    private void Start()
    {
        runner.AddCallbacks(this);
    }
    private void Update()
    {
        if(coolDownTimer>0){
            coolDownTimer -= Time.deltaTime;
        }
        if(coolDownTimer<=0){
            EnableButton();
        }
    }

    public void CreateRoom(){
        string roomCode = roomCodeInput.text;

        if(string.IsNullOrEmpty(roomCode)){
            Debug.Log("Room Code cannot be empty");
            return;
        }
        isCreatingRoom=true;
        roomCodeToCheck=roomCode;
        runner.AddCallbacks(this);
        runner.JoinSessionLobby(SessionLobby.ClientServer);
        StartCoolDown();
    }
    public void JoinRoom(){
        string roomCode = roomCodeInput.text;

        if(string.IsNullOrEmpty(roomCode)){
            Debug.Log("Room Code cannot be empty");
            return;
        }
        isCreatingRoom=false;
        roomCodeToCheck=roomCode;
        runner.AddCallbacks(this);
        runner.JoinSessionLobby(SessionLobby.ClientServer);
        StartCoolDown();
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){
        bool roomExists = sessionList.Exists(session => session.Name == roomCodeToCheck);
        if(isCreatingRoom){
            if(roomExists){
                Debug.Log($"Room with {roomCodeToCheck} already exists, please try another code.");
            }
            else{
                Debug.Log($"Create new room with code {roomCodeToCheck}");
                StartGame(GameMode.Shared, roomCodeToCheck);
            }
        }
        else{
            if(roomExists){
                Debug.Log($"Create new room with code {roomCodeToCheck}");
                StartGame(GameMode.Shared, roomCodeToCheck);
            }
            else{
                Debug.Log($"Room with code {roomCodeToCheck} does not exist, please check again");
            }
        }
        runner.RemoveCallbacks(this);
    }

    private async void StartGame(GameMode gameMode, string sessionName){
        var startGameArgs = new StartGameArgs(){
            GameMode=GameMode.Shared,
            SessionName=sessionName,
            SceneManager=gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        var result = await runner.StartGame(startGameArgs);
        if(result.Ok){
            Debug.Log($"create room successfully {sessionName}");
            UIScreen.SetActive(false);
        }
    }
    private void DisableButton(){
        createButton.interactable=false;
        joinButton.interactable=false;
    }
    private void EnableButton(){
        createButton.interactable=true;
        joinButton.interactable=true;
    }
    private void StartCoolDown(){
        coolDownTimer = CoolDownDuration;
        DisableButton();
    }

    public void OnConnectedToServer(NetworkRunner runner){}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}
    public void OnInput(NetworkRunner runner, NetworkInput input){}
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player){}
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player){}
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
    public void OnSceneLoadDone(NetworkRunner runner){}
    public void OnSceneLoadStart(NetworkRunner runner){}
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
}