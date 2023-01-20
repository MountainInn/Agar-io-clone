uisng UniRx;

public class CellController : MonoBehaviour
{
    public IObservable<Vector3> mousePositionStream;
    public IObservable<Unit> splitKeypressStream;
    public IObservable<Unit> ejectKeypressStream;

    public void Awake()
    {
        // mousePositionStream =
    }
}
