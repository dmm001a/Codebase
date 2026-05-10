<?php

   if (session_status() === PHP_SESSION_NONE) {
      session_name("Disaster_Recovery");
      session_start();
   }


    require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

    mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));    

?>

<!DOCTYPE html>
<html lang="en">
 <head>
   <title>Disaster Recovery Metrics</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <?php echo(mOutputHTMLHead()); ?>
 
    <script>
         function mSetHxHPlanLock(iTestID, bLockValue) {

            try {


                  let iLockValue = -1;
                  let oHTMLTable = null;
                  let arrFieldNames = [];
                  let arrFieldValues = [];

                  iLockValue = mToggleBetweenBitBoolean(bLockValue);

                  arrFieldNames.push("Lock");
                  arrFieldNames.push("Test_ID");

                  arrFieldValues.push(iLockValue);
                  arrFieldValues.push(iTestID);            

                  //Update the Test Lock Value
                  mCallExecuteSQL(cstURL.ExecuteSQL, cstCommandType.Update, "tb_Test", arrFieldNames, arrFieldValues);
                  
                  //Lock The Fieldset
                  oHTMLTable = window.opener.document.getElementById("StandardTable");
                  mSetFieldsetLock("FstLockStandardTable", oHTMLTable, true);

                  alert("Test " + iTestID + " is locked.  You can undo this lock by going to the Test Page and updating the record there.");
                  
            } catch (oError) {
                  mSetStatus("mSetHxHPlanLock", oError);
            }                         
         }

    </script>
 </head>
 
 <body style="margin-top: 0px; margin-left: 0px;">

    <?php


         enum enuMetricStartStop: int {
            case DRDeclaration = 5585;
            case ARPCompletion = 5590;
            case AppDataSyncCompletion = 5620;
            case DBDataSyncCompletion = 5630;
         }

      try {    



         $sTestTypeDescription = "";
         $sApplication = "";
         $sPhaseDescription = "";
         $sStartTime = "";
         $sEndTime = "";
         $sDeclareEndTime = "";
         $sMetricDescription = "";
         $sDuration = null;
         $sSQL = "";         
         $iApplicationID = -1;
         $iPhaseID = -1;         
         $iTestID = -1;
         $bCanLockScores = true;

         $arrRST = [];
         $arrValues = [];       
         $arrOutputValues = [];

         $oDB = null;

         $iApplicationID  = mGetApplicationID($_SERVER['REQUEST_URI']);
         $iTestID = $_GET['TestID'] ?? -1;
         $oDB = new cDatabase();
         
         $arrValues[] = $iTestID;
         $arrValues[] = enuMetricStartStop::DRDeclaration->value; 
         $arrValues[] = enuMetricStartStop::ARPCompletion->value; 
         $arrValues[] = enuMetricStartStop::AppDataSyncCompletion->value; 
         $arrValues[] = enuMetricStartStop::DBDataSyncCompletion->value; 
         $arrValues[] = $iTestID;
         

            $sSQL = $sSQL . " SELECT ";
            $sSQL = $sSQL . "     tb_Lookup_Test_Type.Lookup_Desc as Test_Type,";
            $sSQL = $sSQL . "     tb_Lookup_Application.Lookup_Desc as Test_Application,";
            $sSQL = $sSQL . "     tb_Plan.DR_Plan_Phase,";
            $sSQL = $sSQL . "     tb_Lookup_DR_Plan_Phase.Lookup_Desc as Phase_Description,";
            $sSQL = $sSQL . "     tb_Plan.Actual_Start_Time,";
            $sSQL = $sSQL . "     tb_Plan.Actual_End_Time";
            $sSQL = $sSQL . " FROM tb_Plan AS tb_Plan";
            $sSQL = $sSQL . "     INNER JOIN [Ikawsoft_Central].[dbo].tb_Lookup AS tb_Lookup_Application";
            $sSQL = $sSQL . "         ON tb_Plan.Application_ID = tb_Lookup_Application.Lookup_ID";
            $sSQL = $sSQL . "     INNER JOIN [Ikawsoft_Central].[dbo].tb_Lookup AS tb_Lookup_Test_Type";
            $sSQL = $sSQL . "         ON tb_Plan.Test_Type = tb_Lookup_Test_Type.Lookup_ID";
            $sSQL = $sSQL . "     INNER JOIN [Ikawsoft_Central].[dbo].tb_Lookup AS tb_Lookup_DR_Plan_Phase";
            $sSQL = $sSQL . "         ON tb_Plan.DR_Plan_Phase = tb_Lookup_DR_Plan_Phase.Lookup_ID";
            $sSQL = $sSQL . " WHERE tb_Plan.Test_ID = ?";
            $sSQL = $sSQL . "     AND tb_Plan.Row_Type = 1";
            $sSQL = $sSQL . "     AND tb_Plan.Task_Status <> 5400";
            $sSQL = $sSQL . "     AND tb_Plan.DR_Plan_Phase IN (?, ?, ?, ?)";
            $sSQL = $sSQL . "     AND tb_Plan.Application_ID IN (SELECT Application_ID FROM tb_Test_Application WHERE Test_ID = ?)";
            $sSQL = $sSQL . " ORDER BY";
            $sSQL = $sSQL . "     tb_Lookup_Test_Type.Lookup_Desc,";
            $sSQL = $sSQL . "     tb_Lookup_Application.Lookup_Desc,";
            $sSQL = $sSQL . "     CASE tb_Plan.DR_Plan_Phase";
            $sSQL = $sSQL . "         WHEN 5585 THEN 1";
            $sSQL = $sSQL . "         WHEN 5590 THEN 2";
            $sSQL = $sSQL . "         WHEN 5620 THEN 3";
            $sSQL = $sSQL . "         WHEN 5630 THEN 4";
            $sSQL = $sSQL . "         ELSE 99";
            $sSQL = $sSQL . "     END";

            $arrRST = $oDB->mGetRecordsetArray($iApplicationID, $sSQL, $arrValues);

            foreach ($arrRST as $iRowIndex => $arrRow) {

                  $sApplication = $arrRow["Test_Application"];
                  $sTestTypeDescription = $arrRow["Test_Type"];

                  $iPhaseID = (int)$arrRow["DR_Plan_Phase"];
                  $sPhaseDescription = $arrRow["Phase_Description"];

                  if ($iPhaseID === enuMetricStartStop::DRDeclaration->value) {
                     $sStartTime = $arrRow["Actual_End_Time"];
                     continue;
                  } else if ($iPhaseID === enuMetricStartStop::ARPCompletion->value) {
                     $sEndTime = $arrRow["Actual_End_Time"];
                     $sMetricDescription = "RTA";
                  } else if ($iPhaseID === enuMetricStartStop::AppDataSyncCompletion->value) {
                     $sEndTime = $arrRow["Actual_End_Time"];
                     $sMetricDescription = "RPA (App)";
                  } else if ($iPhaseID === enuMetricStartStop::DBDataSyncCompletion->value) {
                     $sEndTime = $arrRow["Actual_End_Time"];
                     $sMetricDescription = "RPA (DB)";                     
                  }

                  if (mIsValidTime($sStartTime) && mIsValidTime($sEndTime)) {
                     $sDuration = (string) mGetSubtractMetricTimes($sStartTime, $sEndTime);
                  } else {
                     $sDuration = $sMetricDescription . " does not have complete start/end timings in the plan.";
                     $bCanLockScores = false;
                  }

                  $arrOutputValues[] = [
                     "Test Type" => $sTestTypeDescription,
                     "Application" => $sApplication,
                     "Metric" => $sMetricDescription,
                     "Duration (Minutes)" => $sDuration,
                  ];
         
            }

            $oDB->mEchoRecordsetArray($arrOutputValues);
            $oDB = null;

      } catch (Throwable $oError) {
          throw $oError;
      }           
    
    ?>

    <div style="text-align:right; padding-right:35px;padding-top:20px;">
      <button <?= $bCanLockScores=== false ? 'disabled' : '' ?> name="btnLockScores" id="btnLockScores" value="" onclick="mSetHxHPlanLock(<?php echo $iTestID; ?>, true)" class="btn btn-custom btn-primary rounded-pill px-4 border-0 d-inline-flex align-items-center gap-2 py-2 use-element-alignment" title="Lock Scores" style="width: 165px; text-align: center !important; text-align-last: center !important;">
         <i class="bi bi-plus-lg"></i><span class="d-none d-sm-inline font-weight-bold">Lock Scores</span>
      </button>
   </div>
   <br>
   <br>
   *Your scores can be changed until you lock the scores.  Please lock the scores when your test is complete.
    <?php
        mIncludeStatusBar($_SERVER['REQUEST_URI'], mGetApplicationID($_SERVER['REQUEST_URI'], true), $_SESSION["FullUserName"], $_SESSION["UserTypeDescription"]);    
        mIncludeJSInclude($_SERVER['REQUEST_URI'], -1, false);
    ?>
   </body>

