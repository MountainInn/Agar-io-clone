using UnityEngine;
using Photon.Pun;
using Zenject;

public class CellGroup : MonoBehaviourPunCallbacks
{
    private ushort cellCount;

    private RoomManager roomManager;

    [Inject]
    public void Construct(RoomManager roomManager)
    {
        this.roomManager = roomManager;
    }

    public void IncrementCellCount()
    {
        cellCount++;
    }
    public void DecrementCellCount()
    {
        cellCount--;

        if (cellCount == 0)
        {
            roomManager.LeaveRoom();
        }
    }
}
