
        async function mGetPrimaryKey(arrPageHTMLControls) {

            try {

                let oPrimaryKeyRow = null;

                oPrimaryKeyRow = mFindFromArray (arrPageHTMLControls, "PrimaryKey", 1);

                alert(oPrimaryKeyRow.DBFieldName);

            } catch (oError) {
                mSetStatus("mSaveRelatedTable", oError);
            }
        }

        async function mSaveRelatedTable(oRow, oExtensibilityArgument) {

            try {

                let sRelatedTableName = "";
                let sRelatedFieldName = "";
                //let sPrimaryKeyFieldName = "";
                let sRelatedRemovalClause = "";
                let iDropDownValue = -1;
                let arrExistingApplicationID = [];
                let arrFieldName = [];
                let arrFieldValue = [];
                let arrSelectedValue = [];
                let oArgumentData = new FormData();
                let oHTMLControl = null;
                let oArrayFind = null;
                let oControlInstance = null;
                let oSelectedOptions = null;
                let oExecuteSQLResult = null;

                if (!oExtensibilityArgument) {
                    throw new Error("oExtensibilityArgument cannot be null.");
                }

                for (const arrControlRow of arrHTMLNoButtonControlTable) {

                    //Get the control class
                    oHTMLControl = mGetHTMLControlSettingObject(arrControlRow);

                    if ((oHTMLControl.ControlType === cstControlType.MultiSelectDropDown) && (oHTMLControl.DBFieldName.includes("."))) {

                        //Get the Table Name for the foreign key table
                        sRelatedTableName = oHTMLControl.DBFieldName.split(".")[0];
                        sRelatedFieldName = oHTMLControl.DBFieldName.split(".")[1];

                        oControlInstance = oRow.querySelector(`[name="${oHTMLControl.ControlName}"]`);

                        if (oControlInstance) {

                            oSelectedOptions = oControlInstance.querySelectorAll(".multi-select-option.multi-select-selected[data-value]");

                            if (oExtensibilityArgument.CommandType === cstCommandType.Update) {
                                oArgumentData.append("ActionID", cstGetRecordsetAction.Test_Applications);
                                oArgumentData.append("SQL:TableName", sRelatedTableName);
                                oArgumentData.append("SQL:FieldName", sRelatedFieldName);
                                oArgumentData.append("SQL:KeyField", oExtensibilityArgument.KeyFieldName);
                                oArgumentData.append("KeyFieldValue", oExtensibilityArgument.KeyFieldValue);

                                arrExistingApplicationID = await mGetRecordset(cstURL.GetRecordset, oArgumentData);
                            }

                            for (const [iOptionIndex, oOption] of oSelectedOptions.entries()) {

                                iDropDownValue = oOption.getAttribute("data-value");

                                if (mIsNumeric(iDropDownValue) === true) {
                                   iDropDownValue = Number(iDropDownValue);
                                }

                                if (iOptionIndex < oSelectedOptions.length - 1) {
                                    sRelatedRemovalClause += iDropDownValue + ", ";
                                } else {
                                    sRelatedRemovalClause += iDropDownValue;
                                }

                                oArrayFind = mFindFromArray(arrExistingApplicationID, sRelatedFieldName, iDropDownValue);

                                if (oArrayFind === null || typeof oArrayFind === "undefined") {
                                    //arrFieldName.splice(0, arrFieldName.length);
                                    //arrFieldValue.splice(0, arrFieldValue.length);

                                    arrFieldName = [];
                                    arrFieldValue = [];

                                    arrFieldName.push(oExtensibilityArgument.KeyFieldName);
                                    arrFieldValue.push(oExtensibilityArgument.KeyFieldValue);

                                    arrFieldName.push(sRelatedFieldName);
                                    arrFieldValue.push(iDropDownValue);

                                    //Insert
                                    oExecuteSQLResult = await mCallExecuteSQL(
                                        cstURL.ExecuteSQL,
                                        cstCommandType.Insert,
                                        sRelatedTableName,
                                        arrFieldName,
                                        arrFieldValue
                                    );

                                    if (oExecuteSQLResult.error === true) {
                                        throw new Error(oExecuteSQLResult.message);
                                    } else {
                                        oExecuteSQLResult = null;
                                    }
                                }
                            }

                            //Remove Any Values from Table That are Not Selected in Multi-Select (Cover De-select Scenario)
                            //------------------------------------------------------------------------------------------------
                                //Empty The Arrays
                                arrFieldName.splice(0, arrFieldName.length);
                                arrFieldValue.splice(0, arrFieldValue.length);      
                                                    
                                arrFieldName.push(sRelatedFieldName);
                                arrFieldValue.push(sRelatedRemovalClause);

                                //Delete Rows From The Related Table Where Key Is Not in Multi-Select
                                oExecuteSQLResult = await mCallExecuteSQL(
                                    cstURL.ExecuteSQL,
                                    cstCommandType.DeleteWhereNotIn,
                                    sRelatedTableName,
                                    arrFieldName,
                                    arrFieldValue
                                );                            

                        } else {
                            throw new Error("mSaveRelatedTable: Unable to retrieve control instance.");
                        }
                    }
                }

            } catch (oError) {
                mSetStatus("mSaveRelatedTable", oError);
                throw oError; 
            }
        }

        async function mUpdateSequenceID(oRow) {

            let iRowID = 0;
            let iSequenceID = 0;
            let arrFieldNames = [];
            let arrFieldValues = [];
            let oControl = null;
            let oExecuteSQLResult = null;

            try {

                oControl = oRow.querySelector('[name="txtGridRowID"]');
                if (!oControl || oControl.tagName !== 'INPUT' || !mIsNumeric(oControl.value)) {
                    throw new Error("Row not found, invalid or not numeric.");
                } else {
                    iRowID = Number(oControl.value);
                }

                oControl = oRow.querySelector('[name="txtGridSequenceID"]');
                if (!oControl || oControl.tagName !== 'INPUT' || !mIsNumeric(oControl.value)) {
                    throw new Error("Sequence ID not found, invalid or not numeric.");
                } else {
                    iSequenceID = Number(oControl.value);
                }

                oRow.setAttribute("data-dirty", "true");
                arrFieldNames.push("Sequence_ID");
                arrFieldValues.push(iSequenceID);
                arrFieldNames.push("Row_ID");
                arrFieldValues.push(iRowID);

                oExecuteSQLResult = await mCallExecuteSQL(cstURL.ExecuteSQL, cstCommandType.Update, gsTableName, arrFieldNames, arrFieldValues);

                if (oExecuteSQLResult.error === true) {
                    throw new Error(oExecuteSQLResult.message);
                } else {
                    oExecuteSQLResult = null;
                }

                /*if (oResult) {
                    mOutputTable(arrHTMLControlTable);
                    mSetStatus("Rows successfully resequenced in database.");
                }*/

                oRow.setAttribute("data-dirty", "false");
                
            } catch (oError) {
                mSetStatus("mUpdateSequenceID: Failed to save resequenced row in database.", oError);
            }
        }

        async function mHandleTestSelect(iPlatformID, iYearID, iTestID) {
            try {

                let iTestTypeID = -1;
                let bSuccess = false;
                let arrTestInHxHPlan = [];
                let arrJSONRst = [];
                let oArgumentData = null;

                oArgumentData = new FormData();
                oArgumentData.append("SelectedTestPlan", iTestID);

                await mSetSessionValues(cstURL.SetSession, oArgumentData);
                oArgumentData = null;

                await mSetGlobalTestSettings(iPlatformID, iYearID, iTestID);

                arrTestInHxHPlan = await mGetTestFromHxHPlan(cstURL.GetRecordset, cstGetRecordsetAction["Check_Test_From_HxH_Plan"], iTestID);

                if (Array.isArray(arrTestInHxHPlan) && arrTestInHxHPlan.length === 0) {
                    const bResponse = confirm("This DR Test does not have a loaded template for the Hour By Hour Plan. Do you want to load one? Test ID: " + iTestID);

                    if (bResponse === true) {
                        iTestTypeID = await mGetTestTypeID(iPlatformID, iYearID, iTestID);

                        if (iTestTypeID > 0) {
                            bSuccess = await mInsertTestTemplate(cstURL.LoadPlanTemplate, iTestID, iPlatformID, iYearID, iTestTypeID);
                        }
                        
                    } else {
                        bSuccess = bResponse;
                    }
                }

                if ((Array.isArray(arrTestInHxHPlan) && arrTestInHxHPlan.length > 0) || (bSuccess === true)) {

                    oArgumentData = new FormData();
                    oArgumentData.append("ActionID", cstGetRecordsetAction["Application ID"]);
                    oArgumentData.append("TestID", iTestID);

                    arrJSONRst = await mGetRecordset(cstURL.GetRecordset, oArgumentData);

                    if (Array.isArray(arrJSONRst) && arrJSONRst.length > 0) {

                        gsApplicationTestAppIDList = arrJSONRst[0].Application_ID;
                        await mOutputHxHPlan(iPlatformID, iTestID);
                        await mBindTableEventListeners();


                    } else {
                        alert("No applications found for this Hour by Hour Plan test.  Please contact the administrator.");
                    }
                
                }

                await mFinalizePage();        
                
            } catch (oError) {
                mSetStatus("mHandleTestSelect", oError);
            }        
        }

        async function mHandleTemplateSelect(iPlatformID, iYearID, iTestTypeID) {
            try {       

                let arrFieldName = [];
                let arrFieldValue = [];
                let arrJSONRst = [];                
                let oArgumentData = null;
                let oExecuteSQLResult = null;

                oArgumentData = new FormData();
                oArgumentData.append("ActionID", cstGetRecordsetAction["Plan Template"]);
                oArgumentData.append("PlatformID", iPlatformID);
                oArgumentData.append("YearID", iYearID);
                oArgumentData.append("TestType", iTestTypeID);

                arrJSONRst = await mGetRecordset(cstURL.GetRecordset, oArgumentData);

                    if (Array.isArray(arrJSONRst) && arrJSONRst.length > 0) {

                        //Create a backup of the current plan template
                        arrFieldValue.push(giUserID);
                        arrFieldValue.push(iPlatformID);
                        arrFieldValue.push(iYearID);
                        arrFieldValue.push(iTestTypeID);

                        oExecuteSQLResult = await mCallExecuteSQL(cstURL.InsertPlanTemplateBackup, cstCommandType.Insert, cstTableName["Plan Template Backup"], arrFieldName, arrFieldValue);

                        if (oExecuteSQLResult.error === true) {
                            throw new Error(oExecuteSQLResult.message);
                        } else {
                            oExecuteSQLResult = null;
                        }                           
                        arrFieldName = [];
                        arrFieldValue = [];

                        await mOutputTable(arrHTMLControlTable, iTestTypeID);
                        await mBindTableEventListeners();

                    } else {
                        alert("No templates found for this test type.  Please contact the administrator.");
                    }

                oArgumentData = null;

            } catch (oError) {
                mSetStatus("mHandleTemplateSelect", oError);
            }        
        } 


    function mTogglePlanTimingLockOnExit(iPageID, bLockPlan) {
        try { 

            if (iPageID === dctPage["Hour By Hour Plan"]) {

                let oArgumentData = {};

                oArgumentData.Test_ID = giTestID;
                oArgumentData.Locked_By_ID = giUserID;
                oArgumentData.Lock_Plan = bLockPlan;

                navigator.sendBeacon(cstURL.TogglePlanTimingLock, JSON.stringify(oArgumentData));
                
            }
        } catch (oError) {
            mSetStatus("mTogglePlanTimingLockOnExit", oError);
        } 
    }

    async function mOutputArtifact(sArtifactContent, oDiv) {

            try {

               let sOutputString = "";
               let iEndOfLinePosition = -1;
               let iSearchStart = 0;
               let iImageStartIndex = null;
               let iImageCount = 0;
               let oArtifactLabelSpan = null;
               const cstEndOfLineDelimiter = "|||||";
               const cstBase64Header = "iVBORw0";


               do {

                  iEndOfLinePosition = sArtifactContent.indexOf(cstEndOfLineDelimiter, iSearchStart);

                  if (iEndOfLinePosition !== -1) {

                    if (iSearchStart === 0) {
                        oDiv.replaceChildren();
                        oDiv.appendChild(document.createElement("br"));
                        oDiv.appendChild(document.createElement("br"));
                    }

                     sOutputString = sArtifactContent.substring(iSearchStart, iEndOfLinePosition);
                     

                     //Artficat Image
                     if (sOutputString.substring(0, cstBase64Header.length) === cstBase64Header) {

                        const oArtifactImage = document.createElement("img");
                        oArtifactImage.src = "data:image/png;base64," + sOutputString;

                        oDiv.appendChild(oArtifactImage);
                        oDiv.appendChild(document.createElement("br"));
                        oDiv.appendChild(document.createElement("br"));
                        iImageCount = iImageCount + 1;

                     //Artifact Text Label
                     } else if (sOutputString.length > 0) {

                        oArtifactLabelSpan = mGetArtifactLabelSpan(gliTestID, gliRowID, gliImageCount, sOutputString);
                        oDiv.appendChild(oArtifactLabelSpan);   
                        
                     }

                     iSearchStart = iEndOfLinePosition + (cstEndOfLineDelimiter.length);
                  }

               } while (iEndOfLinePosition !== -1);

               return(iImageCount);

            } catch (oError) {
               mSetStatus("mOutputArtifact", oError);
            }             
         }

         function mGetArtifactLabelSpan(iTestID, iRowID, iImageCount, sLabelString = "") {
            try {
               
               const oArtifactLabelSpan = document.createElement("span");
               oArtifactLabelSpan.className = "clsArtifactHeaderLabel";

               const oBold = document.createElement("b");

               if (String(sLabelString).trim().length === 0) {
                     oBold.textContent =
                        "Test ID: " + iTestID +
                        "\t\tRow ID: " + iRowID +
                        "\t\tImage ID: " + iImageCount +
                        "\t\tDescription:  ";
               } else {
                     oBold.textContent = sLabelString;
               }

               oArtifactLabelSpan.appendChild(oBold);
               return oArtifactLabelSpan;

            } catch (oError) {
               mSetStatus("mGetArtifactLabelSpan", oError);
            }
         }         