
    /*function downloadTableAsExcel(tableId, filename = 'export.xls') {
        try {

            const table = document.getElementById(tableId);
            const html = table.outerHTML.replace(/ /g, '%20');
            const uri = 'data:application/vnd.ms-excel,' + html;

            const link = document.createElement('a');
            link.href = uri;
            link.download = filename;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

        } catch (oError) {
           mSetStatus("downloadTableAsExcel", oError);
        }
    }*/

    
    function mOpenBrowserWindow(sWindowName, iWidth, iHeight, bResizable = true, bScrollbars = true, sPageURL = "") {

        try {        

                let sResizableValue = "";
                let sScrollBarsValue = "";

                if (mIsNumeric(iWidth) && mIsNumeric(iHeight)) {

                    // Calculate centered position
                    const iLeft = (window.screen.width  - iWidth)  / 2;
                    const iTop  = (window.screen.height - iHeight) / 2;

                    if (bResizable === false) {
                        sResizableValue = "no";
                    } else {
                        sResizableValue = "yes";
                    }

                    if (bScrollbars === false) {
                        sScrollBarsValue = "no";
                    } else {
                        sScrollBarsValue = "yes";
                    }

                    const sWindowFeatures =
                        "width=" + iWidth +
                        ",height=" + iHeight +
                        ",left=" + iLeft +
                        ",top=" + iTop +
                        ",resizable=" + sResizableValue +      // allows maximize button
                        ",scrollbars=yes" + sScrollBarsValue;

                    const oBrowserWindow = window.open(sPageURL, sWindowName, sWindowFeatures);

                    return(oBrowserWindow);

                } else {
                    throw new Error("Width and Height must be numeric.");
                }

        } catch (oError) {
           mSetStatus("mOpenBrowserWindow", oError);
        }                 

    }

    async function mDisplayNotificationAlert(sAlertMessage) {
        try {

            const oNotification = new Notyf({
                types: [
                        {
                            type: 'BroadcastNotification',
                            className: 'BroadcastNotificationStyle',
                            background: cstHexColorCode.YellowAlert,
                            duration: 6000,
                            dismissible: true
                        }
                    ],
                    position: { x: 'center', y: 'top' }
                });

            oNotification.open({
                type: 'BroadcastNotification',
                message: sAlertMessage,
                dismissible: true
            });

        } catch (oError) {
           mSetStatus("mDisplayNotificationAlert", oError);
        }    
    }

    async function mFinalizePage() {
        try {  

            mHandlePageSetup();
            mSetStatus("");


        } catch (oError) {
           mSetStatus("mFinalize", oError);
        }              
    }

    function mGetStandardTableControlSetting(sControlConfig) {
        try {  

            let sReturnValue = "";

            if (sControlConfig.length === 23) {
                sReturnValue = sControlConfig.slice(-1);
            } else {
                //console.log(sControlConfig + "Control Config is not 23 characters");
            }

            return(sReturnValue);

        } catch (oError) {
           mSetStatus("mGetStandardTableControlSetting", oError);
        }              
    }    

    function mGetCallerFunctionName() {
        try {          

            const stack = new Error().stack.split("\n");
            return stack[3]?.trim(); // 0=Error, 1=this function, 2=wrapper, 3=actual caller

        } catch (oError) {
           mSetStatus("mGetCallerFunctionName", oError);
        }    
    }

    function mGetUUID() {
        try {        

            if (crypto.randomUUID) {
                return crypto.randomUUID();
            }

            // Fallback for older browsers and http
            return ([1e7]+-1e3+-4e3+-8e3+-1e11).replace(/[018]/g, c =>
                (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
            );

        } catch (oError) {
            mSetStatus("mGetUUID", oError);
        }        
    }


    function mCopyToClipboard(vClipboardValue, sAddOnMessage = "") {
        try {     

            navigator.clipboard.writeText(vClipboardValue).then(() => {
                alert(vClipboardValue + "  is now copied to your clipboard.  " + sAddOnMessage);
            });

        } catch (oError) {
            mSetStatus("mCopyToClipboard", oError);
        }             
    }

    async function mUploadImage(oImage, sUploadDirectoryPath) {

        try {

            const oFormData = new FormData();
            oFormData.append("UploadImage", oImage, "temp_name.png");
            oFormData.append("UploadDirectoryPath", encodeURIComponent(sUploadDirectoryPath));

            const sUrl = cstURL.UploadArtifactImage;

            const oResponse = await fetch(sUrl, {
                method: "POST",
                body: oFormData
            });

            if (!oResponse.ok) {
                throw new Error(oResponse.status);
            }

            const oJSON = await oResponse.json();

            if (oJSON.error === true) {
                throw new Error(oJSON.message || "Unknown upload error");
            }

            return oJSON;

        } catch (oError) {
            mSetStatus("mUploadImage", oError);
        }
    }

