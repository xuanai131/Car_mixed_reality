const path = require("path");
const express = require("express");
const WebSocket = require("ws");
// const { stringify } = require("querystring");
const app = express();

const WS_PORT = 8888;
const HTTP_PORT = 8000;

const wsServer = new WebSocket.Server({ port: WS_PORT }, () => console.log(`WS Server is listening at ${WS_PORT}`));

// const wss = new WebSocket.Server({port: 8080},()=>{
// 	console.log('server started')
// })


// wss.on('listening',()=>{
// 	console.log('server is listening on port 8080')
// })

// wss.on('connection',(ws)=>{
// 	ws.on('message',(data)=>{
// 		console.log('data received %o ' + data)
// 		ws.send(data)
// 	})
// })


// wss.on('connection',(wsServer)=>{
// 	data = "Hello"
// 	wsServer.send(data)
// })


let connectedClients = [];
wsServer.on("connection", (ws, req) => {
    console.log("Connected");
    connectedClients.push(ws);

    ws.on("message", (data) => {
        var l = data.byteLength
            //console.log('data received %o' + l);
        connectedClients.forEach((ws, i) => {
            if (connectedClients[i] == ws && ws.readyState === ws.OPEN) {
                ws.send(data);
                //console.log('data received %o');
            } else {
                connectedClients.splice(i, 1);
                //console.log('nothing');
            }
        });
    });
});

app.get("/client", (req, res) => res.sendFile(path.resolve(__dirname, "./client.html")));
app.listen(HTTP_PORT, () => console.log(`HTTP server listening at ${HTTP_PORT}`));