using System;
using System.ComponentModel;
using System.Reflection;

namespace ShiftAssignerServer.Models
{
    public enum RoleState : int
    {
        [Description("Worker")]
        Worker = 1,

        [Description("Shift leader")]
        ShiftLeader = 2,

        [Description("Company boss")]
        Boss = 1000
    }

    public static class RoleStateExtensions
    {
        /// <summary>
        /// Get the Description attribute for a RoleState enum value, or the enum name when no Description is present.
        /// </summary>
        public static string GetDescription(this RoleState role)
        {
            var fi = role.GetType().GetField(role.ToString());
            if (fi == null) return role.ToString();
            var attr = fi.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? role.ToString();
        }

        /// <summary>
        /// Try parse an int to RoleState; returns null if parsing fails.
        /// </summary>
        public static RoleState? FromInt(int value)
        {
            if (Enum.IsDefined(typeof(RoleState), value))
            {
                return (RoleState)value;
            }
            return null;
        }
    }

    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}