using System;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Maui.Storage;

namespace TEST_APP
{
    public class XmlReaderService
    {
        public async Task<string[,]> ReadXml(String Event)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(Event);
            using var reader = new StreamReader(stream);
            string xmlContent = await reader.ReadToEndAsync();

            XDocument doc = XDocument.Parse(xmlContent);

            var events = doc.Descendants("Event").ToList();
            string[,] eventMatrix = new string[events.Count, 3];

            for (int i = 0; i < events.Count; i++)
            {
                var eventElement = events[i];
                string title = eventElement.Element("Title")?.Value ?? "Unknown Title";
                string data = eventElement.Element("Data")?.Value ?? "Unknown Date";
                string color = eventElement.Element("Color")?.Value ?? "Unknown Color";

                eventMatrix[i, 0] = title; 
                eventMatrix[i, 1] = data;
                eventMatrix[i, 2] = color;
            }

            return eventMatrix;
        }
    }
}