<?php
   function mGetSubtractMetricTimes($sStartTime, $sEndTime) {
      try {

         $sTempStartTime = "";
         $sTempEndTime = "";
         $iDifferenceInMinutes = -1;
         $dtStartTime = null;
         $dtEndTime = null;
         $dtInterval = null;

         if (!is_null($sStartTime) && !is_null($sEndTime)) {

            // Normalize to remove fractional seconds
            $sTempStartTime = substr($sStartTime, 0, 8); // "03:28:00"
            $sTempEndTime   = substr($sEndTime,   0, 8); // "04:10:00"

            // Convert to DateTime objects (use a dummy date)
            $dtStartTime = new DateTime("2000-01-01 " . $sTempStartTime);
            $dtEndTime   = new DateTime("2000-01-01 " . $sTempEndTime);

            // Compute the difference
            $dtInterval = $dtStartTime->diff($dtEndTime);

            // Convert to minutes
            $iDifferenceInMinutes = ($dtInterval->h * 60) + $dtInterval->i + ($dtInterval->s / 60);

         }

         return($iDifferenceInMinutes);

      } catch (Throwable $oError) {
          throw $oError;
      }       
    }
    
   function mIsValidTime($sTime): bool {

      try{

            $bIsValidTime = false;
            $dtDateTime = null;
            $arrErrors = [];

            if (is_null($sTime) === true) {
               $bIsValidTime = false;
            } else {

               // Trim fractional seconds if present
               $sTime = substr($sTime, 0, 8); // HH:MM:SS

               $dtDateTime = DateTime::createFromFormat('H:i:s', $sTime);

               $arrErrors = DateTime::getLastErrors();
               if ($arrErrors === false) {
                     $arrErrors = array('warning_count' => 0, 'error_count' => 0);
               }

               if ($dtDateTime !== false && ($arrErrors['warning_count'] === 0 && $arrErrors['error_count'] === 0)) {
                     $bIsValidTime = true;
               }

            }

            return($bIsValidTime);
            
      } catch (Throwable $oError) {
          throw $oError;
      }           
   }

   ?>
</html>    