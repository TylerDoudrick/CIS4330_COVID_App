using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CIS4330_COVID_Web
{
    public class Person
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}