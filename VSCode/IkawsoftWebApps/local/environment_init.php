<?php


    if (!defined('URL_SEPARATOR')) {
        define('URL_SEPARATOR', '/');
    }

        
    class cstURL {
        public static string $WebServerURLRoot;
        public static string $WebServerFileRoot;
        public static string $EnvironmentURLRoot;
        public static string $EnvironmentFileRoot;
        public static string $ApplicationURLRoot;
        public static string $ApplicationFileRoot;
        public static string $ServerIncludeURLRoot;
        public static string $ServerIncludeFileRoot;

        public static function init($sWebServerURLRoot, $sWebServerFileRoot, $sEnvironmentURLRoot, $sEnvironmentFileRoot, $sApplicationURLRoot, $sApplicationFileRoot, $sServerIncludeURLRoot, $sServerIncludeFileRoot) {
            self::$WebServerURLRoot      = $sWebServerURLRoot;
            self::$WebServerFileRoot     = $sWebServerFileRoot;
            self::$EnvironmentURLRoot    = $sEnvironmentURLRoot;
            self::$EnvironmentFileRoot   = $sEnvironmentFileRoot;
            self::$ApplicationURLRoot    = $sApplicationURLRoot;
            self::$ApplicationFileRoot   = $sApplicationFileRoot;
            self::$ServerIncludeURLRoot  = $sServerIncludeURLRoot;
            self::$ServerIncludeFileRoot = $sServerIncludeFileRoot;
        }
    }
    
    //Initialize thee Class
    $sWebServerRootURL = "https://" . $_SERVER['HTTP_HOST'];

    cstURL::init(
            $sWebServerRootURL, //Web Server URL Root
            $_SERVER['DOCUMENT_ROOT'], //Web Server File Root
            $sWebServerRootURL . URL_SEPARATOR . mGetApplicationEnvironment($_SERVER['REQUEST_URI']), //Environment URL Root
            $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . mGetApplicationEnvironment($_SERVER['REQUEST_URI']), //Environment File Root
            mGetRootPath(ApplicationID::DR, enuPathRootType::URL, enuPathType::ApplicationFolder, enuFolderType::NoSubFolder), //Application URL Root
            mGetRootPath(ApplicationID::DR, enuPathRootType::File, enuPathType::ApplicationFolder, enuFolderType::NoSubFolder), //Application File Root
            mGetRootPath(ApplicationID::DR, enuPathRootType::URL, enuPathType::SharedLibraryFolder, enuFolderType::NoSubFolder), //Server Include URL Root
            mGetRootPath(ApplicationID::DR, enuPathRootType::File, enuPathType::SharedLibraryFolder, enuFolderType::NoSubFolder)); //Server Include File Root
    
    function mGetErrorLogPath() {
	    try {

            $sErrorLogPath = "";

            $sErrorLogPath = mGetRootPath(ApplicationID::DR, enuPathRootType::File, enuPathType::ApplicationFolder, enuFolderType::NoSubFolder);
            $sErrorLogPath = $sErrorLogPath . 'app_error.txt';

            return($sErrorLogPath);

        } catch (Throwable $oError) {
            throw $oError;
        }
    }

    function mLogString($sOutputString) {

        try {

            $sDelimiter = "";
            $sLogFilePath = "";
            $sLogString = "";
            $sHeaderRow = "Timestamp|Full User Name|User ID|User Type|Environment|Root URL|Application ID|Selected Platform|Selected Year|Selected Page|Message";
            $sDateTimeFormat = "";
            $dtLastErrorDateTime = null;
            $dtNowDateTime = null;
            $bLoggingResult = false;
            $bSendErrorEmail = false;


            $sLogFilePath = mGetErrorLogPath();
            $sDateTimeFormat = "Y-m-d H:i:s";
            $sDelimiter = "|";

            clearstatcache(true, $sLogFilePath);
            if (filesize($sLogFilePath) === 0) {
                error_log($sHeaderRow . PHP_EOL, 3, $sLogFilePath);
            }

            $sLogString = date('m-d-Y h:i:s A') . $sDelimiter;
            $sLogString = $sLogString . $_SESSION["FullUserName"] . $sDelimiter;
            $sLogString = $sLogString . $_SESSION["UserID"] . $sDelimiter;            
            $sLogString = $sLogString . $_SESSION["UserType"] . $sDelimiter;                            
            $sLogString = $sLogString . $_SESSION["Application_Environment"] . $sDelimiter;                    
            $sLogString = $sLogString . $_SESSION["root_url"] . $sDelimiter;                    
            $sLogString = $sLogString . $_SESSION["Application_ID"] . $sDelimiter;                    
            $sLogString = $sLogString . $_SESSION["SelectedPlatform"] . $sDelimiter;                    
            $sLogString = $sLogString . $_SESSION["SelectedYear"] . $sDelimiter;                    
            $sLogString = $sLogString . $_SESSION["SelectedPage"] . $sDelimiter;                                
            $sLogString = $sLogString . $sOutputString;                                
            $sLogString = str_replace(["\r", "\n"], "", $sLogString);
            $sLogString = $sLogString . PHP_EOL;

            //Log The Message
            $bLoggingResult = error_log($sLogString, 3, $sLogFilePath);

            //Send An E-mail if it's an error and it has been 3 seconds since the last log
            if (stripos($sLogString, "Error") !== false) {   

                if (isset($_SESSION["LastErrorDateTime"]) && $_SESSION["LastErrorDateTime"] !== "") {

                    if (mIsDateTime($_SESSION["LastErrorDateTime"], $sDateTimeFormat) === true) {
                        $dtLastErrorDateTime = new DateTime($_SESSION["LastErrorDateTime"]);
                        $dtNowDateTime = new DateTime();

                        if ($dtNowDateTime->getTimestamp() - $dtLastErrorDateTime->getTimestamp() >= 7) {
                            $bSendErrorEmail = true;
                        }
                    }

                } else if (isset($_SESSION["LastErrorDateTime"]) && $_SESSION["LastErrorDateTime"] === "") {
                    $bSendErrorEmail = true;
                }

                if ($bSendErrorEmail === true) {
                        mSendAdminEmail($sLogString);
                        $_SESSION["LastErrorDateTime"] = date($sDateTimeFormat);
                }
            }

            if ($bLoggingResult === false) {
                echo("Logging Failed" .  $sLogFilePath);
            }
            
        } catch (Throwable $oError) {
            echo("Error: An Error has occurred in mLogString. The error details are " . $oError->getMessage());
        }
    }

    //This function takes in a String and checks if it is a valid DateTime
    function mIsDateTime($sValueToCheck, $sDateTimeFormat = "Y-m-d H:i:s") {
        try {

            $sFormattedDateTime = "";
            $bIsDateTimeReturnBoolean = false;

            if (is_string($sValueToCheck) && trim($sValueToCheck) !== "") {

                $dtReturnDateTime = DateTime::createFromFormat($sDateTimeFormat, $sValueToCheck);

                if ($dtReturnDateTime !== false) {
                    $bIsDateTimeReturnBoolean = true;
                } else {
                    $bIsDateTimeReturnBoolean = false;
                }

                if ($bIsDateTimeReturnBoolean === true) {
                    $sFormattedDateTime = $dtReturnDateTime->format($sDateTimeFormat);

                    if ($sFormattedDateTime === $sValueToCheck) {
                        $bIsDateTimeReturnBoolean = true;
                    } else {
                        $bIsDateTimeReturnBoolean = false;
                    }
                }
            }

            return $bIsDateTimeReturnBoolean;

        } catch (Throwable $oError) {
            throw $oError;
        }        
    }


    function mBase64url_Encode($sInputString) {

        $sReturnString = "";

        try {
            // Standard base64, then make it URL-safe
            $sReturnString = rtrim(strtr(base64_encode($sInputString), '+/', '-_'), '=');

            return ($sReturnString);

        } catch (Throwable $oError) {
            echo($oError->getMessage());

        }

    }

    function mBase64url_Decode($sInputString) {

        // Pad with '=' if needed
        $sReturnString = "";
        $sPadString = "";

        try {

            $sPadString = strlen($sInputString) % 4;
            if ($sPadString > 0) {
                $sInputString .= str_repeat('=', 4 - $sPadString);
            }

            $sReturnString = base64_decode(strtr($sInputString, '-_', '+/'));

            return ($sReturnString);

        } catch (Throwable $oError) {
            throw $oError;
        }

    }

    function mEncryptString($sInputString, $sEncryptionKey) {

        $sCipher = "";
        $sInitiatlizationVector = "";
        $iInitiatlizationVectorLength = 0;
        $sReturnString = "";

        try {

            $sCipher = "AES-256-CBC";
            $iInitiatlizationVectorLength = openssl_cipher_iv_length($sCipher);
            $sInitiatlizationVector = openssl_random_pseudo_bytes($iInitiatlizationVectorLength);

            $ciphertext = openssl_encrypt($sInputString, $sCipher, $sEncryptionKey, OPENSSL_RAW_DATA, $sInitiatlizationVector);

            $sReturnString = mBase64url_Encode($sInitiatlizationVector . $ciphertext);

            return ($sReturnString);

        } catch (Throwable $oError) {
            throw $oError;
        }

    }

    function mDecryptString($sInputString, $sEncryptionKey) {

        $sCipher = "";
        $iInitializationVectorLength = 0;
        $sReturnString = "";
        $sInitializationVector = "";

        try {
            $sCipher = "AES-256-CBC";
            $iInitializationVectorLength = openssl_cipher_iv_length($sCipher);

            // Decode the URL-safe base64 string
            $sReturnString = mBase64url_Decode($sInputString);

            // Extract IV and ciphertext
            $sInitializationVector = substr($sReturnString, 0, $iInitializationVectorLength);
            $sReturnString = substr($sReturnString, $iInitializationVectorLength);

            // Decrypt using ciphertext + IV
            $sReturnString = openssl_decrypt($sReturnString, $sCipher, $sEncryptionKey, OPENSSL_RAW_DATA, $sInitializationVector);

            if ($sReturnString === false) {
                throw new Exception("Decryption failed: Input String: " . $sInputString . " EncryptionKey: " . bin2hex($sEncryptionKey));
            }

            return ($sReturnString);

        } catch (Throwable $oError) {
            throw $oError;
        }
    }

    function mIsIso($sValue) {
        // Declare variables
        $sPattern = "";
        $iMatchResult = 0;

        try {
            // Defensive check: must be a non-empty string
            if (!is_string($sValue)) {
                return false;
            }

            if ($sValue === "") {
                return false;
            }

            // Define ISO 8601 pattern
            $sPattern = '/^
                \d{4}-\d{2}-\d{2}          # Year-Month-Day
                T
                \d{2}:\d{2}                # Hour:Minute
                (:\d{2}(\.\d{1,7})?)?      # Optional seconds and fractional seconds
                (Z|[+-]\d{2}:\d{2})?       # Optional timezone
            $/x';

            // Execute pattern match
            $iMatchResult = preg_match($sPattern, $sValue);

            // Return result
            if ($iMatchResult === 1) {
                return true;
            }

            return false;

        } catch (Throwable $oError) {
            throw $oError;
        }
    }


   
    function mGetApplicationVersionNumber(ApplicationID $iApplicationID) {
	try {

	    $sVersionNumberFilePath = "";
	    $sVersionNumber = "";

	    $sVersionNumberFilePath = mGetRootPath($iApplicationID, enuPathRootType::File, enuPathType::ApplicationFolder, enuFolderType::NoSubFolder) . "version_number.txt";

	    $sVersionNumber = trim(file_get_contents($sVersionNumberFilePath));

	    return $sVersionNumber;

	} catch (Throwable $oError) {
	    throw $oError;
	}
    }


    function mSingleQuotes($sInputString) {

        $sReturnString = "";

        try {

            $sReturnString = $sInputString;
            $sReturnString = "'" . $sReturnString . "'";

            return($sReturnString);

        } catch (Throwable $oError) {
            throw $oError;
        }
    }
    

?>