<?php 

    header('Content-Type: application/json');


    try {

        require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

        mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));  
        
        $iApplicationID = mGetApplicationID($_SERVER['REQUEST_URI']);
        $bLockPlanTiming = null;
        $arrFields = [];
        $arrValues = [];
        $oBeaconData = null;
        $oDB = null;
        
        $bSuccess = false;


        $oBeaconData = json_decode(file_get_contents("php://input"), true);

        // Validate input
        if (!isset($oBeaconData["Test_ID"]) || !isset($oBeaconData["Locked_By_ID"]) || !isset($oBeaconData["Lock_Plan"])) {
            throw new Exception("No Test ID, Lock Action and/or Locked By ID received.");
        } else {

            $bLockPlanTiming = (bool)$oBeaconData["Lock_Plan"];

            if ($bLockPlanTiming === true) {
                $arrValues[] = intval($oBeaconData["Locked_By_ID"]);
            } else if ($bLockPlanTiming === false) {
                $arrValues[] = -1;
            }

            $arrValues[] = intval($oBeaconData["Test_ID"]);    
        }

            $sSQL = "UPDATE tb_Plan SET Locked_By_ID = ? WHERE Test_ID = ?";
            
            if ($bLockPlanTiming === false) {
                $arrValues[] = intval($oBeaconData["Locked_By_ID"]);  
                $sSQL = $sSQL . " AND Locked_By_ID = ?";
            }
        
        $oDB = new cDatabase();
            
           $iReturnValue = $oDB->mExecuteSQL($iApplicationID, enuCommandType::Update, "", $arrFields, $arrValues, $sSQL);
        
        $oDB = null;

        $bSuccess = true;
        
	    echo json_encode(['success' => $bSuccess]);
        
    } catch (Throwable $oError) {
        mLogString("Toggle_Plan_Timing_Lock: " . $oError->getMessage());
        exit;
    }


    
?>
