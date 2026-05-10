<?php


enum enuDBConnType: int {
    case sODBC = 1;
    case sSQLSRV = 2;
}

enum enuCommandType: int {
    case nonSpecified = 0;
    case Insert = 1;
    case Update = 2;
    case Delete = 3;
    case DeleteWhereIn = 4;    
    case DeleteWhereNotIn = 5;        
}

    
    require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';
		
class cDatabase {
    
    public function mOpenDatabase($iApplicationID, &$oConn, enuDBConnType $eDBConnType = enuDBConnType::sSQLSRV) {

        try {

            $sApplicationName = "";
	        $sApplicationEnvironment = "";
            $sDSN = "";
            $sDBUserName = "";
            $sDBPassword = "";

	        $sApplicationEnvironment = mGetApplicationEnvironment($_SERVER['REQUEST_URI']);


            if (!is_numeric($iApplicationID) || ($iApplicationID <0)) {
                throw new Exception("Application ID must be passed in and zero or greater to create a database connection.");
            }

            If ($eDBConnType == enuDBConnType::sODBC) {
                
                throw new Exception("ODBC support has been deprecated.");

            } else If ($eDBConnType == enuDBConnType::sSQLSRV) {

                $sDSN  = mGetServerEnvironmentVariable(ApplicationID::from($iApplicationID), enuPathType::DBDSN);
                $sDBUserName  = mGetServerEnvironmentVariable(ApplicationID::from($iApplicationID), enuPathType::DBUserName);
                $sDBPassword  = mGetServerEnvironmentVariable(ApplicationID::from($iApplicationID), enuPathType::DBPassword);

                //append the application environment to the database name in the DSN if not prod
                $sDSN = preg_replace('/Database=/', 'Database=' . $sApplicationEnvironment . '_', $sDSN, 1);

                if (strlen(trim($sDSN)) === 0 || strlen(trim($sDBUserName)) === 0 || strlen(trim($sDBPassword)) === 0) {
                    throw new Exception("DSN, Username and Password cannot be blank.  Please check environment variable.");
                }
                    
                    $oConn = new PDO($sDSN, $sDBUserName, $sDBPassword);
                    $oConn->setAttribute(PDO::SQLSRV_ATTR_FETCHES_NUMERIC_TYPE, true);
                    $oConn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

             }

        } catch (Throwable $oError) {

            throw $oError;

        }
    }

     private function mFilterArrayByTableName(array $arrTableCache, string $sTableName): array { $arrFiltered = [];
        try {

            foreach ($arrTableCache as $oRow) {
                if (
                    isset($oRow['TABLE_NAME'], $oRow['COLUMN_NAME'], $oRow['DATA_TYPE']) &&
                    $oRow['TABLE_NAME'] === $sTableName
                ) {
                    $arrFiltered[] = [
                        'COLUMN_NAME' => $oRow['COLUMN_NAME'],
                        'DATA_TYPE'   => $oRow['DATA_TYPE']
                    ];
                }
            }
        } catch (Throwable $oError) {

            throw $oError;

        } finally {
            //none
        }

        return($arrFiltered);
    }


