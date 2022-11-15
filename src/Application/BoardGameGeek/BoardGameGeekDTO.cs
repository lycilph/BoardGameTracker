using System.Xml.Serialization;

namespace BoardGameTracker.Application.BoardGameGeek;

// Disables naming violations
#pragma warning disable IDE1006

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

    [XmlElement("name")]
    public TextValuePairDTO name { get; set; } = new TextValuePairDTO();
    [XmlElement("yearpublished")]
    public TextValuePairDTO yearpublished { get; set; } = new TextValuePairDTO();
    [XmlElement("thumbnail")]
    public TextValuePairDTO thumbnail { get; set; } = new TextValuePairDTO();
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

#pragma warning restore IDE1006