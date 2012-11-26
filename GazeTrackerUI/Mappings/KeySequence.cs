using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GazeTrackerUI.Mappings
{
    class KeySequence
    {
        private static Dictionary<String, String> codes;
        private Stack<String> lastKeyName;
        private StringBuilder sequence;
        private StringBuilder text;

        public KeySequence()
            : this("")
        {

        }

        public KeySequence(String t)
        {
            LoadCodes();

            lastKeyName = new Stack<String>();
            text = new StringBuilder(t);
            sequence = new StringBuilder(Parse(t, true));
        }

        public String Parse(String text)
        {
            return Parse(text, false);
        }

        public String Parse(String text, bool persist)
        {
            StringBuilder sequence = new StringBuilder();

            if (text.Length > 0)
            {
                String[] tokens = text.Split(",+".ToCharArray());
                foreach (String token in tokens)
                {
                    sequence.Append(codes[token]);
                    if (persist)
                    {
                        lastKeyName.Push(token);
                    }                    
                }
            }            

            return sequence.ToString();
        }

        public KeySequence Append(String keyName)
        {
            String code;
            if (codes.TryGetValue(keyName, out code))
            {
                sequence.Append(code);

                if (lastKeyName.Count > 0 && !(CtrlFlag || AltFlag || ShiftFlag))
                {
                    text.Append(",");
                }
                
                switch (code)
                {
                    case "^":
                        if(!CtrlFlag)
                        {
                            CtrlFlag = true;
                            text.Append("CTRL+");
                            lastKeyName.Push("CTRL");
                        }
                        break;
                    case "+":
                        if (!ShiftFlag)
                        {
                            ShiftFlag = true;
                            text.Append("SHIFT+");
                            lastKeyName.Push("SHIFT");
                        }
                        break;
                    case "%":
                        if (!AltFlag)
                        {
                            AltFlag = true;
                            text.Append("ALT+");
                            lastKeyName.Push("ALT");
                        }                        
                        break;
                    default:
                        text.Append(keyName);
                        lastKeyName.Push(keyName);
                        ClearFlags();
                        break;
                }                
                
            }
            return this;
        }

        public void Clear()
        {
            sequence.Length = 0;
            text.Length = 0;
            lastKeyName.Clear();
        }

        private void ClearFlags()
        {
            CtrlFlag = false;
            AltFlag = false;
            ShiftFlag = false;
        }

        public void Detach()
        {
            if (lastKeyName.Count > 0)
            {
                int removeComma = lastKeyName.Count > 1 ? 1 : 0;
                String k = lastKeyName.Pop();
                String code = codes[k];
                
                int removePlus = 0;
                if ("^%+".Contains(code))
                {
                    removePlus = 1;
                }

                sequence = sequence.Remove(
                    sequence.Length - code.Length, code.Length);

                text = text.Remove(
                    text.Length - k.ToString().Length - removeComma - removePlus,
                    k.ToString().Length + removeComma + removePlus);
            }
        }

        public override string ToString()
        {
            return text.ToString();
        }

        private void LoadCodes()
        {
            if (codes == null)
            {
                codes = new Dictionary<String, String>();
                codes.Add(Key.LeftCtrl.ToString(), "^");
                codes.Add(Key.RightCtrl.ToString(), "^");
                codes.Add("CTRL", "^");
                codes.Add(Key.LeftAlt.ToString(), "%");
                codes.Add(Key.RightAlt.ToString(), "%");
                codes.Add("ALT", "%");
                codes.Add(Key.LeftShift.ToString(), "+");
                codes.Add(Key.RightShift.ToString(), "+");
                codes.Add("SHIFT", "+");
                codes.Add(Key.Left.ToString(), "{LEFT}");
                codes.Add(Key.Right.ToString(), "{RIGHT}");
                codes.Add(Key.Up.ToString(), "{UP}");
                codes.Add(Key.Down.ToString(), "{DOWN}");
                codes.Add(Key.A.ToString(), "A");
                codes.Add(Key.B.ToString(), "B");
                codes.Add(Key.C.ToString(), "C");

            }
        }

        public bool CtrlFlag { get; set; }

        public bool AltFlag { get; set; }

        public bool ShiftFlag { get; set; }
    }
}
