<?php
    //Initialize the application and session variables
    try {

        if (!defined('URL_SEPARATOR')) {
            define('URL_SEPARATOR', '/');
        }

	    require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';
        
        mIncludePage(enuIncludePackageID::InitOnly, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));

        //date_default_timezone_set("America/Chicago");   
        $gApplicationFileDirectory = mGetServerEnvironmentVariable(ApplicationID::DR,enuPathType::ApplicationFolder);
        $gServerFileDirectory = mGetServerEnvironmentVariable(ApplicationID::DR,enuPathType::SharedLibraryFolder);

        $gApplicationURLDirectory = str_ireplace(DIRECTORY_SEPARATOR, URL_SEPARATOR, $gApplicationFileDirectory);
        $gServerURLDirectory = str_ireplace(DIRECTORY_SEPARATOR, URL_SEPARATOR, $gServerFileDirectory);

	    $IncludePage = ""; 
        $sCurrentPageName = basename($_SERVER['PHP_SELF']);
        $sEnvironmentServerID = getenv('Server_ID');

	
        //----------------------------------------------------------------------------------
        //Begin Session Configuration
        //----------------------------------------------------------------------------------
        

        if (session_status() === PHP_SESSION_NONE) {
            session_name("Disaster_Recovery");
            session_start();
        }
        
        
        if (empty($_SESSION["Started"])) {
            
            $_SESSION["Started"] = true;

            //Set Application ID
            $_SESSION["Application_ID"] = ApplicationID::DR->value;

            //Pathing Setup
            $_SESSION["root_url"] = mGetRootURL($_SERVER['HTTP_HOST']);
            $_SESSION["Application_Environment"] = mGetApplicationEnvironment($_SERVER['REQUEST_URI']);
            $_SESSION["application_url_directory"] = $gApplicationURLDirectory;
            $_SESSION["server_url_directory"] = $gServerURLDirectory;

            //Is Locally Running
            if (gethostname() === 'Spyro') {
                $_SESSION["Local_Instance"] = false;
            } else {
                $_SESSION["Local_Instance"] = false;
            }

            //Operating System
            if (strtoupper(substr(PHP_OS, 0, 3)) === 'WIN') {
                $_SESSION["Application_Platform"] = "Microsoft";
            } elseif (strpos(PHP_OS, "Linux") === true) {
                $_SESSION["Application_Platform"] = "Linux";
            } else {
                $_SESSION["Application_Platform"] = "NA";
            }

            //User Login Access
            $_SESSION["LoggedIn"] = 0;
            $_SESSION["FullUserName"] = "";
            $_SESSION["UserType"] = -1;
            $_SESSION["UserTypeDescription"] = "";
            $_SESSION["UserID"] = -1;
            $_SESSION["PlatformAccess"] = -1;
            $_SESSION["SelectedPlatform"] = -1;
            $_SESSION["EncryptionKey"] = "";

            //Session Variable Setup
            $_SESSION["SelectedYear"] = -1;
            $_SESSION["SelectedPage"] = -1;
            $_SESSION["SelectedTestPlan"] = -1;
            $_SESSION["ErrorMessage"] = "";
            $_SESSION["LastErrorDateTime"] = "";
            $_SESSION["SQLScript"] = "";

            //----------------------------------------------------------------------------------
            //End Session Configuration
            //----------------------------------------------------------------------------------
        }

	    //-------------------------------------
            //Error Handling
	    error_reporting(E_ALL);
	    ini_set('display_errors', TRUE);
	    ini_set('error_log', mGetErrorLogPath());

	    // 1. Convert non-fatal errors to exceptions
	    set_error_handler(function ($errno, $errstr, $errfile, $errline) {
		throw new ErrorException($errstr, 0, $errno, $errfile, $errline);
	    });

	    // 2. Handle uncaught exceptions
	    set_exception_handler('mPHPErrorHandler');

	    // 3. Capture fatal errors
	    register_shutdown_function(function () {
		try {

		    $sErrorMessage = "";
		    $arrError = error_get_last();

		    if ($arrError !== null) {
                $sErrorMessage  = date('Y-m-d H:i:s') . " [FATAL] {$arrError['message']} in {$arrError['file']}:{$arrError['line']}\n";
                mLogString($sErrorMessage);
		    }

		} catch (Throwable $oError) {
		    echo $oError->getMessage();
		}
	    });
	    //--------------------------

   
	    //Includes
	    mIncludePage(enuIncludePackageID::VendorClass, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));
	    mIncludePage(enuIncludePackageID::SharedPHPLib, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));

	    if (empty($_SESSION["SQLScript"])) {
		    $_SESSION["SQLScript"] = mGetSQLScriptArray();
	    }


            //If the user doesn't have session logged in set and the page is not login or js_include then send them to login.
            if ($sCurrentPageName !== 'login.php' && $sCurrentPageName !== 'js_include.php' && $_SESSION['LoggedIn'] === 0) {

                $sLoginPageURL = $_SESSION["root_url"] . URL_SEPARATOR . $_SESSION["Application_Environment"] . $_SESSION["application_url_directory"] . "login.php";
                header("Location: " . $sLoginPageURL);
                exit;
            }


    } catch (Throwable $oError) {

        echo $oError->getMessage();

    }

   
    function mOutputHTMLHead() {

        try {

            $sAppUIFolderPath = "";
            $sOutputLine = "";
            $sAppUIFolderPath = mGetRootPath(ApplicationID::DR, enuPathRootType::URL, enuPathType::ApplicationFolder, enuFolderType::ui);

            //Bootstrap for New Design
            $sOutputLine = "<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css'>\n";
            $sOutputLine = $sOutputLine . "<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.1/font/bootstrap-icons.css'>\n";

            //Notify
            $sOutputLine = $sOutputLine . "<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/notyf@3/notyf.min.css'>\n";
            $sOutputLine = $sOutputLine . "<script src='https://cdn.jsdelivr.net/npm/notyf@3/notyf.min.js'></script>\n";
    
            //SheetJS
            $sOutputLine = $sOutputLine . "<script src='https://cdn.sheetjs.com/xlsx-latest/package/dist/xlsx.full.min.js'></script>\n";

            //DocX: https://www.npmjs.com/package/docx            
            $sOutputLine = $sOutputLine . "<script src='https://unpkg.com/docx@8.0.0/build/index.js'></script>\n";

            $tVersionTime = time(); 

            //App CSS files
            $sOutputLine .= "<link rel='stylesheet' type='text/css' href='{$sAppUIFolderPath}styles.css?v={$tVersionTime}'>\n";
            $sOutputLine .= "<link rel='stylesheet' type='text/css' href='{$sAppUIFolderPath}plan_styles.css?v={$tVersionTime}'>\n";

            $sOutputLine .= "<link rel='icon' href='data:,'>";

            return($sOutputLine);

        } catch (Throwable $oError) {
            throw $oError;
        }        
    }


    function mGetSQLScriptArray() {

        try {

	    $iApplicationID = -1;
	    $sApplicationName = "";
	    $arrRecordset = [];

	    $sApplicationName = mGetApplicationName($_SERVER['REQUEST_URI']);

	    if (strlen(trim($sApplicationName)) > 0) {

            $iApplicationID = mGetApplicationID($_SERVER['REQUEST_URI']);

            if (!$iApplicationID || $iApplicationID < 1) {
                throw new Exception("mGetSQLScriptArray: Application ID cannot be less than 1: " . $sApplicationName . " Application ID: " . $iApplicationID);
            }


            $oDB = new cDatabase();

            $sSQL = "SELECT * FROM [Ikawsoft_Central].[dbo].tb_SQL_Script WHERE Application_ID = " . $iApplicationID;

                $arrRecordset = $oDB->mGetRecordsetArray($iApplicationID , $sSQL);

                if (empty($arrRecordset)) {
                throw new Exception("mGetSQLScriptArray: Array is empty for Application Name: " . $sApplicationName . " Application ID: " . $iApplicationID);
                }


            return $arrRecordset;

            $arrRecordset = [];
            $oDB = null;
	    }

        } catch (Throwable $oError) {
            throw $oError;
        }

    }

    function mPHPErrorHandler(Throwable $oErrorArgument) {
        try {

            //Buid the Log and Error Page String
            //$sDateTimeStamp  = date('m-d-Y h:i:s A');
            $sStringToLog  = " Error Message: " . $oErrorArgument->getMessage();                        
            $sStringToLog = $sStringToLog . " Error Location: {$oErrorArgument->getFile()} on line {$oErrorArgument->getLine()}";
            $sErrorPageErrorMessage = urlencode($sStringToLog);
	        $iApplicationID = $_SESSION["Application_ID"];


            //Log The String
            mLogString($sStringToLog);
            
            //E-mail the Error
            mSendAdminEmail($sStringToLog);
            
             //Redirect to error.php
            if (session_status() === PHP_SESSION_ACTIVE) {
                $_SESSION["ErrorMessage"] = $sStringToLog;
                if (ob_get_level() > 0) {
                    ob_clean();
                }
                
                $sRedirectLocation = mGetRootPath(ApplicationID::from($iApplicationID), enuPathRootType::URL, enuPathType::ApplicationFolder, enuFolderType::NoSubFolder) . "error.php";
                $sRedirectLocation = $sRedirectLocation . "?ErrorMessage=" . $sErrorPageErrorMessage;
                header("Location: {$sRedirectLocation}", true);
                die();
            }

        } catch (Throwable $oError) {
            mLogString("Error: An Error has occurred in mPHPErrorHandler. The error details are " . $oError->getMessage());
        }
    }


    function mSendAdminEmail($sStringToEmail) {
        try {
            $oSendEmail = null;

            $oSendEmail = new MailBabyService();
                  $oSendEmail->mSendEmail("donotreply@ikawsoft.com", "DR Tracker", "devon.manelski@wolterskluwer.com", "DR Tracker Error", $sStringToEmail);
            $oSendEmail = null;

        } catch (Throwable $oError) {
            throw $oError;
        }        
    }
    function mGetPassword($intLength = 10) {

        $sReturnPassword = "";

        try {

            // Define character pools
            $sUpper   = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
            $sLower   = 'abcdefghijklmnopqrstuvwxyz';
            $sDigits  = '0123456789';
            $sSymbols = '!@#$%^&*()-_=+[]{}|;:,.<>?';
            $sAll     = $sUpper . $sLower . $sDigits . $sSymbols;

            // Defensive check
            if ($intLength < 5) {
                throw new exception('Password length must be at least 5.');
            }

            // Initialize password array
            $arrPassword = [];

            // Ensure at least one character from each category
            $arrPassword[] = substr($sUpper, random_int(0, strlen($sUpper) - 1), 1);
            $arrPassword[] = substr($sLower, random_int(0, strlen($sLower) - 1), 1);
            $arrPassword[] = substr($sDigits, random_int(0, strlen($sDigits) - 1), 1);
            $arrPassword[] = substr($sSymbols, random_int(0, strlen($sSymbols) - 1), 1);

            // Fill remaining characters
            for ($intIndex = 4; $intIndex < $intLength; $intIndex++) {
                $arrPassword[] = substr($sAll, random_int(0, strlen($sAll) - 1), 1);
            }

            // Shuffle and return
            shuffle($arrPassword);
            $sReturnPassword = implode('', $arrPassword);

            return($sReturnPassword);

        } catch (Throwable $oError) {

            throw $oError;

        }
    }

   function mOutputTestDropDown($sDropDownName) {
        try {

            $sDropDownID = "";
            $sStyle = "";
            $sIDField = "";
            $sValueField = "";
            $sSQL = "";
            $sApplicationName = "";
            $sDropDownID = $sDropDownName;
            $sIDField = "Test_ID";
            $sValueField = "Test_Desc";
            $sStyle = "width: auto;min-width: fit-content;";
            $iApplicationID = -1;
            $bSuccess = false;
            $oDropDown = null;

	        $sApplicationName = mGetApplicationName($_SERVER['REQUEST_URI']);

            if (strlen(trim($sApplicationName)) > 0) {

                $iApplicationID = mGetApplicationID($_SERVER['REQUEST_URI'], $sApplicationName);

                if (!$iApplicationID || $iApplicationID < 1) {
                    throw new Exception("mOutputTestDropDown: Application ID cannot be less than 1: " . $sApplicationName . " Application ID: " . $iApplicationID);
                }
            }

            $oDropDown = new cControl();

                $sSQL = mGetSQLStatementFromSessionArray(25);
                
                $sSQL = str_ireplace("tb_Test.Year_ID = ?", "tb_Test.Year_ID = " . $_SESSION["SelectedYear"], $sSQL);
                $bSuccess = $oDropDown -> mOutputDropDown($iApplicationID, $sDropDownName, $sDropDownID, $sStyle, $sIDField, $sValueField, "", "", "", $sSQL, "form-select form-select-sm w-auto");

                if ($bSuccess !== true) {
                    echo("<b>No DR Tests found for this platform and year.</b>");
                }

            $oDropDown = null;

        } catch (Throwable $oError) {

            throw $oError;
        }
    }


    function mGetSQLStatementFromSessionArray($iActionID, &$arrSQLRowForActionID = null) {
        try {

            $sSQL = "";
            $arrSQLActionIDFilter = [];
            $arrSessionSQLRowMatch = [];

            $arrSQLScriptRecordset = $_SESSION["SQLScript"];

            $arrSQLActionIDFilter = array_filter($arrSQLScriptRecordset, fn($arrSQLTableRow) => (int)$arrSQLTableRow["Action_ID"] === $iActionID);
            $arrSessionSQLRowMatch = reset($arrSQLActionIDFilter);

            if (count($arrSQLActionIDFilter) === 1) {
                $sSQL = $arrSessionSQLRowMatch["SQL"];
                $arrSQLRowForActionID = $arrSessionSQLRowMatch;
            } else {
                throw new Exception(" SQL could not be found in Session Array using Action ID:  " . (string)$iActionID);
            }

            if (strlen(trim($sSQL)) > 0) {
                return($sSQL);
            } else {
                throw new Exception(" SQL string returned from Session Array is empty for Action ID:  " . (string)$iActionID);
            }
        } catch (Throwable $oError) {
            throw $oError;
        }
    }
   
?>