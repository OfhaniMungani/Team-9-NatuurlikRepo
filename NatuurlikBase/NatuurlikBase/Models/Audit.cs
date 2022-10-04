namespace NatuurlikBase.Models
{
    public class Audit
    {
        public int Id { get; set; }
        public string? ActorFullName { get; set; }
        public string EntityName { get; set; }
        public string ActionType { get; set; }
        public DateTime DateTime { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }
}