    public function mExecuteSQL(
        $iApplicationID, 
        enuCommandType $eCommandType = enuCommandType::nonSpecified, 
        string $sTableName = "", 
        array $arrFields = [], 
        array $arrValues = [], 
        string $sSQL = "", 
        string $sIdentityColumnName = "") {
        
        $sFieldString = "";
        $sValueString = "";
        $sIdentityString = "";
        $sWhereClause = "";
        $sSQLOperator = "";
        $iResult = -1;
        $bDirectSQLExecute = false;
        $arrPlaceholders = [];
        $arrWhereClauses = [];        
        $oConnDB = null;
        $oCommandStatement = null;
        
        try {

            //Load arrFields & ArrValues
            if ($eCommandType !== enuCommandType::nonSpecified && strlen(trim($sSQL)) === 0) {

                if (count($arrFields) !== count($arrValues)) {
                    throw new Exception("Field and value arrays must be non-empty and of equal length.");
                }

                $sFieldString = implode(',', $arrFields);
                $arrPlaceholders = array_fill(0, count($arrValues), '?');
                $sValueString = implode(',', $arrPlaceholders);
                
            }

            //Insert Statement without SQL
            if ($eCommandType === enuCommandType::Insert &&  strlen(trim($sSQL)) === 0) {

                if (strlen($sIdentityColumnName) > 0) {
                    $sIdentityString = "OUTPUT INSERTED." . $sIdentityColumnName;
                }
                $sSQL = "INSERT INTO " . $sTableName . " (" . $sFieldString . ") " . $sIdentityString . " VALUES (" . $sValueString . ")";

            //Delete Statement without SQL
            } else if ($eCommandType === enuCommandType::Delete && strlen(trim($sSQL)) === 0) {
                $arrWhereClauses = array_map(function($sField) {
                    return "$sField = ?";
                }, $arrFields);

                $sWhereClause = implode(' AND ', $arrWhereClauses);
                
                if (strlen($sWhereClause) > 0) {
                    $sSQL = "DELETE FROM " . $sTableName . " WHERE " . $sWhereClause;

                } else {
                    mLogString("arrField: " . print_r($arrFields, true));
                    mLogString("arrValue: " . print_r($arrValues, true));                    
                    throw new Exception("All delete statements must have where clause.");
                }

            //Delete WhereIn OR WhereNotIn Statement without SQL
            } else if (($eCommandType === enuCommandType::DeleteWhereIn || $eCommandType === enuCommandType::DeleteWhereNotIn) && strlen(trim($sSQL)) === 0) {
                $bDirectSQLExecute = true;

                if (is_array($arrFields) && count($arrFields) === 1) {

                    if ($eCommandType === enuCommandType::DeleteWhereIn) {
                        $sSQLOperator = "IN";
                    } else if ($eCommandType === enuCommandType::DeleteWhereNotIn) {
                        $sSQLOperator = "NOT IN";
                    } else {
                        throw new Exception("DELETE WhereIn/WhereNotIn SQL Operator could not be determined.");
                    }

                    $sValueString = implode(',', $arrValues);
                    
                    $sWhereClause = array_values($arrFields)[0] . " " . $sSQLOperator . " (" . $sValueString . ")";
                    
                } else {
                    throw new Exception("To run a DeleteWhereIn Or NotIn ArrFields is required and must have only 1 value.");
                }
                
                if (strlen($sWhereClause) > 0) {
                    $sSQL = "DELETE FROM " . $sTableName . " WHERE " . $sWhereClause;
                } else {
                    throw new Exception("All delete statements must have a where clause.");
                }
                    mLogString("Where In SQL " . $sSQL);

            //Update Statement without SQL
            } else if ($eCommandType === enuCommandType::Update && strlen(trim($sSQL)) === 0) {

                // Assume last field is the primary key
                $iLastIndex = count($arrFields) - 1;
                $sKeyField = $arrFields[$iLastIndex];
                $sWhereClause = $sKeyField . " = ?";

                // Build SET clause from all fields except the last
                $arrSetClauses = [];
                for ($i = 0; $i < $iLastIndex; $i++) {
                    $arrSetClauses[] = $arrFields[$i] . " = ?";
                }
                $sSetClause = implode(', ', $arrSetClauses);

                // Final SQL
                $sSQL = "UPDATE " . $sTableName . " SET " . $sSetClause . " WHERE " . $sWhereClause;

            //SQL Included for any type update execute statement
            } else if (strlen(trim($sSQL)) !== 0) {
                //Do Nothing and let the rest of the code handle it
            } else {
                throw new Exception("Command type must be specified to execute SQL command.  Command Type:  " .  $eCommandType->value);
            }

            //Open DB
            $this->mOpenDatabase($iApplicationID, $oConnDB);

            //Append environment prefixes
            $sSQL = $this->mSQLScrub($sSQL);
                      
            if ($bDirectSQLExecute === false) {
                //Prepare SQL for Command
                $oCommandStatement = $oConnDB->prepare($sSQL);

                //Bind Values and Fields to Command Statement
                for ($iValueCounter = 0; $iValueCounter < count($arrValues); $iValueCounter++) {

                    $sFieldName = $arrFields[$iValueCounter] ?? null;
                    $sFieldValue = $arrValues[$iValueCounter];


                    // If this is a binary field, encode to base64 text before binding
                    if ($sFieldName !== null && $this->mIsBinaryField($sFieldName)) {
                        $sFieldValue = base64_encode($sFieldValue);
                    }

                    // Bind everything as string (or NULL)
                    if (is_null($sFieldValue)) {
                        $oCommandStatement->bindValue($iValueCounter + 1, null, PDO::PARAM_NULL);
                    } else {
                        $oCommandStatement->bindValue($iValueCounter + 1, $sFieldValue, PDO::PARAM_STR);
                    }

                }

                // Execute SQL
                if (!$oCommandStatement->execute()) {
                    mLogString("SQL execution failed.");
                    $iResult = -1;
                } else {
                    // If identity column name was provided, fetch the identity value
                    if (strlen($sIdentityColumnName) > 0) {
                        $iResult = $oCommandStatement->fetchColumn();   // <-- identity value
                    } else {
                        $iResult = $oCommandStatement->rowCount();      // <-- records affected
                    }
                }

            } else if ($bDirectSQLExecute === true) {
                $iResult = $oConnDB->exec($sSQL);   // returns number of affected rows
            }

            return $iResult;

        } catch (Throwable $oError) {
            mLogString("Error Occurred in srv_class.php: ");
            mLogString("Error Message: " . $oError->getMessage());
            mLogString("Field String " . print_r($arrFields, true));
            mLogString("Value String " . print_r($arrValues, true));
            mLogString("SQL " . $sSQL);
            throw $oError;
        } finally {
            if ($oConnDB) {
                $this->mCloseDatabase($oConnDB);
            }
        }

    }
  
