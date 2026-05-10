<?php 

    header('Content-Type: application/json');


    try {

        require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

        mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));  
        
        $iApplicationID = mGetApplicationID($_SERVER['REQUEST_URI']);
        $iReturnValue = -1;
        $arrFields = [];
        $arrValues = [];
        $oDB = null;
        
        
        $bSuccess = false;

        if (isset($_POST["arrValues"])) {
            $arrValues = $_POST["arrValues"];
            $arrValues = json_decode($arrValues, true);
        }

        
        $oDB = new cDatabase();

            $sSQL = "INSERT INTO tb_Plan_Template_Backup (
                        Backup_Creator
                		,Platform_ID
                        ,Year_ID
                        ,Row_Type
                        ,Row_ID
                        ,Application_ID
                        ,Component_ID
                        ,Test_Type
                        ,DR_Plan_Phase
                        ,Task_Name
                        ,Task_Details
                        ,Task_Status
                        ,Jira_Rainier_Reference
                        ,Resource_Team
                        ,Planned_Resource
                        ,Planned_Duration
                        ,Predecessor
                        ,Batch
                        ,Sequence_ID)
                        SELECT 
                            ?,
                            Platform_ID,
                            Year_ID,                
                            Row_Type, 
                            Row_ID, 
                            Application_ID, 
                            Component_ID, 
                            Test_Type, 
                            DR_Plan_Phase, 
                            Task_Name, 
                            Task_Details, 
                            Task_Status, 
                            Jira_Rainier_Reference, 
                            Resource_Team, 
                            Planned_Resource, 
                            Planned_Duration, 
                            Predecessor, 
                            Batch, 
                            Sequence_ID
                        FROM tb_Plan_Template 
                        WHERE Platform_ID = ? AND Year_ID = ?
                            AND ? IN (
                                SELECT LTRIM(RTRIM(value))
                                FROM STRING_SPLIT(Test_Type, ',')
                            )";
                

           $iReturnValue = $oDB->mExecuteSQL($iApplicationID, enuCommandType::Insert, "", $arrFields, $arrValues, $sSQL);
        
        $oDB = null;

        $bSuccess = true;
	    echo json_encode(['success' => $bSuccess]);
        
    } catch (Throwable $oError) {
        $bSuccess = false;
        mLogString("Insert_Plan_Template_Backup: " . $oError->getMessage());

        echo json_encode([
            'error'            => true,
            'code'             => $oError->getCode() ?: 5000,
            'message'          => $oError->getMessage(),
            'errornumber' => isset($oError->errorInfo[1]) ? $oError->errorInfo[1] : null,
            'details'          => $oError->getTraceAsString(),
            'recordsAffected'  => $iReturnValue
        ], JSON_PRETTY_PRINT);
    }


    
?>
