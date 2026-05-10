<?php

    session_start();
    
    require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

    mIncludePage(enuIncludePackageID::InitOnly, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));    

    require_once mGetRootPath(ApplicationID::None, enuPathRootType::File, enuPathType::SharedLibraryFolder, enuFolderType::lib) . 'srv_class.php';
    
    /*if (empty($_SESSION["Started"]) || $_SESSION['LoggedIn'] === 0) {
	echo("access denied");
	die();
    }*/

    $iApplicationID = ApplicationID::DR->value;
    
    
    
?>

<!DOCTYPE html>
<html lang="en">
 <head>
   <title>Disaster Recovery Tracker</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    
 
 </head>
 
    <body style="margin-top: 0px; margin-left: 0px;">
     
            <form name="frmViewTable" action"" method="post">
                    <?php
                      try {
			    
			    $sApplicationEnvironment = "";

			    $sApplicationEnvironment = mGetApplicationEnvironment($_SERVER['REQUEST_URI']);


			    $sSQL  = "SELECT ROW_NUMBER() OVER (ORDER BY Table_Name) AS Table_ID, Table_Name ";
			    $sSQL .= "FROM DR_Tracker.INFORMATION_SCHEMA.TABLES ";
			    $sSQL .= "WHERE TABLE_TYPE = 'BASE TABLE' ";
			    $sSQL .= "AND Table_Name LIKE 'tb\_%' ESCAPE '\' ";

			    $sSQL .= "UNION ALL ";

			    $sSQL .= "SELECT 0 AS Table_ID, '[Ikawsoft_Central].[dbo].' + TABLE_NAME AS Table_Name ";
			    $sSQL .= "FROM Ikawsoft_Central.INFORMATION_SCHEMA.TABLES ";
			    $sSQL .= "WHERE TABLE_TYPE = 'BASE TABLE' ";
			    $sSQL .= "AND Table_Name = 'tb_Lookup' ";

			    $sSQL .= "ORDER BY Table_Name";

                $sSQL = str_ireplace('Ikawsoft_Central', $sApplicationEnvironment . '_Ikawsoft_Central', $sSQL);
			    $sSQL = str_ireplace('DR_Tracker', $sApplicationEnvironment . '_DR_Tracker', $sSQL);
			    
                          $oDropDown = new cControl();
                              $oDropDown->mOutputDropDown($iApplicationID, "cboTable", "cboTable", "", "Table_Name", "Table_Name", "", "", "", $sSQL, "");
                          $oDropDown = null;

                      } catch (Throwable $oError) {
                          echo($oError->getMessage());
                      }
                    ?>
                    <button type="submit">View Table</button>
              </form>
     
              <?php     
                      try {              
                            if ($_SERVER['REQUEST_METHOD'] === 'POST') {

                                $sTableName = "" ;
                                $sSQL = "";
                                $arrRST = [];
                                $oDB = null;

                                $oDB = new cDatabase();

                                    $sTableName = $_POST['cboTable']; 
                                    $sSQL = "SELECT * FROM " . $sTableName;

                                    $arrRST = $oDB->mGetRecordsetArray($iApplicationID, $sSQL);

                                    $oDB->mEchoRecordsetArray($arrRST);

                                $oDB = null;                    
                            }
                      } catch (Throwable $oError) {
                          echo($oError->getMessage());
                      }                            
              ?>
        
   
     
 </body>
 
</html>