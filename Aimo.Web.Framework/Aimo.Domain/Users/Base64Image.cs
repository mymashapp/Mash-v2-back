namespace Aimo.Domain.Users;

public class Base64Image
{
    public readonly byte[] Bytes;
    public readonly string Extension;
    public Base64Image(string base64String)
    {
        try
        {
            Extension = base64String.Split(';')[0].Split('/')[1];
            Bytes = Convert.FromBase64String(base64String.Split(',')[1]);
        }
        catch
        {
            throw new AppException("invalid base64 string");
        }
    }
}