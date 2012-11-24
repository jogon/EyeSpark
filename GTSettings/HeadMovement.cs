using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GTSettings
{
    public class HeadMovement
    {
        public const string Name = "HeadMovementSettings";
        private Dictionary<string, Dictionary<string, string>> mappings;
       
        #region Constructor
        public HeadMovement()
        {
            mappings = new Dictionary<string, Dictionary<string, string>>();
        }
        #endregion

        #region Public Methods
        public Dictionary<string, string> GetMapping(string name)
        {
            Dictionary<string, string> map;
            if (mappings.TryGetValue(name, out map))
            {
                return map;
            }
            return null;

        }

        public void SaveMapping(string selectedItem, 
            Dictionary<string, string> selectedMap)
        {
            if (mappings.ContainsKey(selectedItem))
            {
                mappings[selectedItem] = selectedMap;
            }
            else
            {
                mappings.Add(selectedItem, selectedMap);
            }
        }

        public void WriteConfigFile(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(Name);
            foreach (String appName in mappings.Keys)
            {
                xmlWriter.WriteStartElement("Mapping");

                Settings.WriteElement(xmlWriter, "ApplicationName", appName);
                Dictionary<String, String> map = mappings[appName];

                foreach (String gesture in map.Keys)
                {
                    xmlWriter.WriteStartElement("Gesture");
                    Settings.WriteElement(xmlWriter, "GestureName", gesture);
                    Settings.WriteElement(xmlWriter, "Sequence", map[gesture]);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            
        }

        public void LoadConfigFile(XmlReader xmlReader)
        {
            while (xmlReader.ReadToFollowing("Mapping"))
            {
                XmlReader mapReader = xmlReader.ReadSubtree();
                mapReader.ReadToFollowing("ApplicationName");
                String appName = mapReader.ReadString();
                
                Dictionary<String, String> map =
                    new Dictionary<string, string>();

                while (mapReader.ReadToFollowing("GestureName"))
                {
                    string gesture = mapReader.ReadString();

                    mapReader.ReadToFollowing("Sequence");
                    map.Add(gesture, mapReader.ReadString());
                }
                mappings.Add(appName, map);
            }            
        }
        #endregion
    }
}
