using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpMulticastChatHW.DBModel
{
    public class DBUser
    {
        public int Id { get; set; }
        [MinLength(4), MaxLength(32)]
        public string Name { get; set; }
        [MinLength(6), MaxLength(128)]
        public string Password { get; set; }
    }
}
