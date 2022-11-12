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

public class TextValuePairDTO
{
    [XmlText]
    public string text { get; set; } = string.Empty;
    [XmlAttribute]
    public string value { get; set; } = string.Empty;
}

#pragma warning restore IDE1006