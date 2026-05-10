<?php
    $sSessionName = "";
    $sFormAction = "";
    $sSubmittedFormAction = "";

    $sSessionName = $_GET["Session_Name"] ?? "";
    $sSubmittedFormAction  = $_GET["Form_Action"] ?? "";

    $sFormAction = htmlspecialchars($_SERVER['PHP_SELF']);
    $sFormAction = $sFormAction . "?Form_Action=Kill_Session" . "&Session_Name=" . $sSessionName;

    session_name($sSessionName);
    session_start();
?>

<html>
    <head>

    </head>

    <body>
        <form method="post" action="<?php echo($sFormAction); ?>">
            <button type="submit">Kill Session</button>
        </form>

        <?php
            if ($sSubmittedFormAction === "Kill_Session") {
                mKillSession();
            }

            mOutputSessionDetails();

        ?>
    </body>
</html>



<?php

    function mOutputSessionDetails() {

        try {

            echo "<pre>";
                echo json_encode($_SESSION, JSON_PRETTY_PRINT);
            echo "</pre>";

        } catch (Throwable $oError) {
            echo($oError->getMessage());
        }

    }

    function mKillSession() {

        try {

            setcookie(session_name(), '', time() - 3600, '/');

            session_unset();      // clear $_SESSION array
            session_destroy();    // delete the session file

            echo("session killed");

        } catch (Throwable $oError) {
            echo($oError->getMessage());
        }
    }
?>