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

            $sSQL = "INSERT INTO tb_Plan_Backup (
                        Backup_Creator
                        ,Platform_ID
                        ,Row_ID
                        ,Year_ID
                        ,Test_ID
                        ,Row_Type
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
                        ,Start_Date
                        ,Planned_Start_Time
                        ,Planned_End_Time
                        ,Actual_Start_Time
                        ,Actual_End_Time
                        ,Predecessor
                        ,Planned_Start_Date_and_Time_CT
                        ,Planned_End_Date_and_Time_CT
                        ,Actual_Start_Date_and_Time_CT
                        ,Actual_End_Date_and_Time_CT
                        ,Batch
                        ,Locked_By_ID
                        ,Sequence_ID)
                        SELECT 
                            ?
                            ,Platform_ID
                            ,Row_ID
                            ,Year_ID
                            ,Test_ID
                            ,Row_Type
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
                            ,Start_Date
                            ,Planned_Start_Time
                            ,Planned_End_Time
                            ,Actual_Start_Time
                            ,Actual_End_Time
                            ,Predecessor
                            ,Planned_Start_Date_and_Time_CT
                            ,Planned_End_Date_and_Time_CT
                            ,Actual_Start_Date_and_Time_CT
                            ,Actual_End_Date_and_Time_CT
                            ,Batch
                            ,Locked_By_ID
                            ,Sequence_ID
                        FROM tb_Plan
                        WHERE Platform_ID = ? AND Year_ID = ?
                            AND Test_ID = ?";
                
            
           $iReturnValue = $oDB->mExecuteSQL($iApplicationID, enuCommandType::Insert, "", $arrFields, $arrValues, $sSQL);
           
        
        $oDB = null;

        $bSuccess = true;
	    echo json_encode(['success' => $bSuccess]);
        
    } catch (Throwable $oError) {
        $bSuccess = false;
        mLogString("Insert_Plan_Backup: " . $oError->getMessage());

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
