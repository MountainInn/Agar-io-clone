using System;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

public class CellController : MonoBehaviour
{
    public IObservable<Vector3> mousePositionStream;
    public IObservable<bool> splitKeypressStream;
    public IObservable<bool> ejectKeypressStream;

    public void Awake()
    {
        mousePositionStream =
            this.UpdateAsObservable()
            .Select(_=> Input.mousePosition);


        KeyCode splitKeyCode = KeyCode.Space;

        splitKeypressStream =
            this.UpdateAsObservable()
            .Select(_=> Input.GetKeyDown(splitKeyCode))
            .Where(b => b == true);


        KeyCode ejectKeyCode = KeyCode.E;

        ejectKeypressStream =
            this.UpdateAsObservable()
            .Select(_=> Input.GetKeyDown(ejectKeyCode))
            .Where(b => b == true);
    }
}
