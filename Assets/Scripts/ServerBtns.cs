using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ServerBtns : MonoBehaviourPunCallbacks
{
    public string roomCode; 
    public static string pendingRoomCode;

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            pendingRoomCode = roomCode;

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                JoinTargetRoom();
            }
        }
    }

    public override void OnConnectedToMasterAsync()
    {
        if (!string.IsNullOrEmpty(pendingRoomCode))
        {
            JoinTargetRoom();
        }
    }

    private void JoinTargetRoom()
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 10,
            IsVisible = true,
            IsOpen = true
        };

        string target = pendingRoomCode;
        pendingRoomCode = null;
        PhotonNetwork.JoinOrCreateRoom(target, options, TypedLobby.Default);
    }
}