  /* public function mGetDBTableSchema(): array { $arrSchema = [];

    $oConnDB = null;
    $oRst = null;
    $arrTableSchemaArray = [];
    
    try {
        $this->mOpenDatabase($oConnDB);

        $sSQL = "
            SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = 'dbo' AND Table_Name <> 'sysdiagrams';
        ";

        $oRst = odbc_exec($oConnDB, $sSQL);
        if (!$oRst) {
            throw new Exception("Schema query failed: " . odbc_errormsg($oConnDB));
        }

        while ($oRow = odbc_fetch_array($oRst)) {
            $arrTableSchemaArray[] = $oRow;
        }   


        } catch (Throwable $oError) {

            throw $oError;

        } finally {
            $this->mCloseRecordset($oRst);
            $this->mCloseDatabase($oConnDB);
        }

        return($arrTableSchemaArray);
    }*/

    public function mGetRecordsetArray($iApplicationID, $sSQL, array $arrParameters = []) {

        $oConnDB = null;
        $arrRecordset = [];

        try {
            
            $this->mOpenDatabase($iApplicationID, $oConnDB);
            $sSQL = $this->mSQLScrub($sSQL);
            

            if (!empty($arrParameters)) {

                // PDO replacement for odbc_prepare + odbc_execute
                $oRst = $oConnDB->prepare($sSQL);
                $oRst->execute($arrParameters);

            } else {

                // PDO replacement for odbc_exec
                $oRst = $oConnDB->query($sSQL);
            }

            $arrRecordset = $oRst->fetchAll(PDO::FETCH_ASSOC);


            return $arrRecordset;

        } catch (Throwable $oError) {
            mLogString("SQL " . $sSQL);
            mLogString("arrParameters" . print_r($arrParameters, true));
            throw $oError;

        } finally {
            $this->mCloseRecordset($oRst);
            $this->mCloseDatabase($oConnDB);
        }

    }

