<?php


    $sApplicationFileDirectory = "";
    $sServerFileDirectory = "";
    $sIncludeFilePath = "";

    require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

    mIncludePage(enuIncludePackageID::InitOnly, mGetApplicationEnvironment($_SERVER['REQUEST_URI'])); 


?>

<!DOCTYPE html>
<html lang="en">
<head>
    <title>Disaster Recovery Tracker</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">


</head>

<body style="margin-top: 0px; margin-left: 0px;">


    <h2>Broadcast Message to All Platform Users</h2>

         <span  style="margin-left:425px;">
            Choose Platform:
            <select name="cboPlatform" id="cboPlatform">
                <option value=""></option>
                <option value="55">Health Advantage Ecosystem</option>
                <option value="5490">FCC</option>
                <option value="5495">STEP</option>
                <option value="0">Global</option>
            </select>
         </span>
        <br>
        <textarea style="width: 750px; height: 500px;"></textarea>
        <br>
        <button type="submit" style="margin-left:625px" name="btnSendMessage" onclick="mSendMessage();">
            Send Message
        </button>





    <?php
        echo("<script src='" . mGetRootPath(ApplicationID::DR, enuPathRootType::URL, enuPathType::SharedLibraryFolder, enuFolderType::lib, getenv('Application_File_Directory'), getenv('Server_File_Directory')) . "js_include.php?v=1.0'></script>");
    ?>

<script>


    function mSendMessage() {

        try {

            const iPlatformID = Number(document.getElementById("cboPlatform").value);
            const sMessageText = String(document.getElementById("txtMessage").value);

            if (mIsNumeric(iPlatformID) && sMessageText.trim().length > 0) {
                giPlatformID = iPlatformID;

                if (iPlatFormID === 0) {
                    cstBroadcastMessageType.GlobalMessage.Message = sMessageText;
                    oBroadcastClient.TargetObject = null;
                    oBroadcastClient.RowID = "";
                    oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.GlobalMessage.ID);
                    cstBroadcastMessageType.GlobalMessage.Message = "";
                } else {
                    cstBroadcastMessageType.PlatformMessage.Message = sMessageText;
                    oBroadcastClient.TargetObject = null;
                    oBroadcastClient.RowID = "";
                    oBroadcastClient.BroadcastMessage(cstBroadcastMessageType.PlatformMessage.ID);
                    cstBroadcastMessageType.PlatformMessage.Message = "";
                }

            } else {
                alert("Platform ID must be selected and Message Text must be entered.");
            }

        } catch (oError) {
           mSetStatus("mSendMessage", oError);
        }
    }

</script>



</body>
</html>

