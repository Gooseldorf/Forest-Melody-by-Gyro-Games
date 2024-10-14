using System;

public class TimeManager
{
    private static TimeManager instance = null;

    public static TimeManager Instance {  
        get {  
            if (instance == null) {  
                instance = new TimeManager();  
            }  
            return instance;  
        }  
    }

    public float CurrentTime { get; private set; }

    public void CalculateCurrentTime()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan elapsedTime = currentTime - PlayerData.Instance.GameStart;
        CurrentTime = (float)Math.Round(elapsedTime.TotalSeconds, 3);
    }

    public float GetTimeOffset()
    {
        double offset = CurrentTime - Math.Floor(CurrentTime);
        double roundedOffset = Math.Round(offset, 3);
        return (float)roundedOffset;
    }
    
    public float GetActiveTimeInOffline(float duration, float activationTime, float timeInOffline)
    {
        if (activationTime <= 0) return 0;
        float activeOfflineTime;
        if((CurrentTime - activationTime) > duration)
        {
            activeOfflineTime = duration;
        }
        else
        {
            activeOfflineTime = CurrentTime - activationTime;
        }
        return timeInOffline > activeOfflineTime ? timeInOffline - activeOfflineTime : timeInOffline;
    }
}
