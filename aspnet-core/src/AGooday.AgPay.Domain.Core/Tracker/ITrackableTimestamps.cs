namespace AGooday.AgPay.Domain.Core.Tracker
{
    public interface ITrackableTimestamps
    {
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
