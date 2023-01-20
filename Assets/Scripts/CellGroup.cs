public class CellGroup : MonoBehaviourPunCallbacks
{
    private ushort cellCount;

    public void IncrementCellCount()
    {
        cellCount++;
    }
    public void DecrementCellCount()
    {
        cellCount--;

        if (cellCount == 0)
        {
            Debug.Log("You Lose!");
        }
    }

    [Inject]
    public void Construct()
    {

    }
}
