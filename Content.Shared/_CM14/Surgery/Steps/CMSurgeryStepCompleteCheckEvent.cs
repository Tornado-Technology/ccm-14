namespace Content.Shared._CM14.Surgery.Steps;

[ByRefEvent]
public record struct CMSurgeryStepCompleteCheckEvent(EntityUid Body, EntityUid Part, bool Cancelled = false);
