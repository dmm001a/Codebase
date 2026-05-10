<?php 

    header('Content-Type: application/json');


    try {

        require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

        mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));  
        
        $iApplicationID = mGetApplicationID($_SERVER['REQUEST_URI']);
        $iReturnValue = -1;
        $bSuccess = false;
        $arrFields = [];
        $arrValues = [];        
        $oDB = null;

        if (isset($_POST["arrValues"])) {
            $arrValues = $_POST["arrValues"];
            $arrValues = json_decode($arrValues, true);
        }

        
        $oDB = new cDatabase();

            $sSQL = "INSERT INTO tb_Documentation (
                Platform_ID,
                Year_ID,
                Application_ID,
                Document_ID,
                Document_Status,
                Sequence_ID)
                SELECT
                    ?,                          -- Platform_ID
                    ?,                          -- Year_ID
                    LookupApplication.Lookup_ID AS Application_ID,
                    LookupDocument.Lookup_ID AS Document_ID,
                    230 AS Document_Status,
                    ROW_NUMBER() OVER (ORDER BY LookupApplication.Lookup_ID, LookupDocument.Lookup_ID) AS Sequence_ID
                FROM [Ikawsoft_Central].[dbo].tb_Lookup LookupApplication
                CROSS JOIN [Ikawsoft_Central].[dbo].tb_Lookup LookupDocument
                WHERE LookupApplication.Platform_ID = ?
                AND LookupApplication.Category = 'Application'
                AND LookupDocument.Category = 'DR_Document'";

            $iReturnValue = $oDB->mExecuteSQL($iApplicationID, enuCommandType::Insert, "", $arrFields, $arrValues, $sSQL);
        
        $oDB = null;

        $bSuccess = true;
	    echo json_encode(['success' => $bSuccess]);
        
    } catch (Throwable $oError) {
        $bSuccess = false;
        mLogString("bulk_DR_Document_Insert: " . $oError->getMessage());

        echo json_encode([
            'error'            => true,
            'code'             => $oError->getCode() ?: 5000,
            'message'          => $oError->getMessage(),
            'errornumber' => isset($oError->errorInfo[1]) ? $oError->errorInfo[1] : null,
            'details'          => $oError->getTraceAsString(),
            'recordsAffected'  => $iReturnValue,
        ], JSON_PRETTY_PRINT);
    }


    
?>
