using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSettings
{
    public class HeadMovement
    {
        private Dictionary<string, string> map = new Dictionary<string, string>();

        public HeadMovement() 
        {
        }

        public Dictionary<string, string> GetMapping(string name) 
        {  
            return map;
        }

        public void SaveMapping(string selectedItem, Dictionary<string, string> selectedMap)
        {
            throw new NotImplementedException();
        }
    }
}
