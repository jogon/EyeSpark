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
                if (lastKeyName.Count > 0 && !(CtrlFlag || AltFlag || ShiftFlag))
                {
                    text.Append(",");
                }

                switch (code)
                {
                    case "^":
                        if (!CtrlFlag)
                        {
                            CtrlFlag = true;
                            text.Append("CTRL+");
                            lastKeyName.Push("CTRL");
                            sequence.Append(code);
                        }
                        break;
                    case "+":
                        if (!ShiftFlag)
                        {
                            ShiftFlag = true;
                            text.Append("SHIFT+");
                            lastKeyName.Push("SHIFT");
                            sequence.Append(code);
                        }
                        break;
                    case "%":
                        if (!AltFlag)
                        {
                            AltFlag = true;
                            text.Append("ALT+");
                            lastKeyName.Push("ALT");
                            sequence.Append(code);
                        }
                        break;
                    default:
                        text.Append(keyName);
                        lastKeyName.Push(keyName);
                        sequence.Append(code);
                        ClearFlags();
                        break;
                }

            }
            else 
            {
                Console.WriteLine("Unrecognized Key: " + keyName);
            }
            return this;
        }

        public void Clear()
        {
            sequence.Length = 0;
            text.Length = 0;
            lastKeyName.Clear();
            ClearFlags();
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
            return sequence.ToString();
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

                codes.Add(Key.Back.ToString(), "{BS}");
                codes.Add(Key.Delete.ToString(), "{DEL}");
                codes.Add(Key.CapsLock.ToString(), "{CAPSLOCK}");
                codes.Add(Key.End.ToString(), "{END}");
                codes.Add(Key.Enter.ToString(), "~");
                codes.Add(Key.Escape.ToString(), "{ESC}");
                codes.Add(Key.Help.ToString(), "{HELP}");
                codes.Add(Key.Home.ToString(), "{HOME}");
                codes.Add(Key.Insert.ToString(), "{INS}");
                codes.Add(Key.NumLock.ToString(), "{HELP}");
                codes.Add(Key.PageDown.ToString(), "{PGDN}");
                codes.Add(Key.PageUp.ToString(), "{PGUP}");
                codes.Add(Key.Scroll.ToString(), "{SCROLLOCK}");
                codes.Add(Key.Tab.ToString(), "{TAB}");

                codes.Add(Key.Add.ToString(), "{ADD}");
                codes.Add(Key.Subtract.ToString(), "{SUBTRACT}");
                codes.Add(Key.Multiply.ToString(), "{MULTIPLY}");
                codes.Add(Key.Divide.ToString(), "{DIVIDE}");

                codes.Add(Key.D0.ToString(), "0");
                codes.Add(Key.D1.ToString(), "1");
                codes.Add(Key.D2.ToString(), "2");
                codes.Add(Key.D3.ToString(), "3");
                codes.Add(Key.D4.ToString(), "4");
                codes.Add(Key.D5.ToString(), "5");
                codes.Add(Key.D6.ToString(), "6");
                codes.Add(Key.D7.ToString(), "7");
                codes.Add(Key.D8.ToString(), "8");
                codes.Add(Key.D9.ToString(), "9");

                codes.Add(Key.OemComma.ToString(), ",");
                codes.Add(Key.OemPeriod.ToString(), ".");
                codes.Add(Key.OemSemicolon.ToString(), ";");

                codes.Add(Key.F1.ToString(), "{F1}");
                codes.Add(Key.F2.ToString(), "{F2}");
                codes.Add(Key.F3.ToString(), "{F3}");
                codes.Add(Key.F4.ToString(), "{F4}");
                codes.Add(Key.F5.ToString(), "{F5}");
                codes.Add(Key.F6.ToString(), "{F6}");
                codes.Add(Key.F7.ToString(), "{F7}");
                codes.Add(Key.F8.ToString(), "{F8}");
                codes.Add(Key.F9.ToString(), "{F9}");
                codes.Add(Key.F10.ToString(), "{F10}");
                codes.Add(Key.F11.ToString(), "{F11}");
                codes.Add(Key.F12.ToString(), "{F12}");

                codes.Add(Key.A.ToString(), "a");
                codes.Add(Key.B.ToString(), "b");
                codes.Add(Key.C.ToString(), "c");
                codes.Add(Key.D.ToString(), "d");
                codes.Add(Key.E.ToString(), "e");
                codes.Add(Key.F.ToString(), "f");
                codes.Add(Key.G.ToString(), "g");
                codes.Add(Key.H.ToString(), "h");
                codes.Add(Key.I.ToString(), "i");
                codes.Add(Key.J.ToString(), "j");
                codes.Add(Key.K.ToString(), "k");
                codes.Add(Key.L.ToString(), "l");
                codes.Add(Key.M.ToString(), "m");
                codes.Add(Key.N.ToString(), "n");
                codes.Add(Key.O.ToString(), "o");
                codes.Add(Key.P.ToString(), "p");
                codes.Add(Key.Q.ToString(), "q");
                codes.Add(Key.R.ToString(), "r");
                codes.Add(Key.S.ToString(), "s");
                codes.Add(Key.T.ToString(), "t");
                codes.Add(Key.U.ToString(), "u");
                codes.Add(Key.V.ToString(), "v");
                codes.Add(Key.W.ToString(), "w");
                codes.Add(Key.X.ToString(), "x");
                codes.Add(Key.Y.ToString(), "y");
                codes.Add(Key.Z.ToString(), "z");


            }
        }

        public int Length
        {
            get {
                return sequence.Length;
            }
        }

        public String Text
        {
            get {
                return text.ToString();
            }
        }
        public bool CtrlFlag { get; set; }

        public bool AltFlag { get; set; }

        public bool ShiftFlag { get; set; }
    }
}
