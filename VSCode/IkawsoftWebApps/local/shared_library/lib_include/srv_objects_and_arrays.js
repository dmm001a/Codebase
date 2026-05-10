    
    
    function mGetHTMLControlSettingObject(arrControlRow) {

        try {

            let oControl = null;

            if (arrControlRow === null || typeof arrControlRow !== "object" || Array.isArray(arrControlRow)) {
                alert("Invalid arrControlRow:", arrControlRow);
                return null;
            }

            oControl = new cControl();
            
            for (const [sPropertyName, vPropertyValue] of Object.entries(arrControlRow)) {

                if (sPropertyName in oControl) {
                    try {

                        oControl[sPropertyName] = vPropertyValue; 

                    } catch (oBindError) {

                        throw new Error(`Error setting ${sPropertyName}:`, oBindError.message);
                        
                    }
                }

            }

            return oControl;

        } catch (oError) {
            mSetStatus("mGetHTMLControlSettingObject", oError);
        }               
    }
    
    function mGetHTMLTable(iTableType) {
        try {

            let oHTMLTable = null;
            
            oHTMLTable = document.getElementById(cstTableTypeInText[iTableType]); //For tables use ID (not name)

            if (oHTMLTable) {
                oHTMLTable.TableType = iTableType;
            } else {
                console.log(cstTableTypeInText[iTableType] + " does not exist.");
            }

            return oHTMLTable;
            
        } catch (oError) {
            mSetStatus("mGetHTMLTable", oError);
        }   
    }

    /*function mGetHTMLControlByName(sControlName) {
        try {

            let oControlNodeList = null;
            let oHTMLControl = null;

            if (!sControlName || sControlName.length === 0) {
                throw new Error("Control name not provided.");
            }

            oControlNodeList = document.querySelectorAll('[name="' + sControlName + '"]');

            if (oControlNodeList.length === 1) {
                oHTMLControl = oControlNodeList[0];
            } else {
                throw new Error("This method retrieved either no controls or more than one control with the control name:  " + sControlName);                
            }

            return oHTMLControl;
            
        } catch (oError) {
            mSetStatus("mGetHTMLControlByName", oError);
        }   
    }*/

    /*function mIsEmptyArray(arrArray, bCheckArrayLength = false) {
        try {
            
            let bEmptyArray = true;
                
            if (!Array.isArray(arrArray)) {
                bEmptyArray = true;
            } else if (Array.isArray(arrArray) && (bCheckArrayLength === true && (arrArray.length === 0))) {
                bEmptyArray = true;
            } else {
                bEmptyArray = false;
            }

            return(bEmptyArray);

        } catch (oError) {
            mSetStatus("mIsEmptyArray", oError);
            return(false);
        }           
    }*/
    
    function mGetHTMLElement(iHTMLElementType, sElementIdentifier, oHTMLObject = document) {
        try {

            let oHTMLElement = null;

                if (iHTMLElementType === cstGetHTMLElementType.ID) {

                    oHTMLElement = oHTMLObject.querySelector('[id="' + sElementIdentifier + '"]')
                    
                } else if (iHTMLElementType === cstGetHTMLElementType.Name) {
                    
                    oHTMLElement = oHTMLObject.querySelector('[name="' + sElementIdentifier + '"]')
                } else {
                    const oLocalError = new Error();
                    oLocalError.AlertMessage = "Element Type could not be found.  " + iHTMLElementType;
                    throw oLocalError;
                }


            if (!oHTMLElement) {
                console.error(sElementIdentifier + " does not exist.");
            }

            return oHTMLElement;
            
        } catch (oError) {
            mSetStatus("mGetHTMLElement", oError);
        }   
    }

    
    function mGetPlainHTMLTableArray(iTableType) {

        try {

            let arrPlainHTMLTableConfig = [];

            if (iTableType === cstTableType.TestSummaryTable) {
                    arrPlainHTMLTableConfig = [
                            {
                                HTML_Table_Header_Name: "Test ID",
		                        HTML_Table_Cell_Width: cstWidthSetting.Auto,								
                                HTML_Table_Cell_Align: cstAlignSetting.Center,   
                                DB_Table_Column_Name: "Test_ID"                                                             
                            },
                            {
                                HTML_Table_Header_Name: "Application",
		                        HTML_Table_Cell_Width: cstWidthSetting.Auto,								
                                HTML_Table_Cell_Align: cstAlignSetting.Center,   
                                DB_Table_Column_Name: "Application_Name"                                                             
                            },
                            {
                                HTML_Table_Header_Name: "Test Type",
		                        HTML_Table_Cell_Width: cstWidthSetting.Auto,								
                                HTML_Table_Cell_Align: cstAlignSetting.Center,   
                                DB_Table_Column_Name: "Test_Type"                                                             
                            },                             
                            {
                                HTML_Table_Header_Name: "Test Description",
		                        HTML_Table_Cell_Width: cstWidthSetting.Auto,								
                                HTML_Table_Cell_Align: cstAlignSetting.Center,   
                                DB_Table_Column_Name: "Test_Description"                                                             
                            },                                        
                            {
                                HTML_Table_Header_Name: "Component",
		                        HTML_Table_Cell_Width: cstWidthSetting.Auto,								
                                HTML_Table_Cell_Align: cstAlignSetting.Center,   
                                DB_Table_Column_Name: "Component_Description"                                                             
                            },              
                            {
                                HTML_Table_Header_Name: "Test Date",
		                        HTML_Table_Cell_Width: cstWidthSetting.Auto,								
                                HTML_Table_Cell_Align: cstAlignSetting.Center,   
                                DB_Table_Column_Name: "Test_Date"                                                             
                            },    
                            {
                                HTML_Table_Header_Name: "Environment",
		                        HTML_Table_Cell_Width: cstWidthSetting.Auto,								
                                HTML_Table_Cell_Align: cstAlignSetting.Center,   
                                DB_Table_Column_Name: "Environment_Description"                                                             
                            },        
                            {
                                HTML_Table_Header_Name: "Component Type",
		                        HTML_Table_Cell_Width: cstWidthSetting.Auto,								
                                HTML_Table_Cell_Align: cstAlignSetting.Center,   
                                DB_Table_Column_Name: "Component_Type"                                                             
                            },      
                            {
                                HTML_Table_Header_Name: "Primary Hosting Site",
		                        HTML_Table_Cell_Width: cstWidthSetting.Auto,								
                                HTML_Table_Cell_Align: cstAlignSetting.Center,   
                                DB_Table_Column_Name: "Primary_Hosting_Site"                                                             
                            },
                            {
                                HTML_Table_Header_Name: "Secondary Hosting Site",
		                        HTML_Table_Cell_Width: cstWidthSetting.Auto,								
                                HTML_Table_Cell_Align: cstAlignSetting.Center,   
                                DB_Table_Column_Name: "Secondary_Hosting_Site"                                                             
                            }                                                                                                                                                       
                        ]
            }

            return (arrPlainHTMLTableConfig);

        } catch (oError) {
            mSetStatus("mGetPlainHTMLTableArray", oError);
        }   
        
    }



        async function mPopulateLookupDictionary(dctToPopulate, sCategory) {

            try {

                let arrLocalLookup = [];                

                arrLocalLookup = arrLookupTable.filter(arrItem => arrItem.Category === sCategory);

                arrLocalLookup.forEach((arrItem, index) => {
                    dctToPopulate[arrItem.Lookup_Desc] = arrItem.Lookup_ID;
                });

            } catch (oError) {
                mSetStatus("mPopulateLookupDictionary", oError);
            }
        }


       function mFindFromArray (arrSearchArray, sColumnName, vColumnValue) {
            try {

                let oArrayFind = null;

                oArrayFind = arrSearchArray.find(arrItem => arrItem[sColumnName] === vColumnValue);

                return(oArrayFind);

            } catch (oError) {
                mSetStatus("mFindFromArray", oError);
            }                        
       }


       function mFilterFromArray (arrSearchArray, sFilterColumnName1, vFilterColumnValue1, sFilterColumnName2 = "", vFilterColumnValue2 = "") {
            try {

                let arrArrayFilter = [];

                if (sFilterColumnName2 === "") {
                    arrArrayFilter =  arrSearchArray.filter(arrItem => arrItem[sFilterColumnName1] === vFilterColumnValue1);
                } else if ((sFilterColumnName1 !== "") && (sFilterColumnName2 !== "")) {
                    arrArrayFilter = arrSearchArray.filter(arrItem => arrItem[sFilterColumnName1] === vFilterColumnValue1 && arrItem[sFilterColumnName2] === vFilterColumnValue2);
                }

                return(arrArrayFilter);

            } catch (oError) {
                mSetStatus("mFilterFromArray", oError);
            }                        
       }

       function mGetKeyFromValue(dctSource, vValue) {

            let vReturnKey = null;

            try {

                for (let vKey in dctSource) {

                    if (typeof dctSource[vKey] === "string") {

                        if (String(dctSource[vKey]).toUpperCase() === String(vValue).toUpperCase()) {

                            if (isNaN(vKey)) {
                                vReturnKey = vKey;
                            } else if (String(vKey).trim() === "") {
                                vReturnKey = vKey;
                            } else {
                                vReturnKey = Number(vKey);
                            }

                            return vReturnKey;
                        }

                    } else if (typeof dctSource[vKey] === "number") {

                        if (dctSource[vKey] === Number(vValue)) {

                            if (isNaN(vKey)) {
                                vReturnKey = vKey;
                            } else {
                                vReturnKey = Number(vKey);
                            }

                            return vReturnKey;
                        }
                    }
                }

                if (String(vValue).length === 0) {
                    return "";
                } else {
                    return null;
                }

            } catch (oError) {
                mSetStatus("mGetKeyFromValue", oError);
            }
        }