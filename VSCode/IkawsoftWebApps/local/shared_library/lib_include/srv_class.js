    const cstElementType = {
        Row: 1,
        Cell: 2,
    };

    const cstValidationType = {
        DataRequired: 1,
    };

    const cstBroadcastMessageType = {
        ControlUpdate: {
            ID: 1,
            Message: ""
        },
        TableRowInsert: {
            ID: 2,
            Message: "A new record has been added by another user. The record has been automatically added to your table."
        },
        TableRowDelete: {
            ID: 3,
            Message: "A record has been deleted by another user. The record has been removed from your table."
        },
        MessageOnly: {
            ID: 4,
            Message: ""
        },
        PlatformMessage: {
            ID: 5,
            Message: ""
        },
        GlobalMessage: {
            ID: 6,
            Message: ""
        },
    };

    // Add numeric keys that reference the same objects
    cstBroadcastMessageType[1] = cstBroadcastMessageType.ControlUpdate;
    cstBroadcastMessageType[2] = cstBroadcastMessageType.TableRowInsert;
    cstBroadcastMessageType[3] = cstBroadcastMessageType.TableRowDelete;
    cstBroadcastMessageType[4] = cstBroadcastMessageType.MessageOnly;
    cstBroadcastMessageType[5] = cstBroadcastMessageType.PlatformMessage;
    cstBroadcastMessageType[6] = cstBroadcastMessageType.GlobalMessage;



    // Define allowed control types as an enumeration-like object

    class cControl {

        // Control Properties
        set Row_ID(iRowID) {
            this._Row_ID = Number(iRowID);
        }

        get Row_ID() {
            return this._Row_ID;
        }

        set Page_ID(iPageID) {
            this._Page_ID = Number(iPageID);
        }

        get Page_ID() {
            return this._Page_ID;
        }

        set ControlType(iControlType) {
            if (!Object.values(cstControlType).includes(Number(iControlType))) {
                console.error("cControl:set ControlType: Attempting to set an invalid control type: " . iControlType.toString);
            }

            this._ControlType = Number(iControlType);
        }

        get ControlType() {
            return this._ControlType;
        }


        set ControlName(sControlName) {
            this._ControlName = sControlName;
            this._ControlID = sControlName; // ControlID = ControlName when set
        }

        get ControlName() {
            return this._ControlName;
        }

        set ControlID(sControlID) {
            this._ControlID = sControlID;
        }

        get ControlID() {
            return this._ControlID;
        }

        set ControlLabel(sControlLabel) {
            this._ControlLabel = sControlLabel;
        }

        get ControlLabel() {
            return this._ControlLabel;
        }

        set PageExceptionList(sPageExceptionList) {

            if (!sPageExceptionList) {
                sPageExceptionList = "";
            }
            this._PageExceptionList = sPageExceptionList;
        }

        get PageExceptionList() {
            return this._PageExceptionList;
        }        

        set DBFieldName(sDBFieldName) {
            this._DBFieldName = sDBFieldName;
        }

        get DBFieldName() {
            return this._DBFieldName;
        }

        set CellWidth(sCellWidth) {
            //this._CellWidth = "width:" + sCellWidth +";";
            this._CellWidth = sCellWidth;
        }

        get CellWidth() {
            return this._CellWidth;
        }

        set CellAlignment(iCellAlignment) {
            let sCellAlignment = "";

            sCellAlignment = cstAlignInText[iCellAlignment];

            this._CellAlignment = sCellAlignment;
        }

        get CellAlignment() {
            return this._CellAlignment;
        }

        set ControlWidth(sControlWidth) {
            //this._ControlWidth = "width:" + sControlWidth +";";
            this._ControlWidth = sControlWidth;
        }

        get ControlWidth() {
            return this._ControlWidth;
        }

        set ControlAlignment(iControlAlignment) {
            
            let sControlAlignment = "";

            sControlAlignment = cstAlignInText[iControlAlignment];

            this._ControlAlignment = sControlAlignment;            
        }

        get ControlAlignment() {
            return this._ControlAlignment;
        }

        set CssClass(sCssClass) {
            this._CssClass = sCssClass;
        }

        get CssClass() {
            return this._CssClass;
        }

        set DefaultValue(vDefaultValue) {
            this._DefaultValue = vDefaultValue;
        }

        get DefaultValue() {
            return this._DefaultValue;
        }

        set ControlConfig(sControlConfig) {
            this._ControlConfig = sControlConfig;
        }

        get ControlConfig() {
            return this._ControlConfig;
        }    

        set Required(vBoolRequired) {
           this._Required = this.mConvertBitToBoolean(vBoolRequired);            
        }

        get Required() {
            return this._Required;
        }

        set RequiredForAdd(vBoolRequiredForAdd) {
            this._RequiredForAdd = this.mConvertBitToBoolean(vBoolRequiredForAdd);        
        }

        get RequiredForAdd() {
            return this._RequiredForAdd;
        }

        set LookupIDField(sLookupIDField) {
            this._LookupIDField = sLookupIDField;
        }

        get LookupIDField() {
            return (this._LookupIDField === null || this._LookupIDField === undefined || this._LookupIDField === "") ? "Lookup_ID": this._LookupIDField;
        }

        set LookupDisplayField(sLookupDisplayField) {
            this._LookupDisplayField = sLookupDisplayField;
        }

        get LookupDisplayField() {
            return (this._LookupDisplayField === null || this._LookupDisplayField === undefined || this._LookupDisplayField === "") ? "Lookup_Desc": this._LookupDisplayField;
        }

        set LookupCategory(sLookupCategory) {
            this._LookupCategory = sLookupCategory;
        }

        get LookupCategory() {
            return this._LookupCategory;
        }

        set LookupArray(arrLookupTable) {
            this._LookupArray = arrLookupTable;
        }

        get LookupArray() {
            return this._LookupArray;
        }

        set ProcedureToCall(sProcedureToCall) {
            this._ProcedureToCall = sProcedureToCall;
        }

        get ProcedureToCall() {
            return this._ProcedureToCall;
        }

        set PrimaryKey(vBoolPrimaryKey) {
            this._PrimaryKey = this.mConvertBitToBoolean(vBoolPrimaryKey);
        }

        get PrimaryKey() {
            return this._PrimaryKey;
        }

        //Control Methods
        constructor() {
            try {

                
            } catch (oError) {
                mSetStatus("cControl: Constructor", oError);
            }                 
        }
        
        mGetControlSetting(iTableType, iRowType, sControlConfigArg = '') {
            try {
                
                let sControlConfig = "";
                let iControlSetting = -1;
                let iTableTypeIndexInString = -1;
                let iRowTypeIndexInString = -1;
              
                
                if (sControlConfigArg.length !== 0) {
                    sControlConfig = sControlConfigArg;
                } else {
                    sControlConfig = this.ControlConfig;
                }

                //This is done to set where you find the table type in the config string
                if (iTableType === cstTableType.AddTable) {
                    iTableTypeIndexInString = 0;
                } else if (iTableType === cstTableType.StandardTable) {
                    iTableTypeIndexInString = 1;
                } else if (iTableType === cstTableType.ToolbarTable) {
                    iTableTypeIndexInString = 2;                    
                } else {
                    throw new Error("Only Add Table, Standard Table and currently supported in ControlConfigString");
                }

                //This is done to set where you find the row type in the config string
                if (iRowType === cstRowType.ColumnHeader) {
                    iRowTypeIndexInString = 0;
                } else if (iRowType === cstRowType.Header) {
                    iRowTypeIndexInString = 1;
                } else if (iRowType === cstRowType.Detail) {
                    iRowTypeIndexInString = 2;
                } else {
                    throw new Error("Only ColumnHeader, Header and Detail currently supported in ControlConfigString");
                }
                
                const arrTableSettings = sControlConfig.split("|");

                const sTableSegmentByTableType = arrTableSettings[iTableTypeIndexInString];
                if (!sTableSegmentByTableType) {
                    return Number(iControlSetting);
                }

                const arrTableRowControlSettings = sTableSegmentByTableType.split(":");

                const sTableRowControlSetting = arrTableRowControlSettings[iRowTypeIndexInString];
                if (!sTableRowControlSetting) {
                    return Number(iControlSetting);
                }

                // Control setting is the 3rd digit
                iControlSetting = sTableRowControlSetting[2]
      
                return Number(iControlSetting);

            } catch (oError) {
                mSetStatus("cControl: mGetControlSetting", oError);
            }                 
        }

        mGetControlInstance(oRow) {
            try {

                let oControlInstance = null;

                if (this.ControlType !== cstControlType.MultiSelectDropDown) {

                    oControlInstance = oRow.querySelector(`[name="${this.ControlName}"]`);
                    
                } else if (this.ControlType === cstControlType.MultiSelectDropDown) {
                    
                    oControlInstance = oRow.querySelector(`[name="${this.ControlName}"]`);
                }

                return(oControlInstance);

            } catch (oError) {
                mSetStatus("cControl: mGetControlInstance", oError);
            }       
        }

        mIsControlRequired(iTableType) {
            try {
                
                if ((iTableType = cstTableType.AddTable) && (this.ControlType !== cstControlType.Button)){
                    return(this.RequiredForAdd);
                } else {
                    return(this.Required);
                }

                
            } catch (oError) {
                mSetStatus("cControl: mIsControlRequired", oError);
            }    
        }

        mIsControlDataValid(oControlInstance, iValidationType) {
            try {

                let bValidControl = false;

                if (iValidationType === cstValidationType.DataRequired) {

                    //Most Controls
                    if ((this.ControlType !== cstControlType.DropDown) && (this.ControlType !== cstControlType.MultiSelectDropDown)) {

                            if (oControlInstance.value.trim() != "") {
                                bValidControl = true;
                            }

                    //Drop Down
                    } else if (this.ControlType === cstControlType.DropDown) {

                            if (oControlInstance.selectedIndex !== 0) {
                                bValidControl = true;
                            }
                    
                    //Multiselect 
                    } else if (this.ControlType === cstControlType.MultiSelectDropDown) {
                                
                            const oSelectedOptions = oControlInstance.querySelectorAll(".multi-select-option.multi-select-selected[data-value]");

                            if (oSelectedOptions.length !== 0) {
                                bValidControl = true;
                            }

                    }

                    return(bValidControl);
                }
                
            } catch (oError) {
                mSetStatus("cControl: mIsControlDataValid", oError);
            }               
        }

        mGetControlValue() {
            try {

                alert("cControl: mGetControlValue: Not Implemented");

            } catch (oError) {
                mSetStatus("cControl: mGetControlValue", oError);
            }               
        }

        mGetControlFieldName() {
            try {

                return(this.DBFieldName);

            } catch (oError) {
                mSetStatus("cControl: mGetControlFieldName", oError);
            }               
        }

        mIsControlPrimaryKey() {
            try {

                return(this.PrimaryKey);
                
            } catch (oError) {
                mSetStatus("cControl: mIsControlPrimaryKey", oError);
            }               
        }

        mConvertBitToBoolean(iBit) {
            try {

                let bReturnBoolean = null;

                if (iBit === null) {
                    bReturnBoolean = false;
                } else {

                    iBit = Number(iBit);

                    if (iBit === 0) {
                        bReturnBoolean = false;
                    } else if (iBit === 1) {
                        bReturnBoolean = true;
                    } else {
                        throw new Error("mConvertBitToBoolean: iBit must be 0 or 1");
                    }
                }

                bReturnBoolean = Boolean(bReturnBoolean);

                return (bReturnBoolean);

            } catch (oError) {
                mSetStatus("cControl: mConvertBitToBoolean", oError);
            }              
        }

    }

 
  class cDOMManagement {

        set TableType (iTableType) {
            this._TableType = iTableType;
        }

        get TableType() {
            return this._TableType;
        }        

        set RowType (iRowType) {
            this._RowType = iRowType;
        }

        get RowType() {
            return this._RowType;
        }         

        set ControlSetting (iControlSetting) {
            this._iControlSetting = iControlSetting;
        }

        get ControlSetting() {
            return this._iControlSetting;
        }    

        set UserTypeID(iUserTypeID) {
            this._UserTypeID = iUserTypeID;
        }

        get UserTypeID() {

            let iDefaultUserTypeID = -1;

            if (mIsNumeric(this._UserTypeID)) {
                iDefaultUserTypeID = this._UserTypeID;
            } else {
                iDefaultUserTypeID = 0;
                mSetStatus("cDOMManagement.get UserTypeID", "Default User Type ID of 0 was used.");
            }

            return iDefaultUserTypeID;
        }        

        set TableReference(oHTMLTable) {
            this._TableReference = oHTMLTable;
        }

        get TableReference() {
            return this._TableReference;
        }        

        get HTMLTableID() {
            return this._HTMLTableID;
        }


        set TableSection(oTableSection) {
            
            this._TableSection = oTableSection;

        }

        get TableSection() {
            return this._TableSection;
        }

        set TableRow(oTableRow) {
            
            this._TableRow = oTableRow;

        }

        get TableRow() {
            return this._TableRow;
        }

        set TableCell(oTableCell) {
            
            this._TableCell = oTableCell;

        }

        get TableCell() {
            return this._TableCell;
        }


        set PageRowData(arrPageDataRow) {
            
            if (Array.isArray(arrPageDataRow)) {
                this._PageRowData = arrPageDataRow[0] ?? {};
            } else {
                this._PageRowData = arrPageDataRow ?? {};
            }
        }

        get PageRowData() {
            return this._PageRowData;
        }        

        set ControlClass(oControlClass) {
            
            this._ControlClass = oControlClass;

            this.ControlSetting = this.ControlClass.mGetControlSetting(this.TableType, this.RowType);

        }

        get ControlClass() {

            if (this._ControlClass) {
                return this._ControlClass;
            } else {
                alert("cDOMManagement:ControlClass: Control Class Property must be set");
                return;
            }
        }        

        constructor(oHTMLTable, iTableType, iRowType) {

            try {

                if (!oHTMLTable) {
                    throw new Error("oHTMLTable is required");
                }

                if (Number.isNaN(iTableType)) {
                    throw new Error("iTableType is Nan");
                }

                if (Number.isNaN(iRowType)) {
                    throw new Error("iRowType is Nan");
                }

                this.TableReference = oHTMLTable; 
                this.TableType = iTableType;
                this.RowType = iRowType;
                

            } catch (oError) {
                mSetStatus("oDOMManagement: Constructor", oError);
            }              
        }

        async mCreateTableRow(iUserType = 0) {
            try {

                let oHTMLTableSection = null;
                let oHTMLTableRow = null;
                let oHTMLTableCell = null;

                if (this.RowType === cstRowType.ColumnHeader) {

                    oHTMLTableSection = this.TableReference.querySelector("thead");

                 } else if (this.RowType === cstRowType.Header || this.RowType === cstRowType.Detail) {

                    oHTMLTableSection = this.TableReference.querySelector("tbody");

                }
 
                if (!oHTMLTableSection) {
                    throw new Error(iRowType.toString() + " Row Types for oHTMLTableSection do not exist");
                }

                oHTMLTableRow = this.mCreateElement(cstElementType.Row, this.RowType);

                //Handle Draggable Column Feater
                this.mHandleDraggable(oHTMLTableRow, this.TableType, this.RowType, iUserType);

                this.TableSection = oHTMLTableSection
                this.TableRow = oHTMLTableRow;
                oHTMLTableSection.appendChild(oHTMLTableRow);

                return (oHTMLTableRow);
                
            } catch (oError) {
                mSetStatus("mCreateTableRow", oError);
            }        
        }


 

        mCreateElement(iElementType) {
            try {

                let oHTMLElement = null;
                let sElementName = "";
                let sElementClass = "";

                if (iElementType === cstElementType.Row) {
                    sElementName = "tr";
                } else if (iElementType === cstElementType.Cell && this.RowType === cstRowType.ColumnHeader) {
                    sElementName = "th";
                } else if (iElementType === cstElementType.Cell) {
                    sElementName = "td";
                } else {
                    throw new Error("Only Row and Cell ElementType are supported.");
                }

               
                oHTMLElement = document.createElement(sElementName);

                sElementClass = this.mGetElementClass(iElementType)
                oHTMLElement.className = sElementClass;

                if (iElementType === cstElementType.Cell) {
                    oHTMLElement.style.textAlign = this.ControlClass.CellAlignment;
                }

                return(oHTMLElement);

            } catch (oError) {
                mSetStatus("mCreateElement", oError);
            }
        }

        mGetElementClass(iElementType) {
            try {     

                let sElementClass = "";

                if (iElementType === cstElementType.Row) {
                     if (this.RowType === cstRowType.ColumnHeader) {
                        sElementClass = "TableRowColumnHeader";
                     } else if (this.RowType === cstRowType.Header) {
                        sElementClass = "TableRowHeader";
                     } else if (this.RowType === cstRowType.Detail) {
                        sElementClass = "TableRowDetail";
                     }
                } else if (iElementType === cstElementType.Cell) {
                     if (this.RowType === cstRowType.ColumnHeader) {
                        sElementClass = "TableCellColumnHeader";
                     } else if (this.RowType === cstRowType.Header) {
                        sElementClass = "TableCellDetailHeader";
                     } else if (this.RowType === cstRowType.Detail) {
                        sElementClass = "TableCellDetail";
                     }                    
                }

                return(sElementClass);

            } catch (oError) {
                mSetStatus("mSetElementClass", oError);
            }                       
        }

        async mCreateTableCell() {
            try {

                
                let oHTMLTableCell = null;
                let bControlSetToNotInclude = false;
                let bControlIsOnPageExceptionList = false;
                let bOutputCell = true;

                //This stops the output if the control is set for do not include OR the page is on the exception list in the html control table
                bControlSetToNotInclude = Number(this.ControlSetting) === cstControlSetting.Do_Not_Include;
                bControlIsOnPageExceptionList = this.ControlClass.PageExceptionList.split(",").map(Number).includes(Number(giPageID));

              
                if (bControlSetToNotInclude === true || bControlIsOnPageExceptionList === true) {
                    bOutputCell = false;
                }

                
                if (bOutputCell === true) {
                    
                    oHTMLTableCell = this.mCreateElement(cstElementType.Cell);

                    this.TableCell = oHTMLTableCell;

                    
                    this.mAddTableCellContent();
                }
                
            } catch (oError) {
                mSetStatus("mCreateTableCell", oError);
            }    
        }


        mAddPaddingCell(oHTMLTableRow) {
            try {


                let oHTMLTableCell = null;

                oHTMLTableCell = this.mCreateElement(cstElementType.Cell);

                if (this.ControlSetting === cstControlSetting.Cell_Invisible) {
                    oHTMLTableCell.style.display = "none";                  
                } else {
                    oHTMLTableCell.style.width = this.ControlClass.CellWidth;
                    oHTMLTableCell.style.textAlign = this.ControlClass.CellAlignment;
                }

                oHTMLTableRow.appendChild(oHTMLTableCell);
                
            } catch (oError) {
                mSetStatus("mAddPaddingCell", oError);
            }    
        }


        mAddTableCellContent() {
            try {

                this.TableCell.CellWidth = this.ControlClass.CellWidth;
                this.TableCell.CellAlignment = this.ControlClass.CellAlignment;

                if (this.ControlSetting !== cstControlSetting.Do_Not_Include) {

                    //Blank
                    if (this.ControlSetting === cstControlSetting.Blank) {
                        
                        this.TableCell.textContent = "";

                    //Text Column Header
                    } else if (this.ControlSetting === cstControlSetting.Text) {

                        if (Number(this.ControlClass.Row_ID) === 64 && (this.RowType === cstRowType.Header)) { 
                            this.TableCell.innerHTML = this.PageRowData.Task_Name;
                            this.TableCell.id = "StandardTableHeaderRowTaskDescription"; 
                        } else {
                            this.TableCell.innerHTML = this.ControlClass.ControlLabel;
                        }

                    } else {
                        this.mAddHTMLControl();
                    }

                    if (this.ControlSetting === cstControlSetting.Cell_Invisible) {
                        this.TableCell.style.display = "none";
                        this.TableCell.VisibilityState = cstControlSetting.Cell_Invisible;
                    }
                    this.TableRow.appendChild(this.TableCell);

                }
                
            } catch (oError) {
                mSetStatus("mAddTableCellContent", oError);
            }    
        }

        mAddHTMLControl() {
            try {
                
                let oDOMControl = null;
                let sFilterPrefix = "";
                let sCreateElementText = "";
                let sControlTypeText = "";
                    
                    if (this.ControlClass.ControlType === cstControlType.Textbox) {

                        sCreateElementText = "input";
                        sControlTypeText = "text";


                    } else if (this.ControlClass.ControlType === cstControlType.Timebox) {
                        sCreateElementText = "input";
                        sControlTypeText = "time";


                    } else if (this.ControlClass.ControlType === cstControlType.DatePicker) {
                        sCreateElementText = "input";
                        sControlTypeText = "date";

                    } else if (this.ControlClass.ControlType === cstControlType.CommentBox) {
                        sCreateElementText = "textarea";
                        sControlTypeText = "time";

                    } else if (this.ControlClass.ControlType === cstControlType.DateTimePicker) {
                        sCreateElementText = "input";
                        sControlTypeText = "datetime-local";


                    } else if (this.ControlClass.ControlType === cstControlType.Hidden) {
                        sCreateElementText = "input";
                        sControlTypeText = "hidden";

                    } else if (this.ControlClass.ControlType === cstControlType.DropDown) {

                        sCreateElementText = "select";
                        sControlTypeText = "";

                    } else if (this.ControlClass.ControlType === cstControlType.Checkbox) {

                        sCreateElementText = "input";
                        sControlTypeText = "checkbox";                        

                    } else if (this.ControlClass.ControlType === cstControlType.MultiSelectDropDown) {
                        
                        sCreateElementText = "select";
                        sControlTypeText = "";

                    } else if (this.ControlClass.ControlType === cstControlType.Button && this.RowType !== cstRowType.ColumnHeader && this.RowType !== cstRowType.DetailHeader) {
                        
                            sCreateElementText = "button";
                            sControlTypeText = "";

                        
                    }

                    //Create Control 
                    if (sCreateElementText.trim().length > 0) {
                        oDOMControl = document.createElement(sCreateElementText);
                        if (sControlTypeText.trim().length > 0) {
                            oDOMControl.type = sControlTypeText;
                        }

                        //Init Control
                        this.mInitControl(oDOMControl);
                        this.TableCell.appendChild(oDOMControl);
                        this.mAddMultiSelectChangeEvent(oDOMControl);                        
                    }


            } catch (oError) {
                mSetStatus("mAddHTMLControl", oError);
            }                
        }

        mAddMultiSelectChangeEvent(oDOMControl) {

            try {
                
                if (oDOMControl.hasAttribute("data-multi-select")) {

                    if (!oDOMControl._MultiSelectReference) {

                        const instance = new MultiSelect(oDOMControl, {
                            onChange(value, label, optionElement) {
                                instance.element.dispatchEvent(new Event("change", { bubbles: true }));
                            }
                        });

                        oDOMControl._MultiSelectReference = instance;
                    }
                }

            } catch (oError) {
                mSetStatus("mAddMultiSelectChangeEvent", oError);
            } 
        }

       mHandleDraggable(oHTMLTableRow, iUserType = -1) {

            try {

                let oHTMLTableCell = null;

                if (this.TableType === cstTableType.StandardTable ) {
                    oHTMLTableCell = this.mCreateElement(cstElementType.Cell, this.RowType);

                    if (iUserType > 0) {
                        if (this.RowType !== cstRowType.ColumnHeader) {
                            oHTMLTableRow.draggable = true;
                            oHTMLTableCell.style.width = "35px";
                            oHTMLTableCell.classList.add("drag-handle");
                        }

                        if (this.RowType === cstRowType.Detail) {
                            oHTMLTableCell.textContent = "☰";
                            oHTMLTableCell.style.color = cstHexColorCode.DarkBlue;
                            oHTMLTableCell.style.fontSize = "16px";     
                            //oHTMLTableCell.style.fontWeight = "bold";
                        }
                    }

                    oHTMLTableRow.appendChild(oHTMLTableCell);
                }

                
                
                oHTMLTableCell = null;

            } catch (oError) {
                mSetStatus("mHandleDraggable", oError);
            }
        }

        mInitControl (oDOMControl) {

            try {

                this.mSetNameID(oDOMControl);
                this.mSetFilter(oDOMControl);
                this.mSetWidth(oDOMControl);
                this.mSetValue(oDOMControl);
                this.mSetReadOnlyProperty(oDOMControl);
                this.mSetAlignment(oDOMControl);
                this.mSetControlClass(oDOMControl);
                this.mSetControlAttribute(oDOMControl);
                this.mSetVisibility(oDOMControl);
                this.mSetRequiredProperty(oDOMControl);
                this.mSetupDropDown(oDOMControl);
                this.mSetupButton(oDOMControl);

            } catch (oError) {
                mSetStatus("mInitControl", oError);
            }   
        }

        mSetControlClass(oDOMControl) {
            try {


                //let vCssControlClass = null;
                let sCssControlClass = "";
                let sElementAlignmentClass = "";
                
                //vCssControlClass = this.ControlClass.CssClass;
                sCssControlClass = String(this.ControlClass?.CssClass ?? "");

                //if class is set in HTMLControl table then use, otherwise default to else logic
                if (sCssControlClass.length > 0 && (!sCssControlClass.includes(" "))) {
                    
                    oDOMControl.classList.add(sCssControlClass);
                    
                } else {
                    
                    if (this.ControlClass.ControlType === cstControlType.Textbox || this.ControlClass.ControlType === cstControlType.Timebox) {
                        sElementAlignmentClass = "form-control form-control-sm";
                    } else if (this.ControlClass.ControlType === cstControlType.DropDown) {
                        sElementAlignmentClass = "form-select";
                    } else if (this.ControlClass.ControlType === cstControlType.DatePicker) {
                        sElementAlignmentClass = "form-control";
                    } else if (this.ControlClass.ControlType === cstControlType.DateTimePicker) {
                        sElementAlignmentClass = "form-control";
                    } else if (this.ControlClass.ControlType === cstControlType.Button) {
                        sElementAlignmentClass = "btn btn-custom btn-primary rounded-pill px-4 border-0 d-inline-flex align-items-center gap-2 py-2";
                    }
                    
                    //Append
                    if (this.ControlSetting === cstControlSetting.Control_Visible || this.ControlSetting === cstControlSetting.Control_Read_Only_Visible || this.ControlSetting === cstControlSetting.Filter) {
                        if ((sElementAlignmentClass) && (sElementAlignmentClass.length > 0)) {
                            sElementAlignmentClass = sElementAlignmentClass + " use-element-alignment";
                        } else {
                            sElementAlignmentClass = "use-element-alignment"; 
                        }
                    }   
                    
		    
                    if ((sElementAlignmentClass) && (sElementAlignmentClass.length > 0)) {
                        oDOMControl.classList.add(...sElementAlignmentClass.split(" "));
                    }
		     
                }

            } catch (oError) {
                mSetStatus("mSetControlClass", oError);
            }                   
        }

        mSetupDropDown(oDOMControl) {
            try {

                //Add Drop Down Options
                if ((this.ControlClass.ControlType === cstControlType.DropDown) || (this.ControlClass.ControlType === cstControlType.MultiSelectDropDown)) {
                    this.mAddDropDownOptions(oDOMControl);
                }

            } catch (oError) {
                mSetStatus("mSetupDropDown", oError);
            }                   
        }

        mSetupButton(oDOMControl) {
            try {

                let sControlClass = "";

                sControlClass = String(this.ControlClass?.CssClass ?? "");
                
                //Set Button Display Content
                if (this.ControlClass.ControlType === cstControlType.Button) {
                    if (this.mAddButtonImage(oDOMControl) === false && (sControlClass === "")) {

                        oDOMControl.innerHTML = `<i class="bi bi-plus-lg"></i>&nbsp;<span class="d-none d-sm-inline font-weight-bold">${this.ControlClass.ControlLabel}</span>`;

                    } else if (this.mAddButtonImage(oDOMControl) === false && (sControlClass.substring(0, 3) === "bi ")) {
                        
                        oDOMControl.innerHTML = `<i class="${sControlClass}"></i>&nbsp;<span class="d-none d-sm-inline font-weight-bold">${this.ControlClass.ControlLabel}</span>`;

                    } else if ((this.mAddButtonImage(oDOMControl) === false) && (sControlClass.substring(0, 2) !== "bi")) {

                        oDOMControl.innerHTML = `<span>${this.ControlClass.ControlLabel}</span>`;

                    }
                    
                    oDOMControl.title = this.ControlClass.DefaultValue;
                }      

            } catch (oError) {
                mSetStatus("mSetupButton", oError);
            }                   
        }

        mSetControlAttribute(oDOMControl) {
            try {

                //Standard Multi-Select Attributes
                if (this.ControlClass.ControlType === cstControlType.MultiSelectDropDown) {
                        oDOMControl.setAttribute("multiple", "");
                        oDOMControl.setAttribute("data-multi-select", "");
                        oDOMControl.setAttribute("data-width", this.ControlClass.ControlWidth);
                }         

            } catch (oError) {
                mSetStatus("mSetControlAttribute", oError);
            }                   
        }

        mSetVisibility(oDOMControl) {
           try {

                //Make the control invisible if it is a Header Row in an HxH Plan Table
                if (this.ControlSetting === cstControlSetting.Control_Invisible) {
                    oDOMControl.style.display = "none";
                    oDOMControl.VisibilityState = cstControlSetting.Control_Invisible;
                }

           } catch (oError) {
                mSetStatus("mSetVisibility", oError);
           }   
        }            

        mSetAlignment(oDOMControl) {
           try { 

                //Control Text Alignment
                if (this.RowType === cstRowType.ColumnHeader) {
                    oDOMControl.style.setProperty("text-align", "center", "important");
                    oDOMControl.style.setProperty("text-align-last", "center", "important");
                } else {
                    oDOMControl.style.setProperty("text-align", this.ControlClass.ControlAlignment, "important");
                    oDOMControl.style.setProperty("text-align-last", this.ControlClass.ControlAlignment, "important");
                }

           } catch (oError) {
                mSetStatus("mSetAlignment", oError);
           }                  
        }

        mSetWidth(oDOMControl) {
           try { 

                //Control Width
                oDOMControl.style.width = this.ControlClass.ControlWidth;  
                

           } catch (oError) {
                mSetStatus("mSetWidth", oError);
           }                  
        }
      
        mSetNameID(oDOMControl) {
            try {

                let sAppendToControlID = "";
                let iMaximumInteger = -1;

                iMaximumInteger = 1000;

                //Set Control Name
                oDOMControl.name = this.ControlClass.ControlName;

                //This is used to get the controlID extension
                if (this.PageRowData["Row_ID"]) {
                    sAppendToControlID = "_" + String(this.PageRowData?.["Row_ID"]);
                } else {
                    sAppendToControlID = "_" + String(mRandomInt(iMaximumInteger));
                }
                //Set Control ID
                oDOMControl.id = String(this.ControlClass.ControlID) + sAppendToControlID;


            } catch (oError) {
                mSetStatus("mSetNameID", oError);
            }               
        }

        mSetFilter(oDOMControl) {
            try {

                //Set Control Filter Properties
                if (this.ControlSetting === cstControlSetting.Filter) {
                    oDOMControl.setAttribute("FilterControl", "");
                }

            } catch (oError) {
                mSetStatus("mSetFilter", oError);
            }                  
        }

        mSetReadOnlyProperty(oDOMControl) {
            try {

                let bReadOnlyDisabled = false;

                //Check Read Only Eligibility
                if (this.UserTypeID === cstUserType.Standard) { //check user type for read only config
                    bReadOnlyDisabled = true;
                } else if (this.ControlSetting === cstControlSetting.Control_Read_Only_Visible) {
                    bReadOnlyDisabled = true;
                /*} else { // check control for read only setting
                    bReadOnlyDisabled = (this.ControlClass.ReadOnly === true) || (this.ControlClass.Disabled === true);
                    bReadOnlyDisabled = Boolean(bReadOnlyDisabled);*/
                }

                //Set Read Only Control Properties 
                if (bReadOnlyDisabled === true) {
                    if (this.ControlClass.ControlType !== cstControlType.DropDown && this.ControlClass.ControlType !== cstControlType.MultiSelectDropDown) {
                        oDOMControl.disabled = bReadOnlyDisabled;

                    } else if (this.ControlClass.ControlType === cstControlType.MultiSelectDropDown) {
                        oDOMControl.setAttribute("data-readonly", "true");
                        oDOMControl.setAttribute("data-bgcolor", cstHexColorCode.SuperLightGrey);
                    } else if (this.ControlClass.ControlType === cstControlType.DropDown) {
                        //Drop down doesn't support readonly so if set for read only or disabled then disable
                        oDOMControl.disabled = bReadOnlyDisabled;
                        //oDOMControl.style.backgroundColor = cstHexColorCode.Silver;
                    }                
                }

            } catch (oError) {
                mSetStatus("mSetReadOnlyProperty", oError);
            }   
        }
        
        mSetRequiredProperty(oDOMControl) {
            try {


                if (this.TableType === cstTableType.AddTable) {
                    if ((this.ControlClass.RequiredForAdd === true || this.ControlClass.Required === true) && (this.ControlClass.ControlType !== cstControlType.MultiSelectDropDown)) {
                       oDOMControl.required = true;
                    }
                }

            } catch (oError) {
                mSetStatus("mSetRequiredProperty", oError);
            }
        }

        mSetValue(oDOMControl) {

            try {

                let sControlValue = "";

                if ((this.ControlSetting === cstControlSetting.Text) || (this.ControlSetting === cstControlSetting.Filter && this.ControlClass.ControlType === cstControlType.Textbox)) {
                    
                    oDOMControl.value = this.ControlClass.ControlLabel;

                } else if ((this.ControlSetting === cstControlSetting.Cell_Invisible || 
                            this.ControlSetting === cstControlSetting.Control_Invisible ||
                            this.ControlSetting === cstControlSetting.Control_Read_Only_Visible || 
                            this.ControlSetting === cstControlSetting.Control_Visible) && 
                            (this.ControlClass.ControlType !== cstControlType.MultiSelectDropDown)) {

                    if (this.ControlClass.ControlType === cstControlType.Timebox) {
                        
                        sControlValue = this.PageRowData?.[this.ControlClass.DBFieldName] ?? "";
                        sControlValue = mConvertDBTimeToTimeBoxTime(sControlValue);
                    } else {
                        sControlValue = this.PageRowData?.[this.ControlClass.DBFieldName] ?? "";
                    } 

                    if (this.ControlClass.ControlType === cstControlType.Checkbox) {
                        oDOMControl.checked = mToggleBetweenBitBoolean(sControlValue);
                    }
                    
                    oDOMControl.value = sControlValue;
                    
                }

                //For Column Header Override
                if (this.RowType === cstRowType.ColumnHeader && this.ControlSetting === cstControlSetting.Filter && oDOMControl instanceof HTMLSelectElement) {
                        oDOMControl.selectedIndex = 0;
                }
                

            } catch (oError) {
                mSetStatus("mSetValue", oError);
            }
        }        
        
        mAddButtonImage(oButton) {
            try {

                let sImageSRC = "";
                let bAddImage = false;


                // Check for img: prefix
                if (this.ControlClass.ControlLabel?.startsWith("img:")) {

                    // Extract the image path after "img:"
                    sImageSRC = sApplicationURLRoot + this.ControlClass.ControlLabel.slice(4);

                    // Apply inline styles directly to the button
                    oButton.style.width = "32px";
                    oButton.style.height = "32px";
                    //oButton.style.padding = "0";
                    oButton.style.backgroundImage = `url('${sImageSRC}')`;
                    oButton.style.backgroundSize = "24px 24px";
                    oButton.style.backgroundRepeat = "no-repeat";
                    oButton.style.backgroundPosition = "center";
                    //oButton.style.backgroundColor = "transparent";
                    oButton.style.border = "none";
                    oButton.style.cursor = "pointer";

                    // Accessibility
                    oButton.setAttribute("aria-label", "Button");

                    bAddImage = true;
                }

                return bAddImage;

            } catch (oError) {
                mSetStatus("mAddButtonImage", oError);
            }
        }

        mAddDropDownOptions(oDOMControl) {
            try {

                let sDBFieldName = "";
                let sCategory = (this.ControlClass.LookupCategory || "").toUpperCase();
                let sPopulatedValue = "";
                let sAlignment = "";
                let bRelatedTableValue = false;
                let oDropDownOption = null;
                let arrFilteredLookupValues = [];

                if (!(oDOMControl instanceof HTMLSelectElement)) {
                    throw new Error("oDOMControl must be of HTMLSelectElement instance type.");
                } else if (sCategory.length === 0) {
                    throw new Error("Lookup Category is required for a Drop Down value population.");
                }

                sAlignment = cstAlignInText[String(this.ControlClass.ControlAlignment)];
                arrFilteredLookupValues = this.ControlClass.LookupArray.filter(arrRow => arrRow.Category.toUpperCase() === sCategory);

                if (arrFilteredLookupValues && arrFilteredLookupValues.length > 0) {
                    bRelatedTableValue = false;

                    //Add the first drop down option
                    if (this.ControlSetting === cstControlSetting.Filter) {
                    
                                oDropDownOption = document.createElement("option");
                                oDropDownOption.value = "";   
                                oDropDownOption.textContent = this.ControlClass.ControlLabel;
                                oDOMControl.appendChild(oDropDownOption);

                                oDropDownOption = null;

                    } else if (this.ControlClass.ControlType !== cstControlType.MultiSelectDropDown) {
                        
                        oDropDownOption = document.createElement("option");
                        oDropDownOption.value = "";
                        oDropDownOption.textContent = "";

                        oDOMControl.appendChild(oDropDownOption);
                        oDropDownOption = null;
                    }

                    //Add the rest of drop down option
                    arrFilteredLookupValues.forEach(arrRow => {
                            

                            oDropDownOption = oDOMControl.ownerDocument.createElement("option");
                            oDropDownOption.value = arrRow.Lookup_ID;
                            oDropDownOption.textContent = arrRow.Lookup_Desc;

                            if (this.ControlClass.DBFieldName.includes(".")) {
                                bRelatedTableValue = true;
                                sDBFieldName = this.ControlClass.DBFieldName.split(".")[1];
                            } else {
                                bRelatedTableValue = false;
                                sDBFieldName = this.ControlClass.DBFieldName;
                            }

                            sPopulatedValue = String(this.PageRowData?.[sDBFieldName] ?? "");

                            if (bRelatedTableValue === true) {
                                sPopulatedValue = sPopulatedValue.replace(/ /g, "");
                            }

                            sPopulatedValue = sPopulatedValue.trim();

                            if ((this.ControlClass.ControlType != cstControlType.MultiSelectDropDown) && (String(oDropDownOption.value) === sPopulatedValue)) {
                                oDropDownOption.selected = true;
                            } else if ((this.ControlClass.ControlType === cstControlType.MultiSelectDropDown) && (this.TableType != cstTableType.AddTable)) {
                                
                                if (sPopulatedValue.split(",").includes(String(oDropDownOption.value))) {
                                    oDropDownOption.setAttribute("selected", "");
                                }

                            }
                            
                            oDOMControl.appendChild(oDropDownOption);
                            oDropDownOption = null;
                    });

                }

            } catch (oError) {
                mSetStatus("mAddDropDownOptions", oError);
            }                
        }


  }



    class cExtensibilityArgument {
        constructor({
            CommandType = -1,
            FieldNames = [],
            FieldValues = [],
            NewInsertID = -1,
            KeyFieldName = "",
            KeyFieldValue = "",
            oRowReference = null,
            ReturnValue = null,
        } = {}) {
            this.CommandType = CommandType;
            this.FieldNames = FieldNames;
            this.FieldValues = FieldValues;
            this.NewInsertID = NewInsertID;
            this.KeyFieldName = KeyFieldName;
            this.KeyFieldValue = KeyFieldValue;
            this.oRowReference = oRowReference;
            this.ReturnValue = ReturnValue;
        }
    }

    //Broadcast Class
    class cBroadcastClient {

            
            set BroadcastClientURL(sBroadcastClientURL) {
                this._BroadcastClientURL = sBroadcastClientURL;
            }

            get BroadcastClientURL() {
                return this._BroadcastClientURL;
            } 

            set BroadcastMessageType(iBroadcastMessageType) {
                this._BroadcastMessageType = iBroadcastMessageType;
            }

            get BroadcastMessageType() {
                return this._BroadcastMessageType;
            } 

            set RowID(iRowID) {
                this._RowID = iRowID;
            }

            get RowID() {
                return this._RowID;
            } 


            set TargetObject(oTargetObject) {
                try {

                    if (oTargetObject) {

                        if (oTargetObject instanceof HTMLDivElement && oTargetObject.classList.contains("multi-select")) {

                            // Do nothing, it's already the multiselect DIV

                        } else if (oTargetObject instanceof HTMLDivElement) {

                            oTargetObject = oTargetObject.querySelector(".multi-select");

                            if (!oTargetObject) {
                                throw new Error("TargetObject type is DIV but is not found as a multi-select.");
                            }
                        }
                    }

                    this._TargetObject = oTargetObject;

                } catch (oError) {
                    mSetStatus("cBroadcastClient: set TargetObject:", oError);
                }
            }

            get TargetObject() {
                return this._TargetObject;
            } 

        constructor(sWebSocketServer) {
            this.BroadcastClientURL = sWebSocketServer;
            this.oSocket = null;

            this.Init();
        }

        Init() {
            try {
                this.oSocket = new WebSocket(cstURL.WebSocketServer);

                this.oSocket.onopen = (oEvent) => this.OnOpen(oEvent);
                this.oSocket.onclose = (oEvent) => this.OnClose(oEvent);
                this.oSocket.onerror = (oError) => this.OnError(oError);
                this.oSocket.onmessage = (oEvent) => this.ReceiveMessage(oEvent);
                //this.oSocket.onmessage = this.ReceiveMessage.bind(this);

            } catch (oError) {
                mSetStatus("cBroadcastClient.mInit", oError);
            }
        }
       


        OnOpen(oEvent) {
            try {
                console.log("? WebSocket connection open:  " + String(mGetCurrentCentralTime(true, true)));
            } catch (oError) {
                mSetStatus("cBroadcastClient.mOnOpen", oError);
            }
        }

        OnClose(oEvent) {
            try {
                let bNormalWebSocketClose = false;

                if ((oEvent.code === 1000 || oEvent.code === 1001) && oEvent.wasClean === true) {
                    bNormalWebSocketClose = true;
                    console.log("Normal broadcast server close: " + String(mGetCurrentCentralTime(true, true)));
                }

                if (!bNormalWebSocketClose) {
                    console.warn("?? WebSocket connection closed: " + String(mGetCurrentCentralTime(true, true)), {
                        code: oEvent.code,
                        reason: oEvent.reason,
                        wasClean: oEvent.wasClean
                    });
                }

                if (!bIsWebSocketReconnecting) {
                    bIsWebSocketReconnecting = true;

                    setTimeout(() => {
                        console.log("?? Reconnecting to broadcast server… " + String(mGetCurrentCentralTime(true, true)));
                        oBroadcastClient = new cBroadcastClient(cstURL.WebSocketServer);
                        bIsWebSocketReconnecting = false;
                    }, 1000); // 1 second delay prevents reconnect storms

                }

            } catch (oError) {
                mSetStatus("cBroadcastClient.mOnClose", oError);
            }
        }

        OnError(oError) {
            try {

                mSetStatus("cBroadcastClient.mOnError", oError);

            } catch (oErrorLocal) {
                alert("cBroadcastClient.oError " + oErrorLocal.message);
            }
        }

        BroadcastMessage(iBroadcastMessageType) {
            try {

                let oMessageToSend = {};
                
                    this.BroadcastMessageType = iBroadcastMessageType;
                    
                    if (this.IsValidObject() === true) {
                        oMessageToSend = this.GetMessageToSend();

                        if (this.oSocket.readyState === WebSocket.OPEN) {
                            this.oSocket.send(JSON.stringify(oMessageToSend));
                            console.log("Broadcast Message sent:", oMessageToSend);
                        } else {
                            console.error("? Cannot send message — WebSocket not open", {
                                readyState: this.oSocket.readyState
                            });
                        }

                        console.log("Value change broadcasted to other users.");
                    }
                    
            } catch (oError) {
                mSetStatus("cBroadcastClient.mBroadcastMessage", oError);
            }
        }

        async ReceiveMessage(oEvent) {
            try {

                const oBroadcastMessage = JSON.parse(oEvent.data);
		
		    console.log("BROADCAST MESSAGE TYPE:", typeof oEvent.data, oEvent.data);

		    if (oEvent.data instanceof Blob) {
			oEvent.data.text().then(t => {
			    console.log("BROADCAST MESSAGE BLOB TEXT:", t);
			});
		    } else {
			console.log("BROADCAST MESSAGE RAW:", oEvent.data);
		    }

                if (this.IsBroadcastMatch(oBroadcastMessage, gsApplicationEnvironment, gsClientID, giPlatformID, giYearID, giPageID)) {

                    if (oBroadcastMessage.BroadcastMessageType === cstBroadcastMessageType.ControlUpdate.ID) {
                        await this.UpdateControl(oBroadcastMessage);

                    } else if (oBroadcastMessage.BroadcastMessageType === cstBroadcastMessageType.TableRowInsert.ID) {
                        await this.InsertHTMLTableRow(oBroadcastMessage);

                    } else if (oBroadcastMessage.BroadcastMessageType === cstBroadcastMessageType.TableRowDelete.ID) {
                        await this.DeleteHTMLTableRow(oBroadcastMessage);

                    } else if (oBroadcastMessage.BroadcastMessageType === cstBroadcastMessageType.MessageOnly.ID) {
                        await mDisplayNotificationAlert(oBroadcastMessage.Message);
                    }
                }

            } catch (oError) {
                mSetStatus("cBroadcastClient.ReceiveMessage", oError);
            }
        }

        async UpdateControl(oBroadcastMessage) {

            try {

                const oTargetControl = document.getElementsByName(oBroadcastMessage.ObjectName)[oBroadcastMessage.ObjectIndex];

                if (oTargetControl) {

                    this.TargetObject = oTargetControl;
                    
                    if (this.IsValidObject() === true) {

                            this.SetEventSuppression(this.TargetObject, true);

                            if (this.TargetObject instanceof HTMLInputElement 
                                    || this.TargetObject instanceof HTMLSelectElement 
                                    || this.TargetObject instanceof HTMLTextAreaElement) {
                                
                                    this.TargetObject.style.backgroundColor = cstHexColorCode.LightBlue;

                                    //Does a delay before resetting to white
                                    setTimeout(() => {
                                        this.TargetObject.value = oBroadcastMessage.ObjectValue;
                                        this.TargetObject.style.backgroundColor = cstHexColorCode.White;
                                    }, 2500); // 

                                    
                            } else if (this.TargetObject.classList.contains("multi-select")) {

                                    mSetMultiSelectValues(this.TargetObject, oBroadcastMessage.ObjectValue);
                            }

                            this.SetEventSuppression(this.TargetObject, false);
                    }
                }

            } catch (oError) {
                mSetStatus("cBroadcastClient.UpdateControl", oError);
            }                        
        }


        async InsertHTMLTableRow(oBroadcastMessage) {

            try { 
                
                let sTableName = "";
                let iRowID = -1;
                let iActionID = -1;
                let arrRowData = [];
                let oHTMLTable = null;
                let oArgumentData = null;
                
                sTableName = cstTableName[gsPageName];
                iActionID = cstGetRecordsetAction.PageRow;
                iRowID = Number(oBroadcastMessage.ObjectValue);

                if (typeof iRowID === "undefined") {
                    throw new Error("iRowID is undefined.  This function requires a Row ID.");
                }

                //oHTMLTable = document.getElementById();
                oHTMLTable = mGetHTMLTable(oBroadcastMessage.TableID);
                oHTMLTable.TableType = oBroadcastMessage.TableID;

                if (await mCheckHTMLControlForMatch(oHTMLTable, "txtGridRowID", iRowID) === false) {
                    
                    oArgumentData = new FormData();
                    oArgumentData.append("SQL:TableName", sTableName);
                    oArgumentData.append("PlatformID", giPlatformID);
                    oArgumentData.append("YearID", giYearID);
                    oArgumentData.append("RowID", String(iRowID));
                    oArgumentData.append("ActionID", iActionID);                    
                    
                    arrRowData = await mGetRecordset(cstURL.GetRecordset, oArgumentData);                    

                    await mOutputDOM(oHTMLTable, cstRowType.Detail, giUserTypeID, arrHTMLControlTable, true, arrRowData);   
                    await mDisplayNotificationAlert(oBroadcastMessage.Message);                 

                    oArgumentData = null;
                }

                oHTMLTable = null;
                

            } catch (oError) {
                mSetStatus("cBroadcastClient.DeleteHTMLTableRow", oError);
            }                        
        }            

        //The lesson learned here is sometimes the html doesn't reflect the dynamic value assigned to the control.  this approach resolves that issue.
        async DeleteHTMLTableRow(oBroadcastMessage) {
            try {

                let sControlName = "";
                let sQuerySelectorString = "";
                let iRowID = -1;
                let oHTMLTable = null;
                let oControl = null;
                
                oHTMLTable = mGetHTMLTable(oBroadcastMessage.TableID);
                
                iRowID = oBroadcastMessage.ObjectValue;
                sControlName = oBroadcastMessage.ObjectName;

                if (sControlName.toUpperCase() === "TXTGRIDROWID") {
                    const oControls = oHTMLTable.querySelectorAll(`tbody tr [name="${sControlName}"]`);

                    oControl = null;

                    // This assumes one match, then it breaks and removes (just remove the break to look through all records)
                    for (const ctrl of oControls) {
                        if (ctrl.value == iRowID) {
                            oControl = ctrl;
                            break;
                        }
                    }

                    if (oControl) {
                        const oRow = oControl.closest("tr");
                        if (oRow) {
                            oRow.remove();
                            mDisplayNotificationAlert(oBroadcastMessage.Message);
                        }
                    }
                } else {
                    throw new Error("DeleteHTMLTableRow only currently supports using the Row ID to delete a table row.  Attempted Control Name:" + sControlName);
                }

                
            } catch (oError) {
                mSetStatus("cBroadcastClient.DeleteHTMLTableRow", oError);
            }
        }

        IsBroadcastMatch(oBroadcastMessage, sApplicationEnvironment, sClientID, iPlatformID, iYearID, iPageID) {

            let bIsBroadcastMatch = false;

            try {

                //Global Message Broadcast
               if (String(sApplicationEnvironment) === String(oBroadcastMessage.ApplicationEnvironment) && this.BroadcastMessageType === cstBroadcastMessageType.GlobalMessage.ID) {
                        bIsBroadcastMatch = true;

               //Platform Message Broadcast
               } else if (this.BroadcastMessageType === cstBroadcastMessageType.PlatformMessage.ID) {
                    if (String(sApplicationEnvironment) === String(oBroadcastMessage.ApplicationEnvironment) && Number(iPlatformID) === Number(oBroadcastMessage.PlatformID)) {
                        bIsBroadcastMatch = true;
                    }
               } else if (
			   (String(sApplicationEnvironment) === String(oBroadcastMessage.ApplicationEnvironment))
			&& (String(sClientID) !== String(oBroadcastMessage.ClientID))
                        && (Number(iPlatformID) === Number(oBroadcastMessage.PlatformID)) 
                        && (Number(iYearID) === Number(oBroadcastMessage.YearID)) 
                        && (Number(iPageID) === Number(oBroadcastMessage.PageID))) {

                    bIsBroadcastMatch = true;
                }

                return(bIsBroadcastMatch);

            } catch (oError) {
                mSetStatus("cBroadcastClient.IsBroadcastMatch", oError);
                bIsBroadcastMatch = false;
                return(bIsBroadcastMatch);
            }
        }

        SetEventSuppression(oControl, bEventSupressionState) {
            try {
                
                if ((!oControl) || (typeof bEventSupressionState !== "boolean")) {
                    throw new Error("Control or Event Suppression State is required.")
                }
                
                oControl._SuppressChangeEvent = bEventSupressionState;

            } catch (oError) {
                mSetStatus("cBroadcastClient.SetEventSuppression", oError);
            }  
        }

        GetMessageToSend() {
            try {

                let oMessageToSend = {};
                let iHTMLTableID = -1;
                let sObjectName = "";
                let iObjectIndex = -1;
                let vObjectValue = null;

                
                if (this.BroadcastMessageType !== cstBroadcastMessageType.MessageOnly.ID) {
                    //Set Message Name and If Applicable Table ID
                    if (this.TargetObject instanceof HTMLInputElement || this.TargetObject instanceof HTMLSelectElement || this.TargetObject instanceof HTMLTextAreaElement) {
                        sObjectName = this.TargetObject.name;
                    } else if (this.TargetObject.classList.contains("multi-select")) {
                        sObjectName = this.TargetObject.getAttribute("name");
                    } else if (this.TargetObject instanceof HTMLTableRowElement) {

                        iHTMLTableID = cstTableType.StandardTable;

                        const arrPrimaryKeyControl = mFilterFromArray(arrHTMLControlTable, "PrimaryKey", 1);

                        if (arrPrimaryKeyControl.length === 1) {
                            sObjectName = arrPrimaryKeyControl[0]?.ControlName;
                        } else {
                            throw new Error("arrPrimarykeyControl cannot have more than one record/row in it.")
                        }
                        
                    }
                

                
                    //Set Message Index and Value
                    if (this.TargetObject instanceof HTMLTableRowElement) {
                        iObjectIndex = -1;
                        //vObjectValue = this.TargetObject.querySelector(`[name="${sObjectName}"]`)?.value;
                        vObjectValue = this.RowID;
                    } else {
                        iObjectIndex = mGetControlIndex(this.TargetObject);
                        vObjectValue = this.GetControlValue().replace(/\s+/g, "");
                    }
                }
                

                oMessageToSend = {
		    ApplicationEnvironment: gsApplicationEnvironment,
                    ClientID: gsClientID,
                    PlatformID: giPlatformID,
                    YearID: giYearID,
                    PageID: giPageID,
                    BroadcastMessageType: this.BroadcastMessageType,
                    TableID: iHTMLTableID,
                    ObjectName: sObjectName,
                    ObjectIndex: iObjectIndex,
                    ObjectValue: vObjectValue,
                    Message: cstBroadcastMessageType[this.BroadcastMessageType].Message,
                };

                 return(oMessageToSend);

            } catch (oError) {
                mSetStatus("cBroadcastClient.GetMessageToSend", oError);
            }            
        }



        IsValidObject() {
            try {

                let oTempObject = null;
                let bValidObject = false;

                if (this.BroadcastMessageType === cstBroadcastMessageType.ControlUpdate.ID) {
                    oTempObject = this.TargetObject;
                    
                    if (!oTempObject) {
                        throw new Error("cBroadcastClient.IsValidObject: For Control Update an Control Object Reference is required.");
                        bValidObject = false;
                        return;
                    }

                    if (oTempObject instanceof HTMLInputElement || oTempObject instanceof HTMLSelectElement || oTempObject instanceof HTMLTextAreaElement) {
                        bValidObject = true;
                    } else if (oTempObject instanceof HTMLTableRowElement) {
                        bValidObject = true;
                    } else if (oTempObject.classList.contains("multi-select")) {
                        
                        if (oTempObject) {
                            bValidObject = true;
                        }
                        
                    } else {
                        throw new Error("cBroadcastClient.IsValidObject: Control type could not be found.");
                        bValidObject = false;
                        return;
                    }

                } else if (this.BroadcastMessageType === cstBroadcastMessageType.TableRowDelete.ID) {
                    oTempObject = this.TargetObject;

                    if (oTempObject instanceof HTMLTableRowElement) {
                        bValidObject = true;
                    } else {
                        throw new Error("cBroadcastClient.IsValidObject: For Broadcast of row delete, a row TargetObject must be set.");
                        bValidObject = false;
                        return;
                    }

                } else {
                    bValidObject = true;
                }

                oTempObject = null;

                return(bValidObject);

            } catch (oError) {
                mSetStatus("cBroadcastClient.IsValidObject", oError);
            }            
        }

        GetControlValue() {
            try {

            
                let oTempControl = null;
                let vControlValue = null;

                oTempControl = this.TargetObject;

                if (oTempControl instanceof HTMLInputElement || oTempControl instanceof HTMLSelectElement || oTempControl instanceof HTMLTextAreaElement) {
                    
                    vControlValue = oTempControl.value;

                } else if (oTempControl.classList.contains("multi-select")) {
                    
                    vControlValue = mGetMultiSelectValues(oTempControl);

                }

                oTempControl = null;

                return(vControlValue);

            } catch (oError) {
                mSetStatus("cBroadcastClient.GetControlValue", oError);
            }
        }
    }

   /*
 * Created by David Adams
 * https://codeshack.io/multi-select-dropdown-html-javascript/
 * 
 * Released under the MIT license
 */
    class MultiSelect {

        constructor(element, options = {}) {
            let defaults = {
                placeholder: 'Select item(s)',
                max: null,
                search: true,
                selectAll: true,
                listAll: true,
                closeListOnItemSelect: false,
                name: '',
                width: '',
                height: '',
                dropdownWidth: '',
                dropdownHeight: '',
                tabindex: 0,
                data: [],
                readonly: false,
                onChange: function() {},
                onSelect: function() {},
                onUnselect: function() {}
            };

                this.options = Object.assign(defaults, options);
                this.selectElement = typeof element === 'string' ? document.querySelector(element) : element;

                // Detect whether the original <select> was hidden
                this.isHidden =
                    this.selectElement.hidden ||                                   // <select hidden>
                    this.selectElement.style.display === "none" ||                 // <select style="display:none">
                    this.selectElement.dataset.hidden === "true";                  // <select data-hidden="true">

                // Detect background color for the generated multi-select control
                this.backgroundColor = this.selectElement.dataset.bgcolor || "";   

                this.readonly = this.selectElement.dataset.readonly === "true";

                for(const prop in this.selectElement.dataset) {
                    if (this.options[prop] !== undefined) {
                        this.options[prop] = this.selectElement.dataset[prop];
                    }
                }
                
            this.name = this.selectElement.getAttribute('name') ? this.selectElement.getAttribute('name') : 'multi-select-' + Math.floor(Math.random() * 1000000);
            if (!this.options.data.length) {
                let options = this.selectElement.querySelectorAll('option');
                for (let i = 0; i < options.length; i++) {
                    this.options.data.push({
                        value: options[i].value,
                        text: options[i].innerHTML,
                        selected: options[i].selected,
                        html: options[i].getAttribute('data-html')
                    });
                }
            }
            this.element = this._template();
            this.selectElement.replaceWith(this.element);
            this._updateSelected();
            this._eventHandlers();
        }

        _template() {
            let optionsHTML = '';
            for (let i = 0; i < this.data.length; i++) {
                optionsHTML += `
                    <div class="multi-select-option${this.selectedValues.includes(this.data[i].value) ? ' multi-select-selected' : ''}" data-value="${this.data[i].value}">
                        <span class="multi-select-option-radio"></span>
                        <span class="multi-select-option-text">${this.data[i].html ? this.data[i].html : this.data[i].text}</span>
                    </div>
                `;
            }

            let selectAllHTML = '';
            if (this.options.selectAll === true || this.options.selectAll === 'true') {
                selectAllHTML = `<div class="multi-select-all">
                    <span class="multi-select-option-radio"></span>
                    <span class="multi-select-option-text">Select all</span>
                </div>`;
            }

            let template = `
                <div class="multi-select ${this.name}" name="${this.name}"${this.selectElement.id ? ' id="' + this.selectElement.id + '"' : ''} tabindex="${this.options.tabindex}" style="${this.options.width ? 'width:' + this.options.width + ';' : ''}${this.options.height ? 'height:' + this.options.height + ';' : ''}${this.backgroundColor ? 'background-color:' + this.backgroundColor + ';' : ''}${this.isHidden ? 'display:none;' : ''}">
                    ${this.selectedValues.map(value => `<input type="hidden" name="${this.name}[]" value="${value}">`).join('')}
                    <div class="multi-select-header" style="${this.options.width ? 'width:' + this.options.width + ';' : ''}${this.options.height ? 'height:' + this.options.height + ';' : ''}${this.backgroundColor ? 'background-color:' + this.backgroundColor + ';' : ''}">
                        <span class="multi-select-header-max">${this.options.max ? this.selectedValues.length + '/' + this.options.max : ''}</span>
                        <span class="multi-select-header-placeholder">${this.placeholder}</span>
                    </div>
                    <div class="multi-select-options" style="${this.options.dropdownWidth ? 'width:' + this.options.dropdownWidth + ';' : ''}${this.options.dropdownHeight ? 'height:' + this.options.dropdownHeight + ';' : ''}${this.backgroundColor ? 'background-color:' + this.backgroundColor + ';' : ''}">
                        ${this.options.search === true || this.options.search === 'true' ? '<input type="text" class="multi-select-search" placeholder="Search...">' : ''}
                        ${selectAllHTML}
                        ${optionsHTML}
                    </div>
                </div>
            `;


            let element = document.createElement('div');

            element.innerHTML = template;
            return element;
        }

        _eventHandlers() {
            let headerElement = this.element.querySelector('.multi-select-header');

            this.element.querySelectorAll('.multi-select-option').forEach(option => {
                option.onclick = () => {

                    // READONLY GUARD (1)
                    if (this.readonly) return;

                    let selected = true;
                    if (!option.classList.contains('multi-select-selected')) {
                        if (this.options.max && this.selectedValues.length >= this.options.max) {
                            return;
                        }
                        option.classList.add('multi-select-selected');
                        if (this.options.listAll === true || this.options.listAll === 'true') {
                            if (this.element.querySelector('.multi-select-header-option')) {
                                let opt = Array.from(this.element.querySelectorAll('.multi-select-header-option')).pop();
                                opt.insertAdjacentHTML('afterend', `<span class="multi-select-header-option" data-value="${option.dataset.value}">${option.querySelector('.multi-select-option-text').innerHTML}</span>`);
                            } else {
                                headerElement.insertAdjacentHTML('afterbegin', `<span class="multi-select-header-option" data-value="${option.dataset.value}">${option.querySelector('.multi-select-option-text').innerHTML}</span>`);
                            }
                        }
                        this.element.querySelector('.multi-select').insertAdjacentHTML('afterbegin', `<input type="hidden" name="${this.name}[]" value="${option.dataset.value}">`);
                        this.data.filter(data => data.value == option.dataset.value)[0].selected = true;
                    } else {
                        option.classList.remove('multi-select-selected');
                        this.element.querySelectorAll('.multi-select-header-option').forEach(headerOption => headerOption.dataset.value == option.dataset.value ? headerOption.remove() : '');
                        this.element.querySelector(`input[value="${option.dataset.value}"]`).remove();
                        this.data.filter(data => data.value == option.dataset.value)[0].selected = false;
                        selected = false;
                    }
                    if (this.options.listAll === false || this.options.listAll === 'false') {
                        if (this.element.querySelector('.multi-select-header-option')) {
                            this.element.querySelector('.multi-select-header-option').remove();
                        }
                        headerElement.insertAdjacentHTML('afterbegin', `<span class="multi-select-header-option">${this.selectedValues.length} selected</span>`);
                    }
                    if (!this.element.querySelector('.multi-select-header-option')) {
                        headerElement.insertAdjacentHTML('afterbegin', `<span class="multi-select-header-placeholder">${this.placeholder}</span>`);
                    } else if (this.element.querySelector('.multi-select-header-placeholder')) {
                        this.element.querySelector('.multi-select-header-placeholder').remove();
                    }
                    if (this.options.max) {
                        this.element.querySelector('.multi-select-header-max').innerHTML = this.selectedValues.length + '/' + this.options.max;
                    }
                    if (this.options.search === true || this.options.search === 'true') {
                        this.element.querySelector('.multi-select-search').value = '';
                    }
                    this.element.querySelectorAll('.multi-select-option').forEach(option => option.style.display = 'flex');
                    if (this.options.closeListOnItemSelect === true || this.options.closeListOnItemSelect === 'true') {
                        headerElement.classList.remove('multi-select-header-active');
                    }
                    this.options.onChange(option.dataset.value, option.querySelector('.multi-select-option-text').innerHTML, option);
                    if (selected) {
                        this.options.onSelect(option.dataset.value, option.querySelector('.multi-select-option-text').innerHTML, option);
                    } else {
                        this.options.onUnselect(option.dataset.value, option.querySelector('.multi-select-option-text').innerHTML, option);
                    }
                };
            });

            headerElement.onclick = () => {

                // READONLY GUARD (2)
                if (this.readonly) return;
                
                headerElement.classList.toggle('multi-select-header-active');
            };

            /*this.element.addEventListener("click", (evt) => {
                if (this.readonly) return;

                // Ignore clicks inside the dropdown list
                if (evt.target.closest(".multi-select-options")) return;

                headerElement.classList.toggle("multi-select-header-active");
            });

            this.element.addEventListener("click", (evt) => {
                if (this.readonly) return;

                // Ignore clicks inside the dropdown list
                if (evt.target.closest(".multi-select-options")) return;

                // Only toggle if the click is inside the header OR the empty wrapper area
                if (evt.target.closest(".multi-select-header") || evt.target === this.element) {
                    headerElement.classList.toggle("multi-select-header-active");
                }
            });*/


            if (this.options.search === true || this.options.search === 'true') {
                let search = this.element.querySelector('.multi-select-search');
                search.oninput = () => {
                    this.element.querySelectorAll('.multi-select-option').forEach(option => {
                        option.style.display = option.querySelector('.multi-select-option-text').innerHTML.toLowerCase().indexOf(search.value.toLowerCase()) > -1 ? 'flex' : 'none';
                    });
                };
            }

            if (this.options.selectAll === true || this.options.selectAll === 'true') {
                let selectAllButton = this.element.querySelector('.multi-select-all');
                selectAllButton.onclick = () => {

                    // READONLY GUARD (3)
                    if (this.readonly) return;

                    let allSelected = selectAllButton.classList.contains('multi-select-selected');
                    this.element.querySelectorAll('.multi-select-option').forEach(option => {
                        let dataItem = this.data.find(data => data.value == option.dataset.value);
                        if (dataItem && ((allSelected && dataItem.selected) || (!allSelected && !dataItem.selected))) {
                            option.click();
                        }
                    });
                    selectAllButton.classList.toggle('multi-select-selected');
                };
            }

            if (this.selectElement.id && document.querySelector('label[for="' + this.selectElement.id + '"]')) {
                document.querySelector('label[for="' + this.selectElement.id + '"]').onclick = () => {

                    // READONLY GUARD (4)
                    if (this.readonly) return;

                    headerElement.classList.toggle('multi-select-header-active');
                };
            }

            document.addEventListener('click', event => {
                if (!event.target.closest('.' + this.name) && !event.target.closest('label[for="' + this.selectElement.id + '"]')) {
                    headerElement.classList.remove('multi-select-header-active');
                }
            });
        }

        _updateSelected() {
            if (this.options.listAll === true || this.options.listAll === 'true') {
                this.element.querySelectorAll('.multi-select-option').forEach(option => {
                    if (option.classList.contains('multi-select-selected')) {
                        this.element.querySelector('.multi-select-header').insertAdjacentHTML('afterbegin', `<span class="multi-select-header-option" data-value="${option.dataset.value}">${option.querySelector('.multi-select-option-text').innerHTML}</span>`);
                    }
                });
            } else {
                if (this.selectedValues.length > 0) {
                    this.element.querySelector('.multi-select-header').insertAdjacentHTML('afterbegin', `<span class="multi-select-header-option">${this.selectedValues.length} selected</span>`);
                }
            }
            if (this.element.querySelector('.multi-select-header-option')) {
                this.element.querySelector('.multi-select-header-placeholder').remove();
            }
        }

        get selectedValues() {
            return this.data.filter(data => data.selected).map(data => data.value);
        }

        get selectedItems() {
            return this.data.filter(data => data.selected);
        }

        set data(value) {
            this.options.data = value;
        }

        get data() {
            return this.options.data;
        }

        set selectElement(value) {
            this.options.selectElement = value;
        }

        get selectElement() {
            return this.options.selectElement;
        }

        set element(value) {
            this.options.element = value;
        }

        get element() {
            return this.options.element;
        }

        set placeholder(value) {
            this.options.placeholder = value;
        }

        get placeholder() {
            return this.options.placeholder;
        }

        set name(value) {
            this.options.name = value;
        }

        get name() {
            return this.options.name;
        }

        set width(value) {
            this.options.width = value;
        }

        get width() {
            return this.options.width;
        }

        set tabindex(value) {
            this.options.tabindex = value;
        }

        get tabindex() {
            return this.options.tabindex;
        }

        set height(value) {
            this.options.height = value;
        }

        get height() {
            return this.options.height;
        }

    }