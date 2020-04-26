var mongo = require('mongodb');
var express = require("express");
var bodyParser = require('body-parser');

const path = require('path');

const MongoClient = require('mongodb').MongoClient;
const uri = "mongodb+srv://api:8JhmzQ24r7QbAiEQ@cis4330-apby0.mongodb.net/test?retryWrites=true&w=majority";
var mongoClient = new MongoClient(uri, {
  useNewUrlParser: true
});


var app = require('express')();
var http = require('http').createServer(app);
var io = require('socket.io')(http);

app.use(express.static('public'));

app.use(bodyParser.json());
app.set('view engine', 'ejs');


//  /covid route
app.get('/covid', (req, res) => {
  MongoClient.connect(uri, function (err, client) {

    const db = client.db("app");
    var collection = db.collection("locations");
    //Grab the locations from the db
    collection.find({}).toArray(function (err, results) {


      //Send the locations object to the template file
      res.render('heatmap', {
        "data": results
      });
    });



    client.close();
  });
});

app.post('/api/coordinates', function (req, res) {
  //
  //Uncomment to store the timestamp in the db with the location
  //
  //  To be used if limiting the number of locations we log for each person per day
  //
  //req.body.timestamp = new Date();
  //

  MongoClient.connect(uri, function (err, client) {

    const db = client.db("app");
    var collection = db.collection("locations");
    //Post the json document to the server
    collection.insertOne(req.body);

    //Send the event to the client socket containing the new location
    io.sockets.emit('newData', req.body);

    client.close();
  });

});


io.on('connection', function (socket) {
  console.log("Socket connected: " + socket.id);
});





http.listen(3000, function () {
  console.log('listening on *:3000');
});