using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ErrorManagement.Auth.Infrastructure.Persistence;

[Table("Permissions", Schema = "auth")]
[Index("Code", Name = "UQ_Permissions_Code", IsUnique = true)]
public partial class Permission
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string Code { get; set; } = null!;

    [StringLength(100)]
    public string Module { get; set; } = null!;

    [StringLength(300)]
    public string Description { get; set; } = null!;

    [ForeignKey("PermissionId")]
    [InverseProperty("Permissions")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
