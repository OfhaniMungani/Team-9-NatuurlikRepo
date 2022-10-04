using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace NatuurlikBase.Models
{
    public class AuditLine
    {
        public AuditLine(EntityEntry entry)
        {
            Entry = entry;
        }
        public EntityEntry Entry { get; }
        public string ActorFullName { get; set; }
        public string EntityName { get; set; }
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public AuditType AuditType { get; set; }
        public List<string> ChangedColumns { get; } = new List<string>();
        public Audit ToAudit()
        {
            var audit = new Audit();
            audit.EntityName = EntityName;
            audit.ActionType = AuditType.ToString();
            audit.ActorFullName = ActorFullName;
            audit.DateTime = DateTime.Now;
            audit.OldValues = OldValues.Count == 0 ? "null" : JsonConvert.SerializeObject(OldValues);
            audit.NewValues = NewValues.Count == 0 ? "null" : JsonConvert.SerializeObject(NewValues);
            return audit;
        }
    }
}
