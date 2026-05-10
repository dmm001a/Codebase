<?php



    try {
        header("Content-Type: application/javascript");
        
        if (!defined('URL_SEPARATOR')) {
            define('URL_SEPARATOR', '/');
        }
        
        require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

        mIncludePage(enuIncludePackageID::InitOnly, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));  

        $sIncludeFileContents = "";
        $sApplicationFileDirectory = "";
        $sApplicationFileRootPath= "";
        $sApplicationLibRootPath = "";
        $sFullFilePath = "";
        $sRootPath = "";
        $sFullLogFilePath = "";
        $sLogFileName = "";
        $iApplicationID = isset($_GET['AppID']) ? (int)$_GET['AppID'] : -1;
        $bIncludeUILibrary = isset($_GET['IncludeUILib']) ? (bool)$_GET['IncludeUILib'] : false;
        $eApplicationID = ApplicationID::tryFrom($iApplicationID);
        $arrFilesToInclude = [];

        enum enuFileType: int {
            case SharedLibrary = 0;
            case AppLibrary = 1;
            case AppUI = 2;
        }        

        $sLogFileName = "combined_js.log";

        $sApplicationFileDirectory = mGetServerEnvironmentVariable($eApplicationID, enuPathType::ApplicationFolder);
        $sApplicationFileRootPath = mGetRootPath($eApplicationID, enuPathRootType::File, enuPathType::ApplicationFolder, enuFolderType::NoSubFolder);
        $sFullLogFilePath = $sApplicationFileRootPath .  DIRECTORY_SEPARATOR . $sLogFileName;
        $sApplicationLibRootPath =  mGetRootPath($eApplicationID, enuPathRootType::File, enuPathType::ApplicationFolder, enuFolderType::lib);

        file_put_contents($sFullLogFilePath, ""); //Clear the contents of the log file
        
        $sServerLibRootPath =  mGetRootPath($eApplicationID, enuPathRootType::File, enuPathType::SharedLibraryFolder, enuFolderType::lib);

    

        $arrFilesToInclude = [
            ["srv_global.js", enuFileType::SharedLibrary],
            ["srv_class.js", enuFileType::SharedLibrary],
            ["srv_database_operations.js", enuFileType::SharedLibrary],
            ["srv_datatype_handling.js", enuFileType::SharedLibrary],
            ["srv_forms_and_controls.js", enuFileType::SharedLibrary],
            ["srv_html_output.js", enuFileType::SharedLibrary],
            ["srv_misc.js", enuFileType::SharedLibrary],
            ["srv_objects_and_arrays.js", enuFileType::SharedLibrary],
            ["app_global.js", enuFileType::AppLibrary],
            ["app_event_handler.js", enuFileType::AppUI],
            ["app_class.js", enuFileType::AppLibrary],
            ["app_misc.js", enuFileType::AppLibrary],
            ["app_plan.js", enuFileType::AppUI],
            ["app_init.js", enuFileType::AppLibrary],
            ["app_extensibility.js", enuFileType::AppLibrary]
        ];

               
        //Include startup.js and environment.js from root folders
        //---------------------------------------------------------------
        $sTempFilePath =  cstURL::$WebServerFileRoot . DIRECTORY_SEPARATOR . "startup.js";
        $sIncludeFileContents = file_get_contents($sTempFilePath);
        echo $sIncludeFileContents . "\n";
        
        
        $sTempFilePath = cstURL::$EnvironmentFileRoot . DIRECTORY_SEPARATOR . "environment_init.js";
        $sIncludeFileContents = file_get_contents($sTempFilePath);
        echo $sIncludeFileContents . "\n";
        //---------------------------------------------------------------
     
        foreach ($arrFilesToInclude as [$sFileName, $eFileType]) {

            
            if ($eFileType === enuFileType::SharedLibrary) {
                $sRootPath = $sServerLibRootPath;
            } else if ($eFileType === enuFileType::AppLibrary) {
                $sRootPath = $sApplicationLibRootPath;
            } else if ($eFileType === enuFileType::AppUI) {
                if ($bIncludeUILibrary === true) {
                    $sRootPath = $sApplicationLibRootPath;
                } else {
                    $sRootPath = "";
                }
            } else {
                $sRootPath = "";
            }


        
            if (strlen(trim($sRootPath)) > 0) {

                $sFullFilePath = $sRootPath . $sFileName;

                if (mCheckIfFileExists($sFullFilePath) === true) {
                    $sIncludeFileContents = file_get_contents($sFullFilePath);
                    echo $sIncludeFileContents . "\n";
                    mOutputIncludeFile($sFullLogFilePath, $sIncludeFileContents);

                } elseif (mCheckIfFileExists($sFullFilePath) === false) {
                    mOutputIncludeFile($sFullLogFilePath, $sFileName . " was not found for JSInclude inclusion.");
                }

            } else {
                mOutputIncludeFile($sFullLogFilePath, $sFileName . " could not be included in JSInclude output.");
            }

        }


    } catch (Throwable $oError) {
        throw $oError;
    }  


    // Simple logging function
    function mOutputIncludeFile($sFileLogPath, $sContentToLog) {
        try {

            file_put_contents($sFileLogPath, "[" . date("Y-m-d H:i:s") . "] " . $sContentToLog . "\r\n", FILE_APPEND);

        } catch (Throwable $oError) {
            error_log("mOutputIncludeFile failed: " . $oError->getMessage());
        }    
    }

    function mCheckIfFileExists($sFilePath) {
        try {

            $bFileExists = false;

            if (!file_exists($sFilePath)) {
                $bFileExists = false;

            } else if (file_exists($sFilePath)) {
                $bFileExists = true;
            }

            return ($bFileExists);

        } catch (Throwable $oError) {
            throw $oError;
        }               
    }

?>