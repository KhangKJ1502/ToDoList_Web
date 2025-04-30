using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Reminder
{
    public int ReminderId { get; set; }

    public int TaskId { get; set; }

    public DateTime ReminderTime { get; set; }

    public bool? IsSent { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Task Task { get; set; } = null!;
}
