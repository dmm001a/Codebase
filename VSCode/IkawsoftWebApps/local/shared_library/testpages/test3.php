<!DOCTYPE html>
<html lang="en">
 <head>
   <title>Disaster Recovery Design</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    

 
 </head>
 
 <body style="margin-top: 0px; margin-left: 0px;">

        <table id="StandardTable" class="StandardTable" style="border-spacing:0;width:100%;">
            <thead>
            </thead>
            <tbody>
            </tbody>
        </table>



<input type="text" id="txtMyTextbox" value="" onblur="mBroadcastMessage()" />

<script>



const oSocket = new WebSocket("wss://ikawbroadcastserver-bchabwbvhbhjc4gj.canadacentral-01.azurewebsites.net");

// Catch Connection Open
oSocket.onopen = () => {
  try {
        //There's nothing to catch here other than to log the succesfully open if desired
  } catch (oError) {
      mSetStatus("oSocket.onopen", oError);
  }  
};

// Catch Connection Close
oSocket.onclose = (oEvent) => {

  try {

      let bNormalClose = false;

      if (oEvent.code === 1000 || oEvent.code === 1001) {
        bNormalClose = true;
      }

      if (bNormalClose === false) {
        console.warn("?? WebSocket connection closed", {
          code: oEvent.code,
          reason: oEvent.reason,
          wasClean: oEvent.wasClean
        });
      }
  } catch (oError) {
      mSetStatus("oSocket.onclose", oError);
  }

};

// Catch Connection Error Occurred
oSocket.onerror = (oError) => {
  try {

      mSetStatus("mInsertRecord", oError);

  } catch (oErrorLocal) {
      alert("oSocket.onerror " + oErrorLocal.message);
  }
};

// Received a Message From a Transmitter
oSocket.onmessage = (oEvent) => {
  try {

        
        document.getElementsByName("txtMyTextbox")[0].value = oEvent.data;


  } catch (oError) {
      mSetStatus("oSocket.onmessage", oError);
  }        
};

// Send a Message as a Broadcaster
function mBroadcastMessage() {

    try {

        const input = document.getElementById("txtMyTextbox");
        const message = input.value;

        if (oSocket.readyState === WebSocket.OPEN) {
          oSocket.send(message);
          console.log("?? Message sent:", message);
        } else {
          console.error("? Cannot send message — WebSocket not open", {
            readyState: oSocket.readyState
          });
        }

          alert("sent");

  } catch (oError) {
      mSetStatus("mBroadcastMessage", oError);
  }        
}
</script>



 </body>
</html>