    public function mCloseRecordset(&$oRST) {

        try {

            if ($oRST instanceof PDOStatement) {
                $oRST = null; // PDO frees statements by nulling the handle
            }

        } catch (Throwable $oError) {
            throw $oError;
        }
    }

    private function mSQLScrub($sSQL) {

        try {

            $sApplicationEnvironment = "";
            $sSearchFor = "[Ikawsoft_Central].[dbo].";
            $sReplaceWith = "Ikawsoft_Central].[dbo].";

            $sApplicationEnvironment = mGetApplicationEnvironment ($_SERVER['REQUEST_URI']);

            if (strtolower($sApplicationEnvironment) === "prod") {
                $sReplaceWith = "[prod_" . $sReplaceWith;
            } else if (strtolower($sApplicationEnvironment) === "qa") {                
                $sReplaceWith = "[qa_" . $sReplaceWith;
            } else if (strtolower($sApplicationEnvironment) === "smoke") {      
                $sReplaceWith = "[smoke_" . $sReplaceWith;
            } else if (strtolower($sApplicationEnvironment) === "local") {      
                $sReplaceWith = "[local_" . $sReplaceWith;
            }

            $sSQL = str_ireplace($sSearchFor, $sReplaceWith, $sSQL);

            return ($sSQL);

        } catch (Throwable $oError) {
            throw $oError;
        }            
    }

    public function mCloseDatabase(&$oConn) {

        try {

            if ($oConn instanceof PDO) {
                $oConn = null; // PDO closes connections by nulling the handle
            }

        } catch (Throwable $oError) {

            throw $oError;

        }

    }

    private function mIsBinaryField($sFieldName) {
        try {

            $bIsBinary = false;

            static $cstBinaryField = [
                'Encryption_Key',
            ];

            $bIsBinary = in_array($sFieldName, $cstBinaryField, true);
            //mLogString("IsBinary: {$sFieldName} = " . ($bIsBinary ? 'true' : 'false'));

            return $bIsBinary;

        } catch (Throwable $oError) {
            mLogString($oError->getMessage());
            throw $oError;
        }
    }
    
    public function mEchoRecordsetArray($arrRST) {
        try {
            echo "<table id='ViewTable'>";
                                                    // Output header row
                                                    if (!empty($arrRST)) {
                                                        echo "<tr>";
                                                        foreach (array_keys($arrRST[0]) as $colName) {
                                                            $colName = htmlspecialchars($colName ?? '', ENT_QUOTES);
                                                            echo "<th>" . $colName . "</th>";
                                                        }
                                                        echo "</tr>";
                                                    } else {
                                    echo("no records found");
                                }

                                                    // Output data rows
                                                    foreach ($arrRST as $row) {
                                                        echo "<tr>";
                                                        foreach ($row as $value) {
                                                            $value = htmlspecialchars($value ?? '', ENT_QUOTES);
                                                            echo "<td>" . $value . "</td>";
                                                        }
                                                        echo "</tr>";
                                                    }                        
                            echo "</table>";        

        } catch (Throwable $oError) {
            mLogString($oError->getMessage());
            throw $oError;
        }                            
    }
}

class cControl {


