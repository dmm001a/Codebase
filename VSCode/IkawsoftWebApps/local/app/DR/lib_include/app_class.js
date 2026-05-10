   
    class cDynamicFunction {

       async mToggleAddTable(oRow) {
            try {      

                let oHTMLDiv = mGetHTMLElement(cstGetHTMLElementType.ID, cstElementID.DivAddTable);

                if (oHTMLDiv) {
                    if (oHTMLDiv.style.display === "none") {
                        oHTMLDiv.style.display = "";
                    } else {
                        oHTMLDiv.style.display = "none";
                    }
                }

            } catch (oError) {
                mSetStatus("mToggleAddTable", oError);
            }                      
        }

        async mToggleTimingLock(oRow) {
            try {      

                let iLockUserID = null;
                let arrFieldName = [];
                let arrFieldValue = [];
                let oExecuteSQLResult = null;

                //Get and Check Plan Lock Setting
                iLockUserID = await mGetPlanLockUser(giPlatformID, giYearID, giTestID);

                if (mIsNumeric(iLockUserID)) {

                    //if unlocked then lock
                    if (iLockUserID === -1) {

                        arrFieldName = [];
                        arrFieldValue = [];
                        //Backup The Plan
                        arrFieldValue.push(giUserID);
                        arrFieldValue.push(giPlatformID);
                        arrFieldValue.push(giYearID);
                        arrFieldValue.push(giTestTypeID);
                        oExecuteSQLResult = await mCallExecuteSQL(cstURL.InsertPlanBackup, cstCommandType.Insert, cstTableName["Plan Backup"], arrFieldName, arrFieldValue);

                        if (oExecuteSQLResult.error === true) {
                            throw new Error(oExecuteSQLResult.message);
                        } else {
                            oExecuteSQLResult = null;
                        }
                        arrFieldName = [];
                        arrFieldValue = [];

                        //Lock The Rows With the User ID
                        arrFieldName.push("Locked_By_ID");
                        arrFieldValue.push(giUserID);
                        arrFieldName.push("Test_ID");
                        arrFieldValue.push(giTestID);                        
                        oExecuteSQLResult = await mCallExecuteSQL(cstURL.ExecuteSQL, cstCommandType.Update, cstTableName["Hour By Hour Plan"], arrFieldName, arrFieldValue);

                        if (oExecuteSQLResult.error === true) {
                            throw new Error(oExecuteSQLResult.message);
                        } else {
                            oExecuteSQLResult = null;
                        }                        
                        arrFieldName = [];
                        arrFieldValue = [];

                        //Set the Controls to Editable
                        mToggleTimingControls(false);

                        alert("HxH Plan is now unlocked.");

                    //if locked by current user then unlock
                    } else if (iLockUserID === giUserID) {

                        arrFieldName = [];
                        arrFieldValue = [];
                        //Lock The Rows With the User ID
                        arrFieldName.push("Locked_By_ID");
                        arrFieldValue.push(-1);
                        arrFieldName.push("Test_ID");
                        arrFieldValue.push(giTestID);                        
                        oExecuteSQLResult = await mCallExecuteSQL(cstURL.ExecuteSQL, cstCommandType.Update, cstTableName["Hour By Hour Plan"], arrFieldName, arrFieldValue);

                        if (oExecuteSQLResult.error === true) {
                            throw new Error(oExecuteSQLResult.message);
                        } else {
                            oExecuteSQLResult = null;
                        }                         
                        arrFieldName = [];
                        arrFieldValue = [];

                        mToggleTimingControls(true);

                        alert("HxH Plan is now locked.");

                    //if locked by another user exit and alert user of reason
                    } else if (iLockUserID !== giUserID) {                        
                        alert("This plan's timing (duration and prior task) settings are currently being editable by another user please try again at the another time.");
                    }

                }

            } catch (oError) {
                mSetStatus("mToggleTimingLock", oError);
            }                      
        }


        async mSaveChange(oRow) {
            try {      

                    let sHTMLTableName = "";
                    let iNewSequenceID = -1;
                    let iSQLCommandType = -1;
                    let iTableType = -1;
                    let iRowID = -1;
                    let bRowChanged = false;                
                    let bValidValues = false;
                    let oExecuteSQLResult = null;
                    let arrFieldName = [];
                    let arrFieldValue = [];
                    let oHTMLTable = null;

                    let oExtensibilityArgument = new cExtensibilityArgument;

                    //Get the Table Type
                    oHTMLTable = oRow.closest("table");
                    sHTMLTableName = oHTMLTable.id;
                    iTableType = cstTableType[sHTMLTableName];
                    
                    //Validate and Init
                    if (iTableType === cstTableType.AddTable) {
                        oExtensibilityArgument.CommandType = cstCommandType.Insert;

                        if (mValidateTable(cstTableType.AddTable)) {
                            bValidValues = true;                            
                            arrFieldName.push("Platform_ID");
                            arrFieldValue.push(giPlatformID);
                            arrFieldName.push("Year_ID");
                            arrFieldValue.push(giYearID);   
                        } else {
                            bValidValues = false;                   
                        }                        

                    } else if (iTableType === cstTableType.StandardTable) {
                        oExtensibilityArgument.CommandType = cstCommandType.Update;
                        bRowChanged = Boolean(oRow.hasAttribute("data-dirty"));
                        bValidValues = true;                        
                    }

                    //Populate Array
                    mGetExecuteSQLArray(oExtensibilityArgument.CommandType, iTableType, oRow, arrFieldName, arrFieldValue);                    
                    oExtensibilityArgument.FieldNames = arrFieldName;
                    oExtensibilityArgument.FieldValues = arrFieldValue;

                    //Insert Statement Post Value Validation
                    if (oExtensibilityArgument.CommandType === cstCommandType.Insert && bValidValues === true) {

                            //Get Next Sequence ID And Add to the Insert Array
                            iNewSequenceID = await mGetNewID(cstURL.GetRecordset, giYearID, gsTableName, cstGetRecordsetAction.New_Sequence_ID, arrFieldName, arrFieldValue, -1);
                            if (Number(iNewSequenceID) === -1) {
                                throw new Error("Sequence ID was not successfully retrieved.");
                            }                 

                            //Array Value Push & GetNewID Push
                            await mPageExtensibility(giPageID, oExtensibilityArgument);

                            //Execute the Insert statement using the name and value arrays.
                            oExecuteSQLResult = await mCallExecuteSQL(
                                cstURL.ExecuteSQL,
                                oExtensibilityArgument.CommandType,
                                gsTableName,
                                oExtensibilityArgument.FieldNames,
                                oExtensibilityArgument.FieldValues
                            );

                             //if oExecuteResponse is valid and there is a yearid and page id then save testapplications if required,
                            // render tables and reset add controls
                            if (oExecuteSQLResult && oExecuteSQLResult.error === false) {
                                await mSaveRelatedTable(oRow, oExtensibilityArgument);

                                //Ouput Standard Table
                                await mOutputTable(arrHTMLControlTable, giTestID);
				
                                //Update Planned Times for HxH Plan
                                oExtensibilityArgument.CommandType = cstExtensibilityType.UpdatePlannedTimes;
                                await mPageExtensibility(giPageID, oExtensibilityArgument);
				
                                //Notify Users of Insert via Broadcast
                                oBroadcastClient.TargetObject = oRow;
                                oBroadcastClient.RowID = oExecuteSQLResult.ReturnValue;
                                oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.TableRowInsert.ID);

                            } else {
                                throw new Error(oExecuteSQLResult.message);
                            }

                    //Update Statement
                    } else if (oExtensibilityArgument.CommandType === cstCommandType.Update && bRowChanged === true) {

                        oExecuteSQLResult = await mCallExecuteSQL(cstURL.ExecuteSQL, oExtensibilityArgument.CommandType, gsTableName, oExtensibilityArgument.FieldNames, oExtensibilityArgument.FieldValues);

                        if (oExecuteSQLResult && oExecuteSQLResult.error === false) {
                            oExtensibilityArgument.oRowReference = oRow;

                            oExtensibilityArgument.CommandType = cstExtensibilityType.GetRelatedTableKey;
                            await mPageExtensibility(giPageID, oExtensibilityArgument);
                            await mSaveRelatedTable(oRow, oExtensibilityArgument);
                            
                            oRow.setAttribute("data-dirty", "false");
                            mSetStatus("Record successfully updated.");

                        } else {
                            throw new Error(oExecuteSQLResult.message);
                        }
                    }   

            } catch (oError) {
                mSetStatus("mSaveChange", oError);
            }                      
        }        



        mViewMetric() {
            try {      

                let sURL = "";
                let oBrowserWindow = null;

                sURL = cstURL.FormFolder + "/metric.php?TestID=" + giTestID;

                oBrowserWindow = mOpenBrowserWindow("Metric", 1400, 800, true, true, sURL);

            } catch (oError) {
                mSetStatus("mViewMetric", oError);
            }    
        }


        mViewArtifact(oRow) {
            try {     

                let sURL = "";
                let sQueryString = "";
                let iRowID = -1;
                let oRowID = null;
                let oBrowserWindow = null;

                oRowID = mGetHTMLElement(cstGetHTMLElementType.Name, "txtGridRowID", oRow);

                if (oRowID) {

                    iRowID = Number(oRowID.value);

                    sQueryString = "?PlatformID=" + encodeURIComponent(giPlatformID) + "&";
                    sQueryString = sQueryString + "YearID=" + encodeURIComponent(giYearID) + "&";
                    sQueryString = sQueryString + "TestID=" + encodeURIComponent(giTestID) + "&";
                    sQueryString = sQueryString + "RowID=" + encodeURIComponent(iRowID);

                    sURL = cstURL.FormFolder + "/artifact.php" + sQueryString;

                    oBrowserWindow = mOpenBrowserWindow("Artifacts", 1400, 800, true, true, sURL);                

                } else {

                    throw new Error("oRowID could not be retrieved and is required.");

                }

            } catch (oError) {
                mSetStatus("mViewArtifact", oError);
            }    
        }                 

        async mDownloadArtifact(oRow) {
            try {

                let sTestDescription = "";
                let sArtifactContent = "";                
                let sOutputString = "";
                let iEndOfLinePosition = -1;
                let iSearchStart = 0;
                let sWordExportFileName = "";
                let iImageStartIndex = null;
                let iImageCount = 0;
                let arrRecordset = [];               
                let oArtifactLabelSpan = null;
                let oArgumentData = new FormData();              

                const { Document, Packer, Paragraph, TextRun, ImageRun,  PageOrientation, PageBreak, NumberFormat, PageNumber, AlignmentType, Footer} = docx;               
                const cstEndOfLineDelimiter = "|||||";
                const cstBase64Header = "iVBORw0";
                const dtNow = new Date();
                const sMonth = String(dtNow.getMonth() + 1).padStart(2, "0");
                const sDay = String(dtNow.getDate()).padStart(2, "0");
                const sYear = dtNow.getFullYear();
                const sDate = `${sMonth}.${sDay}.${sYear}`;               

                sWordExportFileName = "DR_Tracker_Export.Artifact_Summary.TestID." + giTestID + "." + sDate + ".docx";  

                oArgumentData.append("ActionID", cstGetRecordsetAction["All_Artifact"]);
                oArgumentData.append("PlatformID", giPlatformID);
                oArgumentData.append("YearID", giYearID);
                oArgumentData.append("TestID", giTestID);

                arrRecordset = await mGetRecordset(cstURL.GetRecordset, oArgumentData);

                if (Array.isArray(arrRecordset) && arrRecordset.length > 0) {

                        const ocboTestDropDown = mGetHTMLElement(cstGetHTMLElementType.ID, "cboSubSelect");

                       sTestDescription = ocboTestDropDown.options[ocboTestDropDown.selectedIndex].text;

                       if (!sTestDescription || sTestDescription.length === 0) {
                            sTestDescription = "Artifact Document for Test ID: " + String(giTestID);
                       } else {
                            sTestDescription = "Artifact Document for Test: " + sTestDescription;
                       }

                        const oDocumentChildren = [];

                        oDocumentChildren.push(new Paragraph({
                            children: [
                                new TextRun({
                                    text: sTestDescription,
                                    bold: true                          
                                }),
                                new TextRun({
                                    break: 2, 
                                }),                                
                            ],
                        }));

                        for (let iRstRow = 0; iRstRow < arrRecordset.length; iRstRow++) {
                            const sArtifactContent = String(arrRecordset[iRstRow].Artifact_Content);
                            iSearchStart = 0;
                            iEndOfLinePosition = 0;

                            while ((iEndOfLinePosition = sArtifactContent.indexOf(cstEndOfLineDelimiter, iSearchStart)) !== -1) {

                                sOutputString = sArtifactContent.substring(iSearchStart, iEndOfLinePosition);

                                // Add Artifact Image
                                if (sOutputString.substring(0, cstBase64Header.length) === cstBase64Header) {
                                    const oWordImage = mBase64ToUint8Array(sOutputString);

                                    oDocumentChildren.push(new Paragraph({
                                        children: [
                                            new ImageRun({
                                                data: oWordImage,
                                                transformation: { width: 800, height: 450 }
                                            })
                                        ],
                                    }));

                                    //Skip Page Break if At End of Document
                                    if (iEndOfLinePosition + cstEndOfLineDelimiter.length < sArtifactContent.length) {
                                        oDocumentChildren.push(new Paragraph({
                                                children: [ new PageBreak() ]
                                            })
                                        );
                                    }

                                    iImageCount = iImageCount + 1;

                                // Add Artifact Text Label
                                } else if (sOutputString.length > 0) {
                                    oArtifactLabelSpan = mGetArtifactLabelSpan(giTestID, giRowID, iImageCount, sOutputString);

                                    oDocumentChildren.push(new Paragraph({
                                        children: [
                                            new TextRun({
                                                text: String(oArtifactLabelSpan.textContent),
                                                bold: false
                                            }),
                                        ],
                                    }));
                                }
                             
                                iSearchStart = iEndOfLinePosition + cstEndOfLineDelimiter.length;

                            }
                        }

                        //Instantiate the Document Object
                        const oArtifactDocument = new Document({
                            sections: [
                                {
                                    properties: {
                                        page: {
                                            size: {
                                                orientation: PageOrientation.LANDSCAPE,
                                                width: 12246,
                                                height: 15817,
                                            },
                                            pageNumbers: {
                                                start: 1,
                                                formatType: NumberFormat.DECIMAL,
                                            },
                                        },
                                    },
                                    children: oDocumentChildren,
                                    footers: {
                                        default: new Footer({
                                            children: [
                                                new Paragraph({
                                                    alignment: AlignmentType.CENTER,
                                                    children: [
                                                        new TextRun("Wolters Kluwer "),
                                                        new TextRun({
                                                            children: ["Page Number: ", PageNumber.CURRENT],
                                                        }),
                                                        new TextRun({
                                                            children: [" to ", PageNumber.TOTAL_PAGES],
                                                        }),
                                                    ],
                                                }),
                                            ],
                                        }),
                                    }, 
                                },
                            ],
                        });


                        // Download The Word Document
                        Packer.toBlob(oArtifactDocument).then(blob => {
                            const url = URL.createObjectURL(blob);
                            const a = document.createElement("a");
                            a.href = url;
                            a.download = sWordExportFileName;
                            a.click();
                            URL.revokeObjectURL(url);
                        });
                    }

            } catch (oError) {
                mSetStatus("mDownloadArtifact", oError);
            }
        }


        async mInsertAllDocumentation(oRow) {
            try {

                let arrFieldNames = [];
                let arrFieldValues = [];
                let oExecuteSQLResult = null;

                arrFieldValues.push(giPlatformID);
                arrFieldValues.push(giYearID);
                arrFieldValues.push(giPlatformID);

                oExecuteSQLResult = await mCallExecuteSQL(cstURL.BulkDocumentInsert, cstCommandType.Insert, gsTableName, arrFieldNames, arrFieldValues);

                if (oExecuteSQLResult.error === true) {
                    throw new Error(oExecuteSQLResult.message);
                } else {
                    oExecuteSQLResult = null;
                }                        

                cstBroadcastMessageType.MessageOnly.Message = "Another user has added all documents to this page.  Please refresh.";
                oBroadcastClient.TargetObject = null;
                oBroadcastClient.RowID = "";
                oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.MessageOnly.ID);
                cstBroadcastMessageType.MessageOnly.Message = "";

                await mOutputTable(arrHTMLControlTable);
                
            } catch (oError) {
                mSetStatus("mInsertAllDocumentation", oError);
            }
        }            

        async mViewRowInfo(oHTMLTableRow) {
            try {

                let sApplication = "";
                let sPlanPhase = "";
                let sTaskDetail = "";
                let sJira = "";
                let sTeam = "";
                let sAssignee = "";
                let sOutputString = "";
                let oAssigneeMultiSelect = null;
                let arrAssignees = [];

                if (oHTMLTableRow) {

                    sApplication = "Application: " + oHTMLTableRow.querySelector('[name="cboGridApplication"]').selectedOptions[0].text;
                    sPlanPhase = "Phase of Plan: " + oHTMLTableRow.querySelector('[name="cboGridPlanPhase"]').selectedOptions[0].text;
                    sTaskDetail = "Task Detail: " + oHTMLTableRow.querySelector('[name="txtGridTaskDetails"]').value;
                    sJira = "Related Jira: " + oHTMLTableRow.querySelector('[name="txtGridJiraRainierReference"]').value;
                    sTeam = "Assigned Team: " + oHTMLTableRow.querySelector('[name="cboGridResourceTeam"]').selectedOptions[0].text;

                    oAssigneeMultiSelect = oHTMLTableRow.querySelector('[name="cboGridMultiPlannedResource"]');
                    arrAssignees = mGetMultiSelectText(oAssigneeMultiSelect);
                    sAssignee = "Assignees: " + arrAssignees.join(", ");

                    sOutputString = mBuildString(sOutputString, sApplication, cstAddToString.CarriageReturn);
                    sOutputString = mBuildString(sOutputString, "", cstAddToString.CarriageReturn);

                    sOutputString = mBuildString(sOutputString, sPlanPhase, cstAddToString.CarriageReturn);
                    sOutputString = mBuildString(sOutputString, "", cstAddToString.CarriageReturn);

                    sOutputString = mBuildString(sOutputString, sTaskDetail, cstAddToString.CarriageReturn);
                    sOutputString = mBuildString(sOutputString, "", cstAddToString.CarriageReturn);

                    sOutputString = mBuildString(sOutputString, sJira, cstAddToString.CarriageReturn);
                    sOutputString = mBuildString(sOutputString, "", cstAddToString.CarriageReturn);

                    sOutputString = mBuildString(sOutputString, sTeam, cstAddToString.CarriageReturn);
                    sOutputString = mBuildString(sOutputString, "", cstAddToString.CarriageReturn);

                    sOutputString = mBuildString(sOutputString, sAssignee, cstAddToString.CarriageReturn);
                    sOutputString = mBuildString(sOutputString, "", cstAddToString.CarriageReturn);

                    alert(sOutputString);
                    
                }
                
            } catch (oError) {
                mSetStatus("mViewRowInfo", oError);
            }
        }       
	
        async mTaskStartStop(oHTMLTableRow) {

            try {

                let iSelectedStatusValue = -1;
                let bTaskStatusChanged = false;
                let oGridButton = null;
                let oGridStatus = null;

                oGridButton = oHTMLTableRow.querySelector('button[name="btnTaskStartStop"]');
                oGridStatus = oHTMLTableRow.querySelector('select[name="cboGridTaskStatus"]');

                iSelectedStatusValue = oGridStatus.value;

                if (mIsNumeric(iSelectedStatusValue)) {

                    iSelectedStatusValue = Number(iSelectedStatusValue);

                    if (iSelectedStatusValue === cstTaskStatus.NotStarted) {
                        oGridStatus.value = cstTaskStatus.InProgress;
                        oGridButton.style.backgroundImage = "url(" + cstImagePath.RedLight + ")";
                        bTaskStatusChanged = true;
                    } else if (iSelectedStatusValue === cstTaskStatus.InProgress) {
                        oGridStatus.value = cstTaskStatus.Complete;
                        oGridButton.style.backgroundImage = "url(" + cstImagePath.BlackLight + ")";
                        bTaskStatusChanged = true;
                    }

                    if (bTaskStatusChanged === true) {
                        mSetActualStartEndTime(oGridStatus); //this also broadcasts the change
                        oGridStatus.dispatchEvent(new Event("change", { bubbles: true }));
                    }

                }
                
            } catch (oError) {
                mSetStatus("mTaskStartStop", oError);
            }                
        }

        async mDeleteRecord(oRow) {

            try {

                let sMessage = "";
                let iRowID = -1;
                let iTestID = -1;
                let bResponse = false;              
                let arrFieldNames = [];
                let arrFieldValues = [];
                let oRowIDControl = null;
                let oTestIDControl = null;
                let oExtensibilityArgument = new cExtensibilityArgument;                

                oRowIDControl = oRow.querySelector("input[name='txtGridRowID']");
                iRowID = oRowIDControl?.value;

                //Get the test ID                
                oExtensibilityArgument.oRowReference = oRow;
                oExtensibilityArgument.CommandType = cstExtensibilityType.GetTestID;
                await mPageExtensibility(giPageID, oExtensibilityArgument);

                bResponse = confirm("Are you sure you want to delete this test record?" +  oExtensibilityArgument.ReturnValue + " Row ID: " + iRowID);

                if (bResponse === true) {
                    let oExecuteSQLResult = null;

                    if (oExtensibilityArgument.KeyFieldValue > 0 && !isNaN(oExtensibilityArgument.KeyFieldValue)) {
                        oExtensibilityArgument.CommandType = cstExtensibilityType.DeleteTest;
                        oExtensibilityArgument.FieldNames.push(oExtensibilityArgument.KeyFieldName);
                        oExtensibilityArgument.FieldValues.push(oExtensibilityArgument.KeyFieldValue);

                        //Delete Related Tables Of Test: tb_Plan, tb_Test_Application, tb_Issue_Log, 
                        await mPageExtensibility(giPageID, oExtensibilityArgument);
                    }

                    arrFieldNames = [];
                    arrFieldValues = [];

                    arrFieldNames = ["Row_ID"];
                    arrFieldValues = [iRowID];

                    oExecuteSQLResult = await mCallExecuteSQL(
                        cstURL.ExecuteSQL,
                        cstCommandType.Delete,
                        gsTableName,
                        arrFieldNames,
                        arrFieldValues
                    );

                    if (oExecuteSQLResult.error === true) {
                        throw new Error(oExecuteSQLResult.message);
                    } else {
                        oExecuteSQLResult = null;
                    }                            

                    oBroadcastClient.TargetObject = oRow;
                    oBroadcastClient.RowID = iRowID;
                    oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.TableRowDelete.ID);
                    await mOutputTable(arrHTMLControlTable);

                }
                
            } catch (oError) {
                mSetStatus("mDeleteRecord", oError);
            }
        }


	async mExportToExcel() {
	    try {        

            let sExcelExportFileName = "";
            let iBodyRowIndex = -1;
            let iHeaderRowIndex = -1;		
            let oBodyRow = null;

            //Get The For the FileName In The Right Format
            const dtNow = new Date();
            const sMonth = String(dtNow.getMonth() + 1).padStart(2, "0");
            const sDay = String(dtNow.getDate()).padStart(2, "0");
            const sYear = dtNow.getFullYear();
            const sDate = `${sMonth}.${sDay}.${sYear}`;

            const oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);

            const oClonedTable = oHTMLTable.cloneNode(true);
            
            const oColumnHeaderRow = oHTMLTable.tHead.rows;
            const oBodyRows = oHTMLTable.tBodies[0].rows;

            sExcelExportFileName = "DR_Tracker_Export." + gsPageName + "." + sDate + ".xlsx";        

            if (oColumnHeaderRow) {
                //Loop Through Column Header Rows
                for (iHeaderRowIndex = 0; iHeaderRowIndex < oColumnHeaderRow.length; iHeaderRowIndex++) {
                const oOrigHeaderRow = oColumnHeaderRow[iHeaderRowIndex];
                const oCloneHeaderRow = oClonedTable.tHead.rows[iHeaderRowIndex];

                for (let iCellIndex = 0; iCellIndex < oOrigHeaderRow.cells.length; iCellIndex++) {
                    const oOrigCell = oOrigHeaderRow.cells[iCellIndex];
                    const oCloneCell = oCloneHeaderRow.cells[iCellIndex];

                    this.mSetExcelCellValue(oOrigCell, oCloneCell);
                }
                }
            }

            if (oBodyRows && oBodyRows.length > 1) {
                for (iBodyRowIndex = 0; iBodyRowIndex < oBodyRows.length; iBodyRowIndex++) {
                oBodyRow = oBodyRows[iBodyRowIndex];

                const oCloneBodyRow = oClonedTable.tBodies[0].rows[iBodyRowIndex];

                for (let iCellIndex = 0; iCellIndex < oBodyRow.cells.length; iCellIndex++) {
                    const oOrigCell = oBodyRow.cells[iCellIndex];
                    const oCloneCell = oCloneBodyRow.cells[iCellIndex];
                    this.mSetExcelCellValue(oOrigCell, oCloneCell);
                    
                }
                }
            }        


            // Export the modified clone
            const oWorkbook = XLSX.utils.table_to_book(oClonedTable, { sheet: gsPageName });
            const oWorksheet = oWorkbook.Sheets[gsPageName];

            const oRange = XLSX.utils.decode_range(oWorksheet["!ref"]);

            for (let iWorksheetRowIndex = oRange.s.r; iWorksheetRowIndex <= oRange.e.r; iWorksheetRowIndex++) {
                this.mSetExcelRowBackgroundColor(oWorksheet, iWorksheetRowIndex, "#F5F5F5");
            }
            
            
            XLSX.writeFile(oWorkbook, sExcelExportFileName);

            alert("Export successful.  Please note that for time values, e.g. Start and End, it is required to change the Number Format to Time for the Excel column.")


	    } catch (oError) {
		    mSetStatus("mExportToExcel", oError);
	    }
	}
	
	mSetExcelRowBackgroundColor(oExcelWorksheet, iRowIndex, sHexColor) {
	    try {
		
		// SheetJS expects hex WITHOUT the leading "#"
		const sRGB = sHexColor.replace("#", "").toUpperCase();
		const oWSRange = XLSX.utils.decode_range(oExcelWorksheet["!ref"]);

		// Loop through all columns in the row
		for (let C = oWSRange.s.c; C <= oWSRange.e.c; C++) {

		    const sCellAddress = XLSX.utils.encode_cell({ r: iRowIndex, c: C });
		    const oCell = oExcelWorksheet[sCellAddress];

		    if (!oCell) continue; // skip empty cells

		    // Apply background fill
		    oCell.s = {
			fill: {
			    patternType: "solid",
			    fgColor: { rgb: sRGB }
			}
		    };
		}
		
	    } catch (oError) {
		mSetStatus("mSetExcelRowBackgroundColor", oError);
	    }	    
	}
	
	mSetExcelCellValue(oOrigCell, oCloneCell) {
	    try {

		let sTextValue = "";

		const oSelect = oOrigCell.querySelector("select");
		const oInput = oOrigCell.querySelector("input:not([type='hidden']):not(.multi-select-search)");
		const oMultiSelectControl = oOrigCell.querySelector("div.multi-select");

		if (oSelect) {
		    sTextValue = oSelect.options[oSelect.selectedIndex].text;

		} else if (oInput) {

		    if (oInput.type === "time") {
			/*alert(oInput.value);
			if (!oInput.value) {
			    // time input is blank → export empty cell
			    sTextValue = "";
			    oCloneCell.v = "";
			    oCloneCell.t = "s"; // string
			    sTextValue = "";
			} else {

			    const [iHour, iMinute] = oInput.value.split(":").map(Number);
			    const iExcelExportTimeInNumber = (iHour * 60 + iMinute) / (24 * 60);

			    oCloneCell.v = iExcelExportTimeInNumber;
			    oCloneCell.t = "n";
			    oCloneCell.z = "hh:mm AM/PM";
			    sTextValue = String(iExcelExportTimeInNumber);
			}*/
			
			sTextValue = oInput.value;
			
		    } else {
			sTextValue = oInput.value;
		    }

		} else if (oMultiSelectControl) {
		    sTextValue = oOrigCell.innerText;

		    if (sTextValue === "Select item(s)") {
			sTextValue = "";
		    }

		} else {
		    sTextValue = oOrigCell.innerHTML;
		}

		oCloneCell.innerHTML = sTextValue;

	    } catch (oError) {
		mSetStatus("mSetExcelCellValue", oError);
	    }
	}
        
        async mSaveAll(oRow = null) {
            try {

                const oChangedRows = document.querySelectorAll("#StandardTable tr[data-dirty='true']");

                if (oChangedRows.length > 0) {

                    for (const oRow of oChangedRows) {
                        await this.mSaveChange(oRow);
                    }


                } else {
                    mSetStatus("No changes to be saved.");
                }

            } catch (oError) {
                mSetStatus("mSaveAll", oError);
            }            
        }

        async mToggleHiddenTableElement(oRow) {
            try {
                
                let sCurrentTableViewState = "";
                let sFutureTableViewState = "";
                let sDisplaySetting = "";
                let sButtonTextSetting = "";
                let iRowIndex = -1;
                let arrHTMLTableRows = [];
                let oHTMLTable = null;
                let oHTMLTableRow = null;
                let oToggleButton = null;
                let oSpanLabel = null;
                let ocboGridStatus = null;

                sButtonTextSetting = "SHOW";
                oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);

                if (oHTMLTable) {
                  
                    sCurrentTableViewState = oHTMLTable._ViewState;

                    if (sCurrentTableViewState === "HIDDEN") {
                        sFutureTableViewState = "DISPLAY";
                        sButtonTextSetting = "Hide";
                    } else if (sCurrentTableViewState === "DISPLAY") {
                        sFutureTableViewState = "HIDDEN";
                        sButtonTextSetting = "Show";
                    }

                    arrHTMLTableRows = oHTMLTable.rows;   
                    
                    for (let iRowIndex = 0; iRowIndex < arrHTMLTableRows.length; iRowIndex++) {
                        
                            oHTMLTableRow = arrHTMLTableRows[iRowIndex];

                            
                            if (sFutureTableViewState === "HIDDEN") { //If the table is showing all rows then loop through and hide the rows with not required status

                                const ocboGridStatus = oHTMLTableRow.querySelector('select[name="cboGridTaskStatus"]');
                                if (Number(ocboGridStatus.value) === cstTaskStatus.NotRequired) {
                                    sDisplaySetting = "none";
                                } else {
                                    sDisplaySetting = "";
                                }
                            } else if (sFutureTableViewState === "DISPLAY") {
                                    sDisplaySetting = "";
                            }

                            oHTMLTableRow.style.display = sDisplaySetting;
                            
                    }

                    oHTMLTable._ViewState = sFutureTableViewState;

                    oHTMLTable = mGetHTMLTable(cstTableType.ToolbarTable);
                    oToggleButton = oHTMLTable.querySelector('tbody button[name="btnViewHidden"]');
                    oSpanLabel = oToggleButton.querySelector("span");

                    if (oSpanLabel) {
                        oSpanLabel.textContent = sButtonTextSetting;
                    }

                }

                mToggleCellControlDisplay();

            } catch (oError) {
                mSetStatus("mToggleHiddenTableElement", oError);
            }
        }

        mFilterTable(sControlName) {

            try {

                let selectedText = "";
                let iControlIncrement = 0;
                let iRowTypeID = -1;
                let oControl = "";
                let vControlValue = "";

                const oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);

                if (!oHTMLTable) {
                    mSetStatus("mFilterTable: oTable is not found.");
                    return;
                }

                const oHeader = oHTMLTable.querySelector("thead");
                if (!oHeader) {
                    mSetStatus("mFilterTable: oHeader is not found.");
                    return;
                }

                const oHeaderFilterControl = oHeader.querySelector(`[name="${sControlName}"]`);
                if (!oHeaderFilterControl) {
                    mSetStatus(`mFilterTable: Filter cell with name "${sControlName}" not found.`);
                    return;
                }
                const vHeaderFilterValue = oHeaderFilterControl.value?.trim() ?? "";


                const oRows = oHTMLTable.querySelector("tbody")?.getElementsByTagName("tr") ?? [];

                for (let i = 0; i < oRows.length; i++) {
                    const oRow = oRows[i];
                    const oRowType = oRow.querySelector(`[name="txtGridRowType"]`);

                    if (oRowType) {
                        iRowTypeID = Number(oRowType.value);
                    }

                    if (iRowTypeID !== cstRowType.Header && iRowTypeID !== cstRowType.ColumnHeader) {
                        const oControl = oRow.querySelector(`[name="${sControlName}"]`);

                        if (!oControl) {
                            mSetStatus(`Row ${i}: Control not found.`);
                            continue;
                        }

                        vControlValue = "";
                        if (oControl.tagName === "SELECT") {
                            vControlValue = oControl.value;
                        } else if (oControl.tagName === "INPUT" && oControl.type === "text") {
                            vControlValue = oControl.value;
                        } else {
                            console.log(`Row ${i}: Unsupported control type.`);
                            continue;
                        }

                        const isMatch =
                            vHeaderFilterValue === "" ||
                            vControlValue === vHeaderFilterValue ||
                            vControlValue.toLowerCase().includes(vHeaderFilterValue.toLowerCase()) ||
                            vHeaderFilterValue === oHeaderFilterControl._originalValue;

                        oRow.style.display = isMatch ? "" : "none";
                    } else {
                        oRow.style.display = "";
                    }
                }

            } catch (oError) {
                mSetStatus("mFilterTable", oError);
            }
        }

        async mTogglePlanSummary(oRow) {
            try {    

                let oHTMLTable = mGetHTMLTable(cstTableType.TestSummaryTable)
                let oToggleButton = null;

                if (oHTMLTable) {
                    oToggleButton = mGetHTMLElement(cstGetHTMLElementType.Name, "btnTogglePlanSummary");

                    if (oHTMLTable.style.display === "none") {
                        oHTMLTable.style.display = "";
                        //oToggleButton.textContent = "Hide Plan";
                    } else {
                        oHTMLTable.style.display = "none";
                        //oToggleButton.textContent = "View Plan";
                    }
                }

            } catch (oError) {
                mSetStatus("mViewPlanSummary", oError);
            }                        
        }

        async mViewDesignDiagram(oRow) {
            try {
                
                let sURL = "";
                let arrRecordset = [];
                let oArgumentData = new FormData();
                let oBrowserWindow = null;

                oBrowserWindow = mOpenBrowserWindow("Diagram", 1400, 800, true, true);

                //get the design link
                oArgumentData.append("ActionID", cstGetRecordsetAction["Design Diagram"]);
                oArgumentData.append("PlatformID", giPlatformID);
                oArgumentData.append("YearID", giYearID);

                arrRecordset = await mGetRecordset(cstURL.GetRecordset, oArgumentData);

                if (arrRecordset && Array.isArray(arrRecordset) && arrRecordset.length > 0) {
                    arrRecordset.forEach(arrRow => {
                        //ctlCRDesignDiagram.innerHTML += arrRow.Diagram_Link + "<br/>";
                        sURL = arrRow.Diagram_Link;
                    });
                }
                
                //Open the window with the Design Link In it
                if (oBrowserWindow) {
                    oBrowserWindow.document.write(`
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <title>DR Design Diagram</title>
                            <style>
                                html, body {
                                    margin: 0;
                                    padding: 0;
                                    height: 100%;
                                }
                                iframe {
                                    width: 100%;
                                    height: 100%;
                                    border: none;
                                }
                            </style>
                        </head>
                        <body>
                            <iframe 
                                allowfullscreen 
                                frameborder="0"
                                src="${sURL}">
                            </iframe>
                        </body>
                        </html>
                    `);
                    oBrowserWindow.document.close();
                }


            } catch (oError) {
                mSetStatus("mViewDesignDiagram", oError);
            }
        }
    }