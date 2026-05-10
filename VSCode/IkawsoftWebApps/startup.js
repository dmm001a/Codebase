    const cstApplicationID = {
        None: 0,
        DR: 1,
        Roadmap: 2,
    };

    let sWebServerURLRoot = "";
    let sWindowLocationhref = "";
    let arrValidEnvironment = [];
      
    arrValidEnvironment = ['local', 'smoke', 'qa', 'prod'];
    
    sWebServerURLRoot = window.location.origin;
    sWindowLocationhref = window.location.href;
    
    
    const sApplicationURLRoot = sWebServerURLRoot + "/" + mGetApplicationEnvironment(window.location.href) + "/app/" + mGetApplicationName(window.location.href) + "/"; 
    
    const sSharedLibraryRootURL = sWebServerURLRoot + "/" + mGetApplicationEnvironment(window.location.href) + "/shared_library/";

    
    function mGetApplicationEnvironment(sFullURL) {
        try {
            
            let sURLExtension = "";
            let sApplicationEnvironment = "";
            let arrURLParts = [];


            sURLExtension = sFullURL.replace(window.location.origin, "");
            sURLExtension = sURLExtension.replace(/^\/+/, "");

            arrURLParts = sURLExtension.split('/');
            sApplicationEnvironment = arrURLParts[0];
	    
	        sApplicationEnvironment = sApplicationEnvironment.toLowerCase();


            if (arrValidEnvironment.includes(sApplicationEnvironment)) {
                return sApplicationEnvironment;
            } else {
                throw new Error("Valid environment is not found in URL.");
            }

        } catch (oError) {
            console.log("Error: " + oError.message);
            throw oError;
        }
    }

    function mGetApplicationID(sURLExtension) {

	    try {

            let iReturnApplicationID = -1;
            
            sURLExtension = sURLExtension.toLowerCase();
            
            for (const [sApplicationName, iApplicationID] of Object.entries(cstApplicationID)) {
            
                if (sURLExtension.includes("/" + sApplicationName.toLowerCase() + "/")) {
                    iReturnApplicationID = iApplicationID;
                    break;
                }
                
            }	    
	    
	    return iReturnApplicationID; 

        } catch (oError) {
            console.log("Error: " + oError.message);
            throw oError;
        }
    }

    function mGetApplicationName(sFullURL, sApplicationName = "") {
        try {
            
            let sURLExtension = "";
            let sApplicationName = "";
            let arrURLParts = [];
            let arrValidApplicationName = [];

            sURLExtension = sFullURL.replace(window.location.origin, "");
            sURLExtension = sURLExtension.replace(/^\/+/, "");

            arrURLParts = sURLExtension.split('/');

            if (sApplicationName.length === 0) {
                sApplicationName = arrURLParts[2];
            }
            
            arrValidApplicationName = ['DR', 'Timeline'];

            if (arrValidApplicationName.includes(sApplicationName)) {
                return sApplicationName;
            } else {
                throw new Error("Valid Application Name is not found in URL.");
            }

        } catch (oError) {
            console.log("Error: " + oError.message);
            throw oError;
        }
    }

    function mSetStatus(sMessage, oErrorObject = null) {

        let sDateStamp = "";
        let sTimeStamp = "";
        let sDateTimeStamp = "";
        let sDateTimeMilliSecondStamp = "";
        let sCurrentStatusMessage = "";
        let iLineNumber = 0;
        let iColumnNumber = 0;
        let bErrorObjectPresent = false;
        let arrStackLines = [];
        let oOriginLine = null;
        let oMatch = null;


        try {

            if (oErrorObject !== null) {
                bErrorObjectPresent = true;
            }

            if ((bErrorObjectPresent === true) && (oErrorObject?.message)) {

                arrStackLines = oErrorObject.stack.split("\n");

                if (arrStackLines.length > 1) {
                    oOriginLine = arrStackLines[1];
                    oMatch = oOriginLine.match(/:(\d+):(\d+)\)?$/);
                    iLineNumber = oMatch ? oMatch[1] : "unknown";
                    iColumnNumber = oMatch ? oMatch[2] : "unknown";
                }

                sMessage = sMessage + " " + "Error Message: " + oErrorObject.message + " Stack Trace: " + oErrorObject.stack;
                sMessage = sMessage + " Line Number: " + iLineNumber + " Column Number:  " + iColumnNumber;
                if (typeof giRowID !== "undefined") {
                    sMessage += " Row ID: " + String(giRowID);
                }

           }


            if (sMessage.length > 0) {

                const dNow = new Date();

                // Format the date part
                const dDatePart = new Intl.DateTimeFormat("en-US", {
                    timeZone: "America/Chicago",
                    year: "numeric",
                    month: "2-digit",
                    day: "2-digit"
                }).format(dNow);

                // Format the time part manually
                const tTimePart = dNow.toLocaleTimeString("en-US", {
                    timeZone: "America/Chicago",
                    hour: "2-digit",
                    minute: "2-digit",
                    second: "2-digit",
                    hour12: true
                });

                // Extract milliseconds
                const iMilliseconds = dNow.getMilliseconds().toString().padStart(3, '0');

                // Inject milliseconds after seconds
                const tTimeWithMs = tTimePart.replace(/(\d{2}:\d{2}:\d{2})/, `$1:${iMilliseconds}`);

                sDateStamp = dDatePart.toString();
                sTimeStamp = tTimePart.toString();
                sDateTimeStamp = " " + sDateStamp + " " + sTimeStamp;
                sDateTimeMilliSecondStamp = " " + sDateStamp + " " + tTimeWithMs.toString();
                

                if (bErrorObjectPresent === false) {
                    console.log(sMessage + sDateTimeMilliSecondStamp);
                } else if (bErrorObjectPresent === true && sMessage.length > 0) {
                    sMessage = "System Error: " + sMessage;
                    console.error(sMessage + sDateTimeMilliSecondStamp);
                }
            }

            if (oErrorObject && "AlertMessage" in oErrorObject) {
                  if (oErrorObject.AlertMessage.length > 0) {
                        alert(oErrorObject.AlertMessage);
                  }
            }            

            if ((bErrorObjectPresent === false) || (bErrorObjectPresent === true && gbFirstError === true)) {
                const oCenterPanel = document.getElementById("status-center");

                if (oCenterPanel) {
                    sCurrentStatusMessage = oCenterPanel.textContent;

                    if (sCurrentStatusMessage.substring(0, 12) !== "System Error:") {
                        oCenterPanel.textContent = sMessage.substring(0, 75) + sDateTimeStamp;
                    }
                    
                } else {
                    console.error("Status bar panel cannot be found.");
                }
                
                if (bErrorObjectPresent === true) {
                    gbFirstError = false;
                }
            }

        } catch (oError) {
            console.error(oError.message);
            throw new Error("Error occurred.  Please review.");
        }

    }
    