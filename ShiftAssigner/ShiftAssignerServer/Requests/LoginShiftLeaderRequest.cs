namespace ShiftAssignerServer.Requests;

public class LoginShiftLeaderRequest 
{
    public string ID { get; set; }

    public string  Password { get; set; }

}

public class LoginShiftLeaderResponse 
{
    public string Token { get; set; }
}