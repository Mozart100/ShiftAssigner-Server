namespace ShiftAssignerServer.Requests;

public class LoginWorkerRequest
{
    public string ID { get; set; }

    public string Password { get; set; }

}

public class LoginWorkerResponse
{
    public string Token { get; set; }
}