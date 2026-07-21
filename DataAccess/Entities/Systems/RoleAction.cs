using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Systems
{
    public class RoleAction : BaseEntity
    {
        public int STTSapXep { get; set; }
        public required string PhanLoai { get; set; } = "Group";
        public int Level { get; set; }
        public required string Role { get; set; }
        public Guid RoleGroupId { get; set; }
        public string? Title { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? Parameter { get; set; }
        public string? Table { get; set; }
        public required string Status { get; set; } = "Active"; //Active, Lock
        public string? UseGroup { get; set; }
        [NotMapped]
        public string? TitleRoleGroup { get; set; }
        public string? Icon { get; set; }
        [NotMapped]
        public List<string> UseGroupList
        {
            get => string.IsNullOrEmpty(UseGroup) ? new List<string>() : UseGroup.Split(',').ToList();
            set => UseGroup = string.Join(",", value);
        }

    }
}
