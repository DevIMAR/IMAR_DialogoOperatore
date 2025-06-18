using Newtonsoft.Json;

namespace IMAR_DialogoOperatore.Domain.Models
{
    public class TaskAsana
    {
        [JsonIgnore]
        public string Id;
        [JsonIgnore]
        public string Approval_status { get; set; }
        [JsonProperty("assignee")]
        public string Assignee { get; set; }
        [JsonIgnore]
        public string Assignee_section { get; set; }
        [JsonIgnore]
        public string Assignee_status { get; set; }
        [JsonProperty("completed")]
        public bool Completed { get; set; }
        //public CompletedBy completed_by { get; set; }
        //public CustomFields custom_fields { get; set; }
        [JsonIgnore]
        public DateTime? Due_at { get; set; }
        public string Due_on { get; set; } = DateTime.Now.ToString();
        //public External external { get; set; }
        [JsonProperty("followers")]
        public List<string> Followers { get; set; }
        [JsonProperty("html_notes")]
        public string Html_notes { get; set; }
        [JsonProperty("liked")]
        public bool Liked { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonIgnore]
        public string Notes { get; set; }
        [JsonProperty("parent")]
        public string Parent { get; set; }
        [JsonProperty("projects")]
        public List<string> Projects { get; set; }
        [JsonIgnore]
        public string Resource_subtype { get; set; }
        public string Start_on { get; set; }
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
        [JsonProperty("workspace")]
        public string Workspace { get; set; }
    }
}
