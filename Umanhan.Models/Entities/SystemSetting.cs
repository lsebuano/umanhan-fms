using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Umanhan.Models.Models;

namespace Umanhan.Models.Entities;

public partial class SystemSetting : IEntity
{
    [Column("SettingId")]
    public Guid Id { get; set; }

    public string SettingName { get; set; }

    public string SettingValue { get; set; }
}
