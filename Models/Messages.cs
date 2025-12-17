using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Messages
    {
        public int Id {get; set;}
        public int SenderId {get; set;}
        public int EmpfaengerId {get; set;}
        public string Message {get; set;}
        public DateTime SentAt {get; set;}

    }
}
