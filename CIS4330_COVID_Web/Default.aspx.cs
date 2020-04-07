using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CIS4330_COVID_Web
{
    public partial class Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {



        }

        protected void btnGrabRecords_Click(object sender, EventArgs e)
        {
            var client = new MongoClient("mongodb+srv://app:y1tumPKKL0olcDEZ@cis4330-apby0.mongodb.net/test?retryWrites=true&w=majority");
            var database = client.GetDatabase("app");
            var collection = database.GetCollection<BsonDocument>("test");
            var list = collection.Find(new BsonDocument())
    .ToList();

            foreach (BsonDocument doc in list)
            {
                Response.Write(doc.ToString());
            }
        }
        protected void btnAddRecord_Click(object sender, EventArgs e)
        {
            var client = new MongoClient("mongodb+srv://app:y1tumPKKL0olcDEZ@cis4330-apby0.mongodb.net/test?retryWrites=true&w=majority");
            var database = client.GetDatabase("app");
            var collection = database.GetCollection<BsonDocument>("test");
            var doc = new BsonDocument { { "Name", txtName.Text } };
            collection.InsertOne(doc);
            var list = collection.Find(new BsonDocument())
    .ToList();

            foreach (BsonDocument docum in list)
            {
                Response.Write(docum.ToString());
            }
        }
    }

}