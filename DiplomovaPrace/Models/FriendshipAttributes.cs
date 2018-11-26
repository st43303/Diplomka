using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiplomovaPrace.Models
{
    [MetadataType(typeof(FriendshipAttributes))]
    public partial class Friendship { }
    public class FriendshipAttributes
    {
        public int ID_UserA { get; set; }
        public int ID_UserB { get; set; }
        public bool Checked { get; set; }
    }
}