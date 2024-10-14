public interface ICooldownable
{
    float Duration { get; }
    float Cd { get; }
    float ActivationTime { get; }
    
    float EndTime => ActivationTime + Duration;
    bool IsActive => TimeManager.Instance.CurrentTime < EndTime;
    bool IsReady => TimeManager.Instance.CurrentTime > EndTime + Cd;
    float TimePassed => TimeManager.Instance.CurrentTime - ActivationTime;
    float TimeLeft => EndTime - TimeManager.Instance.CurrentTime;
}
