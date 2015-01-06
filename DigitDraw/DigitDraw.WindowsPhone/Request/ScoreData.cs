using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DigitDraw.Request
{
    public class ScoreData
    {
        public Dictionary<string, string> FeatureVector
        {
            get;
            set;
        }

        public Dictionary<string, string> GlobalParameters
        {
            get;
            set;
        }

        public ScoreData()
        {
        }
    }
}