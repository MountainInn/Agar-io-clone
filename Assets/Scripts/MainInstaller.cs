using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind(typeof(CellController), typeof(CellGroup), typeof(RoomManager))
            .FromComponentsInHierarchy()
            .AsSingle() ;
    }
}
