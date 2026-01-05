namespace ShiftAssignerServer.Requests;


public class RegisteringShiftLeaderRequest 
{
    public string ID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
}

public class RegisteringShiftLeaderResponse //: RegisterResponse
{
    // public string Tenant { get; set; }
}

