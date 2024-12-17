namespace AGooday.AgPay.Domain.Core.Tracker
{
    public interface ITrackableUser
    {
        long? CreatedUid { get; set; }
        string CreatedBy { get; set; }
        //long? UpdatedUid { get; set; }
        //string UpdatedBy { get; set; }
    }
}
