using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitDraw.Request
{
    public class ScoreRequestData
    {
        public ScoreRequestData()
        {
        }

        public static string GetScoreRequest(int[] pixelValues, string id)
        {
            ScoreRequest scoreRequest = new ScoreRequest()
            {
                Id = id
            };
            ScoreData scoreDatum = new ScoreData()
            {
                GlobalParameters = new Dictionary<string, string>(),
                FeatureVector = new Dictionary<string, string>()
            };
            int num = 1;
            StringBuilder stringBuilder = new StringBuilder();
            scoreDatum.FeatureVector.Add("label", "0");
            for (int i = 0; i < (int)pixelValues.Length; i++)
            {
                object[] str = new object[] { pixelValues[i].ToString() };
                stringBuilder.AppendFormat("{0} ", str);
                int num1 = num;
                num = num1 + 1;
                if (num1 % 28 == 0)
                {
                    stringBuilder.Append(Environment.NewLine);
                }
                scoreDatum.FeatureVector.Add(string.Format("pixel{0}", i), pixelValues[i].ToString());
            }
            scoreRequest.Instance = scoreDatum;
            return JsonConvert.SerializeObject(scoreRequest);
        }

        public static ScoreRequest GetScoreRequestObject(int[] pixelValues, string id)
        {
            ScoreRequest scoreRequest = new ScoreRequest()
            {
                Id = id
            };
            ScoreData scoreDatum = new ScoreData()
            {
                GlobalParameters = new Dictionary<string, string>(),
                FeatureVector = new Dictionary<string, string>()
            };
            int num = 1;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < (int)pixelValues.Length; i++)
            {
                object[] str = new object[] { pixelValues[i].ToString() };
                stringBuilder.AppendFormat("{0} ", str);
                int num1 = num;
                num = num1 + 1;
                if (num1 % 28 == 0)
                {
                    stringBuilder.Append(Environment.NewLine);
                }
                scoreDatum.FeatureVector.Add(string.Format("pixel{0}", i), pixelValues[i].ToString());
            }
            scoreRequest.Instance = scoreDatum;
            return scoreRequest;
        }
    }
}