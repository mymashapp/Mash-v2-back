namespace Aimo.Domain.Users;

public enum PictureType
{
    ProfilePicture = 1,
    Cover = 2,
    Media = 3
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}

public enum GroupType
{
    None=1,
    Two = 2,
    Three = 3,
}
public enum ChatType
{
    Card = 1,
    Private = 2,
}

public enum CardType
{
    Yelp = 1,
    Own = 2,
    Airbnb = 3,
    Groupon = 4
}

public enum SwipeType
{
    Left = 0,
    Right = 1
}

public enum NotificationType
{
    ProfileMatch = 1,
    cardExpire=2,
}