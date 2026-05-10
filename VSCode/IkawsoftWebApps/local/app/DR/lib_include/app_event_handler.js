
    
    async function mHandleMainPageEvent(iPlatformID, iYearID, iPageID, iUserID, iUserTypeID, iSubSelectedValue = -1) {

        try {
	    
            let iApplicationID = -1;
            
            iApplicationID = mGetApplicationID(cstURL.URLExtension);
	    
            iPlatformID  = Number(iPlatformID);
            iYearID      = Number(iYearID);
            iPageID      = Number(iPageID);
            iUserID      = Number(iUserID);
            iUserTypeID  = Number(iUserTypeID);
            iSubSelectedValue = Number(iSubSelectedValue);

            //lock page that is unloading and page that is loading if hxh plan page
            mTogglePlanTimingLockOnExit(giPageID, false);
            mTogglePlanTimingLockOnExit(iPageID, false);

            //Run Init Routines
            await mInitializeApp(iApplicationID, iPlatformID, iYearID, iPageID, iUserID, iUserTypeID);

            //Set Session Variables From Javascript Variables
            const oArgumentData = new FormData();
            oArgumentData.append("SelectedPlatform", iPlatformID);
            oArgumentData.append("SelectedYear", iYearID);
            oArgumentData.append("SelectedPage", iPageID);
            await mSetSessionValues(cstURL.SetSession, oArgumentData);    

            //Load HTML Elements
            await mLoadSubSelectDropdown();
            await mOutputTable(arrHTMLControlTable, iSubSelectedValue);
            

        } catch (oError) {
           mSetStatus("mHandleMainPageEvent", oError);
        }
    }

    async function mStampHTMLControlChange(oRow) {
        try {

            oRow.setAttribute("data-dirty", "true");
            mSetStatus("Changes pending to be saved.");

        } catch (oError) {

            mSetStatus("mStampHTMLControlChange", oError);

        }
        
    }
    

    async function mSetSessionValues(sUrl, oArgumentData) {

        try {
            const oResponse = await fetch(sUrl, {
                method: "POST",
                body: oArgumentData
            });

            if (!oResponse.ok) {
                throw new Error(`HTTP error! Status: ${oResponse.status}`);
            }
            const oData = await oResponse.json();
            
            if (oData.error) {
                mSetStatus("mSetSessionValues Data", oData.message);
            }

            return oData; // ? Data returned to caller

        } catch (oError) {
            mSetStatus("mSetSessionValues", oError);
        }
    }

    

    function mBindSearchBoxListener() {
        try {

            let oSearchTextbox = null;

            oSearchTextbox = document.getElementsByName("txtSearchCriteria")[0];
            
            if (oSearchTextbox) {
                
                document.getElementsByName("txtSearchCriteria")[0].addEventListener("keyup", function () {

                    try {

                        let iRowID = -1;
                        let iRowType = -1;

                        const stRowMatch = new Set();
                        const sSearchText = this.value.toLowerCase();
                        const oRows = document.querySelectorAll("#StandardTable tbody tr");

                        oRows.forEach(oRow => {
                            oRow.style.display = "";
                        });

                        if (sSearchText.length > 0) {
                            oRows.forEach(oRow => {


                                const oRowID = oRow.querySelector("input[name='txtGridRowID']");
                                const oRowType = oRow.querySelector("input[name='txtGridRowType']");


                                iRowID = Number(oRowID.value);
                                iRowType = Number(oRowType?.value);
                                if (Number.isNaN(iRowType)) {
                                    iRowType = -1;
                                }

                                if ((iRowType === -1) || (iRowType === cstRowType.Detail)) {

                                    const oInputs = oRow.querySelectorAll("input[type='text']");
                                    oInputs.forEach(oInput => {
                                        if (oInput.value.toLowerCase().includes(sSearchText)) {
                                            stRowMatch.add(iRowID);
                                        }
                                    });

                                    const oSelects = oRow.querySelectorAll("select");
                                    oSelects.forEach(oSelect => {
                                        const displayText = oSelect.options[oSelect.selectedIndex].text.toLowerCase();
                                        if (displayText.includes(sSearchText)) {
                                            stRowMatch.add(iRowID);
                                        }
                                    });

                                    const oMultiSelects = oRow.querySelectorAll(".multi-select");
                                    oMultiSelects.forEach(oMultiSelect => {
                                        const sMultiSelectValues = oMultiSelect.querySelectorAll(".multi-select-header-option");
                                        sMultiSelectValues.forEach(sMultiSelectValue => {
                                            if (sMultiSelectValue.textContent.toLowerCase().includes(sSearchText)) {
                                                stRowMatch.add(iRowID);
                                            }
                                        });
                                    });

                                }
                            });

                            oRows.forEach(oRow => {
                                const iDisplayRowID = Number(oRow.querySelector("input[name='txtGridRowID']").value);

                                if (stRowMatch.has(iDisplayRowID)) {
                                    oRow.style.display = "";
                                } else {
                                    oRow.style.display = "none";
                                }
                            });
                        }


                    } catch (oInsideError) {
                        mSetStatus("mAddSearchListener (keyup): " + oInsideError.message);
                    }
                });
            } else {
                console.log("Search box not found.");
            }
            
        } catch (oError) {
           mSetStatus("mBindSearchBoxListener", oError);
        }
    }

    /* Row Dragging Handlers */
    try {

        let draggingRow = null;
        let oRow = null;
        let isDraggingRow = false;

        const oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);

        if (oHTMLTable) {

            oHTMLTable.addEventListener("change", (event) => {
                const selectedValue = event.target.value;
            });

            oHTMLTable.addEventListener('dragstart', (e) => {
                if (e.target.tagName === 'TR') {
                    draggingRow = e.target;
                    oRow = draggingRow;
                    isDraggingRow = true;
                    e.target.classList.add('dragging');
                }
            });

            oHTMLTable.addEventListener('dragover', (e) => {
                e.preventDefault();
                const afterElement = getDragAfterElement(oHTMLTable, e.clientY, oRow);
                const tbody = oHTMLTable.querySelector('tbody');

                if (afterElement == null) {
                    tbody.appendChild(draggingRow);
                } else {
                    tbody.insertBefore(draggingRow, afterElement);
                }
            });

            // ?? Add auto-scroll here — still inside your function, but on window
            window.addEventListener("dragover", (e) => {
                
                if (!isDraggingRow) return;

                const scrollMargin = 50;
                const scrollSpeed = 75;

                const y = e.clientY;
                const viewportHeight = window.innerHeight;

                if (y < scrollMargin) {
                    window.scrollBy(0, -scrollSpeed);
                }

                if (y > viewportHeight - scrollMargin) {
                    window.scrollBy(0, scrollSpeed);
                }
            });

            oHTMLTable.addEventListener('dragend', (e) => {

                let oHTMLTable = null;

                if (draggingRow) {
                    oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);

                    const oSequenceID = oHTMLTable.tBodies[0].querySelectorAll('[name="txtGridSequenceID"]');

                    let iInt = 1;
                    for (let oInput of oSequenceID) {
                        if (mIsNumeric(oInput.value)) {
                            oInput.value = iInt;
                            iInt++;
                        }
                    }

                    mUpdateSequenceID(draggingRow);
                    draggingRow.classList.remove('dragging');
                    draggingRow = null;
                    isDraggingRow = false;

                    cstBroadcastMessageType.MessageOnly.Message =
                        "Another user has changed the order of the rows in the table.  Please refresh the page to update the row order.";

                    oBroadcastClient.TargetObject = null;
                    oBroadcastClient.RowID = "";
                    oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.MessageOnly.ID);
                    cstBroadcastMessageType.MessageOnly.Message = "";
                }
            });
        }

    } catch (oError) {
        mSetStatus("Row Dragging Handlers", oError);
    }    
    

    function getDragAfterElement(container, y, oRow) {
        try {
            const draggableElements = [...container.querySelectorAll('tr[draggable]:not(.dragging)')];
            const result = draggableElements.reduce((closest, child) => {
                const box = child.getBoundingClientRect();
                const offset = y - box.top - box.height / 2;
                if (offset < 0 && offset > closest.offset) {
                    return { offset: offset, element: child };
                } else {
                    return closest;
                }
            }, { offset: Number.NEGATIVE_INFINITY });

            return result.element;
        } catch (oError) {
           mSetStatus("getDragAfterElement", oError);
        }
    }

     window.onerror = function(sMessage, sSource, iLineNumber, iColumnNumber, oError) {

        if (gsErrorMessage.length === 0) {
            gsErrorMessage = sMessage + "Source: " + sSource + " Line Number: " + iLineNumber + " Column Number: " + iColumnNumber;
        }

        return true; // prevent default console logging if you want
    };

    

    async function mProcessFilterTableEvent(oEvent) {
        try {

            let oClickedControl = null;
            let oDynamicFunction = null;
            let vFunctionName = null;
            let oArrayControl = [];
            
            oClickedControl = oEvent.target;
            const oRow = oClickedControl.closest("tr");
            
            oArrayControl = mGetPageControlFromArray(oClickedControl.name);
            
            if (oArrayControl) {
                //Control is a button
                if (oClickedControl.hasAttribute("FilterControl")) {
                    vFunctionName = "mFilterTable";

                    if (vFunctionName) {
                    
                        oDynamicFunction = new cDynamicFunction();
                            oDynamicFunction[String(vFunctionName)](oClickedControl.name);
                        oDynamicFunction = null;

                    } else {
                        throw new Error("Function Name could not be found.  Check HTML_Control Table for " + oClickedControl.name);
                    }                
                }
            }

        } catch (oError) {
            mSetStatus("mProcessFilterTableEvent", oError);
        }
    }

    async function mInitializePage() {
        try {

            await mBindTableEventListeners();
            await mBindControlEventListeners();

        } catch (oError) {
            mSetStatus("mBroadcastControlChange", oError);
        }        
    }

    document.addEventListener("DOMContentLoaded", async function () {
        try {

            await mInitializePage();
            

        } catch (oError) {
            mSetStatus("document.addEventListener", oError);
        }    
    });

   async function mButtonClick(oClickedButton) {
        try {
            
            let bInsert = true;
            let vFunctionName = null;
            let oDynamicFunction = null;
            let oArrayControl = [];
            
            const oRow = oClickedButton.closest("tr");
            
            oArrayControl = mGetPageControlFromArray(oClickedButton.name);
            
            if (oArrayControl) {
                //Control is a button
                if (oClickedButton instanceof HTMLButtonElement) {
                    
                    vFunctionName = oArrayControl.ProcedureToCall;
                    
                    if (vFunctionName) {
                        
                        oDynamicFunction = new cDynamicFunction();
                            await oDynamicFunction[String(vFunctionName)](oRow);
                        oDynamicFunction = null;

			            gbFirstError = true;
			
                    } else {
                        throw new Error("Function Name could not be found.  Check HTML_Control Table for " + oClickedButton.id);
                    }
                }
            }

        } catch (oError) {
            mSetStatus("mButtonClick", oError);
        }
    }

    async function mBindTableEventListeners() {

        try {

            let oHTMLStandardTable = null;
            let oThead = null;
            let oTbody = null;
            let oClickedControl = null;

            
            oHTMLStandardTable = mGetHTMLTable(cstTableType.StandardTable);

            if (oHTMLStandardTable) {
                oTbody = oHTMLStandardTable.querySelector("tbody");
                oThead = oHTMLStandardTable.querySelector("thead");

                //---------------------------------------------------------------------------------------------
                //Change: Standard Table
                oHTMLStandardTable.addEventListener("change", async function (oEvent) {

                    const oTarget = oEvent.target;

                    // THEAD Change Listener
                    if (oHTMLStandardTable.tHead.contains(oTarget)) {
                        await mProcessFilterTableEvent(oEvent);
                        return;
                    }

                    // TBODY  Change Listener
                    if (oHTMLStandardTable.tBodies[0].contains(oTarget)) {
                        
                        if (oTarget._SuppressChangeEvent !== true) {
                            const oRow = oTarget.closest("tr");
                            if (oRow) {
                                await mStampHTMLControlChange(oRow);
                            }
                        }

                        await mBroadcastControlChange(oEvent);
                        return;
                    }

                }, true);

 

                //Focus Out: Standard table tbody
                //---------------------------------------------------------------------------------------------
                if (!oHTMLStandardTable._FocusOutBound) {

                    oHTMLStandardTable._FocusOutBound = true;

                        oTbody.addEventListener("focusout", async function (oEvent) {
                            
                            let oDynamicFunction = new cDynamicFunction();                            
                            const oControl = oEvent.target;               // control losing focus
                            const oRow = oControl.closest("tr");          // row of the control
                            

                            if (oRow) {
                                const oNewFocus = oEvent.relatedTarget;       // control receiving focus
                                
                                // leaving the row?
                                if ((oNewFocus && !oRow.contains(oNewFocus)) || (!oNewFocus)) {
                                    await oDynamicFunction.mSaveChange(oRow);
                                }
                            }

                            oDynamicFunction = null;

                        }, true);
                }
            }

            //Handle These Events for The Multi-select.  Changes to the multi-select  don't bubble up to Standard Table
            //---------------------------------------------------------------------------------------------            

            document.querySelectorAll('[data-multi-select]').forEach(function (oMultiSelectDataItem) {
                oMultiSelectDataItem.addEventListener("change", function () {
                    mHandleTableChange(this);
                });
            });
            
            oDynamicFunction = null;

        } catch (oError) {
            mSetStatus("mBindTableEventListeners", oError);
        }
    }

    async function mBindControlEventListeners() {

        try {

            //If the controls have been bound exit the procedure
            if (window.__CONTROL_EVENTS_BOUND__) {
                alert("Double Control Bind to Event Detected,");
                return;
            }
            window.__CONTROL_EVENTS_BOUND__ = true;


            //Bind Search Textbox Listener
            mBindSearchBoxListener();

            let oHTMLStandardTable = null;
            let oThead = null;
            let oTbody = null;
            let oDynamicFunction = null;
            let oClickedControl = null;
            
            oHTMLStandardTable = mGetHTMLTable(cstTableType.StandardTable);

            //Handle These Events for Controls Wired Up in mControlClick
            //---------------------------------------------------------------------------------------------
            document.addEventListener("click", async function (oEvent) {

                const oTargetButton = oEvent.target.closest("button, input[type=button], input[type=submit], input[type=reset]");
                
                if (oTargetButton) {
                    await mButtonClick(oTargetButton);
                }
                
            }, true);
            
            //Bind the Filter Table Controls
            //---------------------------------------------------------------------------------------------   

            if (oHTMLStandardTable) {
                oTbody = oHTMLStandardTable.querySelector("tbody");
                oThead = oHTMLStandardTable.querySelector("thead");
                
                    // Handle Filter Textbox in Thead text clear and restore
                     oThead.addEventListener("focusin", function (e) {

                         // Only apply to textboxes
                         const oControl = e.target;
                    
                         if (oControl.matches('input[type="text"]')) {
                             oControl._originalValue = oControl.value;
                             oControl.value = "";
                         }
                     });

                     // Restore on blur
                     oThead.addEventListener("focusout", function (e) {
                         const oControl = e.target;

                         if (oControl.matches('input[type="text"]')) {
                             // Otherwise restore original
                             oControl.value = oControl._originalValue || "";

                             oDynamicFunction = new cDynamicFunction();
                                 oDynamicFunction.mFilterTable(oControl.name);
                             oDynamicFunction = null;
                         }
                     });
            }            

            //Bind Control Test Load Events For HxH Plan
            //---------------------------------------------------------------------------------------------
            document.querySelectorAll('select[name="cboSubSelect"]').forEach(oSelect => {
                    oSelect.addEventListener("change", async function () {
                        try {
                            
                            let iPlatformID = document.getElementsByName("cboPlatform")[0].value || -1;
                            let iYearID = document.getElementsByName("cboYear")[0].value || -1;
                            let iPageID = document.getElementsByName("cboPage")[0].value || -1;
                            let iSubSelectValue = this.value || -1;

                            iPlatformID = Number(iPlatformID);
                            iYearID = Number(iYearID);
                            iPageID = Number(iPageID);
                            iSubSelectValue = Number(iSubSelectValue);

                            if (iPlatformID > 0 && iYearID > 0 && iPageID > 0 && iSubSelectValue > 0) {

                                if (iPageID === dctPage["Hour By Hour Plan"]) {

                                    mHandleTestSelect(iPlatformID, iYearID, iSubSelectValue);

                                } else if (iPageID === dctPage["Plan Template"]) {

                                    mHandleTemplateSelect(iPlatformID, iYearID, iSubSelectValue);

                                }
                                
                                                        
                            } else {
                                    alert("Platform, Year and Page must be chosen to select a Plan.");       
                            }

                        } catch (oError) {
                            mSetStatus("cboSubSelect_change", oError);
                        }
                    });
                });

            
            //---------------------------------------------------------------------------------------------

            
            oHTMLStandardTable = null;
            oThead = null;
            oTbody = null;
            oDynamicFunction = null;
            oClickedControl = null;
            
        } catch (oError) {
            mSetStatus("mBindControlEventListeners", oError);
        }
    }

    async function mBindHxHPlanControlListeners() {

        try {

                document.querySelectorAll('select[name="cboGridTaskStatus"]').forEach(oSelect => {
                    oSelect.addEventListener("change", async function () {
                        try {

                            const oRow = this.closest("tr");
                            if (oRow !== null) {
                                
                                const oRowType = oRow.querySelector('input[name="txtGridRowType"]');
                                
                                if ((oRowType !== null) && (parseInt(oRowType.value) === cstRowType.Detail)) {
                                    if (oRow.parentElement.tagName === "TBODY") {
                                       await mProcessStatusChange(this);
                                       mSetTaskGoStopButtonState();
                                    }
                                }
                            }

                        } catch (oError) {
                            mSetStatus("cboGridTaskStatus_change", oError);
                        }
                    });
                });


		    document.querySelectorAll('#StandardTable tbody.table-body [name="txtGridPlannedDuration"]')
                .forEach(oInput => {
                oInput.addEventListener("blur", async function () {
                    try {

                        await mUpdatePlannedTimes();

                    } catch (oError) {
                        mSetStatus("txtGridPlannedDuration_blur", oError);
                    }
                });
		    });

            document.querySelectorAll('[name="txtGridPredecessor"]').forEach(oInput => {
                oInput.addEventListener("blur", async function () {
                    try {

                        await mUpdatePlannedTimes();

                    } catch (oError) {
                        mSetStatus("txtGridPredecessor_blur", oError);
                    }
                });
            });

            } catch (oError) {
                mSetStatus("mBindHxHPlanControlListeners", oError);
            }                
        }

    async function mBroadcastControlChange(oEvent) {
        try {

            let oTargetControl = null;
            
            oTargetControl = oEvent.target;
            
            oBroadcastClient.TargetObject = oTargetControl;
            oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.ControlUpdate.ID);

        } catch (oError) {
            mSetStatus("mBroadcastControlChange", oError);
        }
    }

    window.addEventListener("pagehide", () => {
        try {

            mTogglePlanTimingLockOnExit(giPageID, true);

        } catch (oError) {
            mSetStatus("window.addEventListener", oError);
        }
    });

            

    /*function mInitializeMultiSelectControls() {
        try {

                const oMultiSelectHTMLInstances = document.querySelectorAll('[data-multi-select]');

                
                oMultiSelectHTMLInstances.forEach(function (oMultiSelectHTMLInstance) {
                    // Prevent double initialization
                    if (!oMultiSelectHTMLInstance._MultiSelectReference) {

                        const oMultiSelectDIVInstance = new MultiSelect(oMultiSelectHTMLInstance, {
                            
                            onChange(value, label, optionElement) {
                                oMultiSelectDIVInstance.element.dispatchEvent(new Event("change", { bubbles: true }));
                            }
                        });
                        

                        // Store instance
                        oMultiSelectHTMLInstance._MultiSelectReference = oMultiSelectDIVInstance;
                    }
                });

        } catch (oError) {
            mSetStatus("mInitializeMultiSelectControls", oError);
        }
    }*/

        /*try {

        let draggingRow = null;
        let oRow = null;

        const oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);
        

        if (oHTMLTable) {
            oHTMLTable.addEventListener("change", (event) => {
                const selectedValue = event.target.value;
                // your logic here
            });


            oHTMLTable.addEventListener('dragstart', (e) => {
                if (e.target.tagName === 'TR') {
                    draggingRow = e.target;
                    e.target.classList.add('dragging');
                    oRow = draggingRow;
                }
            });

            oHTMLTable.addEventListener('dragover', (e) => {
                e.preventDefault();
                const afterElement = getDragAfterElement(oHTMLTable, e.clientY, oRow);
                const tbody = oHTMLTable.querySelector('tbody');
                if (afterElement == null) {
                    tbody.appendChild(draggingRow);
                } else {
                    tbody.insertBefore(draggingRow, afterElement);
                }
            });

            oHTMLTable.addEventListener('dragend', (e) => {
                if (draggingRow) {

                    const oSequenceID = document.getElementsByName("txtGridSequenceID");
                
                    let iInt = 1;
                    for (let oInput of oSequenceID) {
                        if (mIsNumeric(oInput.value)) {
                            oInput.value = iInt;
                            iInt = iInt + 1;
                        }
                    }

                    mUpdateSequenceID(draggingRow);
                    draggingRow.classList.remove('dragging');
                    draggingRow = null;

                    cstBroadcastMessageType.MessageOnly.Message = "Another user has changed the order of the rows in the table.  Please refresh the page to update the row order.";
                    oBroadcastClient.TargetObject = null;
                    oBroadcastClient.RowID = "";
                    oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.MessageOnly.ID);
                    cstBroadcastMessageType.MessageOnly.Message = "";


                }
            });

        }
    } catch (oError) {
        mSetStatus("Row Dragging Handlers", oError);
    }*/

        /*try {

        let draggingRow = null;
        let oRow = null;

        const oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);
        

        if (oHTMLTable) {
            oHTMLTable.addEventListener("change", (event) => {
                const selectedValue = event.target.value;
                // your logic here
            });


            oHTMLTable.addEventListener('dragstart', (e) => {
                if (e.target.tagName === 'TR') {
                    draggingRow = e.target;
                    e.target.classList.add('dragging');
                    oRow = draggingRow;
                }
            });

            oHTMLTable.addEventListener('dragover', (e) => {
                e.preventDefault();
                const afterElement = getDragAfterElement(oHTMLTable, e.clientY, oRow);
                const tbody = oHTMLTable.querySelector('tbody');
                if (afterElement == null) {
                    tbody.appendChild(draggingRow);
                } else {
                    tbody.insertBefore(draggingRow, afterElement);
                }
            });

            oHTMLTable.addEventListener('dragend', (e) => {
                if (draggingRow) {

                    const oSequenceID = document.getElementsByName("txtGridSequenceID");
                
                    let iInt = 1;
                    for (let oInput of oSequenceID) {
                        if (mIsNumeric(oInput.value)) {
                            oInput.value = iInt;
                            iInt = iInt + 1;
                        }
                    }

                    mUpdateSequenceID(draggingRow);
                    draggingRow.classList.remove('dragging');
                    draggingRow = null;

                    cstBroadcastMessageType.MessageOnly.Message = "Another user has changed the order of the rows in the table.  Please refresh the page to update the row order.";
                    oBroadcastClient.TargetObject = null;
                    oBroadcastClient.RowID = "";
                    oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.MessageOnly.ID);
                    cstBroadcastMessageType.MessageOnly.Message = "";


                }
            });

        }
    } catch (oError) {
        mSetStatus("Row Dragging Handlers", oError);
    }*/
    
    
        /*async function mHandleNavigation(iSelectedPlatformID, iSelectedYearID, iSelectedPageID, iCurrentUserID, iCurrentUserTypeID, iSelectedTestID = -1) {

        try {    

            //Set Session Variables From Javascript Variables
            const oArgumentData = new FormData();
            oArgumentData.append("SelectedPlatform", iSelectedPlatformID);
            oArgumentData.append("SelectedYear", iSelectedYearID);
            oArgumentData.append("SelectedPage", iSelectedPageID);
            
            await mSetSessionValues(cstURL.SetSession, oArgumentData);
            
            if ((iSelectedPlatformID !== -1) && (iSelectedYearID !== -1) && (iSelectedPageID !== -1)) {


                if (iSelectedPageID === dctPage["Hour By Hour Plan"]) {
                    window.location.href = "plan.php";
                } else if (iSelectedPageID !== dctPage["Hour By Hour Plan"]) {
                    window.location.href = "main.php";
                }

               
            }

        } catch (oError) {
           mSetStatus("mHandleNavigation", oError);
        }
    }*/