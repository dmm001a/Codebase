<?php

        header('Content-Type: application/json');

        if (!defined('URL_SEPARATOR')) {
            define('URL_SEPARATOR', '/');
        }
        

        try {

            require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

            mIncludePage(enuIncludePackageID::InitOnly, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));  

            $sUploadDirectoryPath = "";
            $sFileName = "";

            if (!isset($_FILES['UploadImage'])) {
                throw new Exception("Artifact Image is required.");
            } else if (!isset($_POST["UploadDirectoryPath"]) || ($_POST["UploadDirectoryPath"] === "")) {
                throw new Exception("Upload Path is required.");
            }

            $sUploadDirectoryPath = $_POST["UploadDirectoryPath"];

           
            if (!is_dir($sUploadDirectoryPath)) {
                mkdir($sUploadDirectoryPath, 0777, true);
            }

                $sFileName = "paste_" . time() . "_" . rand(1000,9999) . ".png";

                if (move_uploaded_file($_FILES['UploadImage']['tmp_name'], $sUploadDirectoryPath . $sFileName)) {
                    echo json_encode([
                        "success" => true,
                        "url" => $sUploadDirectoryPath . "/" . $sFileName
                    ]);
                } else {
                    throw new Exception("Failed to save file.");
                }

        } catch (Throwable $oError) {
            mLogString($oError->getMessage());

            echo json_encode([
                'error'            => true,
                'code'             => $oError->getCode() ?: 5000,
                'message'          => $oError->getMessage(),
                'errornumber' => isset($oError->errorInfo[1]) ? $oError->errorInfo[1] : null,
                'details'          => $oError->getTraceAsString()
            ], JSON_PRETTY_PRINT);            
        }              
?>