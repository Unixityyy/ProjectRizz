using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static string PendingRoomCode;

    public override void OnConnectedToMasterAsync()
    {
        JoinRoom(string.IsNullOrEmpty(PendingRoomCode) ? "1" : PendingRoomCode);
        PendingRoomCode = null;
    }

    private void JoinRoom(string code)
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 10 };
        PhotonNetwork.JoinOrCreateRoom(code, options, TypedLobby.Default);
    }
}