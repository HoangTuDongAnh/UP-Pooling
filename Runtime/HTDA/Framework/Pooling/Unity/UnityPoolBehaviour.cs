namespace HTDA.Framework.Pooling.Unity
{
    /// <summary>
    /// Controls the order of Reset/SetActive/Callbacks for pooled GameObjects.
    /// </summary>
    public readonly struct UnityPoolBehaviour
    {
        /// <summary>
        /// true: ResetState -> SetActive(true) -> OnSpawned
        /// false: SetActive(true) -> ResetState -> OnSpawned
        /// </summary>
        public readonly bool ResetBeforeActivate;

        /// <summary>
        /// true: OnDespawned -> SetActive(false)
        /// false: SetActive(false) -> OnDespawned
        /// </summary>
        public readonly bool DespawnCallbacksBeforeDeactivate;

        /// <summary>
        /// When spawning with a parent, controls worldPositionStays for SetParent.
        /// </summary>
        public readonly bool WorldPositionStaysOnSpawnParent;

        public UnityPoolBehaviour(
            bool resetBeforeActivate = true,
            bool despawnCallbacksBeforeDeactivate = true,
            bool worldPositionStaysOnSpawnParent = false)
        {
            ResetBeforeActivate = resetBeforeActivate;
            DespawnCallbacksBeforeDeactivate = despawnCallbacksBeforeDeactivate;
            WorldPositionStaysOnSpawnParent = worldPositionStaysOnSpawnParent;
        }

        public static UnityPoolBehaviour Default => new UnityPoolBehaviour();
    }
}