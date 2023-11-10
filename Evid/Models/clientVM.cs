using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Evid.Models
{
    public class clientVM
    {
        public clientVM() 
        {
            this.SpotList = new List<int>(); 
        }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public string Picture { get; set; }
        public HttpPostedFileBase PictureFile { get; set; }
        public bool MaritalStatus { get; set; }
        public List <int>SpotList { get; set; }
    }
}