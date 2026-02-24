# UP-Pooling (HTDA.Framework.Pooling)

Hệ thống pooling tối ưu cho spawn/despawn, hạn chế Instantiate/Destroy.

## Features

- Core: `ObjectPool<T>`, `IPool<T>`
- Unity:
    - `GameObjectPool` (theo prefab)
    - `ComponentPool<T>`
    - `PoolRegistry` (lấy pool theo prefab key)
    - `PooledObject` (tag owner pool)
- Hooks: `IPoolable` (`ResetState`, `OnSpawned`, `OnDespawned`)
- Options: prewarm, max size, expand policy
- Optimization: `PoolableRoot` / `ManualPoolables` (nếu bạn đã thêm)

## Quick start

```csharp
var go = PoolRegistry.Spawn(bulletPrefab, position, rotation);
go.GetComponent<PooledObject>().Despawn();
```

## IPoolable

```csharp
public class Bullet : MonoBehaviour, IPoolable
{
    public void ResetState() {}
    public void OnSpawned() {}
    public void OnDespawned() {}
}
```

## Notes

- Pool phù hợp object spawn nhiều: bullet, vfx, damage text…
- Không nên pool object sống rất lâu (menu root, main systems).
