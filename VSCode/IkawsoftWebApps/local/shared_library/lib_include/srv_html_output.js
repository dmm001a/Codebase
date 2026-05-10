

    async function mOutputToolbar(arrHTMLControlRst) {
        try {

            let oHTMLTable = null;
            let arrButtonOnlyHTMLControlRST = [];


            oHTMLTable = mGetHTMLTable(cstTableType.ToolbarTable);
            oHTMLTable.TableType = cstTableType.ToolbarTable;
            
            mClearTable(cstTableType.ToolbarTable);

            arrButtonOnlyHTMLControlRST = mFilterFromArray(arrHTMLControlRst, "ControlType", cstControlType.Button);
            await mOutputDOM(oHTMLTable, cstRowType.Detail, giUserTypeID, arrButtonOnlyHTMLControlRST, true);


        } catch (oError) {
            mSetStatus("mOutputToolbar", oError);
        }            
    }

    async function mSetHTMLTableRowColor(oRow, sHexColorCode) {
        try {

            oRow.querySelectorAll('td').forEach(td => {
                td.style.backgroundColor = sHexColorCode;
            });

        } catch (oError) {
            mSetStatus("mSetHTMLTableRowColor", oError);
        }                                       
    }
    
    async function mOutputPlainHTMLTable(iPlainHTMLTableID, oFormData) {

        try {
            
            // Declare variables
            let sPlainHTMLTableID = "";
            let arrJSONRst = [];
            let oHTMLTable = null;
            let oArgumentData = null;
            let oResponse = null;

            sPlainHTMLTableID = cstTableTypeInText[iPlainHTMLTableID];
            oHTMLTable = mGetHTMLTable(iPlainHTMLTableID);

            oArgumentData = new FormData()

            if (!oHTMLTable) {
                throw new Error("Table object could not be found.  " + sPlainHTMLTableID);
            } else if ((!oFormData) || (typeof oFormData !== "object") || (Array.isArray(oFormData)) || (Object.keys(oFormData).length === 0)) {
                throw new Error("oFormData is required and must be populated.  ");
            }

            Object.entries(oFormData).forEach(([oFormDataKey, oFormDataValue]) => {
                oArgumentData.append(oFormDataKey, oFormDataValue);
            });
            
            arrJSONRst = await mGetRecordset(cstURL.GetRecordset, oArgumentData);

            if (arrJSONRst.error) {
                mSetStatus("mOutputPlainHTMLTable:  " + arrJSONRst.message);
            } else if (Array.isArray(arrJSONRst) && arrJSONRst.length === 0) {
                //Empties the table header and body rows
                oHTMLTable.tHead && (oHTMLTable.tHead.innerHTML = '');
                oHTMLTable.tBodies[0] && (oHTMLTable.tBodies[0].innerHTML = '');

                document.querySelector('#' + sPlainHTMLTableID + ' thead').innerHTML = "";
                document.querySelector('#' + sPlainHTMLTableID + ' tbody').innerHTML = "";                

            } else {
                const dtTestDate = arrJSONRst[0].Test_Date;
                
                const [dDatePart, tTimePart, sAMPM] = dtTestDate.split(" ");
                
                giPlanStartDate = dDatePart;

                //Get The Test Time in a Javascript Global Variable
                giPlanStartTime24Hr = tTimePart;
                giPlanStartTime = tTimePart.substring(0, 5) + " " + sAMPM; 


                await mBuildPlainHTMLTables(iPlainHTMLTableID, arrJSONRst)
            }
                
        } catch (oError) {
            mSetStatus("mOutputPlainHTMLTable", oError);
        }
    }
    
    //If a year is selected call the php page to retrieve the db values while passing in the selected year for the where clause
    //of the SQL statement
    async function mOutputTable(arrHTMLControlTable, iSubSelectedValue = -1) {
        // Declare variables
        let sTableName = "";
        let sSearchRow = "";        
        let iActionID = -1;
        let bOutputTable = false;
        let arrPageRst = [];
        let arrHTMLControl = [];
        let oHTMLTable = null;
        let oHTMLSearchDiv = null;
        let oArgumentData = null;

        try {
            
            
            mClearTable(cstTableType.AddTable);
            mClearTable(cstTableType.StandardTable);
            //mClearTable(cstTableType.FilterTable);

            if (giPlatformID !== -1 && giYearID !== -1 && giPageID !== -1) {
              
                iActionID = cstGetRecordsetAction[gsPageName];
                sTableName = cstTableName[gsPageName];
                
                if ((giPageID === dctPage["Hour By Hour Plan"] || giPageID === dctPage["Plan Template"]) && (iSubSelectedValue > 0)) {
                    bOutputTable = true;
                } else if (giPageID !== dctPage["Hour By Hour Plan"] && giPageID !== dctPage["Plan Template"]) {
                    bOutputTable = true;
                }

                if (bOutputTable === true)  {

                    oArgumentData = new FormData();
                    oArgumentData.append("SQL:TableName", sTableName);
                    oArgumentData.append("PlatformID", giPlatformID);
                    oArgumentData.append("YearID", giYearID);
                    oArgumentData.append("ActionID", iActionID);                    

                    
                    if ((iSubSelectedValue > 0) && (giPageID === dctPage["Hour By Hour Plan"])) {
                        oArgumentData.append("TestID", iSubSelectedValue);
                    } else if ((iSubSelectedValue > 0) && (giPageID === dctPage["Plan Template"])) {
                        oArgumentData.append("TestType", iSubSelectedValue);
                    }

                    arrPageDataRst = await mGetRecordset(cstURL.GetRecordset, oArgumentData);
                    
                    //Ouput Add Table
                    oHTMLTable = mGetHTMLTable(cstTableType.AddTable);
                    await mBuildTable(oHTMLTable, arrHTMLControlTable);

                    //Output standard Table If Data is Found
                    if ((!arrPageDataRst || arrPageDataRst.length !== 0) && (!arrHTMLControlTable || arrHTMLControlTable.length !== 0)) {
                        oHTMLTable = mGetHTMLTable(cstTableType.StandardTable);

                        await mBuildTable(oHTMLTable, arrHTMLControlTable, arrPageDataRst);
                        await mOutputToolbar(arrHTMLControlTable);
                        
                        
                    }

                    if (giPageID === dctPage["Hour By Hour Plan"]) {
                        await mBindHxHPlanControlListeners();
                    }
		    
                }

                await mFinalizePage();
                gbFirstError = true;		                    
                
            }

        } catch (oError) {
            mSetStatus("mOutputTable", oError);
        }
    }

    function mClearTable(iTableType) {
        try {
            
            let oHTMLTable = mGetHTMLTable(iTableType);
            let sTableID = cstTableTypeInText[iTableType];

            if (oHTMLTable) {

                const otHead = oHTMLTable.querySelector("#" + sTableID + " thead");
                if (otHead) {
                    otHead.replaceChildren(); 
                }

                const otBody = oHTMLTable.querySelector("#" + sTableID + " tbody");
                if (otBody) {
                    otBody.replaceChildren(); 
                }
                
            }

        } catch (oError) {
            mSetStatus("mClearTable", oError);
        }
    }

    async function mBuildTable(oHTMLTable, arrHTMLControlRst, arrPageDataRst = []) {

        try {

            let sOutputString = "";
            let oHeaderTableRow = null;
            let bOpenHTMLTableRow = false;
            let bCloseHTMLTableRow = false;
            let iRowType = 1;
            let bHTMLCellWrap = true;
            let oDOMTableRow = null;
            let bNewRow = false;
            let oHTMLControl = null;
            let oDOMOutput = null;
            

                //Output the Column Header Row
                bNewRow = true;
                bNewRow = await mOutputDOM(oHTMLTable, cstRowType.ColumnHeader, giUserTypeID, arrHTMLControlRst, bNewRow);

                //Output the Add Table Detail Row
                if (oHTMLTable.TableType === cstTableType.AddTable) {

                    bNewRow = true;
                    bNewRow = await mOutputDOM(oHTMLTable, cstRowType.Detail, giUserTypeID, arrHTMLControlRst, bNewRow);

                } else if (oHTMLTable.TableType === cstTableType.StandardTable) {
                    //Output the Standard Table 
                    for (const arrPageDataRow of arrPageDataRst) {
                        bNewRow = true;
                        
                        //This block uses the row type from the db if there, and if not, sets the row type to detail.  The first block
                        //applies to the hxh plan that has a header and detail row in the db table
                        if (arrPageDataRow.Row_Type != null) { 
                            iRowType = Number(arrPageDataRow.Row_Type);
                        } else {
                            iRowType = Number(cstRowType.Detail);
                        }

                        bNewRow = await mOutputDOM(oHTMLTable, iRowType, giUserTypeID, arrHTMLControlRst, bNewRow, arrPageDataRow);
                    }

                }

        } catch (oError) {
            mSetStatus("mBuildTable", oError);
        }
    }

    async function mOutputDOM(oHTMLTable, iRowType, iUserTypeID, arrHTMLControlRst, bNewRow, arrPageDataRow = []) {
            try {

                let oDOMOuput = null;
                let oHTMLTableRow = null;
      

                oDOMOutput = new cDOMManagement(oHTMLTable, oHTMLTable.TableType, iRowType);
                oDOMOutput.UserTypeID = giUserTypeID; //set the User Type ID
                oDOMOutput.PageRowData = arrPageDataRow; //Set the recordset data for the page

                for (const arrControlRow of arrHTMLControlRst) {

                    oHTMLControl = mGetHTMLControlSettingObject(arrControlRow);
                    oHTMLControl.LookupArray = arrLookupTable;  //arrLookupTable is global

   
                    oDOMOutput.ControlClass = oHTMLControl;

                    if (bNewRow === true) {
                        oHTMLTableRow = await oDOMOutput.mCreateTableRow(iUserTypeID);
                    }

                    await oDOMOutput.mCreateTableCell();

                    oHTMLControl = null;
                    bNewRow = false;

                }

                oDOMOutput.mAddPaddingCell(oHTMLTableRow);

                oDOMOutput = null;

                return (bNewRow);

            } catch (oError) {
                mSetStatus("mOutputDOM", oError);
            }
    }




    async function mBuildPlainHTMLTables(iTableType, oData) {

        try {

            //Load the Array
            let arrPlainHTMLTableConfig = [];
            let sPoundPlainHTMLID = "";

            arrPlainHTMLTableConfig = mGetPlainHTMLTableArray(iTableType);

            sPoundPlainHTMLID = '#' + cstTableTypeInText[iTableType];
            
            //Clear the plain table
            document.querySelector(sPoundPlainHTMLID + ' thead').innerHTML = "";
            document.querySelector(sPoundPlainHTMLID + ' tbody').innerHTML = "";

            //Output Plain Table Header Row
            mOutputPlainHeaderRow(iTableType);

            //Output Plain Table Body Rows
            Object.entries(oData).forEach(([oFieldName, oFieldValue]) => {
                mOutputPlainHTMLTableBodyRow(iTableType, oFieldValue);
            });

            //Set Plain Table CSS Systen
            
            const oHTMLTable = mGetHTMLTable(iTableType);
                /*oHTMLTable.style.border = "1px solid #000";
                oHTMLTable.style.marginLeft = "auto";
                oHTMLTable.style.marginRight = "25px";*/
            
            //Set Plain Cell  CSS Systen                
           /* document.querySelectorAll(sPoundPlainHTMLID + ' th, ' + sPoundPlainHTMLID + ' td')
                .forEach(oCell => {
                    oCell.style.border = "1px solid #000";
            });*/

        } catch (oError) {
            mSetStatus("mBuildPlainHTMLTables", oError);
        }
    }


    function mOutputPlainHTMLTableBodyRow(iTableType, oFieldValue) {

        try {
            
            let sPlainHTMLTableID = "";
            let sCellWidth = "";
            let sCellAlign = "";
            let sDBColumnName = "";
            let sBodyRowOutput = "";
            let sTempOutputString = "";
            let vFieldValue = "";
            let arrPlainHTMLTableConfig = [];            

            arrPlainHTMLTableConfig = mGetPlainHTMLTableArray(iTableType);
            sPlainHTMLTableID = cstTableTypeInText[iTableType];

            sBodyRowOutput = mBuildString("", "<tr>")
            
                arrPlainHTMLTableConfig.forEach(function(oItem) {
                    //HTML_Table_Header_Name
                    
                    sCellWidth = oItem.HTML_Table_Cell_Width;
                    sCellAlign = oItem.HTML_Table_Cell_Align;
                    sDBColumnName = oItem.DB_Table_Column_Name;
                    
                    //sTempOutputString = `<td style='width:${sCellWidth};text-align:${sCellAlign}'>`;
                    sTempOutputString = "<td>";

                    sBodyRowOutput = mBuildString(sBodyRowOutput, sTempOutputString);
                        vFieldValue = oFieldValue[sDBColumnName] ?? "";
                        sBodyRowOutput = mBuildString(sBodyRowOutput, String(vFieldValue));  
                    sBodyRowOutput = mBuildString(sBodyRowOutput, "</td>");  
                });
            
            sBodyRowOutput = mBuildString(sBodyRowOutput, "</tr>")

            document.querySelector("#" + sPlainHTMLTableID + " tbody").insertAdjacentHTML("beforeend", sBodyRowOutput);            
          

        } catch (oError) {
            mSetStatus("mOutputPlainHTMLTableBodyRow", oError);
        }            
    }

    function mOutputPlainHeaderRow(iTableType) {

        try {

            let sPlainHTMLTableID = "";
            let sCellWidth = "";
            let sCellAlign = "";
            let sHeaderRowOutput = "";
            let sTempOutputString = "";
            let arrPlainHTMLTableConfig = [];

            arrPlainHTMLTableConfig = mGetPlainHTMLTableArray(iTableType);
            sPlainHTMLTableID = cstTableTypeInText[iTableType];

            sHeaderRowOutput = mBuildString("", "<tr>")
            
                arrPlainHTMLTableConfig.forEach(function(oItem) {
                    //HTML_Table_Header_Name
                    
                    //sCellWidth = oItem.HTML_Table_Cell_Width;
                    //sCellAlign = oItem.HTML_Table_Cell_Align;
                    //sTempOutputString = `<th style='width:${sCellWidth};text-align:${sCellAlign}'>`;
                    sTempOutputString = "<th>";

                    sHeaderRowOutput = mBuildString(sHeaderRowOutput, sTempOutputString);
                        sHeaderRowOutput = mBuildString(sHeaderRowOutput, oItem.HTML_Table_Header_Name);  
                    sHeaderRowOutput = mBuildString(sHeaderRowOutput, "</th>");  
                });
            
            sHeaderRowOutput = mBuildString(sHeaderRowOutput, "</tr>")

            sPlainHTMLTableID = "#" + sPlainHTMLTableID;
            document.querySelector(sPlainHTMLTableID + " thead").insertAdjacentHTML("beforeend", sHeaderRowOutput);

        } catch (oError) {
            mSetStatus("mOutputPlainHeaderRow", oError);
        }            
    }

    async function mOutputHxHPlan(iPlatformID, iTestID) {

            let oFormData = null;

            try {            

                if (iTestID && iTestID > 0) {
                    oFormData = {
                        ActionID: cstGetRecordsetAction["Test_Summary"],
                        PlatformID: iPlatformID,
                        YearID: giYearID,
                        TestID: iTestID
                    };
                    

                    await mOutputPlainHTMLTable(cstTableType.TestSummaryTable, oFormData);
                    await mOutputTable(arrHTMLControlTable, iTestID);
                    await mUpdatePlannedTimes(); 
                    await mSetTaskGoStopButtonState();
		    
		    gbFirstError = true;
                }

            } catch (oError) {
                mSetStatus("mOutputHxHPlan", oError);
            }                           
    }

    async function mOutputDRDesignDiagram(iPlatformID, iYearID, iPageID) {

            try {

                const ctlCRDesignDiagram = document.getElementsByName("DR_Design_Diagram")[0];

                let sPageName = "";
                let arrRecordset = null;
                let oArgumentData = new FormData();
                
                if (ctlCRDesignDiagram) {

                    sPageName = await mGetKeyFromValue(dctPage, iPageID);
                
                    if (sPageName === "Design") {
                        
                        oArgumentData.append("ActionID", cstGetRecordsetAction["Design Diagram"]);
                        oArgumentData.append("PlatformID", iPlatformID);
                        oArgumentData.append("YearID", iYearID);

                        arrRecordset = await mGetRecordset(cstURL.GetRecordset, oArgumentData);

                        if (arrRecordset && Array.isArray(arrRecordset) && arrRecordset.length > 0) {

                            document.getElementsByName("DR_Design_Diagram")[0].innerHTML = "";
                            arrRecordset.forEach(arrRow => {
                                ctlCRDesignDiagram.innerHTML += arrRow.Diagram_Link + "<br/>";
                            });

                        } else {

                            ctlCRDesignDiagram.innerHTML = "";
                            
                        }

                    } else {

                        ctlCRDesignDiagram.innerHTML = "";

                    }
                        
                }
                
            } catch (oError) {
                mSetStatus("mOutputDRDesignDiagram", oError);
            }
        }        