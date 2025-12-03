using ShiftAssignerServer.Requests;

namespace ShiftAssignerServer.Tests.Steps;

public class TenantSenderInfo
{
    public TenantRegisterRequest Request { get; set; }
    public TenantRegisterResponse Response { get; set; }

    public ShiftLeaderSenderInfo ShiftLeaderSenderInfo { get; set; }    

    public string JwtToken => Response.Token;
}

public class RegistrationShiftLeaderSenderInfo
{
    public ShiftLeaderSenderInfo ShiftLeaderSenderInfo { get; set; }    

    // public string JwtToken => Response.Token;

    // public string JwtToken => Response.Token;
}



public class ShiftLeaderSenderInfo
{
    public AddingShiftLeaderRequest Request { get; set; }
    public AddingShiftLeaderResponse Response { get; set; }

    // public string JwtToken => Response.Token;
}

