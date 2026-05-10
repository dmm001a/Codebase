    
    async function mInitializeApp(iApplicationID, iPlatformID, iYearID, iPageID, iUserID, iUserTypeID) {
        try {

            //Load Globals
            await mSetGlobalVariables(iPlatformID, iYearID, iPageID, iUserID, iUserTypeID);
            await mLoadLookupTable(iPlatformID, iUserTypeID, iYearID);

            //Initiatlize Broadcast Client
            if (!oBroadcastClient) {
                oBroadcastClient = new cBroadcastClient(cstURL.WebSocketServer);
            }

            if (iPlatformID !== -1 && iPageID !== -1) {
                await mLoadHTMLControlTable(iApplicationID, iPlatformID, iPageID);
            }
            
            //await mLoadColumnDataTypeArray();
            await mPopulateLookupDictionary(dctPlatform, "Platform");
            await mPopulateLookupDictionary(dctYear, "Year");
            await mPopulateLookupDictionary(dctPage, "Page");
            await mPopulateLookupDictionary(dctUserType, "User_Type");

            //Load Remaining Globals
            gsPlatform = await mGetKeyFromValue(dctPlatform, iPlatformID);
            giYear = await mGetKeyFromValue(dctYear, iYearID);
            gsPageName = await mGetKeyFromValue(dctPage, iPageID);
            gsTableName = cstTableName[gsPageName];
            gsUserType = await mGetKeyFromValue(dctUserType, iUserTypeID);
            
            
        } catch (oError) {
            mSetStatus("mInitializeApp: " + oError.message);
        }
    }

    async function mSetGlobalVariables(iPlatformID, iYearID, iPageID, iUserID, iUserTypeID) {
        try {
            
            gsClientID = mGetUUID();
            giPlatformID = Number(iPlatformID);
            giYearID = Number(iYearID);
            giPageID = Number(iPageID);
            giUserID = Number(iUserID);
            giUserTypeID = Number(iUserTypeID);
	    gsApplicationEnvironment = mGetApplicationEnvironment(window.location.href);

        } catch (oError) {
           mSetStatus("mSetGlobalVariables", oError);
        }
    }

    async function mLoadLookupTable(iPlatformID, iUserTypeID, iYearID) {
        try {
	    
	        let iApplicationID = mGetApplicationID(cstURL.URLExtension);
	    
            let oArgumentData = new FormData();
            oArgumentData.append("ActionID", cstGetRecordsetAction.Lookup);
	        oArgumentData.append("ApplicationID1", iApplicationID);            	    	    
            oArgumentData.append("PlatformID1", iPlatformID);
            oArgumentData.append("UserTypeID", iUserTypeID);
            oArgumentData.append("PlatformID2", iPlatformID);
            oArgumentData.append("PlatformID3", iPlatformID);            
            oArgumentData.append("YearID1", iYearID);            	    
            oArgumentData.append("PlatformID4", iPlatformID);   	    
            oArgumentData.append("ApplicationID2", iApplicationID);      	    
            oArgumentData.append("ApplicationID3", iApplicationID);      	    	    
            
            arrLookupTable = await mGetRecordset(cstURL.GetRecordset, oArgumentData);
            
            
        } catch (oError) {
           mSetStatus("mLoadLookupTable", oError);
        }
    }

    async function mLoadHTMLControlTable(iApplicationID, iPlatformID, iPageID) {
        try {
	    
            arrHTMLControlTable = await mGetHTMLControlTable(cstURL.GetRecordset, iApplicationID, iPlatformID, iPageID, cstGetRecordsetAction.HTMLControl);
            arrHTMLNoButtonControlTable = arrHTMLControlTable.filter(oRow => oRow.ControlType !== 15);
            
        } catch (oError) {
           mSetStatus("mLoadHTMLControlTable", oError);
        }
    }

    async function mProcessHourByHourPlanPageLoad() {
    
        try {
        
            let oHTMLObject = null;
            
            if (giPageID === dctPage["Hour By Hour Plan"])  {
                
                oHTMLObject = document.getElementById(cstElementID.DivSubSelect);

                if (oHTMLObject) {
                    oHTMLObject.style.display = "";
                }

                oHTMLObject = document.getElementById(cstElementID.DivTestSummary);

                if (oHTMLObject) {
                    oHTMLObject.style.display = "";
                }                

                mProcessViewInfoButton();
               
                oHTMLObject = mGetHTMLTable(cstTableType.StandardTable);

                mToggleFieldSetLock(cstElementID.FstStandardTable, oHTMLObject, gbTestLock);

            } else if (giPageID === dctPage["Plan Template"])  {

                oHTMLObject = document.getElementById("DivSubSelect");

                if (oHTMLObject) {
                    oHTMLObject.style.display = "";
                }

                oHTMLObject = document.getElementById(cstElementID.DivTestSummary);

                if (oHTMLObject) {
                    oHTMLObject.style.display = "none";
                }      

                oHTMLObject = mGetHTMLTable(cstTableType.StandardTable);

                mToggleFieldSetLock(cstElementID.FstStandardTable, oHTMLObject, false);      

            } else {
                oHTMLObject = document.getElementById("DivSubSelect");

                if (oHTMLObject) {
                    oHTMLObject.style.display = "none";
                }

                oHTMLObject = document.getElementById("DivTestSummary");

                if (oHTMLObject) {
                    oHTMLObject.style.display = "none";
                }      

                oHTMLObject = mGetHTMLTable(cstTableType.StandardTable);

                mToggleFieldSetLock(cstElementID.FstStandardTable, oHTMLObject, false);
            }


        } catch (oError) {
            mSetStatus("mProcessHourByHourPlanLoad", oError);
        }
    }