    public function mOutputDropDown(
        $iApplicationID,
        $sDropDownName,
        $sDropDownID,
        $sStyle,
        $sIDField = "",
        $sValueField = "",
        $sTableName = "",
        $sLookupCategory = "",
        $iDefaultValue = 0, 
        $sSQL = "", 
        $sClass = "") {

        try {

            $sPlatformIDList = "";
            $sPlatformClause = "";
            $sUserType = 0;
            $arrParameters = [];
            $arrPlatformID = "";

            $oDB = new cDatabase();
            $iDefaultValue = (int)$iDefaultValue;

            if (!empty($sLookupCategory)) {
                
                if (strtolower($sLookupCategory) === "platform") {

                    //If it's the standard user or PM/App Owner User only grant access to plaforms that are configured in their tb_user record
                    if ($_SESSION["UserType"] <= 5515 ) {

                        // Get the Platform ID List
                        //-----------------------------------------------------------------
                        mLogString("Session Platform: " . $_SESSION["PlatformAccess"]);
                        $arrPlatformID = explode(',', $_SESSION["PlatformAccess"]);
                        $arrPlatformID = array_map('trim', $arrPlatformID); // clean whitespace
                        $sPlatformIDList = implode(',', array_fill(0, count($arrPlatformID), '?'));
                        $arrParameters = array_merge([$sLookupCategory], $arrPlatformID); 

                        $sPlatformClause = " AND Platform_ID in ($sPlatformIDList) ";

                    //All other users have access to all platforms
                    } else {
                        $arrParameters = array($sLookupCategory);
                    }
  
                    // Set The User Type
                    $sUserType = $_SESSION["UserType"];
                    $arrParameters[] = $sUserType;
		            $arrParameters[] = $iApplicationID;
 
                    $sSQL = "SELECT Lookup_ID, Lookup_Desc FROM [Ikawsoft_Central].[dbo].tb_Lookup WHERE Category = ? And Active = 1 " . $sPlatformClause . "AND (User_Type <= ?) AND Application_ID = ? ORDER BY Order_ID";

                } else if (strtolower($sLookupCategory) === "page") {

                    $arrParameters = array($sLookupCategory);
                    $sUserType = $_SESSION["UserType"];
                    $arrParameters[] = $sUserType;          
                    $arrParameters[] = $_SESSION["SelectedPlatform"];         
		            $arrParameters[] = $iApplicationID;
                    
                    $sSQL = "SELECT Lookup_ID, Lookup_Desc";
                    $sSQL = $sSQL . " FROM [Ikawsoft_Central].[dbo].tb_Lookup";
                    $sSQL = $sSQL . " WHERE Category = ?";
                    $sSQL = $sSQL . " AND Active = 1";
                    $sSQL = $sSQL . " AND (User_Type <= ?)";
                    $sSQL = $sSQL . " AND (EXISTS (SELECT 1 FROM STRING_SPLIT(Platform_ID, ',') AS s WHERE TRY_CAST(s.value AS INT) = ?) OR Platform_ID = 0)";
                    $sSQL = $sSQL . " AND (Application_ID = ?)";
                    $sSQL = $sSQL . " ORDER BY Order_ID;";
                    
                } else {

                    $arrParameters = array($sLookupCategory);
                    $sUserType = $_SESSION["UserType"];
                    $arrParameters[] = $sUserType;     
		            $arrParameters[] = $iApplicationID;               

                    $sSQL = "SELECT Lookup_ID, Lookup_Desc FROM [Ikawsoft_Central].[dbo].tb_Lookup WHERE Category = ? And Active = 1 AND (User_Type <= ?) ";
                    $sSQL = $sSQL . " AND Application_ID = ?";
                    $sSQL = $sSQL . " ORDER BY Order_ID";
                }

                $sIDField = "Lookup_ID";
                $sValueField = "Lookup_Desc";

            } else if (strlen($sSQL) > 0) {

                $arrParameters = [];
            } else {

                // Direct table mode
                $sSQL = "SELECT " . $sIDField . ", " . $sValueField . " FROM " . $sTableName;
            }

            $arrRows = $oDB->mGetRecordsetArray($iApplicationID, $sSQL, $arrParameters);

            if (!empty($arrRows)) {
                echo("<select name='" . $sDropDownName . "' id='" . $sDropDownID . "' class='" . $sClass . "' style='" . $sStyle . "'>");

                    if ($iDefaultValue === 0) {
                        $sSelected = " selected ";
                    } else {
                        $sSelected = " ";
                    }
                    echo("<option " . $sSelected ."value='-1'></option>");

                    foreach ($arrRows as $oRow) {
                        //mLogString("Default Value: " . $iDefaultValue);
                        //mLogString("Row ID Field: " . $oRow[$sIDField]);
                        if (($iDefaultValue !== 0) && ((int)$oRow[$sIDField] === $iDefaultValue)) {
                            $sSelected = " selected ";
                        } else {
                            $sSelected = " ";
                        }
                        //$sSelected = ($oRow[$sIDField] == $iDefaultValue && $iDefaultValue !== 0) ? " selected " : " ";
                        echo("<option" . $sSelected . "value='" . $oRow[$sIDField] . "'>" . $oRow[$sValueField] . "</option>");
                    }

                echo("</select>");
                return true;
            } else {
                return false;

            }

        } catch (Throwable $oError) {
            mLogString("mOutputDropDown: " . $sLookupCategory . " " . $oError->getMessage());

            throw $oError;

        } finally {

            unset($oDB);
            unset($arrRows);

        }
    }


}


