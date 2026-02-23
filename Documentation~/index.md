# UP-Pooling -- Technical Documentation

## 1. Architecture Overview

UP-Pooling gồm 2 tầng:

### Core Layer

-   IPool`<T>`{=html}
-   ObjectPool`<T>`{=html}
-   PoolOptions
-   PoolExpandPolicy

### Unity Layer

-   GameObjectPool
-   ComponentPool`<T>`{=html}
-   PoolRegistry
-   PooledObject
-   IPoolable
-   UnityPoolBehaviour

------------------------------------------------------------------------

## 2. Performance Design

### O(1) Active Tracking

ObjectPool sử dụng:

-   LinkedList`<T>`{=html} để giữ thứ tự active
-   Dictionary\<T, LinkedListNode`<T>`{=html}\> để remove O(1)

Release và ReuseOldest đều O(1).

------------------------------------------------------------------------

## 3. Spawn Lifecycle (Default)

Spawn order:

ResetState → SetActive(true) → OnSpawned

Despawn order:

OnDespawned → SetActive(false)

Có thể cấu hình qua UnityPoolBehaviour.

------------------------------------------------------------------------

## 4. PoolExpandPolicy

-   Expand: tạo thêm instance nếu hết
-   Fail: trả null
-   ReuseOldest: tái sử dụng object active lâu nhất

------------------------------------------------------------------------

## 5. Memory Strategy

-   Cache IPoolable\[\] khi tạo instance
-   Không scan mỗi spawn/despawn
-   Optional:
    -   PoolableRoot → scan subtree
    -   ManualPoolables → zero scan

------------------------------------------------------------------------

## 6. Best Practices

✔ Prewarm nếu spawn nhiều\
✔ Không dùng pool cho object tồn tại lâu\
✔ Không dùng pooling cho prefab cực ít spawn

------------------------------------------------------------------------

## 7. Integration Suggestions

UP-Events: - Publish SpawnedEvent / DespawnedEvent nếu cần

UP-FSM: - State có thể spawn/despawn object

UP-SceneFlow: - ClearAll() khi unload scene

------------------------------------------------------------------------

## 8. Version History

v1.0.0 - O(1) pooling - Configurable spawn lifecycle - Manual & subtree
poolable scan
