    function mResetTableControls(iTableType) {

        try {

            // Declare variables
            let sContentType = "";
            let iCellIndex = -1;
            let iControlIndex = -1;
            let arrControls = [];
            let oHTMLTable = null;
            let oBodyRows = null;
            let oTargetRow = null;
            let oCell = null;
            let oControl = null;
        
            if (iTableType !== cstTableType.AddTable) {
                throw new Error("mResetTableControls is only designed for us on Add Table.");
            }

            oHTMLTable = mGetHTMLTable(iTableType);

            oBodyRows = oHTMLTable.tBodies[0]?.rows;
            
            oTargetRow = oBodyRows[0]; // Resetting controls in first body row

            for (iCellIndex = 0; iCellIndex < oTargetRow.cells.length; iCellIndex++) {
                
                oCell = oTargetRow.cells[iCellIndex];

                arrControls = oCell.querySelectorAll("input[type='text'], select");

                for (iControlIndex = 0; iControlIndex < arrControls.length; iControlIndex++) {
                    oControl = arrControls[iControlIndex];

                    if (oControl.tagName === "INPUT") {
                        oControl.value = "";
                    }

                    if (oControl.tagName === "SELECT") {
                        oControl.selectedIndex = 0;
                    }
                }
            }

        } catch (oError) {
            mSetStatus("mResetTableControls", oError);
        }
    }

    function mValidateTable(iTableType) {

        // Declare variables
        let sValidationFailMessage = "";
        let arrControlSetting = [];
        let arrValidationErrors = [];
        let oHTMLTable = null;
        let oHTMLControlSetting = null;
        let oRow = null;
        let oControlInstance = null;

        try {
            
            if (iTableType === cstTableType.AddTable) {

                oHTMLTable = mGetHTMLTable(iTableType);
                
                arrControlSetting = mFilterFromArray(arrHTMLControlTable, "RequiredForAdd", 1);
                oRow = oHTMLTable.tBodies[0].rows[0];
                
                arrControlSetting.forEach((arrControlSettingRow, iIndexControlCounter) => {
                    
                    oHTMLControlSetting = mGetHTMLControlSettingObject(arrControlSettingRow);

                    oControlInstance = oHTMLControlSetting.mGetControlInstance(oRow);

                    if (oHTMLControlSetting.mIsControlRequired(iTableType)) {
                        if (oHTMLControlSetting.mIsControlDataValid(oControlInstance, cstValidationType.DataRequired) === false) {
                            sValidationFailMessage = oHTMLControlSetting.ControlLabel + " is a required field.";
                            arrValidationErrors.push(sValidationFailMessage);
                        }
                    }

                });

            }

            if (arrValidationErrors.length > 0) {
                sValidationFailMessage = arrValidationErrors.join("\n");
                alert(sValidationFailMessage);
                return false;
            }

        } catch (oError) {
            mSetStatus("mValidateTable", oError);
        }

        return true;
    }


    async function mCheckHTMLControlForMatch(oHTMLTable, sControlName, iRowID) {
        try {

            let bMatchFound = false;

            const oControls = oHTMLTable.querySelectorAll(`[name="${sControlName}"]`);

            for (const oControl of oControls) {
                if (Number(oControl.value) === Number(iRowID)) {
                    bMatchFound = true;
                    break;
                }
            }

            return(bMatchFound);

        } catch (oError) {
            mSetStatus("mValidateTable", oError);
        }
    }

    function mFindRowFromElement(oElement) {

        try {
            // Native table rows
            let oRow = oElement.closest("tr");
            if (oRow) return oRow;

            // If the control is inside a cell but the TR is higher
            const oCell = oElement.closest("td, th");
            if (oCell) {
                oRow = oCell.closest("tr");
                if (oRow) return oRow;
            }

            // Common grid fallbacks (div-based tables)
            oRow = oElement.closest('[role="row"], .grid-row, .ag-row, .k-grid tr');
            if (oRow) return oRow;

            // Final attempt: walk up manually
            let n = oElement.parentElement;
            while (n) {
                if (n.tagName === "TR") return n;
                n = n.parentElement;
            }

            return null;
            
        } catch (oError) {
            mSetStatus("mFindRowFromElement", oError);
        }        
    }

     
    function mDisableButtons() {

        try {
            
            
            let oHTMLTable = null;
            let oButton = null;

            //Disable the Bulk Save Button based on User Type ID
          
            oButton = document.getElementsByName("btnSaveAll")[0];


            if ((oButton) && (giUserTypeID === 0)){
                oButton.disabled = true;
            }

            //Disable the Insert All Document Button Based on a row or more being in the standard table.
            
            if (giPageID === dctPage["Documentation"]) {
                oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);
                const oTBody = oHTMLTable.tBodies[0];
                const iRowCount = oTBody ? oTBody.rows.length : 0;
                if (oHTMLTable && iRowCount > 0) {
                    oButton = document.getElementsByName("btnInsertAllDocumentation")[0];
                    oButton.disabled = true;
                }
            }

        } catch (oError) {
            mSetStatus("mDisableButtons", oError);
        }
    }


    
    function mGetPageControlFromArray(sControlName) {
        
        try {
            let arrControl = [];
            
            arrControl = arrHTMLControlTable.find(arrRow => (arrRow.Platform_ID === 0 || arrRow.Platform_ID === giPlatformID) && (arrRow.Page_ID === 0 || arrRow.Page_ID === giPageID) && arrRow.ControlName === sControlName);

            return(arrControl);

        } catch (oError) {
            mSetStatus("mGetPageControlFromArray", oError);
        }            
    }

     
    function mGetControlIndex(oTargetControl) {
        try {

            let iControlIndex = -1;
            let arrControls = [];

            
            if (oTargetControl instanceof HTMLInputElement || oTargetControl instanceof HTMLSelectElement || oTargetControl instanceof HTMLTextAreaElement) {
                arrControls = document.getElementsByName(oTargetControl.name);    
            } else if (oTargetControl.classList.contains("multi-select")) {
                arrControls = document.getElementsByName(oTargetControl.getAttribute("name"));                
            }

            iControlIndex = Array.prototype.indexOf.call(arrControls, oTargetControl);

            return (iControlIndex);
            
        } catch (oError) {
            mSetStatus("mGetControlIndex", oError);
        }              
    }

    function mGetMultiSelectValues(oMultiSelectControl, bTextValue = false) {
	try {

	    let vControlValue = "";
	    
	    if (oMultiSelectControl && oMultiSelectControl.classList.contains("multi-select")) {
		
		const oSelectedOptions = oMultiSelectControl.querySelectorAll(".multi-select-option.multi-select-selected[data-value]");

		oSelectedOptions.forEach((oSelectedOption, iIndex) => {

		    let sValue = "";

		    if (bTextValue === false) {
			// Return the numeric/string ID
			sValue = oSelectedOption.getAttribute("data-value");
		    } else if (bTextValue === true) {
			// Return the visible text
			
			const oText = oSelectedOption.querySelector(".multi-select-option-text");
			sValue = oText ? oText.textContent.trim() : "";
		    }

		    vControlValue += sValue;

		    if (iIndex < oSelectedOptions.length - 1) {
			vControlValue += ",";
		    }
		});
	    }

	    return vControlValue;

	} catch (oError) {
	    mSetStatus("mGetMultiSelectValues", oError);
	}
    }

    function mSetMultiSelectValues(oMultiSelectControl, sSelectedValueList) {
        try {

            let bComplete = false;

            const arrValues = sSelectedValueList.split(",");

            // 1. Clear any existing selections first (safety)
            mClearMultiSelectValues(oMultiSelectControl);

            // 2. Loop through all dropdown options
            const oAllOptions = oMultiSelectControl.querySelectorAll(".multi-select-option[data-value]");

            oAllOptions.forEach(oOption => {
                const sValue = oOption.dataset.value;

                if (arrValues.includes(sValue)) {

                    // Mark as selected
                    oOption.classList.add("multi-select-selected");

                    // Add hidden input
                    oMultiSelectControl.insertAdjacentHTML("afterbegin", `<input type="hidden" name="${oMultiSelectControl.getAttribute("name")}[]" value="${sValue}">`);

                    // Add visible header chip
                    const oHeader = oMultiSelectControl.querySelector(".multi-select-header");
                    const sText = oOption.querySelector(".multi-select-option-text").innerHTML;

                    oHeader.insertAdjacentHTML("afterbegin",`<span class="multi-select-header-option" data-value="${sValue}">${sText}</span>`);
                }
            });

            
            // 3. Remove placeholder if any selections exist
            const oHeader = oMultiSelectControl.querySelector(".multi-select-header");
            const oPlaceholder = oHeader.querySelector(".multi-select-header-placeholder");

            if (oPlaceholder && arrValues.length > 0) {
                oPlaceholder.remove();
            }

            
            // 4. Update max counter if present
            const oMax = oHeader.querySelector(".multi-select-header-max");
            if (oMax && oMax.innerText.length > 0) {
                const sMax = oMax.innerText.split("/")[1]; // right side of "0/5"
                oMax.innerText = `${arrValues.length}/${sMax}`;
            }

            bComplete = true;
            return bComplete;

        } catch (oError) {
            mSetStatus("mSetMultiSelectValues", oError);
        }
    }

    function mClearMultiSelectValues(oMultiSelectControl) {
        try {

            let bComplete = false;

            // 1. Remove selected state from dropdown options
            const oDropDownSelectedOptions = oMultiSelectControl.querySelectorAll(".multi-select-option.multi-select-selected[data-value]");

            oDropDownSelectedOptions.forEach(oDropDownOption => {
                oDropDownOption.classList.remove("multi-select-selected");
            });


            // 2. Remove hidden input fields
            const oHiddenInputs = oMultiSelectControl.querySelectorAll('input[type="hidden"][name$="[]"]');

            oHiddenInputs.forEach(oInput => oInput.remove());


            // 3. Clear visible header chips
            const oHeader = oMultiSelectControl.querySelector(".multi-select-header");

            if (oHeader) {
                oHeader.querySelectorAll(".multi-select-header-option").forEach(oChip => oChip.remove());

                // 4. Restore placeholder if missing
                if (!oHeader.querySelector(".multi-select-header-placeholder")) {
                    const sPlaceholder = oMultiSelectControl.querySelector(".multi-select-header-placeholder")?.innerText || oMultiSelectControl.multiSelectInstance?.placeholder || "Select item(s)";

                    oHeader.insertAdjacentHTML("afterbegin", `<span class="multi-select-header-placeholder">${sPlaceholder}</span>`);
                }

                // 5. Reset max counter if present
                const oMax = oHeader.querySelector(".multi-select-header-max");
                if (oMax && oMax.innerText.length > 0) {
                    const sMax = oMax.innerText.split("/")[1]; // right side of "0/5"
                    oMax.innerText = sMax ? `0/${sMax}` : "";
                }
            }

            bComplete = true;
            return bComplete;

        } catch (oError) {
            mSetStatus("mClearMultiSelectValues", oError);
        }
    }

    function mIsValueSelectedInMultiSelect(oMultiSelectControl, iUserID, iUserTypeID) {
        try {

            let oSelectedOptions = null;
            let bReturnBoolean = false;

            //If the user type ID is return than the standard user type id, then always return true
            if (iUserTypeID > cstUserType.Standard) {
                bReturnBoolean = true;
            } else {
                oSelectedOptions = oMultiSelectControl.querySelectorAll(".multi-select-option.multi-select-selected[data-value]");

                oSelectedOptions.forEach((oSelectedOption, iIndexOfOptions) => {
                    const vSelectedUserID = oSelectedOption.getAttribute("data-value");

                    if (mIsNumeric(vSelectedUserID)) {
                        if (Number(iUserID) === Number(vSelectedUserID)) {
                            bReturnBoolean = true;
                        }
                    } else {
                        throw new Error("mIsValueSelectedInMultiSelect only supports numeric input values for the multiselect drop down values");
                    }
                });
            }

            return(bReturnBoolean);

        } catch (oError) {
            mSetStatus("mIsValueSelectedInMultiSelect", oError);
            return(false);
        }        
    }



    function mToggleCellControlDisplay() {
        try {

            let sCurrentStateDisplayValue = "";
            let sFutureStateDisplayValue = "";
            let iRowTypeID = -1;
            let oRowType = null;
            let oHTMLTable = null;
            let arrHTMLTableRows = [];
            
            oHTMLTable = mGetHTMLTable(cstTableType.StandardTable)

            arrHTMLTableRows = oHTMLTable.rows;   
                    
            for (let iRowIndex = 0; iRowIndex < arrHTMLTableRows.length; iRowIndex++) {
                    oHTMLTableRow = arrHTMLTableRows[iRowIndex];

                    oHTMLTableRow.querySelectorAll("td, th").forEach(oHTMLTableCell => {

                        const oControl = oHTMLTableCell.querySelector("input, select, textarea, button");

                        if (oControl && oControl.type !== "hidden") {
                            //For the Table Cell, Flip the Display Value Regardless of Table Row Type
                            if (oHTMLTableCell.VisibilityState === cstControlSetting.Cell_Invisible) {
                                sCurrentStateDisplayValue = oHTMLTableCell.style.display;
                                sFutureStateDisplayValue = mToggleValue(sCurrentStateDisplayValue, "none", "");
                                oHTMLTableCell.style.display = sFutureStateDisplayValue;
                            }
                        
                            //For the Control in The Table Cell, Flip the Display Value If It the Visibility Flag is Set to Invisible and its a Detail Row
                            if (oHTMLTableCell.tagName !== "TH" && oControl.VisibilityState === cstControlSetting.Control_Invisible) {
                                oRowType = oHTMLTableRow.querySelector('[name="txtGridRowType"]');
                                iRowTypeID = Number(oRowType.value);
                                if (iRowTypeID && iRowTypeID === cstRowType.Detail) {
                                    sCurrentStateDisplayValue = oControl.style.display;
                                    sFutureStateDisplayValue = mToggleValue(sCurrentStateDisplayValue, "none", "");
                                    oControl.style.display = sFutureStateDisplayValue;
                                }
                            }
                        }
                    });

            }

        } catch (oError) {
           mSetStatus("mToggleCellControlDisplay", oError);
        }    
    }



    function mGetMultiSelectText(oMultiSelectControl) {
        try {

            let arrSelectedID = [];
            let arrSelectedTextValue = [];

            arrSelectedID = oMultiSelectControl.querySelectorAll(".multi-select-option.multi-select-selected .multi-select-option-text");

            arrSelectedID.forEach(arrItem => arrSelectedTextValue.push(arrItem.textContent.trim()));

            return arrSelectedTextValue;   // returns array of selected text labels

        } catch (oError) {
            mSetStatus("mGetMultiSelectText", oError);
        }         
    }

    async function mLoadDropDownFromDB(oDropDown, iGetRecordsetAction, oArgumentData, sIDField, sDisplayField) {
        try {

            //Get Recordset
            let bEmptyDropDown = false;


            const arrDropDownValues = await mGetRecordset(cstURL.GetRecordset, oArgumentData);
            bEmptyDropDown = !Array.isArray(arrDropDownValues) || arrDropDownValues.length === 0;

            //Clear Drop Down Options
            while (oDropDown.options.length > 0) {
                oDropDown.remove(0);
            }
    
            if (Array.isArray(arrDropDownValues)) {
                //Add Blank Option
                const oBlankOption = document.createElement("option");
                oBlankOption.value = "";
                oBlankOption.textContent = "";   // visible blank
                oDropDown.appendChild(oBlankOption);

                //Populate options from DB
                arrDropDownValues.forEach(oDropDownValue => {
                    const oOption = document.createElement("option");
                    oOption.value = oDropDownValue[sIDField];
                    oOption.textContent = oDropDownValue[sDisplayField];
                    oDropDown.appendChild(oOption);
                });
            }

            // Optional: always select the blank option
            oDropDown.selectedIndex = 0;

            return(bEmptyDropDown);

        } catch (oError) {
            mSetStatus("mLoadDropDownFromDB", oError);
            return(true);
        }
    }

    async function mLoadDropDownFromArray(oDropDown, arrArrayForLoad, sIDField, sDisplayField) {
        try {

            //Get Recordset
            let bEmptyDropDown = false;

            
            bEmptyDropDown = !Array.isArray(arrArrayForLoad) || arrArrayForLoad.length === 0;

            //Clear Drop Down Options
            while (oDropDown.options.length > 0) {
                oDropDown.remove(0);
            }
    
            if (Array.isArray(arrArrayForLoad)) {
                //Add Blank Option
                const oBlankOption = document.createElement("option");
                oBlankOption.value = "";
                oBlankOption.textContent = "";   // visible blank
                oDropDown.appendChild(oBlankOption);

                //Populate options from DB
                arrArrayForLoad.forEach(oDropDownValue => {
                    const oOption = document.createElement("option");
                    oOption.value = oDropDownValue[sIDField];
                    oOption.textContent = oDropDownValue[sDisplayField];
                    oDropDown.appendChild(oOption);
                });
            }

            // Optional: always select the blank option
            oDropDown.selectedIndex = 0;

            return(bEmptyDropDown);

        } catch (oError) {
            mSetStatus("mLoadDropDownFromArray", oError);
            return(true);
        }
    }

    function mToggleFieldSetLock(sFieldSetName, oHTMLTable, bLock) {
        try {

            let sMultiSelectPointerEvent = "";
            let sDivBackgroundColor = "";
            
            //Set FieldsetLock Value
            document.getElementById(sFieldSetName).disabled = bLock;

            //Set MultiSelect Lock Value
            if (oHTMLTable) {

                if (bLock === true) {
                    sMultiSelectPointerEvent = "none";
                    sDivBackgroundColor = cstHexColorCode.SuperLightGrey;
                } else {
                    sMultiSelectPointerEvent = "";
                    sDivBackgroundColor = cstHexColorCode.White;
                }

                oHTMLTable.querySelectorAll(".multi-select")
                    .forEach(oElem => {
                        oElem.style.pointerEvents = sMultiSelectPointerEvent;
                        oElem.style.backgroundColor = sDivBackgroundColor;
                    });

            }


        } catch (oError) {
            mSetStatus("mToggleFieldsetLock", oError);
        }        
    }    


    function mToggleEnablementRowControls(oRow, bDisable) {

        try {

            let sControlSetting = "";
            let sBackgroundColor = "";
            let sMultiSelectPointerEvent = "";
            let oHTMLControl = null;
            let arrHTMLControl = [];

            if (oRow) {

               if (bDisable === true) {
                    sMultiSelectPointerEvent = "none";
                    sBackgroundColor = cstHexColorCode.SuperLightGrey;
                } else {
                    sMultiSelectPointerEvent = "";
                    sBackgroundColor = cstHexColorCode.White;
                }

                for (let iArrayRowIndex = 0; iArrayRowIndex < arrHTMLNoButtonControlTable.length; iArrayRowIndex++) {

                    oHTMLControl = mGetHTMLElement(cstGetHTMLElementType.Name, arrHTMLNoButtonControlTable[iArrayRowIndex].ControlName, oRow);                    

                    if (arrHTMLNoButtonControlTable[iArrayRowIndex].ControlType !== cstControlType.MultiSelectDropDown) {
                        

                        sControlSetting = mGetStandardTableControlSetting(arrHTMLNoButtonControlTable[iArrayRowIndex].ControlConfig);

                        if ((mIsNumeric(sControlSetting) === true) && (Number(sControlSetting) !== cstControlSetting.Control_Read_Only_Visible)) { 
                            oHTMLControl.disabled = bDisable;
                            oHTMLControl.style.backgroundColor = sBackgroundColor;
                        }

                    } else if (arrHTMLNoButtonControlTable[iArrayRowIndex].ControlType === cstControlType.MultiSelectDropDown) {

                       if (bDisable === true) {
                            oHTMLControl.classList.add("multi-select-disabled");
                        } else if (bDisable === false) {
                            oHTMLControl.classList.remove("multi-select-disabled");
                        }

                        oHTMLControl.style.backgroundColor = sBackgroundColor;     

                    }
                    
                }


                /*const oControls = oRow.querySelectorAll("input, select, textarea, button");
                oControls.forEach(oControl => {
                    if (oControl.name !== "cboGridTaskStatus") {
                        arrHTMLControl = mFilterFromArray(arrHTMLControlTable, "Page_ID", dctPage["Hour By Hour Plan"],"ControlName", oControl.name)

                        sControlSetting = mGetStandardTableControlSetting(arrHTMLControl[0].ControlConfig);

                        if ((mIsNumeric(sControlSetting) === true) && (Number(sControlSetting) !== cstControlSetting.Control_Read_Only_Visible)) { 
                            oControl.disabled = bDisable;
                            oControl.style.backgroundColor = sBackgroundColor;
                        }
                    }
                });

                
                const oMultiSelects = oRow.querySelectorAll(".multi-select");
                oMultiSelects.forEach(oMultiSelect => {

                        if (bDisable === true) {
                            oMultiSelect.classList.add("multi-select-disabled");
                        } else if (bDisable === false) {
                            oMultiSelect.classList.remove("multi-select-disabled");
                        }

                        oMultiSelect.style.backgroundColor = sBackgroundColor;
                });*/

            }

        } catch (oError) {
            mSetStatus("mToggleEnablementRowControls", oError);
        }         
    }

    function mGetInputControlValueByControlName(oRow, sControlName, iDataType) {
        try {

            let vReturnValue = null;
            let oControlReference = null;
            
            oControlReference = oRow.querySelector("input[name='" + sControlName + "']");

            if (oControlReference) {

                vReturnValue = oControlReference?.value;
                
                if (iDataType === cstDataType.String) {
                    vReturnValue = String(vReturnValue);
                } if (iDataType === cstDataType.Number) {
                    vReturnValue = Number(vReturnValue);
                } if (iDataType === cstDataType.DataTime) {
                    throw new Error("Not yet implemented.");
                } if (iDataType === cstDataType.Boolean) {
                    throw new Error("Not yet implemented.");
                }

            }

            return(vReturnValue);

        } catch (oError) {
            mSetStatus("mGetInputControlValueByControlName", oError);
        }   
    }

    async function mGetDivContentAsString(oDiv) {
        try {

            let sReturnValue = "";

            for (const oNode of oDiv.childNodes) {

                // Handle Artifact Label Text
                if (oNode.nodeType === Node.ELEMENT_NODE && oNode.tagName === "SPAN") {

                    if (oNode.className === "clsArtifactHeaderLabel") {
                        const sTextValue = oNode.textContent.trim();
                        if (sTextValue.length > 0) {
                            sReturnValue += sTextValue + "|||||";
                        }
                    }
                }


                // IMAGE
                else if (oNode.nodeType === Node.ELEMENT_NODE && oNode.tagName === "IMG") {
                        
                        const oBlob = await fetch(oNode.src).then(r => r.blob());
                        const sBase64Value = await mBlobToBase64(oBlob);

                        sReturnValue = sReturnValue + sBase64Value + "|||||";
                }
            }
            
            return sReturnValue;


        } catch (oError) {
            alert("mGetDivContentAsString " + oError.message);
        }                
    }
