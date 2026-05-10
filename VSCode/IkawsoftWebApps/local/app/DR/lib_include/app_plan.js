

    
    async function mGetTestFromHxHPlan(sUrl, iActionID, iTestID) {
        try {

            let arrHxHPlanTestRst = [];

            const oArgumentData = new FormData();
            oArgumentData.append("ActionID", iActionID);
            oArgumentData.append("TestID", iTestID);
            
            arrHxHPlanTestRst = mGetRecordset(sUrl, oArgumentData);

            return(arrHxHPlanTestRst);

        } catch (oError) {
            mSetStatus("mGetTestFromHxHPlan", oError);
        }
    }
    
    async function mInsertTestTemplate(sUrl, iTestID, iPlatformID, iYearID, iTestTypeID) {
        try {

            const oArgumentData = new FormData();
            oArgumentData.append("PlatformID", iPlatformID);
            oArgumentData.append("YearID", iYearID);
            oArgumentData.append("TestID", iTestID);
            oArgumentData.append("TestTypeID", iTestTypeID);

            const oResponse = await fetch(sUrl, {
                method: "POST",
                body: oArgumentData
            });

            if (!oResponse.ok) {
                throw new Error(`HTTP error! Status: ${oResponse.status}`);
            }

            const oData = await oResponse.json();

   
            if (oData && oData.success === true) {    
                return true;
            } else if (oData && oData.error === true) {
                throw new Error(oData.message || "Unknown backend error");
            }

        } catch (oError) {
            oError.AlertMessage = "Addition of new plan failed.";
            mSetStatus("mInsertTestTemplate", oError);
            return false;
        }
    }


    async function mGetTestTypeID(iPlatformID, iYearID, iTestID) {

        try {

            let iTestTypeID = -1;
            let arrRST = null;
            const oArgumentData = new FormData();
            oArgumentData.append("ActionID", cstGetRecordsetAction.Test_Type);
            oArgumentData.append("PlatformID", iPlatformID);
            oArgumentData.append("TestID", iTestID);
            oArgumentData.append("YearID", iYearID);

            arrRST = await mGetRecordset(cstURL.GetRecordset, oArgumentData)

            if (mIsEmpty(arrRST) === false) {
                if (arrRST.length === 1) {
                    iTestTypeID = Number(arrRST[0].Test_Type);

                    if (Number.isNaN(iTestTypeID)) {
                        throw new Error("Invalid Test_Type value returned");
                    }                    
                } else {
                    throw new Error("More than one Test Type ID returned.");    
                }
            } if (mIsEmpty(arrRST) === true) {
                throw new Error("No Test_Type returned");
            }

   
            return iTestTypeID;

        } catch (oError) {
            mSetStatus("mGetTestTypeID", oError);
            return -1; 
        }
    }


    function mGetMaxFinishTime(arrPredecessor) {

        try {

            let sGridPredecessorID = "";
            let sGridPlannedEndTime = "";
            let sMaxPlannedEndTime = "";                
            let iThisPredecessorIndexCounter = 0;
            let iGridIndexCounter = 0;
            let iGridRowID = 0;
            let iCurrentPredecessorID = 0;
            let tPlannedEndTime = null;                
            let tMaxPlannedEndTime = null;
            let oHTMLTable = null;
            let oHTMLTableRow = null;

            for (iThisPredecessorIndexCounter = 0; iThisPredecessorIndexCounter < arrPredecessor.length; iThisPredecessorIndexCounter++) {
                iCurrentPredecessorID = Number(arrPredecessor[iThisPredecessorIndexCounter]);

                // Loop through all rows of StandardTable
                oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);
                oHTMLTableRow = oHTMLTable.getElementsByTagName("tr");

                for (iGridIndexCounter = 0; iGridIndexCounter < oHTMLTableRow.length; iGridIndexCounter++) {
                    
                    const oGridRowID = oHTMLTableRow[iGridIndexCounter].querySelector("input[name='txtGridRowID']");
                    const oGridPredecessor = oHTMLTableRow[iGridIndexCounter].querySelector("input[name='txtGridPredecessor']");
                    const oGridPlannedEndTime = oHTMLTableRow[iGridIndexCounter].querySelector("input[name='txtGridPlannedEndTime']");

                    iGridRowID = Number(oGridRowID.value);
                    sGridPredecessorID = oGridPredecessor.value;
                    sGridPlannedEndTime = oGridPlannedEndTime.value;

                    const arrGridPredecessorID = sGridPredecessorID.split(",").map(s => s.trim());
                    
                    if (mCheckForPredecessor(arrGridPredecessorID.split(","), iCurrentPredecessorID)) {
                        if (sMaxPlannedEndTime.length === 0) {
                            sMaxPlannedEndTime = sGridPlannedEndTime;
                        } else {
                            tPlannedEndTime = mConvertTimeToDateTime(sGridPlannedEndTime);
                            tMaxPlannedEndTime = mConvertTimeToDateTime(sMaxPlannedEndTime);

                            if (tMaxPlannedEndTime < tPlannedEndTime) {
                                sMaxPlannedEndTime = sGridPlannedEndTime;
                            }
                        }
                    }
                }

                return(sMaxPlannedEndTime);

            }

        } catch (oError) {
            mSetStatus("mGetMaxFinishTime", oError);
        }                    
    }


    function mCheckForPredecessor(arrPredecessors, iPredecessor) {
        try {

            return arrPredecessors.includes(iPredecessor);

        } catch (oError) {
            mSetStatus("mCheckForPredecessor", oError);
        }                       
    }


    async function mReIndexPredecessors(sThisRowID, sThisRowPredecessor) {

            let oPredecessor = null;
            let iThisRowID = 0;
            let iIndexingRowID = 0;
            let arrThisRowPredecessor = [];
            let arrIndexingPredecessor = [];

        try {
            
            iThisRowID = Number(sThisRowID);
            arrThisRowPredecessor = sThisRowPredecessor.split(",");
            oPredecessor = await mGetPredecessorObject();
            
            Object.entries(oPredecessor).forEach(([sIndexingRowID, sIndexingPredecessor]) => {
                iIndexingRowID = Number(sIndexingRowID);
                arrIndexingPredecessor = sIndexingPredecessor.split(",");
            });


        } catch (oError) {
            mSetStatus("mReIndexPredecessors", oError);
        }
    }

    async function mGetPredecessorObject() {

        try {
            const oNodeList = document.querySelectorAll("tr");
            const arrRowArray = [...oNodeList];

            //Populate arrRowMap with All the Predecessor Values
            const oRowIDPredecessor = arrRowArray.reduce((arrPredecessor, tr) => {
                const sRowID = tr.querySelector("input[name='txtGridRowID']")?.value || "-1";
                const iRowID = parseInt(sRowID, 10);
                const sPredecessorValue = tr.querySelector("input[name='txtGridPredecessor']")?.value || "";

                if (!isNaN(iRowID)) {
                    arrPredecessor[iRowID] = -1;
                }

                return arrPredecessor;
            }, {});

            return oRowIDPredecessor;

        } catch (oError) {
            mSetStatus("mGetPredecessorObject", oError);
        }
    }

    function mAddMinutesToTime(vTime, iDuration) {
        let tOriginalTime = null;
        let tNewTime = null;
        let iNewMinute = 0;

        
        try {
            
            const sToday = new Date().toISOString().split("T")[0]; // "YYYY-MM-DD"
            tOriginalTime = new Date(`${sToday} ${vTime}`);
            
            
            iNewMinute = tOriginalTime.getMinutes() + Number(iDuration);
            
            tOriginalTime.setMinutes(iNewMinute);
            
            tNewTime = tOriginalTime.toLocaleTimeString('en-US', {hour: '2-digit', minute: '2-digit', hour12: false});
            
            return tNewTime;

        } catch (oError) {
            mSetStatus("mAddMinutesToTime", oError);
        }
    }            
    
    async function mUpdatePlannedTimes() {

            
            let iHTMLRowCount = -1;
            let iHxHPlanArrayRowCount = -1;
            let iHxHPlanIndexCounter = -1;
            let iHxHPreviousHeaderRowIndex = -1;
            let iNextRowType = -1;
            let bDoubleHeaderRowUpdate = { value: false };
            let arrHxHPlanValues = [];
            let oHTMLTable = null;

        try {

            oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);
            const iHTMLRowCount = oHTMLTable.tBodies[0].rows.length; //does not include the thead/tr (the header row)
            
            arrHxHPlanValues = mGetHxHPlanValueArray(oHTMLTable);
            iHxHPlanArrayRowCount = Number(arrHxHPlanValues.length);
            

            //Entire Plan Validation Block: Confirm HTML Row Count and Array Row Count Are Equal
            //-------------------------------------
            if (iHTMLRowCount != iHxHPlanArrayRowCount) {
                const sMessage = "mUpdatePlannedTimes:  There is a mismatch between the array of controls and the html table row count.  The system cannot proceed.  Please contact gbs-es-disaster-recovery@wolterskluwer.com with a screenshot of what you see.";
                alert(sMessage);
                mSetStatus(sMessage);
                return;
            } 
            
            //Set all related rows in the plan to not required when the application is not included in the test
            await mSetUpRequiredPlanRows(oHTMLTable, arrHxHPlanValues);
            
            await mSetTaskGoStopButtonState();

            //Start the loop
            for (let iHxBPlanIndexCounter = 0; iHxBPlanIndexCounter < iHxHPlanArrayRowCount; iHxBPlanIndexCounter++) {

                    
                    giRowID = arrHxHPlanValues[iHxBPlanIndexCounter].RowID;

                    //const sDebugMessage = "Current iHxBPlanIndexCounter Output:  " + iHxBPlanIndexCounter + " Current Row ID:  " + arrHxHPlanValues[iHxBPlanIndexCounter].RowID;
                    //console.log(sDebugMessage);
                    /*if (arrHxHPlanValues[iHxBPlanIndexCounter].RowID === 135) {
                        
                    }*/

                    //-------------------------------------
                    //In Loop Validation Block
                    //-------------------------------------
                        //Check for Predecessor
                        if (mIsEmpty(arrHxHPlanValues[iHxBPlanIndexCounter].Predecessor)) {
                            const sMessage = "mUpdatePlannedTimes:  Row ID - " + arrHxHPlanValues[iHxBPlanIndexCounter].RowID + " Predecessor values can never be empty.  The system cannot proceed.  Please contact gbs-es-disaster-recovery@wolterskluwer.com with a screenshot of what you see.";
                            alert(sMessage);
                            mSetStatus(sMessage);
                            return;                        
                        }

                        //Check if Predecessor Exists and Manage The Case Where it Doesn't Exist
                        if (!mCanManagePredecessorExistence(arrHxHPlanValues, iHxBPlanIndexCounter)) {
                            const sMessage = "mUpdatePlannedTimes: Row ID - " + arrHxHPlanValues[iHxBPlanIndexCounter].RowID + " Predecessor not found in HxH Plan.  The system cannot proceed.  Please contact gbs-es-disaster-recovery@wolterskluwer.com with a screenshot of what you see.";
                            alert(sMessage);
                            mSetStatus(sMessage);
                            return;       
                        }

                    //-------------------------------------
                    //Loop Logic Execution
                    //-------------------------------------
                        if (arrHxHPlanValues[iHxBPlanIndexCounter].RowType === cstRowType.Header) {

                                //First Header Row
                                if (iHxBPlanIndexCounter === arrHxHPlanValues.findIndex(oRow => oRow.RowType === cstRowType.Header)) {
                                        //Do Nothing

                                //Remaining Header Rows
                                } else {

                                    iHxHPreviousHeaderRowIndex = arrHxHPlanValues.findLastIndex((arrRow, iIndexer) => iIndexer < iHxBPlanIndexCounter && arrRow.RowType === 0);
                                    mPopulateHeaderRowStartTimeEndTime(iHxHPreviousHeaderRowIndex, iHxBPlanIndexCounter, arrHxHPlanValues);

                                }

                        } else if (arrHxHPlanValues[iHxBPlanIndexCounter].RowType === cstRowType.Detail) {

                                //Details Row With a Predecessor of 0
                                if (Number(arrHxHPlanValues[iHxBPlanIndexCounter].Predecessor.trim()) === 0) {
                                    
                                    arrHxHPlanValues[iHxBPlanIndexCounter].StartTime = giPlanStartTime24Hr;

                                    if (arrHxHPlanValues[iHxBPlanIndexCounter].TaskStatus !== cstTaskStatus.NotRequired) {
                                        arrHxHPlanValues[iHxBPlanIndexCounter].EndTime = mAddMinutesToTime(giPlanStartTime24Hr, arrHxHPlanValues[iHxBPlanIndexCounter].Duration);
                                    } else {
                                        arrHxHPlanValues[iHxBPlanIndexCounter].EndTime = giPlanStartTime24Hr;
                                    }
                                    
                                //Last Detail Row                                        
                                } else if (iHxBPlanIndexCounter === (iHxHPlanArrayRowCount - 1)) { //Array Count is 1 based but indexcounter is 0 based

                                    mPopulateDetailRowStartTimeEndTime(iHxBPlanIndexCounter, arrHxHPlanValues);
                                    iHxHPreviousHeaderRowIndex = arrHxHPlanValues.findLastIndex((arrRow, iIndexer) => iIndexer < iHxBPlanIndexCounter && arrRow.RowType === cstRowType.Header);
                                    
                                    mPopulateHeaderRowStartTimeEndTime(iHxHPreviousHeaderRowIndex, (iHxBPlanIndexCounter + 1), arrHxHPlanValues); //have to add 1 because the last row is not a header row

                                //Other Detail Rows                                        
                                } else {
                                    
                                    mPopulateDetailRowStartTimeEndTime(iHxBPlanIndexCounter, arrHxHPlanValues);

                                }

                                
                        } else {
                            const sMessage = "mUpdatePlannedTimes:  The Row Type is Not 0 and Is Not 1.  The system only supports those row types.  The system cannot proceed.  Please contact gbs-es-disaster-recovery@wolterskluwer.com with a screenshot of what you see.";
                            alert(sMessage);
                            mSetStatus(sMessage);
                            return;
                        }
                }

                mSetControlHxHPlanTimings(oHTMLTable, arrHxHPlanValues);


        } catch (oError) {
            mSetStatus("mUpdatePlannedTimes", oError);
        }
    }

    async function mSetTaskGoStopButtonState() {
        try {

            let iSelectedValue = -1;
            let bDisable = false;
            let sBackgroundColor = "";
            let arrTaskStartStopButtons = [];
            let oHTMLTable = null;
            let oHTMLTableRow = null;
            let oTBody = null;
            let oGridPlannedResource = null;
            let oGridStatus = null;

            if (giPageID === dctPage["Hour By Hour Plan"]) {

                oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);
                oTBody = oHTMLTable.querySelector('tbody');
                arrTaskStartStopButtons = oTBody.querySelectorAll('tr button[name="btnTaskStartStop"]');

                arrTaskStartStopButtons.forEach((oTaskGoStopButton, iButtonIndex) => {

                    oHTMLTableRow = oTaskGoStopButton.closest('tr');
                    oRowType =  oHTMLTableRow.querySelector('input[name="txtGridRowType"]');

                    if (Number(oRowType.value) !== cstRowType.Header) {

                        oGridPlannedResource = oHTMLTableRow.querySelector('[class*="cboGridMultiPlannedResource"]');

                        if(mIsValueSelectedInMultiSelect(oGridPlannedResource, giUserID, giUserTypeID)) {

                            oGridStatus  =  oHTMLTableRow.querySelector('select[name="cboGridTaskStatus"]');

                            if (oGridStatus.value.trim() !== "") {
                                iSelectedValue = Number(oGridStatus.value);
                            } else {
                                iSelectedValue = -1;
                            }

                            //Handle Image on Button and Button State
                            if (iSelectedValue === cstTaskStatus.InProgress) {
                                oTaskGoStopButton.style.backgroundImage = "url(" + cstImagePath.RedLight + ")";
                                bDisable = false;
                                oTaskGoStopButton.disabled = false;
                                sBackgroundColor = cstHexColorCode.Purple;
                                
                            } else if ((iSelectedValue === cstTaskStatus.NotRequired) || (iSelectedValue === cstTaskStatus.Complete)) {
                                oTaskGoStopButton.style.backgroundImage = "url(" + cstImagePath.BlackLight + ")";
                                bDisable = true;
                                oTaskGoStopButton.disabled = true;
                                sBackgroundColor = cstHexColorCode.SuperLightGrey;

                            } else if (iSelectedValue === cstTaskStatus.NotStarted) {
                                oTaskGoStopButton.style.backgroundImage = "url(" + cstImagePath.GreenLight + ")";
                                bDisable = false;
                                oTaskGoStopButton.disabled = false;
                                sBackgroundColor = cstHexColorCode.White;
  
                            } else {
                               bDisable = false;
                               sBackgroundColor = cstHexColorCode.White;
                            }

                            mSetHTMLTableRowColor(oHTMLTableRow, sBackgroundColor);
                            mToggleEnablementRowControls(oHTMLTableRow, bDisable);

                        } else {
                            oTaskGoStopButton.style.backgroundImage = "url(" + cstImagePath.GreyLight + ")";
                            oTaskGoStopButton.disabled = true;
                        }

                    }

                    oGridStatus = null;
                    oHTMLTableRow = null;

                });

            }

        } catch (oError) {
            mSetStatus("mTaskGoStopButtonState", oError);
        }
    }

    async function mHandlePageSetup() {
        try {


            let oHTMLObject = null;
            let oHTMLAddTableDiv = null;
            let oHTMLStandardTable = null;
            let oHTMLStandardTableTitle = null; //These are all part of the standard table title, but wouldn't response to all in one collapse
            let oHTMLStandardTableHeaderSection = null;  //These are all part of the standard table title, but wouldn't response to all in one collapse
            let oHTMLStandardTableHeaderIcon = null;  //These are all part of the standard table title, but wouldn't response to all in one collapse
            let oHTMLSearchbox = null;
            let oHTMLToolbarTable = null;


            oHTMLStandardTable = mGetHTMLTable(cstTableType.StandardTable);
            oHTMLAddTableDiv = mGetHTMLElement(cstGetHTMLElementType.ID, cstElementID.DivAddTable);
            oHTMLStandardTableTitle  = document.getElementById("StandardTableTitle");
            oHTMLStandardTableHeaderSection = mGetHTMLElement(cstGetHTMLElementType.ID, cstElementID.DivStandardTableHeader);
            oHTMLStandardTableHeaderIcon = mGetHTMLElement(cstGetHTMLElementType.ID, cstElementID.DivStandardTableHeader);
            oHTMLSearchbox = document.getElementById("SearchRow");
            oHTMLToolbarTable = mGetHTMLTable(cstTableType.ToolbarTable);

            await mProcessHourByHourPlanPageLoad(gsPageName);

            if (oHTMLStandardTable) {
                const oTBody = oHTMLStandardTable.tBodies[0];
                const iStandardTableRowCount = oTBody ? oTBody.rows.length : 0;

                if (Number(iStandardTableRowCount) === 0) { //No Rows in Standard Table
                    if (oHTMLAddTableDiv) {
                        oHTMLAddTableDiv.style.display = "";
                        oHTMLAddTableDiv.style.marginBottom = "0px";
                    }

                    if (oHTMLStandardTableTitle) {
                        oHTMLStandardTableTitle.style.display = "none";
                        oHTMLStandardTableHeaderSection.style.display = "none";
                        oHTMLStandardTableHeaderIcon.style.display = "none";
                        oHTMLSearchbox.style.display = "none";
                        oHTMLToolbarTable.style.display = "none";
                    }

                    oHTMLStandardTable.style.display = "none";

                } else if (Number(iStandardTableRowCount) > 0) {  //Rows Present in Standard Table
                    if (oHTMLAddTableDiv) {
                        oHTMLAddTableDiv.style.display = "none";
                        oHTMLAddTableDiv.style.marginBottom = "25px";
                    }

                    if (oHTMLStandardTableTitle) {
                        oHTMLStandardTableTitle.textContent = gsPageName;
                        oHTMLStandardTableTitle.style.display = "";
                        oHTMLStandardTableHeaderSection.style.display = "";
                        oHTMLStandardTableHeaderIcon.style.display = "";
                        oHTMLSearchbox.style.display = "";
                        oHTMLToolbarTable.style.display = "";
                    }

                    oHTMLStandardTable.style.display = "";
                }
            } else {
                if (oHTMLAddTableDiv) {
                    oHTMLAddTableDiv.style.display = "";
                    oHTMLAddTableDiv.style.marginBottom = "0px";
                }
            }


            mDisableButtons();

        } catch (oError) {
            mSetStatus("mHandlePageSetup", oError);
        }

    }



    async function mSetUpRequiredPlanRows(oHTMLTable, arrHxHPlanValues) {
        try {

            let iHxHPlanArrayRowCount = -1;
            let iRowIndex = -1;
            let iCurrentApplicationSelectedValue = -1;
            let arrApplicationID = [];

            iHxHPlanArrayRowCount = Number(arrHxHPlanValues.length);

            const oRow = oHTMLTable.querySelectorAll("tbody tr");
            const oRowType = oHTMLTable.querySelectorAll("tbody td input[name='txtGridRowType']");
            const oRowID = oHTMLTable.querySelectorAll("tbody td input[name='txtGridRowID']");
            const oApplication = oHTMLTable.querySelectorAll("tbody td select[name='cboGridApplication']");
            const oTaskStatus = oHTMLTable.querySelectorAll("tbody td select[name='cboGridTaskStatus']");

            
            arrApplicationID = gsApplicationTestAppIDList.split(",");
            arrApplicationID = arrApplicationID.map(vItem => vItem.trim());
            arrApplicationID = arrApplicationID.map(vItem => Number(vItem));
            

            for (iRowIndex = 0; iRowIndex < iHxHPlanArrayRowCount; iRowIndex++) {                    
                
                //Set the status to not required if the row applications are not included in the test.
                iCurrentApplicationSelectedValue = Number(oApplication[iRowIndex].value);

                if (!arrApplicationID.includes(iCurrentApplicationSelectedValue) && Number(oRowType[iRowIndex].value) !== 0) {

                    //Set the generic control values
                    oTaskStatus[iRowIndex].value = cstTaskStatus.NotRequired; //not required
                    arrHxHPlanValues[iRowIndex].TaskStatus = cstTaskStatus.NotRequired;

                    //Read Only and grey out the standard controls for the row
                    const oControl = oRow[iRowIndex].querySelectorAll("input, select, textarea, button");
                    oControl.forEach(oItem => {
                        if (oItem.name !== "cboGridTaskStatus") {
                            oItem.disabled = true;
                        }
                    });

                    //Read Only and grey out the multi-select controls
                    const oMultiSelectDIVInstances = oRow[iRowIndex].querySelectorAll('.multi-select');
                    oMultiSelectDIVInstances.forEach(oMultiSelectDIV => {
                        oMultiSelectDIV.classList.add("multi-select-readonly");
                    });

                    //Hide the Row
                    oRow[iRowIndex].querySelectorAll("td").forEach(td => {
                        td.style.backgroundColor = cstHexColorCode.SuperLightGrey;
                    });

                    oRow[iRowIndex].style.display = "none";
                    
                }


            }

            oHTMLTable._ViewState = "HIDDEN";

            

        } catch (oError) {
            mSetStatus("mSetUpRequiredPlanRows", oError);
        }
    }

    function mSetControlHxHPlanTimings(oTable, arrHxHPlanValues) {

        try {

            let iHxHPlanArrayRowCount = -1;
            let iControlSetIndex = -1;

            iHxHPlanArrayRowCount = Number(arrHxHPlanValues.length);

            const oRowID    = oTable.querySelectorAll("tbody td input[name='txtGridRowID']");
            const oRowType    = oTable.querySelectorAll("tbody td input[name='txtGridRowType']");
            const oDuration    = oTable.querySelectorAll("tbody td input[name='txtGridPlannedDuration']");
            const oStartTime   = oTable.querySelectorAll("tbody td input[name='txtGridPlannedStartTime']");
            const oEndTime     = oTable.querySelectorAll("tbody td input[name='txtGridPlannedEndTime']");

            //Loop through that array and set the control values
            for (iControlSetIndex = 0; iControlSetIndex < iHxHPlanArrayRowCount; iControlSetIndex++) {

                        
                            giRowID = Number(oRowID[iControlSetIndex].value);

                            //console.log("mSetHxHPlanTimings: Populating Controls.  Row ID: " + giRowID);

                            const dtTempStartTime = mConvertTimeToDateTime(arrHxHPlanValues[iControlSetIndex].StartTime);
                            const dtTempEndTime = mConvertTimeToDateTime(arrHxHPlanValues[iControlSetIndex].EndTime);
                            //

                            //Set the Start and End Time of Header and Detail Rows From Array
                            if (Number(oRowID[iControlSetIndex].value) === arrHxHPlanValues[iControlSetIndex].RowID) {

                                    oStartTime[iControlSetIndex].value = arrHxHPlanValues[iControlSetIndex].StartTime;
                                    oEndTime[iControlSetIndex].value = arrHxHPlanValues[iControlSetIndex].EndTime;

                                    //If Header Row Calculate and Set the Duration
                                    if (Number(oRowType[iControlSetIndex].value) === cstRowType.Header) {
                                        const iDuration = (dtTempEndTime - dtTempStartTime);
                                        const iDurationInMinutes = iDuration / (1000 * 60);
                                        oDuration[iControlSetIndex].value = Number(iDurationInMinutes);
                                    }

                            } else {

                                mSetStatus("mSetControlHxHPlanTimings:  Control Row ID does not match arrHxHPlanValues Row ID.  Fatal output error.");

                            }
                }
                                                    
        
        } catch (oError) {
            mSetStatus("mSetControlHxHPlanTimings", oError);
        }
    }            

    function mGetMinMaxTime(tTimeInput, tTimeCompare, cDateComparisonMode) {

        try {

            //Set Variables
            const dtDateTimeInput = mConvertTimeToDateTime(tTimeInput);
            const dtDateTimeCompare = mConvertTimeToDateTime(tTimeCompare);

            //Validation
            if (!(dtDateTimeInput instanceof Date) || isNaN(dtDateTimeInput)) throw new Error("dtDateTimeInput Invalid DateTime.");
            if (!(dtDateTimeCompare instanceof Date) || isNaN(dtDateTimeCompare)) throw new Error("dtDateTimeCompare Invalid DateTime.");


            if (cDateComparisonMode === cstDateComparisonMode.Min) {
                return dtDateTimeInput <= dtDateTimeCompare ? tTimeInput : tTimeCompare;
            } else if (cDateComparisonMode === cstDateComparisonMode.Max) {
                return dtDateTimeInput >= dtDateTimeCompare ? tTimeInput : tTimeCompare;
            } else {
                throw new Error("cDateComparisonMode must be 'min' or 'max'");
            }

        } catch (oError) {
            mSetStatus("mGetMinMaxTime", oError);
        }            

    }

    function mGetNextRowType(iCurrentRowIndex, arrHxHPlanValues) {

            let iRowType = -1;

        try {

            iRowType = arrHxHPlanValues[iCurrentRowIndex + 1].RowType;

            return iRowType;

        } catch (oError) {
            mSetStatus("mGetNextRowType", oError);
            iRowType = -1;
            return iRowType;
        }

    }

    function mPopulateHeaderRowStartTimeEndTime(iPreviousHeaderRowIndex, iCurrentHeaderRowIndex, arrHxHPlanValues) {

        let tMinStartTime = null;
        let tMaxEndTime = null;

        try {

            tMinStartTime = arrHxHPlanValues[iPreviousHeaderRowIndex + 1].StartTime;
            tMaxEndTime = arrHxHPlanValues[iPreviousHeaderRowIndex + 1].EndTime;

            for (iRowIndex = iPreviousHeaderRowIndex + 1; iRowIndex < iCurrentHeaderRowIndex; iRowIndex++) {
                if (Number(arrHxHPlanValues[iRowIndex].TaskStatus) !== cstTaskStatus.NotRequired) {
                    tMinStartTime = mGetMinMaxTime(tMinStartTime, arrHxHPlanValues[iRowIndex].StartTime, cstDateComparisonMode.Min);
                    tMaxEndTime = mGetMinMaxTime(tMaxEndTime, arrHxHPlanValues[iRowIndex].EndTime, cstDateComparisonMode.Max);
                }
            }

            arrHxHPlanValues[iPreviousHeaderRowIndex].StartTime = tMinStartTime;
            arrHxHPlanValues[iPreviousHeaderRowIndex].EndTime = tMaxEndTime;

        } catch (oError) {
            mSetStatus("mPopulateHeaderRowStartTimeEndTime", oError);
        }
    }


    function mPopulateDetailRowStartTimeEndTime(iCurrentRowIndex, arrHxHPlanValues) {
            
            let sFailureMessage = "";
            let iRowID = -1;            
            let tMaxPredecessorEndDateTime = null;
            let tStartTime = null;
            let tEndTime = null;

            try {

                iRowID = arrHxHPlanValues[iCurrentRowIndex].RowID;

                tMaxPredecessorEndDateTime = mMaxEndTimeSearch(arrHxHPlanValues, iRowID);

                if (tMaxPredecessorEndDateTime !== null) {

                    //mConvertTimeToDateTime(tMaxPredecessorEndDateTime);
                    //tMaxPredecessorEndDateTime = tMaxPredecessorEndDateTime.toLocaleTimeString('en-US', {hour: '2-digit', minute: '2-digit', hour12: false});

                    tStartTime = tMaxPredecessorEndDateTime;
                    
                    tEndTime = mAddMinutesToTime(tStartTime, arrHxHPlanValues[iCurrentRowIndex].Duration);

                    arrHxHPlanValues[iCurrentRowIndex].StartTime = tStartTime;
                    arrHxHPlanValues[iCurrentRowIndex].EndTime = tEndTime;

                } else {
                    alert("mPopulateDetailRowStartTimeEndTime: Could not identify a Max Prior Task/Predecessor End Time.");
                }

            } catch (oError) {
                mSetStatus("mPopulateDetailRowStartTimeEndTime", oError);
            }
        }

    async function mGetPlanLockUser(iPlatformID, iYearID, iTestID) {
        try {        

            let iPlanLockUser = null;
            let arrPlanLock = [];

            let oArgumentData = new FormData();
            oArgumentData.append("ActionID", cstGetRecordsetAction.Check_Plan_Lock);         	    	    
            oArgumentData.append("PlatformID", iPlatformID);
            oArgumentData.append("YearID", iYearID);            	    
            oArgumentData.append("TestID", iTestID);   
            
            arrPlanLock = await mGetRecordset(cstURL.GetRecordset, oArgumentData);

            //No Locked Rows Found
            if (mIsEmpty(arrPlanLock) === true) {
                iPlanLockUser = -1;
            
            //Rows of 1 Test Are Locked By More Than One User ID Found
            } else if (arrPlanLock.length > 1) {
                throw new Error("More than one sure has the HxH Plan locked.  Please submit a feature/fix request by using the link in the top right of this page.");

            //Return User ID of Locked Rows
            } else {
                iPlanLockUser = Number(arrPlanLock[0].Locked_By_ID ?? -1);
            }

            return(iPlanLockUser);

        } catch (oError) {
            mSetStatus("mGetPlanLockUser", oError);
        }
    }

    async function mToggleTimingControls(bEnabled) {
        try {    

            let oHTMLTable = null;
            let oHTMLTBody = null;
            let oHTMLTableRows = null;
            let oRowType = null;
            let oDurationControl = null;
            let oPredecessorControl = null;

            oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);
            oHTMLTBody = oHTMLTable.tBodies[0];
            oHTMLTableRows = oHTMLTBody.rows;

            for (let iRowIndex = 0; iRowIndex < oHTMLTableRows.length; iRowIndex++) {
                const oCurrentRow = oHTMLTableRows[iRowIndex];
                
                oRowType = mGetHTMLElement(cstGetHTMLElementType.Name, "txtGridRowType", oCurrentRow);

                if ((mIsNumeric(oRowType.value) === true) && (Number(oRowType.value) === cstRowType.Detail)) {
                    oDurationControl = mGetHTMLElement(cstGetHTMLElementType.Name, "txtGridPlannedDuration", oCurrentRow);
                    oPredecessorControl = mGetHTMLElement(cstGetHTMLElementType.Name, "txtGridPredecessor", oCurrentRow);

                    oDurationControl.disabled = bEnabled;
                    oPredecessorControl.disabled = bEnabled;

                    if (bEnabled === true) {
                        oDurationControl.style.color = cstHexColorCode.White;
                        oPredecessorControl.style.color = cstHexColorCode.White;
                    } else if (bEnabled === false) {
                        oDurationControl.style.color = cstHexColorCode.SuperLightGrey;
                        oPredecessorControl.style.color = cstHexColorCode.SuperLightGrey;
                    }
                }

            }


        } catch (oError) {
            mSetStatus("mToggleTimingControls", oError);
        }
    }        
            
    /*function mPopulateDetailRowStartTimeEndTime(iCurrentRowIndex, arrHxHPlanValues) {
            
            let sFailureMessage = "";
            let iPredecessorCounter = 0;
            let iPredecessorRowIndex = 0;
            let iRowID = -1;            
            let tTempPredecessorEndDateTime = null;
            let tMaxPredecessorEndDateTime = null;
            let tStartTime = null;
            let tEndTime = null;
            let bValidPredecessor = false;
            let arrPredecessor = [];
            let oTaskStatus = null;

            try {

                iRowID = arrHxHPlanValues[iCurrentRowIndex].RowID;
                arrPredecessor = mGetPredecessorArray(arrHxHPlanValues[iCurrentRowIndex].Predecessor);
                

                if (arrPredecessor.length > 0) {
                    for (iPredecessorCounter = 0; iPredecessorCounter < arrPredecessor.length; iPredecessorCounter++) {

                        //Get the predecessor row index
                        iPredecessorRowIndex = arrHxHPlanValues.findIndex(arrRow => Number(arrRow.RowID) === Number(arrPredecessor[iPredecessorCounter]));

                        if (iPredecessorRowIndex === -1) {
                            sFailureMessage =
                                "mDetailRowPopulateStartTimeEndTime: The predecessor for the row of the HxH Plan " +
                                "does not exist in the currently loaded HxH Plan. ";

                            sFailureMessage = sFailureMessage + "Current Row ID:  " + iRowID + " Predecessor Row ID: " + arrPredecessor[iPredecessorRowIndex];
                            //alert(sFailureMessage);
                            throw new Error(sFailureMessage);
                        } else {

                            if (arrHxHPlanValues[iPredecessorRowIndex].TaskStatus !== cstTaskStatus.NotRequired) {
                                bValidPredecessor = true;
                                tTempPredecessorEndDateTime = mConvertTimeToDateTime(arrHxHPlanValues[iPredecessorRowIndex].EndTime);

                                if (tMaxPredecessorEndDateTime === null) {
                                    tMaxPredecessorEndDateTime = tTempPredecessorEndDateTime;
                                } else {
                                    if (tTempPredecessorEndDateTime > tMaxPredecessorEndDateTime) {
                                        tMaxPredecessorEndDateTime = tTempPredecessorEndDateTime;
                                    }
                                }
                            }
                        }

                    }
                } else {
                    alert("mPopulateDetailRowStartTimeEndTime: Prior Row/Predecessor value required for Row " + arrHxHPlanValues[iCurrentRowIndex].RowID);
                }

                if (bValidPredecessor === false) {

                    tMaxPredecessorEndDateTime = mMaxEndTimeSearch(arrHxHPlanValues, iRowID);

                    if (tMaxPredecessorEndDateTime !== null) {
                        bValidPredecessor = true;
                    }
                    
                }

                if (bValidPredecessor === true) {

                    tMaxPredecessorEndDateTime = tMaxPredecessorEndDateTime.toLocaleTimeString('en-US', {hour: '2-digit', minute: '2-digit', hour12: false});

                    //If the row is not required then override the duration end time calculation and just set the end to start for 0 mins duration
                    tStartTime = tMaxPredecessorEndDateTime;
                    
                    tEndTime = mAddMinutesToTime(tStartTime, arrHxHPlanValues[iCurrentRowIndex].Duration);

                    arrHxHPlanValues[iCurrentRowIndex].StartTime = tStartTime;
                    arrHxHPlanValues[iCurrentRowIndex].EndTime = tEndTime;

                } else {
                    alert("mPopulateDetailRowStartTimeEndTime: Didn't work.");
                }

            } catch (oError) {
                mSetStatus("mPopulateDetailRowStartTimeEndTime", oError);
            }
        }*/

        function mMaxEndTimeSearch(arrHxHPlanValues, iRowID) {
            try {

                let iRowIDOfPredecessor = -1;
                let bValidMaxEndTimeFound = false;
                let tTempMaxEndTime = null;
                let tReturnMaxEndTime = null;
                
                let arrRow = [];
                let arrPredecessor = [];
                let arrRowOfPredecessor = [];                

                arrRow = arrHxHPlanValues.find(arrTempRow => arrTempRow.RowID === iRowID);

                if (mIsEmpty(arrRow) === true) {
                    throw new Error("Row ID cannot be find in arrHxHPlanValues. Row ID: " + iRowID);
                }

                arrPredecessor = mGetPredecessorArray(arrRow.Predecessor);

                if (arrPredecessor.length > 0) {
                    for (iPredecessorCounter = 0; iPredecessorCounter < arrPredecessor.length; iPredecessorCounter++) {

                        iRowIDOfPredecessor = arrPredecessor[iPredecessorCounter];

                        arrRowOfPredecessor = arrHxHPlanValues.find(arrTempRow => arrTempRow.RowID === iRowIDOfPredecessor);

                        if (mIsEmpty(arrRowOfPredecessor) === true) {
                            throw new Error("Row ID of Predecessor cannot be find in arrHxHPlanValues. Row ID of Predecessor: " + iRowIDOfPredecessor);
                        }

                        if (arrRowOfPredecessor.TaskStatus !== cstTaskStatus.NotRequired) {
                            tTempMaxEndTime = arrRowOfPredecessor.EndTime;
                            bValidMaxEndTimeFound = true;

                            if (!tReturnMaxEndTime || tTempMaxEndTime > tReturnMaxEndTime) {
                                tReturnMaxEndTime = tTempMaxEndTime;
                            }
                        }
                    }
                }

                if (bValidMaxEndTimeFound === false) {

                    // FIX #1: your loop was missing "<"
                    for (iPredecessorCounter = 0; iPredecessorCounter < arrPredecessor.length; iPredecessorCounter++) {

                        iRowIDOfPredecessor = arrPredecessor[iPredecessorCounter];

                        // recursive call
                        tTempMaxEndTime = mMaxEndTimeSearch(arrHxHPlanValues, iRowIDOfPredecessor);

                        if ((tTempMaxEndTime !== null) &&  ((tReturnMaxEndTime === null) || (tTempMaxEndTime > tReturnMaxEndTime))) {

                            tReturnMaxEndTime = tTempMaxEndTime;   
                        }
                    }
                }

                return tReturnMaxEndTime;

            } catch (oError) {
                mSetStatus("mMaxEndTimeSearch", oError);
                return null;
            }
        }



        function mCanManagePredecessorExistence(arrHxHPlanValues, iCurrentRowIndex) {

            let arrHxHPlanRowPredecessor = [];
            let bCanManagePredecessorExistence = false;

            try {

                //Validation Block.  Exit If it's the first row because the first row doesn't ever have a predecessor
                if (iCurrentRowIndex === arrHxHPlanValues.findIndex(oRow => oRow.RowType === 0) || iCurrentRowIndex === arrHxHPlanValues.findIndex(oRow => oRow.RowType === 1)) {
                    bCanManagePredecessorExistence = true;
                    return bCanManagePredecessorExistence;

                //Validation Block. Exit if the predecessor is 0 because 0 doesn't have a predecessor
                } else if (Number(arrHxHPlanValues[iCurrentRowIndex].Predecessor.trim()) === 0) {
                    bCanManagePredecessorExistence = true;
                    return bCanManagePredecessorExistence;
                }

                arrHxHPlanRowPredecessor = mGetPredecessorArray(arrHxHPlanValues[iCurrentRowIndex].Predecessor);

                arrHxHPlanRowPredecessor.forEach(function(iPredecessor, iIndex) {
                    iPredecessor = Number(iPredecessor);

                    
                    const oRowFound = arrHxHPlanValues.find(oEachHxHPlanRow => oEachHxHPlanRow.RowID === iPredecessor);

                    if (oRowFound) {
                        bCanManagePredecessorExistence = true;
                    }
                });

                if (bCanManagePredecessorExistence === false) {

                }

                return bCanManagePredecessorExistence;

            } catch (oError) {
                mSetStatus("mManagePredecessorExistence", oError);
                return false;
            }
        }


        function mGetPreviousRowTypeIndex(arrArrayToSearch, iCurrentIndex, iRowType) {

            try {

                const iPreviousRowTypeIndex = arrArrayToSearch.findLastIndex((oRow, iIndex) => iIndex < iCurrentIndex && oRow.RowType === iRowType);

                return iPreviousRowTypeIndex;

            } catch (oError) {
                mSetStatus("mGetPreviousRowTypeIndex", oError);
                return -1;
            }

        }

        function mGetPredecessorArray(sPredecessorValue) {
            let arrPredecessor = [];

            try {
                if (typeof sPredecessorValue === "string" && sPredecessorValue.trim().length > 0) {
                    arrPredecessor = sPredecessorValue
                        .split(",")
                        .map(s => s.trim())
                        .filter(s => s !== "")
                        .map(Number);
                } else {
                    throw new Error("sPredecessorValue cannot be blank");
                }

                return arrPredecessor;

            } catch (oError) {
                mSetStatus("mGetPredecessorArray", oError);
                return [];
            }
        }

        function mGetHxHPlanValueArray(oTable) {
            try {

                const oRowID       = oTable.querySelectorAll("tbody td input[name='txtGridRowID']");
                const oRowType     = oTable.querySelectorAll("tbody td input[name='txtGridRowType']");
                const oTaskStatus   = oTable.querySelectorAll("tbody td select[name='cboGridTaskStatus']"); 
                const oDuration    = oTable.querySelectorAll("tbody td input[name='txtGridPlannedDuration']");
                const oStartTime   = oTable.querySelectorAll("tbody td input[name='txtGridPlannedStartTime']");
                const oEndTime     = oTable.querySelectorAll("tbody td input[name='txtGridPlannedEndTime']");
                const oPredecessor = oTable.querySelectorAll("tbody td input[name='txtGridPredecessor']");

                const arrReturnHxHPlanValues = Array.from(oRowID, (_, iIndex) => ({
                    RowID:       Number(oRowID[iIndex].value),
                    RowType:     Number(oRowType[iIndex].value),
                    TaskStatus:   Number(oTaskStatus[iIndex].value),
                    Duration:    Number(oDuration[iIndex].value),
                    StartTime:   oStartTime[iIndex].value,
                    EndTime:     oEndTime[iIndex].value,
                    Predecessor: oPredecessor[iIndex].value
                }));

                return(arrReturnHxHPlanValues);

            } catch (oError) {
                mSetStatus("mGetHxHPlanValueArray", oError);
            }
        }

    function mConvertTimeToDateTime(sTime) {

        let tReturnDateTime = null;

        try {

            if (sTime.trim() !== "") {

                // Get today's date in YYYY-MM-DD format
                const dToday = new Date();
                const sYear = dToday.getFullYear();
                const sMonth = String(dToday.getMonth() + 1).padStart(2, '0'); // months are 0-based
                const sDay = String(dToday.getDate()).padStart(2, '0');

                // Build the string exactly like your original
                tReturnDateTime = new Date(`${sYear}-${sMonth}-${sDay} ${sTime}`);


            } else {

                throw new Error("Invalid Time passed into method.");

            }

            return(tReturnDateTime);

        } catch (oError) {
            mSetStatus("mConvertTimeToDateTime", oError);
            return(null);
        }
    }

    function mSetActualStartEndTime(oTaskStatusControl) {
        try {

            let iSelectedTaskStatusValue = -1;
            let oHTMLTableRow = null;
            let oActualStartDate = null;
            let oActualEndDate = null;

            if (!oTaskStatusControl) {
                throw new Error("oTaskStatusControl is not set and is required.")
            } else if (oTaskStatusControl.name.toUpperCase() !== "CBOGRIDTASKSTATUS") {
                return;
            } else {
                iSelectedTaskStatusValue = oTaskStatusControl.value;

                if (mIsNumeric(iSelectedTaskStatusValue) === false) {
                    throw new Error("iSelectedTaskValue is not numeric.")
                } else {
                    iSelectedTaskStatusValue = Number(iSelectedTaskStatusValue);
                }
            }

            oHTMLTableRow = oTaskStatusControl.closest("tr");

            if (oHTMLTableRow) {
                oActualStartDate = oHTMLTableRow.querySelector('input[name="txtGridActualStartTime"]');
                oActualEndDate   = oHTMLTableRow.querySelector('input[name="txtGridActualEndTime"]');

                if ((oActualStartDate) && (oActualEndDate)) {

                    if (iSelectedTaskStatusValue === cstTaskStatus.NotStarted) {
                        oActualStartDate.value = "";
                        oActualEndDate.value = "";

                    } else if (iSelectedTaskStatusValue === cstTaskStatus.InProgress) {
                        oActualStartDate.value = mGetCurrentCentralTime(false, false);
                        oActualEndDate.value = "";

                    } else if (iSelectedTaskStatusValue === cstTaskStatus.Complete) {

                        if (oActualStartDate.value.trim() === "") {
                            oActualStartDate.value = mGetCurrentCentralTime(false, false);
                            oActualEndDate.value = mGetCurrentCentralTime(false, false);
                        } else if (oActualStartDate.value.trim() !== "") {
                            oActualEndDate.value = mGetCurrentCentralTime(false, false);
                        }
                    }

                    oBroadcastClient.TargetObject = oTaskStatusControl;
                    oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.ControlUpdate.ID);

                    oBroadcastClient.TargetObject = oActualStartDate;
                    oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.ControlUpdate.ID);

                    oBroadcastClient.TargetObject = oActualEndDate;
                    oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.ControlUpdate.ID);
                } else {
                    throw new Error("oActualStartDate and oActualEndDate set not successful and is required.");
                }
            } else {
                throw new Error("oHTMLTableRow set not successful and is required.");
            }


        } catch (oError) {
            mSetStatus("mSetActualStartEndTime", oError);
        }
    }

    async function mProcessStatusChange(ocboGridTaskStatus) {

                try {

                    let iStatusID = -1;
                    let oRow = null;
                    let oActualStartTimeControl = null;
                    let oActualEndTimeControl = null;

                    if (ocboGridTaskStatus) {
                        oRow = ocboGridTaskStatus.closest('tr');
                        iStatusID = parseInt(ocboGridTaskStatus.value) || 0;


                        if (oRow) {
                            oActualStartTimeControl = oRow.querySelector('[name="txtGridActualStartTime"]');
                            oActualEndTimeControl   = oRow.querySelector('[name="txtGridActualEndTime"]');

                            if (oActualStartTimeControl && oActualEndTimeControl) {
                                oActualStartTimeControl.style.color = cstHexColorCode.Black;
                                oActualEndTimeControl.style.color = cstHexColorCode.Black;

                                // In Progress
                                if (iStatusID === cstTaskStatus.InProgress) {
                                    oActualStartTimeControl.value = mGetCurrentCentralTime(false, false);
                                    oActualEndTimeControl.value = "";
                                    await mSetHTMLTableRowColor(oRow, "");

                                // Complete
                                } else if (iStatusID === cstTaskStatus.Complete) {
                                    if (oActualStartTimeControl.value === "") {
                                        oActualStartTimeControl.value = mGetCurrentCentralTime(false, false);
                                    }
                                    oActualEndTimeControl.value = mGetCurrentCentralTime(false, false);
                                    await mSetHTMLTableRowColor(oRow, cstHexColorCode.SuperLightGrey);

                                // Not Started / 0
                                } else if (iStatusID === 0 || iStatusID === cstTaskStatus.NotStarted) {
                                    oActualStartTimeControl.value = "";
                                    oActualEndTimeControl.value = "";
                                    await mSetHTMLTableRowColor(oRow, "");

                                // Not Required                                     
                                }  else if (iStatusID === cstTaskStatus.NotRequired) {
                                    oActualStartTimeControl.value = "";
                                    oActualEndTimeControl.value = "";
                                    await mSetHTMLTableRowColor(oRow, cstHexColorCode.SuperLightGrey);
                                }
                            } else {
                                throw new Error("Failed to retrieve StartTime and/or EndTime control reference.");
                            }
                        } else {
                            throw new Error("oRow is required.");
                        }
                    } else {
                        throw new Error("GridTaskStatus argument is required.");
                    }
                } catch (oError) {
                    mSetStatus("mProcessStatusChange", oError);
                }
            }

    async function mLoadSubSelectDropdown() {
        try {

            let bEmptyDropDown = false;
            let oArgumentData = null;
            let oDropDown = null;
            let oDropDownSpan = null;

            
            if (giPageID === dctPage["Hour By Hour Plan"]) {
                
                oDropDown = document.querySelector('[name="cboSubSelect"]');
                oDropDownSpan = document.querySelector('.DropDownMessage');

                if (oDropDown) {
                    
                    oArgumentData = new FormData();
                    oArgumentData.append("ActionID", cstGetRecordsetAction.AllTests);
                    oArgumentData.append("PlatformID", giPlatformID);
                    oArgumentData.append("YearID", giYearID);
                    
                    bEmptyDropDown = await mLoadDropDownFromDB(oDropDown, cstGetRecordsetAction.AllTests, oArgumentData, "Test_ID", "Test_Desc");

                }
                
            } else if (giPageID === dctPage["Plan Template"]) {

                oDropDown = document.querySelector('[name="cboSubSelect"]');
                oDropDownSpan = document.querySelector('.DropDownMessage');

                if (oDropDown) {

                    arrArrayForLoad = mFilterFromArray (arrLookupTable, "Category", "Test_Type");
                    bEmptyDropDown = await mLoadDropDownFromArray(oDropDown, arrArrayForLoad, "Lookup_ID", "Lookup_Desc");

                }

            //If the drop down is not in the logic above, then clear it to avoid persistence
            } else {
                oDropDown = document.querySelector('[name="cboSubSelect"]');
                oDropDownSpan = document.querySelector('.DropDownMessage');

                if (oDropDown) {

                    while (oDropDown.options.length > 0) {
                        oDropDown.remove(0);
                    }

                }
            }

            if (bEmptyDropDown === true && oDropDown) {
                oDropDown.style.display = "none";
                if (oDropDownSpan) {
                    oDropDownSpan.innerHTML = "No data available.";
                }
            } else if (bEmptyDropDown === false && oDropDown) {
                oDropDown.style.display = "";
                oDropDownSpan.innerHTML = "";
            }

        } catch (oError) {
            mSetStatus("mLoadSubSelectDropdown", oError);
        }
    }

    function mProcessViewInfoButton() {
        try {

            let sTaskDetail = "";
            let sRelatedJira = "";

            let oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);
            let oViewInfoButton = null;

            if (oHTMLTable) {

                const oHTMLTBody = oHTMLTable.tBodies[0];
                const oHTMLTableRows = oHTMLTBody.rows;

                for (let iRowIndex = 0; iRowIndex < oHTMLTableRows.length; iRowIndex++) {
                    const oHTMLTableCurrentRow = oHTMLTableRows[iRowIndex];


                    oViewInfoButton = oHTMLTableCurrentRow.querySelector('[name="btnViewTaskDetails"]');

                    if (oViewInfoButton) {

                        //The view info button should be enabled regardless of user type
                        oViewInfoButton.disabled = false;

                        sTaskDetail = oHTMLTableCurrentRow.querySelector('[name="txtGridTaskDetails"]').value;
                        sRelatedJira = oHTMLTableCurrentRow.querySelector('[name="txtGridJiraRainierReference"]').value;

                        if (sTaskDetail.trim().length > 0 || sRelatedJira.trim().length > 0) {
                            oViewInfoButton.style.color = cstHexColorCode.Red;
                            oViewInfoButton.style.borderColor = cstHexColorCode.Red;
                        }
                    }
                }
            }

        } catch (oError) {
            mSetStatus("mProcessViewInfoButton", oError);
        }
    }
    

    async function mSetHxHPlanLock(iTestID, bLockPlan) {
        try {

            


        } catch (oError) {
            mSetStatus("mSetHxHPlanLock", oError);
        }            
    }

    async function mSetGlobalTestSettings(iPlatformID, iYearID, iTestID) {
        try {

            let arrJSONRst = null;
            let oArgumentData = null;


            oArgumentData = new FormData();
            oArgumentData.append("ActionID", cstGetRecordsetAction["Test_Settings"]);                
            oArgumentData.append("PlatformID", iPlatformID);
            oArgumentData.append("YearID", iYearID);
            oArgumentData.append("TestID", iTestID);                
            
            arrJSONRst = await mGetRecordset(cstURL.GetRecordset, oArgumentData);

            if ((Array.isArray(arrJSONRst)) && (arrJSONRst.length > 0)) {
                giTestID = iTestID;
                giTestTypeID = arrJSONRst[0].Test_Type;
                gbTestLock = mConvertBitToBoolean(arrJSONRst[0].Lock);
            }

            oArgumentData = null;
            arrJSONRst = null;

        } catch (oError) {
            mSetStatus("mSetGlobalTestSettings", oError);
        }            
    }

    function formatDateCentral(dateInput, options = {}) {
        const date = new Date(dateInput);
        const centralTime = new Intl.DateTimeFormat('en-US', {
        timeZone: 'America/Chicago', // Central Time (handles CST/CDT automatically)
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        ...options
        }).format(date);

        return centralTime;
    }