using System.Xml.Serialization;

namespace BoardGameTracker.Application.BoardGameGeek;

// Disables naming violations
#pragma warning disable IDE1006

[XmlRoot("user")]
public class BoardGameGeekUserDTO
{
    [XmlAttribute]
    public string id { get; set; } = string.Empty;
    [XmlAttribute]
    public string name { get; set; } = string.Empty;

    public TextValuePairDTO firstname { get; set; } = new();
    public TextValuePairDTO lastname { get; set; } = new();
    public TextValuePairDTO yearregistered { get; set; } = new();
    public TextValuePairDTO lastlogin { get; set; } = new();
    public TextValuePairDTO avatarlink { get; set; } = new();
    public TextValuePairDTO country { get; set; } = new();
    public TextValuePairDTO webaddress { get; set; } = new();
}

[XmlRoot("items")]
public class BoardgamesListDTO<TGame>
{
    [XmlElement("item")]
    public List<TGame> games { get; set; } = new();
}

[XmlType("item")]
public class HotnessBoardgameDTO
{
    [XmlAttribute]
    public string id { get; set; } = string.Empty;
    [XmlAttribute]
    public int rank { get; set; }

    public TextValuePairDTO name { get; set; } = new();
    public TextValuePairDTO yearpublished { get; set; } = new();
    public TextValuePairDTO thumbnail { get; set; } = new();
}

[XmlType("item")]
public class CollectionBoardgameDTO
{
    [XmlAttribute]
    public string objectid { get; set; } = string.Empty;

    public string name { get; set; } = string.Empty;
    public string yearpublished { get; set; } = string.Empty;
    public string image { get; set; } = string.Empty;
    public string thumbnail { get; set; } = string.Empty;
    public int numplays { get; set; }
    public StatusDTO status { get; set; } = new StatusDTO();

    public List<string> GetStatusList()
    {
        var result = new List<string>();

        var skip = new List<string> { "text", "lastmodified" };
        foreach (var info in typeof(StatusDTO).GetProperties())
        {
            if (!skip.Contains(info.Name) &&
                int.TryParse(info.GetValue(status) as string, out int value) &&
                value == 1)
            {
                result.Add(info.Name);
            }
        }

        return result;
    }
}

[XmlType("item")]
public class ThingBoardgameDTO
{
    [XmlAttribute]
    public string id { get; set; } = string.Empty;

    [XmlElement("name")]
    public List<GenericElementDTO> names { get; set; } = new();
    [XmlElement("link")]
    public List<GenericElementDTO> links { get; set; } = new();

    public TextValuePairDTO yearpublished { get; set; } = new();
    public string image { get; set; } = string.Empty;
    public string thumbnail { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public TextValuePairDTO minplayers { get; set; } = new();
    public TextValuePairDTO maxplayers { get; set; } = new();
    public TextValuePairDTO minage { get; set; } = new();
    public TextValuePairDTO playingtime { get; set; } = new();
    public TextValuePairDTO minplaytime { get; set; } = new();
    public TextValuePairDTO maxplaytime { get; set; } = new();
    public StatisticsDTO statistics { get; set; } = new();
}

[XmlType("statistics")]
public class StatisticsDTO
{
    [XmlAttribute]
    public string page { get; set; } = string.Empty;

    public RatingsDTO ratings { get; set; } = new();
}

[XmlType("ratings")]
public class RatingsDTO
{
    public TextValuePairDTO usersrated { get; set; } = new();
    public TextValuePairDTO average { get; set; } = new();
    public TextValuePairDTO bayesaverage { get; set; } = new();
    public TextValuePairDTO stddev { get; set; } = new();
    public TextValuePairDTO median { get; set; } = new();
    public TextValuePairDTO owned { get; set; } = new();
    public TextValuePairDTO trading { get; set; } = new();
    public TextValuePairDTO wanting { get; set; } = new();
    public TextValuePairDTO wishing { get; set; } = new();
    public TextValuePairDTO numcomments { get; set; } = new();
    public TextValuePairDTO numweights { get; set; } = new();
    public TextValuePairDTO averageweight { get; set; } = new();
    public RanksDTO ranks { get; set; } = new();
}

[XmlType("ranks")]
public class RanksDTO
{
    [XmlElement("rank")]
    public List<RankDTO> ranks { get; set; } = new();
}

[XmlType("rank")]
public class RankDTO
{
    [XmlText]
    public string text { get; set; } = string.Empty;
    [XmlAttribute]
    public string type { get; set; } = string.Empty;
    [XmlAttribute]
    public string id { get; set; } = string.Empty;
    [XmlAttribute]
    public string name { get; set; } = string.Empty;
    [XmlAttribute]
    public string friendlyname { get; set; } = string.Empty;
    [XmlAttribute]
    public string value { get; set; } = string.Empty;
    [XmlAttribute]
    public string bayesaverage { get; set; } = string.Empty;
}

[XmlType("status")]
public class StatusDTO
{
    [XmlText]
    public string text { get; set; } = string.Empty;
    [XmlAttribute]
    public string own { get; set; } = string.Empty;
    [XmlAttribute]
    public string prevowned { get; set; } = string.Empty;
    [XmlAttribute]
    public string fortrade { get; set; } = string.Empty;
    [XmlAttribute]
    public string want { get; set; } = string.Empty;
    [XmlAttribute]
    public string wanttoplay { get; set; } = string.Empty;
    [XmlAttribute]
    public string wanttobuy { get; set; } = string.Empty;
    [XmlAttribute]
    public string wishlist { get; set; } = string.Empty;
    [XmlAttribute]
    public string preordered { get; set; } = string.Empty;
    [XmlAttribute]
    public string lastmodified { get; set; } = string.Empty;
}

public class TextValuePairDTO
{
    [XmlText]
    public string text { get; set; } = string.Empty;
    [XmlAttribute]
    public string value { get; set; } = string.Empty;
}

public class GenericElementDTO
{
    [XmlText]
    public string text { get; set; } = string.Empty;
    [XmlAttribute]
    public string type { get; set; } = string.Empty;
    [XmlAttribute]
    public string value { get; set; } = string.Empty;
    [XmlAttribute]
    public string sortindex { get; set; } = string.Empty;
    [XmlAttribute]
    public string id { get; set; } = string.Empty;
}

#pragma warning restore IDE1006