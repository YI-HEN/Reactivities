using System.Text.Json.Serialization;

namespace Application.Profiles
{
    public class UserActivityDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; } 

        [JsonIgnore] //協助查詢，不想返回給客戶端時使用
        public string HostUsername { get; set; }
    }
}