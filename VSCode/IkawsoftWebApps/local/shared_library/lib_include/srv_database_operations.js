        async function mGetRecordset(sUrl, oArgumentData) {

            try {

                let iApplicationID = -1;
                let arrJSONRst = [];

                iApplicationID = mGetApplicationID(cstURL.URLExtension);

                oArgumentData.append("ApplicationID", iApplicationID);

                const oResponseRst = await fetch(sUrl, {
                    method: "POST",
                    body: oArgumentData
                });
                
                if (!oResponseRst.ok) {
                    throw new Error(oResponseRst.status);
                }

                arrJSONRst = await oResponseRst.json();   

                // Normalize: always return an array
                if (!Array.isArray(arrJSONRst)) {
                    arrJSONRst = [arrJSONRst];
                }

                /*if (arrJSONRst.error) {
                    throw new error(arrJSONRst.message);
                }*/

                if (arrJSONRst.length > 0 && arrJSONRst[0]?.error === true) {
                    throw new Error(arrJSONRst[0].message);
                }

                return arrJSONRst;

            } catch (oError) {
                mSetStatus("mGetRecordset", oError);
            }
        }

        async function mGetHTMLControlTable(sGetRecordsetURL, iApplicationID, iPlatformID, iPageID, iActionID) {
            try {
                let oArgumentData = null;

		
                oArgumentData = new FormData();
		        oArgumentData.append("ApplicationID", iApplicationID);
                oArgumentData.append("PlatformID", iPlatformID);
                oArgumentData.append("PageID", iPageID);
                oArgumentData.append("ActionID", iActionID);

                arrHTMLControlTable = await mGetRecordset(sGetRecordsetURL, oArgumentData);

                return(arrHTMLControlTable);

            } catch (oError) {
                mSetStatus("mGetHTMLControlTable", oError);
            }
        }
        
        function mGetExecuteSQLArray(iCommandType, iTableType, oRow, arrFieldNames, arrFieldValues) {
            try {

                //Declare Variables
                let sControlName = "";
                let sDBColumnName = "";
                let sAuditActorIDFieldName = "";
                let sAuditDateTimeFieldName = "";
                let iControlType = -1;
                let iControlSetting = -1;
                let bPrimaryKey = false;
                let bAddTableDisplay = false;
                let bRequiredForAdd = false;
                let bIncludeControl = false;
                let bReadOnlyDisabled = false;
                let bIncludeAudit = false;
                let vControlValue = "";
                let arrPrimaryKeyFieldNames = [];
                let arrPrimaryKeyFieldValues = [];
                let oControl = null;
                let oControlClass = null; //only used to get the control setting value for the control in the array
                
                const cstConvertBlankToNull = new Set([
                    cstControlType.Timebox,
                    cstControlType.DatePicker,
                    cstControlType.DateTimePicker,
                ]);

                //Init Variables
                oControlClass = new cControl;

                //Setup Variables for Routing
                if (iCommandType === cstCommandType.Insert) {
                    bIncludeAudit = true;
                    sAuditActorIDFieldName = "Entered_By";
                    sAuditDateTimeFieldName = "Entered_Date";
                } else if (iCommandType === cstCommandType.Update) {
                    bIncludeAudit = true;
                    sAuditActorIDFieldName = "Last_Update_By";
                    sAuditDateTimeFieldName = "Last_Update_Date";
                }

                arrHTMLNoButtonControlTable.forEach(arrHTMLControl => {
                    
                    //Initialize Variables
                    sControlName = arrHTMLControl.ControlName;
                    iControlType = arrHTMLControl.ControlType;
                    sDBColumnName = arrHTMLControl.DBFieldName;
                    bPrimaryKey = Boolean(arrHTMLControl.PrimaryKey);
                    vControlValue = "";
                    
                    //bAddTableDisplay = Boolean((arrHTMLControl.AddTableDetail === 1)); //This will return false if arrHTMLControl.AddTableDisplay is not 1
                    iControlSetting = oControlClass.mGetControlSetting(iTableType, cstRowType.Detail,  arrHTMLControl.ControlConfig);
                    if (iControlSetting === cstControlSetting.Control_Visible) {
                        bAddTableDisplay = true;
                    } else {
                        bAddTableDisplay = false;
                    }
                    
                    if (iControlSetting === cstControlSetting.Control_Read_Only_Visible) {
                        bReadOnlyDisabled = true;
                    } else {
                        bReadOnlyDisabled = false;
                    }
                    
                    bRequiredForAdd =  Boolean((arrHTMLControl.RequiredForAdd)); 
                    bIncludeControl = false;

                    //One Table Block Of Logic For Control Inclusion
                    if ((iTableType === cstTableType.AddTable) && (bAddTableDisplay === true)) {
                        bIncludeControl = true;
                    } else if (iTableType !== cstTableType.AddTable && (bReadOnlyDisabled === false || bPrimaryKey === true)) {
                        bIncludeControl = true;
                    }

                    //One Control Block Of Logic For Control Inclusion
                    if (iControlType === cstControlType.MultiSelectDropDown && sDBColumnName.includes(".") === true) {      
                        bIncludeControl = false;
                    }

                    if (bIncludeControl === true) {

                        if (iControlType !== cstControlType.MultiSelectDropDown) {

                            oControl = oRow.querySelector(`[name="${sControlName}"]`);
                            vControlValue = oControl.value.trim(); 

                            if (iControlType === cstControlType.Timebox && vControlValue.length > 0) {
                                vControlValue = mConvertTimeToSqlTimeFormat(vControlValue);                              
                            } else if (iControlType === cstControlType.Checkbox) {
                                vControlValue = mToggleBetweenBitBoolean(oControl.checked);
                            }
                            
                            //Convert Zero Length String to Null For Certain Control Types
                            if (cstConvertBlankToNull.has(iControlType) && vControlValue.length === 0) {
                                vControlValue = null;
                            }

                        } else if (iControlType === cstControlType.MultiSelectDropDown) {

                            const oTempControl = oRow.querySelector(`[name="${sControlName}"]`);
                            const oSelectedOptions = oTempControl.querySelectorAll(".multi-select-option.multi-select-selected[data-value]");

                            oSelectedOptions.forEach((oSelectedOption, index) => {
                                const sValue = oSelectedOption.getAttribute("data-value");
                                vControlValue += sValue;
                                if (index < oSelectedOptions.length - 1) {
                                    vControlValue += ",";
                                }
                            });
                        }

                        
                        if (bPrimaryKey === false) {
                            arrFieldNames.push(sDBColumnName);
                            arrFieldValues.push(vControlValue);
                        } else if (bPrimaryKey === true) {
                            arrPrimaryKeyFieldNames.push(sDBColumnName);
                            arrPrimaryKeyFieldValues.push(vControlValue);
                        }
                        
                    }

                });

                //Add Audit Values, i.e. Entered_By, etc
                if (bIncludeAudit === true) {
                    //Who
                    arrFieldNames.push(sAuditActorIDFieldName);
                    arrFieldValues.push(giUserID);

                    //DateTime Stamp
                    arrFieldNames.push(sAuditDateTimeFieldName);
                    dtDateTimeStamp = mGetSQLDateTimeCentral(new Date());
                    arrFieldValues.push(dtDateTimeStamp);
                }

                arrFieldNames.push(...arrPrimaryKeyFieldNames);
                arrFieldValues.push(...arrPrimaryKeyFieldValues);

                oControlClass = null;
                
            } catch (oError) {
                mSetStatus("mGetExecuteSQLArray", oError);
            }
        }

        async function mGetNewID(sRstUrl, iYearID, gsTableName, cGetRecordsetAction, arrFieldName = [], arrFieldValue = [], iTestID = -1) {
                
            try {

                let sColumnName = "";
                let iNewID = -1;
                let iApplicationID = -1;
                let arrJSONRst = null;

                iApplicationID = mGetApplicationID(cstURL.URLExtension);
                
                if (cGetRecordsetAction === cstGetRecordsetAction.New_Sequence_ID) {
                    sColumnName = "Sequence_ID";
                } else if (cGetRecordsetAction === cstGetRecordsetAction.New_Test_ID) {
                    sColumnName = "Test_ID";
                } else if (cGetRecordsetAction === cstGetRecordsetAction.New_Row_ID) {                    
                    sColumnName = "Row_ID";
                } else {
                    throw new Error("Column Get Recordset Action is Invalid for mGetNewID");
                }
                
                const oArgumentData = new FormData();
                oArgumentData.append("ApplicationID", iApplicationID);
                oArgumentData.append("SQL:ColumnName", sColumnName);
                oArgumentData.append("SQL:TableName", gsTableName);  
                oArgumentData.append("PlatformID", giPlatformID);
                oArgumentData.append("YearID", iYearID);
                if (iTestID > -1) {
                    oArgumentData.append("TestID", iTestID);
                }
                oArgumentData.append("ActionID", cGetRecordsetAction);

                const oResponse = await fetch(sRstUrl, {
                    method: "POST",
                    body: oArgumentData
                });

                const sRawText = await oResponse.text();

                if (!oResponse.ok) {
                    throw new Error(`HTTP error! Status: ${oResponse.status}`);
                }

                try {
                    arrJSONRst = JSON.parse(sRawText); // safer than await oResponse.json() after .text()
                    iNewID = arrJSONRst[0].New_ID;

                    arrFieldName.push(sColumnName);
                    arrFieldValue.push(iNewID);

                } catch (jsonError) {
                    mSetStatus("Non-JSON response received:\n" + sRawText);
                    throw new Error("Failed to parse JSON: " + jsonError.message);
                }

                if (arrJSONRst.error) {
                    mSetStatus("mGetNewID", arrJSONRst.message);
                }

                return iNewID;

            } catch (oError) {
                mSetStatus("mGetNewID", oError);
            }
            
        }


        
    async function mCallExecuteSQL(sExecuteSQLURL, iCommandType, sTableName, arrFieldNames, arrFieldValues) {
            
	        let sErrorMessage = "";
            let iApplicationID = -1;
            let iRecordsAffected = 0;
	        let iErrorNumber = -1;
            let oArgumentData = null;
            let oData = null;
            let oErrorData = null;

            iApplicationID = mGetApplicationID(cstURL.URLExtension);

            oArgumentData = new FormData();
            oArgumentData.append("ApplicationID", iApplicationID);
            oArgumentData.append("CommandType", iCommandType);
            oArgumentData.append("TableName", sTableName);
            oArgumentData.append("arrFields", JSON.stringify(arrFieldNames));
            oArgumentData.append("arrValues", JSON.stringify(arrFieldValues));

            try {
		
                const oResponse = await fetch(sExecuteSQLURL, {method: "POST", body: oArgumentData});
	            const oData = await oResponse.json();

                if (oResponse === null || oData === null) {
                    
                    throw new Error("oResponse or oData is null.  Cannot proceed.");
                    
                } else if (oResponse.ok === false || (oData.error === true)) {
                    
                    if (mIsNumeric(oData.errornumber) === true) {
                    
                        iErrorNumber = Number(oData.errornumber);
                        sErrorMessage = mErrorNumberProcessing(iErrorNumber);
                        
                        if (!sErrorMessage || sErrorMessage.length === 0) {
                            sErrorMessage = oData.message;
                        } else {
                            alert(sErrorMessage);
                        }
                        
                        throw new Error(sErrorMessage);
                    }
                    
                } else {
                    return oData; // ? Data returned to caller
                }


            } catch (oError) {
                oData = null;
                mSetStatus("mCallExecuteSQL", oError);
                throw oError; 
            }

            /*oArgumentData = null;
            return oData; // ? Data returned to caller*/
        }

