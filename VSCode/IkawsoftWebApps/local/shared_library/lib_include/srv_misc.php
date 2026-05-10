<?php
        function mIncludeStatusBar($sURLExtension, $eApplicationID, $sFullUserName, $sUserTypeDescription) {
            try {


                /*Output the status bar with the version number*/
                $sVersionNumber = "";
                $sApplicationEnvironment = "";
                
                
                $sVersionNumber = mGetApplicationVersionNumber($eApplicationID);
                $sApplicationEnvironment = mGetApplicationEnvironment($sURLExtension);
                $sApplicationEnvironment = strtoupper($sApplicationEnvironment);
                

                echo("
                        <div id='CustomStatusBar'>
                            <div id='status-left' class='status-left'>Environment: $sApplicationEnvironment | User: $sFullUserName | User Type: $sUserTypeDescription</div>
                            <div id='status-center' class='status-center'></div>
                            <div id='status-right' class='status-right'>Version $sVersionNumber  Licensed under the MIT License.</div>
                        </div>
                    ");

            } catch (Throwable $oError) {
                throw $oError;
            }                
        }
?>
