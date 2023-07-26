namespace FamilyHubs.Referral.Data.Models;

public class EventGridEventEx
{
    public string Id { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateTime EventTime { get; set; }
    public object? Data { get; set; }
}

