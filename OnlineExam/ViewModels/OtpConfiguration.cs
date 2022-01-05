using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.ViewModels
{
    public class OtpConfiguration
    {
        public OtpConfiguration(int time, int length)
        {
            Time = time;
            Length = length;
        }
        public int Time { get; set; }
        public int Length { get; set; }
    }
}