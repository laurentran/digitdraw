using System;
using System.Runtime.CompilerServices;

namespace DigitDraw.Request
{
    public class ScoreRequest
    {
        public string Id
        {
            get;
            set;
        }

        public ScoreData Instance
        {
            get;
            set;
        }

        public ScoreRequest()
        {
        }
    }
}