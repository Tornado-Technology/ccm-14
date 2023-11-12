namespace Content.Shared.Pulling.Components
{
    [RegisterComponent]
    [Access(typeof(SharedPullingStateManagementSystem))]
    public sealed partial class SharedPullerComponent : Component
    {
        [DataField]
        public float WalkModifier = 0.95f;

        [DataField]
        public float SprintModifier = 0.95f;

        // Before changing how this is updated, please see SharedPullerSystem.RefreshMovementSpeed
        public float WalkSpeedModifier => Pulling == default ? 1.0f : WalkModifier;

        public float SprintSpeedModifier => Pulling == default ? 1.0f : SprintModifier;

        [ViewVariables]
        public EntityUid? Pulling { get; set; }

        /// <summary>
        ///     Does this entity need hands to be able to pull something?
        /// </summary>
        [DataField("needsHands")]
        public bool NeedsHands = true;
    }
}
