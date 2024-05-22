using System;

namespace Events.Hangfire.SubPub
{
    public class HangfireJobOptions
    {
        public HangfireJobType HangfireJobType { get; set; } = HangfireJobType.Enqueue;
        public TimeSpan TimeSpan { get; set; } = TimeSpan.Zero;
    }

    public enum HangfireJobType
    {
        Enqueue,
        Schedule
    }
}