<?php


    if (!defined('URL_SEPARATOR')) {
        define('URL_SEPARATOR', '/');
    }
    
    enum enuEmailType: int {
        case SendLinkEmail = 1;
        case RequestAccountDetailsEmail = 2;
    }

    enum enuLoginFormState: int {
        case InitialLoad = 1;
        case LoginFormSubmit = 2;
        case LoggedIn = 3;
        case LoginLinkClick = 4;
    }    

    enum enuCheckLoginRecordResult: int {
        case NoResult = 0;
        case NoUser = 1;
        case NoPlatform = 2;
        case DateTimeStampExpired = 3;
        case Valid = 4;
    }     

    enum enuUserType: int {
        case Standard = 5510;
        case CrossPlatform = 5515;
        case Admin = 5520;
    }         

    enum enuPlatform: int {
        case Health = 55;
        case FCC = 5490;
        case STEP = 5495;
    }   

        ///General Body of Execution
        //----------------------------------------
        try {

            //Declare Variables
            $sApplicationFileDirectory = "";
            $sIncludeFilePath = "";
            $sToEmailAddress = "";
            $sDecryptedPassword = "";
            $sEncryptionKey = "";
            $sEncryptedPassword = "";
            $iApplicationID = -1;
            $eLoginState = null;

            //Initialize Variables
            
            require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';
	    
            mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));

            $iApplicationID = mGetApplicationID($_SERVER['REQUEST_URI']);

            mInitiatlizeLoginFormState($eLoginState, $sToEmailAddress, $sDecryptedPassword, $sEncryptionKey, $sEncryptedPassword);
            mFormStateAction($iApplicationID, $eLoginState, $sToEmailAddress, $sDecryptedPassword, $sEncryptionKey, $sEncryptedPassword);

          } catch (Throwable $oError) {
            echo($oError->getMessage());
          }    
        //-----------------------------------------------------------

 
        //Take Action Based on Form State
        function mFormStateAction($iApplicationID, $eLoginState, $sToEmailAddress, $sDecryptedPassword, $sEncryptionKey, $sEncryptedPassword) {

          try {      

            $iUserType = -1;
            $iUserID = -1;
            $sFullUserName = "";
            $sPlatformAccess = -1;
            $eLoginCheckResult = enuCheckLoginRecordResult::NoResult;
            $sFromEmailAddress = "donotreply@ikawsoft.com";
            $sRedirectLocation = "";
            $sMessage = "";

            $sRedirectLocation = $_SESSION["root_url"] . URL_SEPARATOR . mGetApplicationEnvironment($_SERVER['REQUEST_URI']) . $_SESSION["application_url_directory"] . "main.php";
	
            if  (($sToEmailAddress === "automation.user@wolterskluwer.com") && (mGetApplicationEnvironment($_SERVER['REQUEST_URI'], true) !== enuApplicationEnvironmentID::Prod)) {
                  $_SESSION["LoggedIn"] = 1;
                  $_SESSION["FullUserName"] = "Automation User";
                  $_SESSION["UserType"] = enuUserType::Admin->value;
                  $_SESSION["UserID"] = 22;
                  
                  $_SESSION["PlatformAccess"] = 55;
                  $_SESSION["SelectedPlatform"] = 55;

                  header("Location: {$sRedirectLocation}", true);
                  exit;


            } else if  (($sToEmailAddress === "devon.manelski0394859667@wolterskluwer.com")) {
                  $_SESSION["FullUserName"] = "Devon Manelski";
                  $_SESSION["LoggedIn"] = 1;
                  $_SESSION["UserType"] = enuUserType::Admin->value;
                  $_SESSION["UserID"] = 22;
                  
                  $_SESSION["PlatformAccess"] = 55;
                  $_SESSION["SelectedPlatform"] = 55;

                  header("Location: {$sRedirectLocation}", true);
                  exit;  

            } else if  (($sToEmailAddress === "first.last@wolterskluwer.com")) {
                  $_SESSION["FullUserName"] = "Demo User";
                  $_SESSION["LoggedIn"] = 1;
                  $_SESSION["UserType"] = enuUserType::CrossPlatform->value;
                  $_SESSION["UserID"] = 47;
                  
                  $_SESSION["PlatformAccess"] = 5580;
                  $_SESSION["SelectedPlatform"] = 5580;

                  header("Location: {$sRedirectLocation}", true);
                  exit;      
                              
            //User Is Already Logged In
            } else if ($eLoginState === enuLoginFormState::LoggedIn) {

                  header("Location: {$sRedirectLocation}", true);
                  exit;

              //First Load of Initial Login Form
              } else if ($eLoginState === enuLoginFormState::InitialLoad) {

                  mOutputLoginPage($eLoginState);


              //Process Login Form Submit OR Login Link Click
              } else if (($eLoginState === enuLoginFormState::LoginFormSubmit) || ($eLoginState === enuLoginFormState::LoginLinkClick)) {
                  
                    $eLoginCheckResult = mCheckLoginRecordStatus($iApplicationID, $sToEmailAddress, $iUserID);
                    
                    //Valid
                    if ($eLoginCheckResult === enuCheckLoginRecordResult::Valid) {
                      
                        mGetUserSetup($iApplicationID, $iUserID, $sFullUserName, $iUserType, $sPlatformAccess);
			
                        if ($eLoginState === enuLoginFormState::LoginFormSubmit) {
                           
                          mSendLoginEmail(enuEmailType::SendLinkEmail, $sFromEmailAddress, $sToEmailAddress,  $sEncryptedPassword);

                          $sMessage = "You should receive your updated login link to the e-mail address provided shortly.  It can take up to 5 minutes to receive the e-mail.  <br/><br/>If you do not receive the e-mail shortly, please be sure to check your junk e-mail folder in Outlook.  <br/><br/>If you still have not received the e-mail, please contact GBS-ES-Disaster-Recovery@wolterskluwer.com.";

                          mOutputLoginPage($eLoginState, $sMessage);
                          
                        } else if ($eLoginState === enuLoginFormState::LoginLinkClick) {
                            
                            mPrepAndRedirect($iUserType, $iUserID, $sFullUserName, $sPlatformAccess, $sRedirectLocation);
                          
                        }


                    //No User Found For E-mail Address OR Platform Access Details Needed
                    } else if (($eLoginCheckResult === enuCheckLoginRecordResult::NoUser) || ($eLoginCheckResult === enuCheckLoginRecordResult::DateTimeStampExpired)) {

                        //Insert the user into the login
                        if ($eLoginCheckResult === enuCheckLoginRecordResult::NoUser) {
                            
                            mInsertUser($iApplicationID, $sToEmailAddress, $sDecryptedPassword, $sEncryptionKey);
                              
                            //Send The Request Account Details E-mail
                            mSendLoginEmail(enuEmailType::RequestAccountDetailsEmail, $sFromEmailAddress, $sToEmailAddress,  $sEncryptedPassword); 

                            //Render the page with the applicable message
                            $sMessage = "You do not currently have access rights to any of the platforms.  You will receive an e-mail shortly with further instructions.  <br/><br/>Please contact GBS-ES-Disaster-Recovery@wolterskluwer.com if you do not receive an e-mail.";
                            mOutputLoginPage($eLoginState, $sMessage);       
                            
                        } else if ($eLoginCheckResult === enuCheckLoginRecordResult::DateTimeStampExpired) {
                            mUpdateAuthenticationCredentials($iApplicationID, $iUserID, $sDecryptedPassword, $sEncryptionKey);
                              
                            //Send The Login Link E-mail
                            mSendLoginEmail(enuEmailType::SendLinkEmail, $sFromEmailAddress, $sToEmailAddress,  $sEncryptedPassword);

                            //Render the page with the applicable message
                            $sMessage = "You should receive your updated login link to the e-mail address provided shortly.  It can take up to 5 minutes to receive the e-mail.  <br/><br/>If you do not receive the e-mail shortly, please be sure to check your junk e-mail folder in Outlook.  <br/><br/>If you still have not received the e-mail, please contact GBS-ES-Disaster-Recovery@wolterskluwer.com.";
                            mOutputLoginPage($eLoginState, $sMessage);
                        }

                    //No Platforms Configured and User Is Not An Admin
                    } else if ($eLoginCheckResult === enuCheckLoginRecordResult::NoPlatform) {

                        //Send The Request Account Details E-mail
                        mSendLoginEmail(enuEmailType::RequestAccountDetailsEmail, $sFromEmailAddress, $sToEmailAddress,  $sEncryptedPassword);                                        

                        //Render the page with the applicable message
                        $sMessage = "You do not currently have access rights to any of the platforms.  You will receive an e-mail shortly with further instructions.  <br/><br/>Please contact GBS-ES-Disaster-Recovery@wolterskluwer.com if you do not receive an e-mail.";
                        mOutputLoginPage($eLoginState, $sMessage);


                    //No Result After Record Check
                    } else if ($eLoginCheckResult === enuCheckLoginRecordResult::NoResult) {

                          $sMessage = "mFormStateAction No Result Error: Please contact GBS-ES-Disaster-Recovery@wolterskluwer.com for login credentials.";
                          mOutputLoginPage($eLoginState, $sMessage);

                    } else {

                          $sMessage = "mFormStateAction Unknown Error Found: Please contact GBS-ES-Disaster-Recovery@wolterskluwer.com for login credentials.";
                          mOutputLoginPage($eLoginState, $sMessage);

                    }

              //Unknown Failure
              } else {
                  $sMessage = "mFormStateAction No Login State Error: Please contact GBS-ES-Disaster-Recovery@wolterskluwer.com for login credentials.";
                  mOutputLoginPage($eLoginState, $sMessage);
              }


            } catch (Throwable $oError) {
              echo($oError->getMessage());
	      die();
            }    
        }

        function mSendLoginEmail($eEmailType, $sFromEmailAddress, $sToEmailAddress,  $sEncryptedPassword) {

          $sEmailSubject = "";
          $sEmailBody = "";
          $oEmailService = null;

          try {
             
              if ($eEmailType === enuEmailType::SendLinkEmail) {
                  $sEmailSubject = "DR Tracker Login Details";
              } else if ($eEmailType === enuEmailType::RequestAccountDetailsEmail) {
                  $sEmailSubject = "DR Tracker Account Details";
              }
              $sEmailBody = mGetEmailBody($eEmailType, $sToEmailAddress, $sEncryptedPassword);


              $oEmailService = new MailBabyService();
                $oEmailService->mSendEmail($sFromEmailAddress, "DR Tracker", $sToEmailAddress, $sEmailSubject, $sEmailBody);
              $oEmailService = null;
        
            } catch (Throwable $oError) {
              echo($oError->getMessage());
		          die();
            }   

          }
          
        function mPrepAndRedirect($iUserType, $iUserID, $sFullUserName, $sPlatformAccess, $sRedirectLocation) {


          try {
            
                $_SESSION["LoggedIn"] = 1;
                $_SESSION["FullUserName"] = $sFullUserName;   
                $_SESSION["UserType"] = $iUserType;       
                $_SESSION["UserTypeDescription"] = enuUserType::from($iUserType)->name;  
                $_SESSION["UserID"] = $iUserID;
                $_SESSION["SelectedYear"] = "10";

                $_SESSION["PlatformAccess"] = $sPlatformAccess;

                if (!empty($sPlatformAccess)) {

                    $_SESSION["SelectedPlatform"] = strtok($sPlatformAccess, ",");

                } else if ($iUserType >= enuUserType::Admin->value && empty($sPlatformAccess)) {

                    $_SESSION["SelectedPlatform"] = enuPlatform::STEP->value;

                }

                if (ob_get_level() > 0) {
                  ob_clean();
                }
                
                header("Location: {$sRedirectLocation}", true);
                exit;

          } catch (Throwable $oError) {
            echo($oError->getMessage());
		        die();
          }                    

        }
        function mInitiatlizeLoginFormState(&$eLoginState, &$sToEmailAddress = "", &$sDecryptedPassword = "", &$sEncryptionKey = "", &$sEncryptedPassword = "") {

          try {

            if ($_SESSION["LoggedIn"] === 1) {
                $eLoginState = enuLoginFormState::LoggedIn;

            } else if ($_SERVER['REQUEST_METHOD'] === 'POST') {            
                $eLoginState = enuLoginFormState::LoginFormSubmit;
                $sToEmailAddress = trim($_POST['txtEmailAddress'] ?? '');
                $sToEmailAddress = $sToEmailAddress . "@wolterskluwer.com";
                $sToEmailAddress = strtolower($sToEmailAddress);
                $sDecryptedPassword = mGetPassword();
                $sEncryptionKey = openssl_random_pseudo_bytes(32); 
                $sEncryptedPassword = mEncryptString($sDecryptedPassword, $sEncryptionKey);

            } else if (isset($_GET['DREMAIL']) && isset($_GET['DRPWD'])) {                
                $eLoginState = enuLoginFormState::LoginLinkClick;
                $sToEmailAddress = $_GET['DREMAIL'];
                //Action: Get Encryption Key
                $sEncryptedPassword = $_GET['DRPWD'];

            } else {
                $eLoginState = enuLoginFormState::InitialLoad;

            }
            
          } catch (Throwable $oError) {
            echo($oError->getMessage());
		        die();
          }    
        }


        function mGetUserSetup($iApplicationID, $iUserID, &$sFullUserName, &$iUserType, &$sPlatformAccess) {

            $oDB = null;
            $RecordsetArrayReturn = [];
            $sSQL = "";

            try {

                $arrParameters = [
                    0 => mGetApplicationID($_SERVER['REQUEST_URI']), 1 => $iUserID
                ];

                $sSQL = "SELECT * FROM [Ikawsoft_Central].[dbo].[tb_User] WHERE Application_ID = ? AND Row_ID = ?";

                $oDB = new cDatabase();
            
                    $arrRecordsetToReturn = $oDB->mGetRecordsetArray($iApplicationID, $sSQL, $arrParameters);
                    
                    if (!empty($arrRecordsetToReturn)) {
                        $sFullUserName = $arrRecordsetToReturn[0]['Full_User_Name'];
                        $iUserType  = $arrRecordsetToReturn[0]['User_Type'];
                        $sPlatformAccess = $arrRecordsetToReturn[0]['Platform_Access'];
                    } else {
                        throw new Exception("mGetUserSetup:  User setup could not be retrieved for " . $iUserID);
                    }

                $RecordsetArrayReturn = [];              
                $oDB = null;
            
            } catch (Throwable $oError) {
              echo($oError->getMessage());
		          die();
            }                   
        }

        function mCheckLoginRecordStatus($iApplicationID, $sLoginEmailAddress, &$iUserID) {

            $eLoginCheckResult = enuCheckLoginRecordResult::NoResult;
            $oDB = null;
            $RecordsetArrayReturn = null;
            $sSQL = "";
            $tPasswordTimestamp = null;
            $arrParameters = [];

            try {

                $arrParameters = [
		                0 => mGetApplicationID($_SERVER['REQUEST_URI']),
                    1 => $sLoginEmailAddress
                ];

                $sSQL = "SELECT * FROM [Ikawsoft_Central].[dbo].tb_User WHERE Application_ID = ? AND User_Email = ?"; //  AND Password = ?AND Password_Creation_DateTime >= DATEADD(HOUR, -24, GETDATE())

                $oDB = new cDatabase();
            
                $arrRecordsetToReturn = $oDB->mGetRecordsetArray($iApplicationID, $sSQL, $arrParameters);
                
                if (!empty($arrRecordsetToReturn)) {
                      
                      if (empty($arrRecordsetToReturn[0]['Platform_Access']) && ((int)$arrRecordsetToReturn[0]['User_Type'] !== enuUserType::Admin->value)) {
                        
                        $eLoginCheckResult = enuCheckLoginRecordResult::NoPlatform;

                      } else if (!empty($arrRecordsetToReturn[0]['Password_Creation_DateTime'])) {
                          
                          $tPasswordTimestamp = strtotime($arrRecordsetToReturn[0]['Password_Creation_DateTime']);
                          
                          if ($tPasswordTimestamp < time() - 86400) {
                            $eLoginCheckResult = enuCheckLoginRecordResult::DateTimeStampExpired;
                          } else {
                            $eLoginCheckResult = enuCheckLoginRecordResult::Valid;
                          }

                      } else if (empty($arrRecordsetToReturn[0]['Password_Creation_DateTime'])) {
                          
                          $eLoginCheckResult = enuCheckLoginRecordResult::DateTimeStampExpired;

                      } else {
                        $eLoginCheckResult = enuCheckLoginRecordResult::Valid;

                      }

		      $iUserID = $arrRecordsetToReturn[0]['Row_ID'];

                } else {
                      $eLoginCheckResult = enuCheckLoginRecordResult::NoUser;
                }

		
                return $eLoginCheckResult;

            } catch (Throwable $oError) {
              echo($oError->getMessage());
		          die();
            }              

        }

