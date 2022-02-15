
public class Timer
{
    float timerDuration = 0;
    float currentTimer = 0;

    public Timer(float timerDuration, bool startFull)
    {
        this.timerDuration = timerDuration;
        if (startFull) { currentTimer = timerDuration; }
    }

    public bool CountDown(float time)
    {
        currentTimer -= time;
        if (currentTimer <= 0) { return true; }
        return false;
    }

    public void SetNewDuration(float duration, bool resetTimer)
    {
        timerDuration = duration;
        if (true) { ResetTimer(); }
    }

    public void ResetTimer()
    {
        currentTimer = timerDuration;
    }
}
