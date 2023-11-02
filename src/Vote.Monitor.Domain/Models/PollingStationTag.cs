namespace Vote.Monitor.Domain.Models;
public class PollingStationTag
{

    public Guid PollingStationId { get; set; }
    public int TagId { get; set; }
    public PollingStationModel? PollingStation { get; }
    public TagModel? Tag { get; }
}
