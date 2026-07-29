using System;
using System.Collections.Generic;
using System.Text;

namespace CourseRegistrationManagmentSystem.Models;

public class Registrations
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid ClassId { get; set; }

    public DateTime RegsitrationDate { get; set; }

    public string Status { get; set; }
}
