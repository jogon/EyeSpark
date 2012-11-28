using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;

namespace GTSettings
{
    public class HeadMovement : INotifyPropertyChanged
    {
        public const string Name = "HeadMovementSettings";
        private const int DefaultThreshold = 8;

        private Dictionary<string, Dictionary<string, string>> mappings;
        private int yawThreshold = DefaultThreshold;
        private int pitchThreshold = DefaultThreshold;
        private int rollThreshold = DefaultThreshold;

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

            xmlWriter.WriteStartElement("Sensitivity");

            Settings.WriteElement(xmlWriter, "YawThreshold", YawThreshold + "");
            Settings.WriteElement(xmlWriter, "PitchThreshold", PitchThreshold + "");
            Settings.WriteElement(xmlWriter, "RollThreshold", RollThreshold + "");

            xmlWriter.WriteEndElement();
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
            if (xmlReader.ReadToFollowing("Sensitivity"))
            {
                xmlReader.ReadToFollowing("YawThreshold");
                YawThreshold = Convert.ToInt32(xmlReader.ReadString());

                xmlReader.ReadToFollowing("PitchThreshold");
                PitchThreshold = Convert.ToInt32(xmlReader.ReadString());

                xmlReader.ReadToFollowing("RollThreshold");
                RollThreshold = Convert.ToInt32(xmlReader.ReadString());
            }      
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

        #region Properties
        
        public int YawThreshold {
            get { return yawThreshold; }
            set
            {
                yawThreshold = value;
                OnPropertyChanged("yawThreshold");
            } 
        }

        public int PitchThreshold
        {
            get { return pitchThreshold; }
            set
            {
                pitchThreshold = value;
                OnPropertyChanged("pitchThreshold");
            }
        }

        public int RollThreshold
        {
            get { return rollThreshold; }
            set
            {
                rollThreshold = value;
                OnPropertyChanged("rollThreshold");
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string parameter)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(parameter));
            }
        }
    }
}
