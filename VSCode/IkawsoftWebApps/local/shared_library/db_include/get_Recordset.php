<?php 
    header('Content-Type: application/json');


    try {

        require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

        mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));  
        
            $sSQL = "";
            $iActionID = -1;
            $iApplicationID = -1;
            $iReturnValue = -1;
            $arrPostVariableArray = [];
            $arrSQLParameter = [];
            $arrSessionSQLRowMatch = [];
            $oDB = null;

            //throw new Exception("Division by zero is not allowed.");

            $arrPostVariableArray = mGetPostVariablesArray($_POST);
            $iActionID = (int)$arrPostVariableArray["ActionID"];
            
            if (isset($_POST['ApplicationID'])) {
                $iApplicationID = (int)$arrPostVariableArray["ApplicationID"];

                if ($iApplicationID < 0) {
                    throw new Exception("Application ID Must be 0 or greater");
                }
            } else {
                throw new Exception("Application ID is required.");
            }

            $sSQL = mGetSQLStatementFromSessionArray($iActionID, $arrSessionSQLRowMatch);

            if (strlen(trim($sSQL)) > 0) {

                $sSQL = $arrSessionSQLRowMatch["SQL"];

                mGetSQLTableParameterArray($arrSQLParameter, $arrPostVariableArray, $arrSessionSQLRowMatch);
                
                //mLogString(print_r($arrPostVariableArray, true));
                //mLogString(print_r($arrSQLParameter, true));
                //mLogString(print_r($arrSessionSQLRowMatch, true));
                $sSQL = mReplaceSQLWildcard($sSQL, $arrPostVariableArray);
                
                

                $oDB = new cDatabase();
                //mLogString($sSQL);
                $arrRecordsetArray = $oDB->mGetRecordsetArray($iApplicationID, $sSQL, $arrSQLParameter);

                $jsonData = json_encode($arrRecordsetArray, JSON_THROW_ON_ERROR);

                echo($jsonData);

            } else {
                throw new Exception("SQL statement was not found for Action ID: " . (string)$iActionID);
            }


    } catch (Throwable $oError) {
        $sLogMessage = "";

        $sLogMessage = "Error get_Recordset.php: oError Object" . $oError->getMessage();
        $sLogMessage = $sLogMessage . " Error Location: {$oError->getFile()} on line {$oError->getLine()}";
        $sLogMessage = $sLogMessage . " Post Variables: " . print_r($arrPostVariableArray, true);
        $sLogMessage = $sLogMessage . " SQL String " . Chr(13) . $sSQL;
        $sLogMessage = $sLogMessage . " arrSQLParameter passed into mGetRecordsetArray" . Chr(13) . print_r($arrSQLParameter, true);

        if (!empty($arrRecordsetArray)) {
           $sLogMessage = $sLogMessage . " Array Returned After mGetRecordsetArrary Call: " . print_r($arrRecordsetArray, true);
        }
        mLogString($sLogMessage);

        echo json_encode([
            'error'            => true,
            'code'             => $oError->getCode() ?: 5000,
            'message'          => $oError->getMessage(),
            'errornumber' => isset($oError->errorInfo[1]) ? $oError->errorInfo[1] : null,
            'details'          => $oError->getTraceAsString(),
            'recordsAffected'  => $iReturnValue
        ], JSON_PRETTY_PRINT);

    }

    //If there is a replacement variable in the SQL Statement, this replaces these {TableName} with the Value Passed in With oArgumentData
    function mReplaceSQLWildcard($sSQL, $arrPostVariableArray) {

        try {

            $sSearchValue = "";
            $sReplacementValue = "";

            foreach ($arrPostVariableArray as $sKey => $sValue) {
                
                if (strtoupper(substr($sKey, 0, 4)) === "SQL:") {
                    
                    $sSearchValue = "{" . substr($sKey, 4) . "}";
                    $sReplacementValue = $sValue;
                
                    $sSQL = str_ireplace($sSearchValue, $sReplacementValue, $sSQL);
                }
            }
            
            return $sSQL;

        } catch (Throwable $oError) {
            throw($oError);
        }               

    }

    //This uses the Post Variable Array to populate the Parameters that will be passed in as an array argument for the ? values in the sql statement
    function mGetSQLTableParameterArray(&$arrParameter, $arrPostVariableArray, $arrSQLRowMatch) {

        try {
                $arrSQLTableParameter = [];
                
                if (!empty($arrSQLRowMatch["Argument"])) {
                    $arrSQLTableParameter = explode(",", $arrSQLRowMatch["Argument"]);
                    $arrSQLTableParameter = array_map('trim', $arrSQLTableParameter);

                    foreach ($arrSQLTableParameter as $arrSQLTableItem) {

                        if (isset($arrPostVariableArray[$arrSQLTableItem]) && (strlen($arrPostVariableArray[$arrSQLTableItem]) > 0)) {
                            $arrParameter[] = $arrPostVariableArray[$arrSQLTableItem];
                        } else {
                            throw new Exception("mGetParameterArray:  Post Variable could not be found when using the Arguments values from tb_SQL_Script.  Action ID:" . $arrSQLRowMatch["Action_ID"] . " Item: " . $arrSQLTableItem);
                        }

                    }
                }


        } catch (Throwable $oError) {
            throw($oError);
        }        
    }

    function mSetAddTestID(&$iTestID, $arrAddToArray, $iActionID = 0) {

        try {

                //If set, populate the TestID Variable
                if (isset($_POST["TestID"])) {
                    $iTestID = (int)$_POST["TestID"];

                    $arrAddToArray = [
                        0 => $iTestID
                    ];
                    
                } else {
                    throw new Exception("Test ID is required to be provided for this Action ID.  " . $iActionID);
                }

                return $arrAddToArray;

        } catch (Throwable $oError) {
            throw($oError);
        }

    }

    //This Function Takes The Post Variables and Puts Them Into An Array
    function mGetPostVariablesArray($oPost) {

        try {

            $arrPostVariable = [];

            foreach ($oPost as $sPostVariableID => $sPostVariableValue) {
                    
                    $arrPostVariable[$sPostVariableID] = htmlspecialchars($sPostVariableValue);
                  
            }
            
            return $arrPostVariable;

        } catch (Throwable $oError) {
            throw($oError);
        }

    }
?>
