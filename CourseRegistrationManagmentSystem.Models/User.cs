using System;
using System.Collections.Generic;
using System.Text;


public class User
{
    public enum UserRoles
    {
        Admin,
        Student
    }

    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string PasswordHash { get; set; }

    public string FullName { get; set; }

    public UserRoles Role { get; set; }

    public bool IsActive { get; set; }
}
