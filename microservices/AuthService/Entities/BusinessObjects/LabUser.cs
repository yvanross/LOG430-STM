using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;
using Microsoft.AspNetCore.Identity;

namespace Entities
{
    [Table(nameof(LabUser))]
    public sealed class LabUser : IdentityUser
    {
        [Key]
        public override string? UserName { get; set; }

        public string Team { get; init; }

        public string Role { get; init; }

        public LabUser() { }

        public LabUser(string username, string team, string role)
        {
            UserName = username;

            Team = team;
            
            Role = role;
        }
    }
}