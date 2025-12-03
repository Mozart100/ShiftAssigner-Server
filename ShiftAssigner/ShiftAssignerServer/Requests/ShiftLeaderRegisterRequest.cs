namespace ShiftAssignerServer.Requests;

public class AddingShiftLeaderRequest 
{
    public string ID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    // Optional: associate this worker to a supervising shift leader at registration
    public string ShiftLeaderId { get; set; } = string.Empty;
}

public class AddingShiftLeaderResponse : RegisterResponse
{
    public string Tenant { get; set; }
}

