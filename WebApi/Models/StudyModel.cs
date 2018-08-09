using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApi.Models
{
    [DataContract]
    public class StudyModel
    {
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string ApplicationName { get; set; }
        [DataMember]
        public string CreatedDate { get; set; }

    }
}