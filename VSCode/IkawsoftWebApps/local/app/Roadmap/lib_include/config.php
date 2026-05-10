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
            session_name("Roadmap_Builder");
            session_start();
        }
        
        
        if (empty($_SESSION["Started"])) {
            
            $_SESSION["Started"] = true;

            //Set Application ID
            $_SESSION["Application_ID"] = ApplicationID::Roadmap->value;

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

            //Session Variable Setup

            $_SESSION["ErrorMessage"] = "";
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
	    mIncludePage(enuIncludePackageID::PHPClass, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));

	    //$sIncludeFilePath = mGetRootPath(ApplicationID::Roadmap, enuPathRootType::File, enuPathType::SharedLibraryFolder, enuFolderType::lib) . "srv_class.php";
	    //require_once $sIncludeFilePath;


            //If the user doesn't have session logged in set and the page is not login or js_include then send them to login.

        //$_SESSION["SQLScript"] = mGetSQLScriptArray();

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

            return($sOutputLine);

        } catch (Throwable $oError) {
            throw $oError;
        }        
    }


    function mGetSQLScriptArray() {

        try {

	    $oDB = new cDatabase();
	    
            $arrRecordset = [];

            $sSQL = "SELECT * FROM [Ikawsoft_Central].[dbo].tb_SQL_Script";

                $arrRecordset = $oDB->mGetRecordsetArray($sSQL);

            return $arrRecordset;

            $arrRecordset = [];
            $oDB = null;

        } catch (Throwable $oError) {
            throw $oError;
        }

    }

    function mPHPErrorHandler(Throwable $oErrorArgument) {
        try {
            $sDateTimeStamp  = date('m-d-Y h:i:s A');
            $sErrorLocation  = " in {$oErrorArgument->getFile()} on line {$oErrorArgument->getLine()}";
            $sStringToLog    = $sDateTimeStamp . " " . $oErrorArgument->getMessage() . $sErrorLocation;
            mLogString($sStringToLog);

             //Example: redirect logic if needed
            if (session_status() === PHP_SESSION_ACTIVE) {
                $_SESSION["ErrorMessage"] = $sStringToLog;
                if (ob_get_level() > 0) {
                    ob_clean();
                }
                
                $sRedirectLocation = mGetRootPath(ApplicationID::DR, enuPathRootType::URL, enuPathType::ApplicationFolder, enuFolderType::NoSubFolder) . "error.php";
                header("Location: {$sRedirectLocation}", true);
                die();
            }

        } catch (Throwable $oError) {
            mLogString("Error: An Error has occurred in mPHPErrorHandler. The error details are " . $oError->getMessage());
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
                throw new Exception("SQL for could not be found in Session Array using Action ID:  " . (string)$iActionID);
            }

            if (strlen(trim($sSQL)) > 0) {
                return($sSQL);
            } else {
                throw new Exception("SQL string returned from Session Array is empty for Action ID:  " . (string)$iActionID);
            }
        } catch (Throwable $oError) {
            throw $oError;
        }
    }
   
?>