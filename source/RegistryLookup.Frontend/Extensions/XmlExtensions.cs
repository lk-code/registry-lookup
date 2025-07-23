using System.Xml.Linq;

namespace dev.lkcode.RegistryLookup.Frontend.Extensions;

public static class XmlExtensions
{
    public static XDocument AsXmlDocument(this string xml)
    {
        return XDocument.Parse(xml);
    }

    public static XDocument RemoveAttribute(this XDocument element, string attributeName)
    {
        if (element.Root != null)
        {
            element.Root.RemoveAttribute(attributeName);
        }

        return element;
    }

    public static XElement RemoveAttribute(this XElement element, string attributeName)
    {
        XAttribute? attribute = element.Attribute(attributeName);
        if (attribute is not null)
        {
            attribute.Remove();
        }

        return element;
    }

    public static XDocument AddOrUpdateAttribute(this XDocument element, string attributeName, string value)
    {
        if (element.Root is not null)
        {
            element.Root.AddOrUpdateAttribute(attributeName, value);
        }

        return element;
    }

    public static XElement AddOrUpdateAttribute(this XElement element, string attributeName, string value)
    {
        XAttribute? attribute = element.Attribute(attributeName);
        if (attribute is null)
        {
            element.Add(new XAttribute(attributeName, value));
        }
        else
        {
            attribute.Value = value;
        }

        return element;
    }
}
