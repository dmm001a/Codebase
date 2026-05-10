<?php 

    header('Content-Type: application/json');


    try {

        require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

        mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));  
        
            $iPlatformID = 0;
            $iYearID = 0;
            $iTestID = 0;
            $iTestTypeID = 0;
            $iReturnValue = -1;
            $bSuccess = false;
            $arrField = [];
            $arrValue = [];
            $arrRecordsetArrayReturn = null;        

            $oDB = null;

            define("cstGetTestType", 19);

            $oDB = new cDatabase();

            //Get Application ID
            $iApplicationID = mGetApplicationID($_SERVER['REQUEST_URI']);
    
                //Get Platform ID
                if (isset($_POST["PlatformID"])) {
                    $iPlatformID = (int)$_POST["PlatformID"];
                } else {
                    throw new Exception("Platform ID is required to be provided.", 1000);
                }

                //Get Year ID
                if (isset($_POST["YearID"])) {
                    $iYearID = (int)$_POST["YearID"];
                } else {
                    throw new Exception("Test ID is required to be provided.", 1000);
                }                

                //Get Test ID
                if (isset($_POST["TestID"])) {
                    $iTestID = (int)$_POST["TestID"];
                } else {
                    throw new Exception("Test ID is required to be provided.", 1000);
                }      

                //Get Test Type ID
                if (isset($_POST["TestTypeID"])) {
                    $iTestTypeID = (int)$_POST["TestTypeID"];
                } else {
                    throw new Exception("Test Type ID is required to be provided.", 1000);
                }                


                $arrValue = [];                        
                $arrValue[] = $iTestID; 
                $arrValue[] = $iPlatformID; 
                $arrValue[] = $iYearID;
                $arrValue[] = $iTestTypeID;


            $sSQL  = "INSERT INTO tb_Plan (";
                $sSQL .= "Platform_ID, Test_ID, Year_ID, Row_Type, Row_ID, Application_ID, Component_ID, ";
                $sSQL .= "Test_Type, DR_Plan_Phase, Task_Name, Task_Details, Task_Status, Jira_Rainier_Reference, ";
                $sSQL .= "Resource_Team, Planned_Resource, Planned_Duration, Predecessor, ";
                $sSQL .= "Planned_Start_Date_and_Time_CT, Planned_End_Date_and_Time_CT, ";
                $sSQL .= "Actual_Start_Date_and_Time_CT, Actual_End_Date_and_Time_CT, Batch, Sequence_ID) ";
            $sSQL .= "SELECT ";
                $sSQL .= "Platform_ID, ";
                $sSQL .= "? AS Test_ID, ";
                $sSQL .= "Year_ID, ";
                $sSQL .= "Row_Type, Row_ID, Application_ID, Component_ID,  ";
                $sSQL .= "Test_Type, DR_Plan_Phase, Task_Name, Task_Details, Task_Status, Jira_Rainier_Reference, ";
                $sSQL .= "Resource_Team, Planned_Resource, Planned_Duration, Predecessor, ";
                $sSQL .= "Planned_Start_Date_and_Time_CT, Planned_End_Date_and_Time_CT, ";
                $sSQL .= "NULL AS Actual_Start_Date_and_Time_CT, NULL AS Actual_End_Date_and_Time_CT, Batch, ROW_NUMBER() OVER (ORDER BY Row_ID) ";
                $sSQL .= "FROM tb_Plan_Template WHERE ";
                $sSQL .= " Platform_ID = ?";
                $sSQL .= " AND Year_ID = ?";
                $sSQL .= " AND EXISTS (";
                $sSQL .= "SELECT 1 ";
                    $sSQL .= " FROM STRING_SPLIT(Test_Type, ',') AS sEachTestType";
                    $sSQL .= " WHERE LTRIM(RTRIM(sEachTestType.value)) = ?";
                $sSQL .= ")";
            

            $iReturnValue = $oDB->mExecuteSQL($iApplicationID, enuCommandType::nonSpecified, "", $arrField, $arrValue, $sSQL);
            
            $oDB = null;
            
            $bSuccess = true;
            echo json_encode(['success' => $bSuccess]);
        

        } catch (Throwable $oError) {
            $bSuccess = false;
            mLogString("Error: Load_Plan_Template: " . $oError->getMessage() . " Trace String: " . $oError->getTraceAsString());

            echo json_encode([
                'error'            => true,
                'code'             => $oError->getCode() ?: 5000,
                'message'          => "Error: " . $oError->getMessage(),
                'errornumber' => isset($oError->errorInfo[1]) ? $oError->errorInfo[1] : null,
                'details'          => $oError->getTraceAsString(),
                'recordsAffected'  => $iReturnValue
                ], JSON_PRETTY_PRINT);

        }


    
?>
