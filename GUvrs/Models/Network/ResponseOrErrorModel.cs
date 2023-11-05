namespace GUvrs.Models.Network;

public class ResponseOrErrorModel<T>
{
    public bool Success { get; set; }
    public T Response { get; set; }
    public string Error { get; set; }
}