use PHPMailer\PHPMailer\PHPMailer;
use PHPMailer\PHPMailer\Exception;

class MailBabyService {
    private $mail;

    public function __construct() {
        $this->mail = new PHPMailer(true);
        $this->configureSMTP();
    }

    private function configureSMTP() {

        try {
            
            $sMailBabyUserName = getenv('MailBaby_Username');
            $sMailBabyPassword = getenv('MailBaby_Password');

            // Server settings
            $this->mail->isSMTP();
            $this->mail->Host = 'relay.mailbaby.com';
            $this->mail->SMTPAuth = true;
            $this->mail->Username = $sMailBabyUserName;
            $this->mail->Password = $sMailBabyPassword;

            $this->mail->SMTPSecure = PHPMailer::ENCRYPTION_STARTTLS;
            $this->mail->Port = 587;
            $this->mail->SMTPOptions = [
                'ssl' => [
                    'verify_peer' => false,
                    'verify_peer_name' => false,
                    'allow_self_signed' => true
                ]
            ];

        } catch (Throwable $oError) {
            throw($oError);
        }
    }

    public function mSendEmail($sFromEmailAddress, $sFromName, $sToEmailAddress, $sEmailSubject, $sEmailBody) {

        try {

            $bLocalCapture = false;

            // Enable SMTP debug output
            //$this->mail->SMTPDebug = 2;
            //$this->mail->Debugoutput = 'error_log';  // this is in the php error log file

            // Recipients
            $this->mail->setFrom($sFromEmailAddress, $sFromName);
            $this->mail->addReplyTo($sFromEmailAddress);
            $this->mail->addAddress($sToEmailAddress);

            // Content
            $this->mail->isHTML(true);
            $this->mail->Subject = $sEmailSubject;
            $this->mail->Body = $sEmailBody;
            $this->mail->AltBody = strip_tags($sEmailBody); // helps avoid rSPAM

            if (!isset($_SESSION['Local_Instance']) || empty($_SESSION['Local_Instance'])) {
                $bLocalCapture = false;
            } else {
                if ($_SESSION["Local_Instance"] === true) {
                    $bLocalCapture = true;
                } else {
                    $bLocalCapture = false;
                }
            }

            if ($bLocalCapture === true) {
                $this->mail->isSMTP();
                $this->mail->Host = 'localhost';
                $this->mail->SMTPDebug = 0;

                $this->mail->preSend();
                //file_put_contents('email.eml', $this->mail->getSentMIMEMessage());
                mLogString($this->mail->getSentMIMEMessage());
            } else if  ($bLocalCapture === false) {
                $this->mail->send();
            }

            $this->mail->clearAddresses();
            $this->mail->clearReplyTos();

            return ['success' => true, 'message' => 'Email sent successfully'];

        } catch (Throwable $oError) {
            throw($oError);
        }
    }
}

?>