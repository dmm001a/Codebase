<?php
    //$_SERVER['REQUEST_URI']

    
    enum enuIncludePackageID: int {
        case None = 0;
        case InitOnly = 1;
        case InitConfig = 2;
        case VendorClass = 3;
        case PHPClass = 4;     
        case SharedPHPLib = 5;
    }
    
    enum ApplicationID: int {
        case None = 0;
        case DR = 1;
        case Roadmap = 2;
    }

    enum enuApplicationEnvironmentID: int {
        case None = 0;
        case Local = 1;
        case Smoke = 2;
        case QA = 3;
        case Stage = 4;
        case Prod = 5;
    }

    enum enuPathRootType: int {
        case File = 0;
        case URL  = 1;
    }

    enum enuPathType: int {
        case ApplicationFolder = 0;
        case SharedLibraryFolder  = 1;
        case WebServerRootURL = 2;
        case DBDSN = 3;
        case DBUserName = 4;
        case DBPassword = 5;
    }

    enum enuFolderType: int {
        case NoSubFolder = 0;
        case ui = 1;
        case lib  = 2;
        case DB  = 3;
    }

    function mGetApplicationEnvironment ($sURLExtension, $bReturnApplicationEnvironmentID = false) {
        
        try {

            $sApplicationEnvironmentVariableName = "";
            $arrURLParts = [];
            $arrValidEnvironment = [];
            
            $sURLExtension = ltrim($sURLExtension, '/');
            
            $arrURLParts = explode('/', $sURLExtension);
            $sApplicationEnvironmentVariableName = $arrURLParts[0];
	        $sApplicationEnvironmentVariableName =  strtolower($sApplicationEnvironmentVariableName);
            
            $arrValidEnvironment = ['local', 'smoke', 'qa', 'stage', 'prod'];

            if ($bReturnApplicationEnvironmentID === false) {

                if (in_array($sApplicationEnvironmentVariableName, $arrValidEnvironment, true)) {
                    return($sApplicationEnvironmentVariableName);
                } else {
                    throw new Exception("Valid environment is not found in URL.");
                }

            } else if ($bReturnApplicationEnvironmentID === true) {

                foreach (enuApplicationEnvironmentID::cases() as $oEnumValue) {
                    if (strcasecmp($oEnumValue->name, $sApplicationEnvironmentVariableName) === 0) {
                        return $oEnumValue->value;
                    }
                }

                return enuApplicationEnvironmentID::None->value;
            }
            
        } catch (Throwable $oError) {

            echo("Error:  " . $oError->getMessage());
            die();
        }
        
    }

    function mGetApplicationID($sURLExtension, $bReturnAsEnum = false) {

        try {

            $vApplicationID = -1;

            $sApplicationName = mGetApplicationName($sURLExtension);

            foreach (ApplicationID::cases() as $eApplicationName) {
                if (strcasecmp($eApplicationName->name, $sApplicationName) === 0) {

                    if ($bReturnAsEnum === true) {
                        $vApplicationID = $eApplicationName;        
                    } else if ($bReturnAsEnum === false) {
                        $vApplicationID = (int)$eApplicationName->value;        
                    }
                    break;
                }
            }

	        return ($vApplicationID);

        } catch (Throwable $oError) {
                echo("Error:  " . $oError->getMessage());
                die();
        }
    }
    

    function mGetApplicationName($sURLExtension) {

        try {

            $sApplicationName = "";
            $iAppPositionIndex = -1;
            $arrURLParts = [];

            $arrURLParts = explode('/', trim($sURLExtension, '/'));

            // Find "app" and take the next segment
            $iAppPositionIndex = array_search('app', $arrURLParts);

            if ($iAppPositionIndex !== false && isset($arrURLParts[$iAppPositionIndex + 1])) {
                $sApplicationName = $arrURLParts[$iAppPositionIndex + 1];
            } else {
                $sApplicationName = '';
            }
            
            return ($sApplicationName);

        } catch (Throwable $oError) {
                echo("Error:  " . $oError->getMessage());
                die();
        }

    }

    function mIncludePage (enuIncludePackageID $ienuIncludePackageID, $sApplicationEnvironment) {
        try {
            
            $sTempPath = "";
            
            if ($ienuIncludePackageID === enuIncludePackageID::InitOnly) {
                
                $sTempPath  = DIRECTORY_SEPARATOR . $sApplicationEnvironment . DIRECTORY_SEPARATOR . 'environment_init.php';
                $sIncludeFilePath = $_SERVER['DOCUMENT_ROOT'] . $sTempPath;

                require_once $sIncludeFilePath;
                
            } else if ($ienuIncludePackageID === enuIncludePackageID::InitConfig) {
                
                $sTempPath  = DIRECTORY_SEPARATOR . $sApplicationEnvironment . DIRECTORY_SEPARATOR . 'environment_init.php';
                $sIncludeFilePath = $_SERVER['DOCUMENT_ROOT'] . $sTempPath;
                require_once $sIncludeFilePath;
		
                $sApplicationFileDirectory = mGetServerEnvironmentVariable(ApplicationID::DR, enuPathType::ApplicationFolder);
                $sIncludeFilePath  = $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . $sApplicationEnvironment  . $sApplicationFileDirectory . 'lib_include' . DIRECTORY_SEPARATOR . 'config.php';
                require_once $sIncludeFilePath;

            } else if ($ienuIncludePackageID === enuIncludePackageID::VendorClass) {
              
                require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'vendor' . DIRECTORY_SEPARATOR . 'autoload.php';

             } else if ($ienuIncludePackageID === enuIncludePackageID::PHPClass) {

                $sIncludeFilePath = mGetRootPath(ApplicationID::None, enuPathRootType::File, enuPathType::SharedLibraryFolder, enuFolderType::lib) . "srv_class.php";
                require_once $sIncludeFilePath;

             } else if ($ienuIncludePackageID === enuIncludePackageID::SharedPHPLib) {

                $sIncludeFilePath = mGetRootPath(ApplicationID::None, enuPathRootType::File, enuPathType::SharedLibraryFolder, enuFolderType::lib) . "srv_class.php";
                require_once $sIncludeFilePath;       
                
                $sIncludeFilePath = mGetRootPath(ApplicationID::None, enuPathRootType::File, enuPathType::SharedLibraryFolder, enuFolderType::lib) . "srv_misc.php";
                require_once $sIncludeFilePath;       
                
            }
            
        } catch (Throwable $oError) {
            echo("Error:  " . $oError->getMessage());
            die();
        }     
        
    }

    function mGetRootPath(ApplicationID $eApplicationID, enuPathRootType $ePathRootType, enuPathType $ePathType, enuFolderType $eFolderType) {

        try {
        
            $sPathRoot = "";
            $sRoutingPath = "";
            $sFolder = "";
            $sEnvironmentVariableName = "";
            $sReturnPath = "";

            if ($eApplicationID === null) {
                throw new Exception("ApplicationID is required by mGetRootPath");
            }

            //PATHROOT: Get the root as a file path or a url path
            if ($ePathRootType === enuPathRootType::File) {
                $sPathRoot = $_SERVER['DOCUMENT_ROOT']; //File
            } else if ($ePathRootType === enuPathRootType::URL) {
                $sPathRoot = mGetRootURL($_SERVER['HTTP_HOST']); //URL
            } else {
                throw new Exception("mGetRootPath: $ePathRootType must be 1 or 2.  Value Provided: " . $ePathRootType->value);
            }
            
            //enuPathType: Get the subdirectories for applications or the server include
            if ($ePathType === enuPathType::ApplicationFolder && $eApplicationID !== ApplicationID::None) {

                $sRoutingPath = mGetServerEnvironmentVariable($eApplicationID, $ePathType);

            } else if ($ePathType === enuPathType::SharedLibraryFolder) {
                $sRoutingPath = mGetServerEnvironmentVariable($eApplicationID, $ePathType);
            } else if ($ePathType === enuPathType::WebServerRootURL) {
                $sRoutingPath = "/";     
            } else {
                throw new Exception("mGetRootPath: $ePathType must be 1 or 2.  Value Provided: " . $ePathType->value);
            }

            //enuFolderType
            if ($eFolderType === enuFolderType::NoSubFolder) {
                //Application Root or Server Root and not an application enuPathType. No Folder Append
            } else if ($eFolderType === enuFolderType::ui) {   
                $sFolder = $sFolder. "ui_include/"; //ui_include
            } else if ($eFolderType === enuFolderType::lib) {
                $sFolder = $sFolder. "lib_include/"; //lib_include
            } else if ($eFolderType === enuFolderType::DB) {
                $sFolder = $sFolder. "db_include/"; //db_include
            } else {
                throw new Exception("mGetRootPath: $eFolderType must be 1, 2 or 3.  Value Provided: " . $eFolderType->value);
            }

            $sReturnPath = $sPathRoot . "/" . mGetApplicationEnvironment($_SERVER['REQUEST_URI']) . $sRoutingPath . $sFolder;

            // Handle Slashes - Standardize the Slash Types Based on the Whether File or URL Based
            if ($ePathRootType === enuPathRootType::File) {
                $sReturnPath = str_ireplace(URL_SEPARATOR, DIRECTORY_SEPARATOR, $sReturnPath); //File
            } else if ($ePathRootType === enuPathRootType::URL) {
                $sReturnPath = str_ireplace(DIRECTORY_SEPARATOR, URL_SEPARATOR, $sReturnPath); //URL
            } else {
                throw new Exception("mGetRootPath: $ePathRootType must be 1 or 2.  Value Provided: " . $ePathRootType->value);
            }

            return($sReturnPath);

        } catch (Throwable $oError) {
            echo($oError->getMessage());

        }
    }
    
    function mGetRootURL($sHTTPHost) {
        try {

            $sProtocol = "";
            $sHost = "";
            $sRootURL = "";

            $sProtocol = (!empty($_SERVER['HTTPS']) && $_SERVER['HTTPS'] !== 'off') ? "https://" : "http://";
            //$sHost = $_SERVER['HTTP_HOST'];
            $sRootUrl = $sProtocol . $sHTTPHost;

            return $sRootUrl;

        } catch (Throwable $oError) {

            throw $oError;

        }
    }
    
    function mGetServerEnvironmentVariable(ApplicationID $eApplicationID, enuPathType $eServerEnvironmentVariableType) {
        try {

            $sApplicationName = "";
            $sApplicationEnvironmentName = "";
            $sEnvironmentVariableName = "";
            $sReturnValue = "";

            $sApplicationName = $eApplicationID->name;

            if ($eServerEnvironmentVariableType === enuPathType::WebServerRootURL) {

                throw new Exception("WebServerRootURL is not valid for mGetServerEnvironmentVariable.");
                
            } else {
                if ($eServerEnvironmentVariableType === enuPathType::ApplicationFolder) {

                    if ($eApplicationID !== null) {

                        $sEnvironmentVariableName = $sApplicationName . "_Application_File_Directory";
   
                    } else {
                        throw new Exception("Application Type is not configured for mGetServerEnvironmentVariable");
                    }
                } else if ($eServerEnvironmentVariableType === enuPathType::SharedLibraryFolder) {

                        //$sApplicationEnvironmentName = mGetApplicationEnvironment($_SERVER['REQUEST_URI']);

                        $sEnvironmentVariableName = "Server_File_Directory";

                } else if ($eServerEnvironmentVariableType === enuPathType::DBDSN) {
            
                    $sEnvironmentVariableName = $sApplicationName . "_DB_Connection_String";

                } else if ($eServerEnvironmentVariableType === enuPathType::DBUserName) {

                    $sEnvironmentVariableName = $sApplicationName . "_DB_Username";

                } else if ($eServerEnvironmentVariableType === enuPathType::DBPassword) {

                    $sEnvironmentVariableName = $sApplicationName . "_DB_Password";                    
                }

                $sReturnValue = getenv($sEnvironmentVariableName); 

                return($sReturnValue);

            }
        } catch (Throwable $oError) {
            throw $oError;
        }
    }

    function mGetApplicationDirectory(ApplicationID $eApplicationID) {
        
        try {
            
            $sApplicationDirectory = "";
       
            $sApplicationDirectoryEnvironmentVariable = mGetServerEnvironmentVariable($eApplicationID, enuPathType::ApplicationFolder);
            
            $sApplicationDirectory = str_ireplace(DIRECTORY_SEPARATOR . "app" . DIRECTORY_SEPARATOR, "", $sApplicationDirectoryEnvironmentVariable);
            $sApplicationDirectory = rtrim($sApplicationDirectory, DIRECTORY_SEPARATOR);
            
            return($sApplicationDirectory);
            
        } catch (Throwable $oError) {
            throw $oError;
        }
    }

    function mIncludeJSInclude($sURLExtension, $iDirectApplicationID = -1, $bIncludeUILibrary = false) {
        try {

            if ($iDirectApplicationID === -1) {
                $iApplicationID = mGetApplicationID($sURLExtension);
            } else {
                $iApplicationID = $iDirectApplicationID;
            }

            $eApplicationID = ApplicationID::from($iApplicationID);

            //Include JS Include.php
            $sQueryStringVariable = "&AppID=" . urlencode($iApplicationID);
            $sQueryStringVariable = $sQueryStringVariable . "&IncludeUILib=" . urlencode($bIncludeUILibrary);

            $sURL = mGetRootPath($eApplicationID, enuPathRootType::URL, enuPathType::SharedLibraryFolder, enuFolderType::lib) . "js_include.php?v=1.0";
            $sURL = $sURL . $sQueryStringVariable;

            echo("<script src='" . $sURL . "'></script>");

        } catch (Throwable $oError) {
            throw $oError;
        }
    }
?>
