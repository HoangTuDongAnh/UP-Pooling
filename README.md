# UP-Pooling

High-performance, extensible object pooling system for Unity.

UP-Pooling cung cấp hệ thống spawn/despawn tối ưu cho GameObject và
Component, giảm Instantiate/Destroy, giảm GC và tối ưu hiệu năng cho
gameplay.

------------------------------------------------------------------------

## ✨ Features

-   Generic ObjectPool`<T>`{=html} (O(1) active tracking)
-   GameObjectPool & ComponentPool`<T>`{=html}
-   PoolRegistry global manager
-   IPoolable lifecycle hooks
-   Configurable spawn/despawn order
-   Prewarm, MaxSize, ExpandPolicy
-   Optional PoolableRoot (limit scan subtree)
-   Optional ManualPoolables (zero scan)

------------------------------------------------------------------------

## 🚀 Quick Start

### 1️⃣ Spawn via PoolRegistry

``` csharp
var bullet = PoolRegistry.Spawn(bulletPrefab, position, rotation);
```

### 2️⃣ Despawn

``` csharp
bullet.GetComponent<PooledObject>().Despawn();
```

------------------------------------------------------------------------

## 🧱 IPoolable

``` csharp
public class Bullet : MonoBehaviour, IPoolable
{
    public void ResetState() { }
    public void OnSpawned() { }
    public void OnDespawned() { }
}
```

------------------------------------------------------------------------

## ⚙ PoolOptions

``` csharp
var options = new PoolOptions(
    prewarm: 20,
    maxSize: 100,
    expandPolicy: PoolExpandPolicy.Expand
);
```

------------------------------------------------------------------------

## 🔧 Advanced Optimizations

### PoolableRoot

Giới hạn scan IPoolable trong subtree cụ thể.

### ManualPoolables

Tự đăng ký IPoolable để tránh scan hoàn toàn.

------------------------------------------------------------------------

## 📌 Dependency

Depends on: - UP-Core

------------------------------------------------------------------------

## 🎯 Intended Usage

-   Bullet/VFX pooling
-   Enemy spawn systems
-   UI element recycling
-   High-frequency gameplay objects

------------------------------------------------------------------------

## 📄 Version

v1.0.0 -- Optimized O(1) pooling system
