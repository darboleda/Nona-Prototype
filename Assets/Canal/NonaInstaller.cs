using UnityEngine;
using System.Collections.Generic;

using Zenject;

public class TyplessGameObjectFactory : GameObjectFactory, IFactory<GameObject>
{
    public virtual GameObject Create()
    {
        return _container.InstantiatePrefab(_prefab);
    }

    public override IEnumerable<ZenjectResolveException> Validate()
    {
        return _container.ValidateObjectGraph<GameObject>();
    }
}

public class NonaInstaller : MonoInstaller
{
    private class Ticker : ITickable, IInitializable
    {

        public void Tick()
        {

        }

        public void Initialize()
        {

        }
    }


    public BulletMeter bulletMeter;
    public GameObject wallHitPrefab;

    public override void InstallBindings()
    {
        Container.Bind<IInitializable>().ToSingle<Ticker>();
        Container.Bind<ITickable>().ToSingle<Ticker>();

        Container.Bind<INonaInput<string>>().ToSingle<NonaInput>();
        Container.Bind<BulletMeter>().ToSingleInstance(bulletMeter);
        Container.BindGameObjectFactory<Shootable.WallHitFactory>(wallHitPrefab);
    }




}
