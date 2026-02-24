# UP-Pooling – Documentation

## 1. Mục tiêu

- Spawn/despawn ổn định, dễ dùng.
- Giảm Instantiate/Destroy và GC.

## 2. Expand policy

- Expand: tạo thêm instance nếu hết
- Fail: trả null nếu hết
- ReuseOldest: tái sử dụng object active lâu nhất (O(1))

## 3. IPoolable lifecycle order

Mặc định (tuỳ cấu hình nếu bạn đã thêm):
- Spawn: ResetState → SetActive(true) → OnSpawned
- Despawn: OnDespawned → SetActive(false)

## 4. PoolRegistry

- Tạo/lấy pool theo prefab key.
- Khuyến nghị tạo pool sớm (prewarm) cho prefab spawn dày.

## 5. Optimization notes

- Cache IPoolable[] ngay khi create instance.
- Nếu prefab lớn:
    - dùng `PoolableRoot` để giới hạn subtree scan
    - hoặc `ManualPoolables` để zero scan

## 6. Best practices

- Prewarm phù hợp để tránh spike.
- Luôn Despawn qua `PooledObject`/pool thay vì Destroy.
