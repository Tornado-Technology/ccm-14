namespace Content.Server._CM14.Xeno;

public sealed class XenoAnnouncementSystem : EntitySystem
{
    public const float AnnouncmentTime = 600f;

    private float _announcmentTime = 0f;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _announcmentTime += frameTime;

        if (_announcmentTime >= AnnouncmentTime)
        {
            _announcmentTime -= AnnouncmentTime;
            MakeAnnouncement();
        }
    }

    private void MakeAnnouncement()
    {
    }
}
