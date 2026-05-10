<?php

    try {
        
        require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';
        mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));           
        
        if ($_SERVER['REQUEST_METHOD'] === 'POST') {

            foreach ($_POST as $sSessionVariable => $sNewValue) {

                    $_SESSION[$sSessionVariable] = $sNewValue;

            }

            echo json_encode([
                'error' => false,
                'message' => 'Session values set successfully.'
            ]);
            exit;

        }

        // If not a POST request
        throw new Exception('set_Session:  Invalid request: Not a post request type.');

    } catch (Throwable $oError) {
        echo json_encode([
            'error' => true,
            'code' => 5000, // Custom error code
            'message' => $oError->getMessage(),
            'details' => $oError->getMessage()
        ]);
    }
?>