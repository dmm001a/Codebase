

async function mPageExtensibility (iPageID, oExtensibilityArgument) {
    try {

        let sNewIDDisplayMessageName = "";
        let sRecordsetActionName = "";
        let sFieldName = "";
        let sControlName = "";
        let vControlValue = null;
        let iTestID = -1;
        let iNewID = -1;
        let oExecuteSQLResult = null;

        //Variable Init Based on Page
        if (iPageID === dctPage["Test"]) {
            sFieldName = "Test_ID";
            sNewIDDisplayMessageName = "Test ID";
            sRecordsetActionName = "New_Test_ID";
        } else {
            sFieldName = "Row_ID";
            sNewIDDisplayMessageName = "Row ID";
            sRecordsetActionName = "New_Row_ID";
        }
        
        oExtensibilityArgument.KeyFieldName = sFieldName;
        sControlName = "txtGrid" + sFieldName.replaceAll("_", "");
            
        if (oExtensibilityArgument.CommandType === cstExtensibilityType.SQLInsert) {

            //HxH Plan Insert Init
            if (iPageID === dctPage["Hour By Hour Plan"]) {
                oExtensibilityArgument.FieldNames.push("Test_ID");
                oExtensibilityArgument.FieldNames.push("Row_Type");
                oExtensibilityArgument.FieldNames.push("Test_Type");
                oExtensibilityArgument.FieldValues.push(giTestID);
                oExtensibilityArgument.FieldValues.push(cstRowType.Detail);         
                oExtensibilityArgument.FieldValues.push(giTestTypeID);
                iTestID = giTestID;     
            }

            if (iPageID === dctPage["Hour By Hour Plan"] || (iPageID === dctPage["Test"])) {

                iNewID = await mGetNewID(
                    cstURL.GetRecordset, giYearID, gsTableName, cstGetRecordsetAction[sRecordsetActionName], 
                    oExtensibilityArgument.FieldNames, oExtensibilityArgument.FieldValues, iTestID
                );

                
                if (Number(iNewID) === -1) {
                    throw new Error("New " + sNewIDDisplayMessageName + " was not successfully retrieved.");
                }  else {
                    oExtensibilityArgument.KeyFieldValue = iNewID;
                    oExtensibilityArgument.NewInsertID = iNewID;                    
                }
            }

        } else if (oExtensibilityArgument.CommandType === cstExtensibilityType.UpdatePlannedTimes && iPageID === dctPage["Hour By Hour Plan"]) {

            await mUpdatePlannedTimes();

        } else if (oExtensibilityArgument.CommandType === cstExtensibilityType.GetTestID && iPageID === dctPage["Test"]) {       
            
                //iRowID = mGetInputControlValueByControlName(oExtensibilityArgument.oRowReference, txtGridRowID, cstDataType.Number);
               
                iTestID = mGetInputControlValueByControlName(oExtensibilityArgument.oRowReference, "txtGridTestID", cstDataType.Number);

               if (iTestID !== null) {

                   oExtensibilityArgument.KeyFieldName = "Test_ID";
                   oExtensibilityArgument.KeyFieldValue = iTestID;
                   oExtensibilityArgument.ReturnValue = "This will delete the related Hour By Hour Plan for this test. ";

               } else {
                   throw new Error("Failed to retrieve Test ID.");
               }

        } else if (oExtensibilityArgument.CommandType === cstExtensibilityType.DeleteTest && iPageID === dctPage["Test"]) {   

                //iTestID = mGetInputControlValueByControlName(oExtensibilityArgument.oRowReference, "txtGridTestID", cstDataType.Number);
                
                oExecuteSQLResult = await mCallExecuteSQL(
                    cstURL.ExecuteSQL,
                    cstCommandType.Delete,
                    "tb_Issue_Log",
                    oExtensibilityArgument.FieldNames,
                    oExtensibilityArgument.FieldValues
                );

                if (oExecuteSQLResult.error === true) {
                    throw new Error(oExecuteSQLResult.message);
                } else {
                    oExecuteSQLResult = null;
                }                
                
                oExecuteSQLResult = await mCallExecuteSQL(
                    cstURL.ExecuteSQL,
                    cstCommandType.Delete,
                    "tb_Plan",
                    oExtensibilityArgument.FieldNames,
                    oExtensibilityArgument.FieldValues
                );
                
                if (oExecuteSQLResult.error === true) {
                    throw new Error(oExecuteSQLResult.message);
                } else {
                    oExecuteSQLResult = null;
                }    

                oExecuteSQLResult = await mCallExecuteSQL(
                    cstURL.ExecuteSQL,
                    cstCommandType.Delete,
                    "tb_Test_Application",
                    oExtensibilityArgument.FieldNames,
                    oExtensibilityArgument.FieldValues
                );

                if (oExecuteSQLResult.error === true) {
                    throw new Error(oExecuteSQLResult.message);
                } else {
                    oExecuteSQLResult = null;
                }                    

        } else if (oExtensibilityArgument.CommandType === cstExtensibilityType.GetRelatedTableKey) {

            vControlValue = oExtensibilityArgument.oRowReference.querySelector(`input[name="${sControlName}"]`)?.value;

            if (mIsNumeric(vControlValue) === true) {
                oExtensibilityArgument.KeyFieldValue  = Number(vControlValue);
            }
        }

    } catch (oError) {
        mSetStatus("mPageExtensibility", oError);
    }          
}