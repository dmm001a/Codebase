
<?php 

    try {

        require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

        mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));  

        //insert execute logic
        $eCommandType = enuCommandType::nonSpecified;
        $sTableName = "";
        $sEmptySQLString = "";
        $sIdentityColumnName = "";
        $iApplicationID  = -1;
        $arrFieldName = [];
        $arrFieldValue = [];
        
        $iReturnValue = 0;

        if (isset($_POST['ApplicationID'])) {
            $iApplicationID = (int)$_POST["ApplicationID"];

            if ($iApplicationID < 0) {
                throw new Exception("Application ID Must be 0 or greater");
            }
        } else {
            throw new Exception("Application ID is required.");
        }

        if (isset($_POST['CommandType']) && isset($_POST['TableName']) && isset($_POST['arrFields']) && isset($_POST['arrValues'])) {

            
            $eCommandType = enuCommandType::tryFrom((int)$_POST['CommandType']);

            if ($eCommandType === enuCommandType::nonSpecified) {
                throw new Exception("Command Type must be set.");
            }

            $sTableName = $_POST['TableName'];

            if ($eCommandType === enuCommandType::Insert) {

                //Handle insert on table with identity
                $arrRowIDIdentityTables = ['TB_DESIGN', 'TB_DOCUMENTATION', 'TB_INFRASTRUCTURE', 'TB_INVENTORY', 'TB_ISSUE_LOG', 'TB_PLAN', 'TB_TEST'];
                if (in_array(strtoupper($sTableName), $arrRowIDIdentityTables, true)) {
                    $sIdentityColumnName = "Row_ID";
                } else {
                    $sIdentityColumnName = "";            
                }
            }
                
            
            $arrFieldName = $_POST['arrFields'];
            $arrFieldName = json_decode($arrFieldName, true);


            $arrFieldValue = $_POST['arrValues'];
            $arrFieldValue = json_decode($arrFieldValue, true);
            $arrFieldValue = mNormalizeIsoValues($arrFieldValue);

        } else {
            throw new Exception("Command Type, Table Name, Fields and Values are required.");
        }
        
                $oDB = new cDatabase();
                
                    $iReturnValue = $oDB->mExecuteSQL($iApplicationID, $eCommandType, $sTableName, $arrFieldName, $arrFieldValue, $sEmptySQLString, $sIdentityColumnName);

                if ($iReturnValue  < 1) {
                    mLogString("No records affected by SQL execution.");
                }

                ob_end_clean(); // Discard any buffered output
                echo json_encode([
                    'error'            => false,
                    'ReturnValue'  => $iReturnValue
                ]);
            
            $oDB = null;
        
    } catch (Throwable $oError) {
        $iReturnValue = -1;
        mLogString("Error From the page not the class");
        mLogString("Error Command Type " . print_r($eCommandType, true));
        mLogString("Error Table Name " . print_r($sTableName, true));
        mLogString("Error Field String " . print_r($arrFieldName, true));
        mLogString("Error Value String " . print_r($arrFieldValue, true));
        mLogString("Error Message: " . $oError->getMessage());

        ob_end_clean(); // Discard any buffered output
        http_response_code(500);
        
        echo json_encode([
            'error'            => true,
            'code'             => $oError->getCode() ?: 5000,
            'message'          => $oError->getMessage(),
            'errornumber' => isset($oError->errorInfo[1]) ? $oError->errorInfo[1] : null,
            'details'          => $oError->getTraceAsString(),
            'recordsAffected'  => $iReturnValue
        ], JSON_PRETTY_PRINT);


    }


    function mNormalizeIsoValues($arrValues) {
        // Declare variables
        $iIndex = 0;
        $sValue = "";
        $sNormalized = "";

        try {
            if (!is_array($arrValues)) {
                throw new Exception("mNormalizeIsoValues: Input is not an array.");
            }

            foreach ($arrValues as $iIndex => $sValue) {
                if (is_string($sValue)) {
                    if (mIsIso($sValue)) {
                        $sNormalized = str_ireplace("T", " ", $sValue);

                        // Pad seconds if missing
                        if (preg_match('/^\d{4}-\d{2}-\d{2} \d{2}:\d{2}$/', $sNormalized)) {
                            $sNormalized = $sNormalized . ":00";
                        }

                        $arrValues[$iIndex] = $sNormalized;
                    }
                }
            }

            return $arrValues;
        }
        catch (Throwable $oEx) {
            throw new Exception("mNormalizeIsoValues: " . $oEx->getMessage(), 0, $oEx);
        }
    }
?>
