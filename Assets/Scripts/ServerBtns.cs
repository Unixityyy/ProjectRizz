using UnityEngine;
using Photon.Pun;
using Photon.VR;
using Photon.Realtime;

public class ServerBtns : MonoBehaviour
{
    public string roomCode; 

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            RoomManager.PendingRoomCode = roomCode;

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                // If not in a room, the Manager won't trigger OnLeftRoom, 
                // so we manually call the join logic via a public method or join directly.
                PhotonNetwork.JoinOrCreateRoom(roomCode, new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
            }
        }
    }
}