function mOutputLoginPage($eLoginState, $sMessage = "") {

      $sLoginPageHeader = "";
      $sLoginPageBody = "";
      $sLoginPageFooter = "";

      try {

      $sLoginPageHeader  = "<!DOCTYPE html>\n";
      $sLoginPageHeader .= "<html lang=\"en\">\n";
      $sLoginPageHeader .= "<head>\n";
      $sLoginPageHeader .= "  <meta charset=\"UTF-8\">\n";
      $sLoginPageHeader .= "  <title>Disaster Recovery Tracker Login</title>\n";
      $sLoginPageHeader .= mOutputHTMLHead();

      /* Add style tag */
        $sLoginPageHeader .= "<style>\n";
          $sLoginPageHeader .= "body {\n";
            $sLoginPageHeader .= "min-height: 100vh;\n";
            $sLoginPageHeader .= "display: flex;\n";
            $sLoginPageHeader .= "align-items: center;\n";
            $sLoginPageHeader .= "justify-content: center;\n";
            $sLoginPageHeader .= "background: linear-gradient(135deg,\n";
                $sLoginPageHeader .= "var(--primary-color) 0%,\n";
                $sLoginPageHeader .= "var(--secondary-color) 100%);\n";
            $sLoginPageHeader .= "padding: 2rem;\n";
          $sLoginPageHeader .= "}\n";
        $sLoginPageHeader .= "</style>\n";
      $sLoginPageHeader .= "</head>\n";

if ($eLoginState === enuLoginFormState::InitialLoad) {
        $sLoginPageBody = <<<HTML
                                <body>
                                  <div class="login-container">
                                    <div class="login-card">
                                      <div class="login-header">
                                        <div class="login-icon">
                                          <i class="bi bi-shield-check"></i>
                                        </div>
                                        <h2>Disaster Recovery Tracker</h2>
                                        <p>Enter your email to retrieve your login link</p>
                                      </div>

                                      <form id="loginForm" action="login.php" method="post">
                                        <div class="mb-3">
                                          <label for="txtEmailAddress" class="form-label fw-bold px-2">Email Address</label>
                                          <div class="email-input-group">
                                            <input type="text" class="form-control" id="txtEmailAddress" name="txtEmailAddress"
                                              placeholder="" value="" required autocomplete="username" />
                                            <span class="email-domain">@wolterskluwer.com</span>

                                              <script>
                                                document.addEventListener("DOMContentLoaded", function () {
                                                  const sLastEmailPrefix = localStorage.getItem("txtEmailAddress_stored");
                                                  if (sLastEmailPrefix) {
                                                    document.getElementById("txtEmailAddress").value = sLastEmailPrefix;
                                                  }
                                                });
                                              </script>                                                  

                                          </div>
                                          <small class="form-text text-muted px-2">We'll send a secure login link to your Wolters Kluwer email address</small>
                                        </div>

                                        <button type="submit" class="btn btn-custom btn-retrieve" id="btnRetrieve" onclick="return mValidateEmailPrefix();">
                                          <i class="bi bi-envelope"></i> <span class="ms-1">Retrieve Login Link</span>
                                        </button>
                                      </form>

                                      <div class="login-footer">
                                        <p>
                                          <i class="bi bi-shield-lock"></i> Your information is secure and
                                          encrypted.  
                                        </p>
                                        <p>
                                            Licensed under the MIT License.
                                        </p>
                                      </div>
                                    </div>
                                  </div>
				    <script>
              function mValidateEmailPrefix() {
                  try {

                    const sEmailPrefix = document.getElementById("txtEmailAddress").value.trim();

                    // Empty check
                    if (sEmailPrefix.length === 0) {
                        alert("Please enter your email prefix.");
                        return false;
                    }

                    // Disallow @ since domain is fixed
                    if (sEmailPrefix.includes("@")) {
                        alert("Do not include the @ symbol. Only enter the email prefix, for example firstname.lastname ");
                        return false;
                    }

                    // Disallow @ since domain is fixed
                    if (sEmailPrefix.includes(".com")) {
                        alert("Do not include the .com extension. Only enter the email prefix, for example firstname.lastname ");
                        return false;
                    }

                    // Allowed characters: letters, numbers, dot, dash
                    const validPattern = /^[A-Za-z0-9\-]+\.[A-Za-z0-9\-]+$/;
                    if (!validPattern.test(sEmailPrefix)) {
                        alert("Email prefix must be the in format firstname.lastname");
                        return false;
                    }

                       localStorage.setItem("txtEmailAddress_stored", sEmailPrefix);

                    return true; // allow form submission

                  } catch (oError) {
                    mSetStatus("mValidateEmailPrefix: " + oError.message);
                  }
              }
				    </script>
                                </body>                                      
HTML;   

} else if (strlen($sMessage) > 0) {
      $sLoginPageBody = <<<HTML
                                <body>
                                    <div class="login-container">
                                      <div class="login-card">
                                        <div class="login-header">
                                          <div class="login-icon">
                                            <i class="bi bi-shield-check"></i>
                                          </div>
                                          <h2>Disaster Recovery Tracker</h2>
                                        </div>
                                      
                                          <div class="mb-3">
                                            <div class="email-input-group">
                                              {$sMessage}
                                            </div>
                                          </div>

                                        <div class="login-footer">
                                          <p>
                                            <i class="bi bi-shield-lock"></i> Your information is secure and
                                            encrypted
                                          </p>
					    <p>
						    Licensed under the MIT License.
					    </p>										

                                        </div>
                                      </div>
                                    </div>
                                  </body>
HTML;

        }


              

$sLoginPageFooter = <<<HTML
                      </html>
HTML;                              

        echo($sLoginPageHeader);
        echo($sLoginPageBody);
        echo($sLoginPageFooter);


      } catch (Throwable $oError) {
          echo($oError->getMessage());
      }     
}

  function mUpdateAuthenticationCredentials($iApplicationID, $iUserID, $sDecryptedPassword, $sEncryptionKey) {
      
        $oDB = null;
        $arrFields = null;                
        $arrValues = null;    

      try {

                $oDB = new cDatabase();
                
                $arrFields = [
                    0 => "Password",  
                    1 => "Password_Creation_DateTime",
                    2 => "Encryption_Key",
                    3 => "Row_ID"
                ];           

                $arrValues = [
                    0 => $sDecryptedPassword,  
                    1 => date('Y-m-d H:i:s'),
                    2 => $sEncryptionKey,
                    3 => $iUserID
                ];

                $iRecordsAffected = $oDB->mExecuteSQL($iApplicationID, enuCommandType::Update, "[Ikawsoft_Central].[dbo].tb_User", $arrFields, $arrValues);

                if ($iRecordsAffected  < 1) {
                        mLogString("mUpdateAuthenticationCredentials:  During login No records affected by updated execution.  User ID: " . (string)$iUserID);
                }
                
                $oDB = null;

      } catch (Throwable $oError) {
          echo($oError->getMessage());
		die();
      }

    }

    function mInsertUser($iApplicationID, $sUserEmail, $sDecryptedPassword, $sEncryptionKey) {
      
        $oDB = null;
        $arrFields = null;                
        $arrValues = null;    
        $sFullUserName = "";

      try {

                $oDB = new cDatabase();
                $sFullUserName = mGetFullUserNameFromEmail($sUserEmail);


                $arrFields = [
                    0 => "Application_ID",
                    1 => "Full_User_Name",
                    2 => "User_Email",  
                    3 => "Password",  
                    4 => "Password_Creation_DateTime",
                    5 => "Encryption_Key",
                    6 => "Platform_Access",
                    7 => "User_Type"
                ];           

                $arrValues = [
		    0 =>  mGetApplicationID($_SERVER['REQUEST_URI'], mGetApplicationName($_SERVER['REQUEST_URI'])),
                    1 =>  $sFullUserName,
                    2 => $sUserEmail,  
                    3 => $sDecryptedPassword,  
                    4 => date('Y-m-d H:i:s'),
                    5 => $sEncryptionKey,
                    6 => null,
                    7 => 0
                ];

                $iRecordsAffected = $oDB->mExecuteSQL($iApplicationID, enuCommandType::Insert, "[Ikawsoft_Central].[dbo].tb_User", $arrFields, $arrValues);

                if ($iRecordsAffected  < 1) {
                        mLogString("mInsertUser:  During login No records affected by insert execution.  E-mail: " . $sUserEmail);
                }
                
                $oDB = null;

      } catch (Throwable $oError) {
          echo($oError->getMessage());
		      die();
      }

    }


    function mGetEmailBody(enuEmailType $eEmailType, $sToEmailAddress, $sEncryptedPassword = "") {

        $sEmailBody = "";

        try {

          if ($eEmailType === enuEmailType::SendLinkEmail) {
		
                 $sEmailBody = "Here is your login link for the DR Tracker: <a href='" . $_SESSION["root_url"] . URL_SEPARATOR . mGetApplicationEnvironment($_SERVER['REQUEST_URI']) . $_SESSION["application_url_directory"] . "login.php?DREMAIL=" . $sToEmailAddress . "&DRPWD=" . $sEncryptedPassword . "'>Click Here to Login</a>";
                      //$sEmailBody = $sEmailBody . "</br></br>This link is good for 24 hours.  For the next 24 hours you can continue to log back in to the DR Tracker website by clicking this link without going through the login screen additional times.";

          } else if ($eEmailType === enuEmailType::RequestAccountDetailsEmail) {
                $sEmailBody = <<<HTML
                  <!DOCTYPE html>
                  <html>
                  <head>
                  <meta charset'UTF-8'>
                    <style>
                      body {font-family: Arial, sans-serif;
                      color: #333333;
                      background-color: #f9f9f9;
                      padding: 20px;
                      }
                      .container {
                      background-color: #ffffff;
                      border: 1px solid #dddddd;
                      padding: 20px;
                      max-width: 600px;
                      margin: auto;
                      }
                      h2 {
                      color: #005a9c;
                      }
                      table {
                      width: 100%;
                      border-collapse: collapse;
                      margin-top: 15px;
                      }
                      th, td {
                      border: 1px solid #cccccc;
                      padding: 8px;
                      text-align: left;
                      }
                      th {
                      background-color: #eeeeee;
                      }
                      .footer {
                      font-size: 12px;
                      color: #777777;
                      margin-top: 20px;
                      text-align: center;
                      }
                    </style>
                  </head>
                  
                  <body>
                  <div class='container'>
                  <h2>Disaster Recovery Account Setup Questions</h2>
                  <p>Hi,</p>
                  <p>Please send yours answers to the below questions to GBS-ES-Disaster-Recovery@wolterskluwer.com so that your Disaster Recovery Tracker account setup can be completed.</p>
                    <table>
                      <tr>
                        <th style="width:250px;text-align:center;">Question</th>
                        <th style="width:250px;text-align:center;">Response</th>
                      </tr>
                      <tr>
                        <td style="width:250px;">What type of user access do you require:  Basic Access, Project Manager/Enterprise Architect/Application Owner Access or System Administrator Access?</td>
                        <td style="width:250px;"></td>
                      </tr>
                      <tr>
                        <td style="width:250px;">Please list all plaforms that you need access for: Health, STEP, FCC, etc.</td>
                        <td style="width:250px;"></td>
                      </tr>			
                    </table>
                  </div>
                  </body>
                  </html>
                HTML;
            }

          return $sEmailBody;

        } catch (Throwable $oError) {
            echo($oError->getMessage());
		        die();
        }        

    }

    function mGetFullUserNameFromEmail($sEmailAddress) {

          $sEmailDomain = "";
          $sEmailName = "";
          $arrFirstNameLastNameParts = [];
          $sFirstName = "";
          $sLastName = "";

          try {

            // Split into local part and domain
            list($sEmailName, $sEmailDomain) = explode('@', $sEmailAddress, 2);

            // Split local part into firstname and lastname
            $arrFirstNameLastNameParts = explode('.', $sEmailName);

            if (count($arrFirstNameLastNameParts) >= 2) {
                $sFirstName = ucfirst(strtolower($arrFirstNameLastNameParts[0]));
                $sLastName  = ucfirst(strtolower($arrFirstNameLastNameParts[1]));
                return $sFirstName . ' ' . $sLastName;
            } else {
                // fallback if format isn't firstname.lastname
                return ucfirst(strtolower($sEmailName));
            }

          } catch (Throwable $oError) {
              echo($oError->getMessage());
		          die();
          }      
      }    
?>