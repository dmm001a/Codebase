const oHttp = require('http');
const { WebSocket } = require('ws');

// Create HTTP server with request handler
const server = oHttp.createServer((req, oRes) => {
  oRes.writeHead(200, { 'Content-Type': 'text/plain' });
  oRes.end();
});

// Create WebSocket server bound to the same HTTP server
const oWss = new WebSocket.Server({ server });

// Track active connections
const oConnections = new Set();

// Broadcast the Message to the clients
oWss.on('connection', (oWs) => {
  oConnections.add(oWs);
  console.log('?? WebSocket client connected');

  oWs.on('message', (message) => {
    console.log('?? Received message:', message);

    for (const oConn of oConnections) {
      if (oConn.readyState === WebSocket.OPEN) {
        const sMessage = typeof message === "string" ? message : message.toString();   // ? ONLY CHANGE
        oConn.send(sMessage);
      }
    }
  });

  oWs.on('close', () => {
    oConnections.delete(oWs);
  });

  oWs.on('error', (err) => {
    console.error('WebSocket error:', err);
  });
});

setInterval(() => {
  for (const ws of oConnections) {
    if (ws.readyState === WebSocket.OPEN) {
      ws.ping();
    }
  }
}, 30000);

const oPORT = process.env.PORT || 3000;
server.listen(oPORT, () => {
  console.log(`WebSocket server running on port ${oPORT}`);
});



